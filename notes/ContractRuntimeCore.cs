// The .NET Foundation licenses this file to you under the MIT license.
// source: https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Diagnostics/Contracts/Contracts.cs
// source: https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Diagnostics/Contracts/ContractsBCL.cs

namespace System.Diagnostics.Contracts
{
	public static partial class Contract
    {
        /// Without contract rewriting, failing Assert/Assumes end up calling this method.
        /// Code going through the contract rewriter never calls this method. Instead, the rewriter produced failures call
        /// System.Runtime.CompilerServices.ContractHelper.RaiseContractFailedEvent, followed by 
        /// System.Runtime.CompilerServices.ContractHelper.TriggerFailure.
        static partial void ReportFailure(ContractFailureKind failureKind, String userMessage, String conditionText, Exception innerException)
		{
            if (failureKind < ContractFailureKind.Precondition || failureKind > ContractFailureKind.Assume)
                throw new ArgumentException(SR.Format(SR.Arg_EnumIllegalVal, failureKind), nameof(failureKind));
            Contract.EndContractBlock();

            // displayMessage == null means: yes we handled it. Otherwise it is the localized failure message
            var displayMessage = System.Runtime.CompilerServices.ContractHelper.RaiseContractFailedEvent(failureKind, userMessage, conditionText, innerException);

            if (displayMessage == null) return;

            System.Runtime.CompilerServices.ContractHelper.TriggerFailure(failureKind, displayMessage, userMessage, conditionText, innerException);
        }

        /// This method is used internally to trigger a failure indicating to the "programmer" that he is using the interface incorrectly.
        /// It is NEVER used to indicate failure of actual contracts at runtime.
        static partial void AssertMustUseRewriter(ContractFailureKind kind, String contractKind)
        {
            if (_assertingMustUseRewriter)
                System.Diagnostics.Assert.Fail("Asserting that we must use the rewriter went reentrant.", "Didn't rewrite this mscorlib?");
            _assertingMustUseRewriter = true;

            // For better diagnostics, report which assembly is at fault.  Walk up stack and
            // find the first non-mscorlib assembly.
            Assembly thisAssembly = typeof(Contract).Assembly;  // In case we refactor mscorlib, use Contract class instead of Object.
            StackTrace stack = new StackTrace();
            Assembly probablyNotRewritten = null;
            for (int i = 0; i < stack.FrameCount; i++)
            {
                Assembly caller = stack.GetFrame(i).GetMethod().DeclaringType.Assembly;
                if (caller != thisAssembly)
                {
                    probablyNotRewritten = caller;
                    break;
                }
            }

            if (probablyNotRewritten == null)
                probablyNotRewritten = thisAssembly;
            String simpleName = probablyNotRewritten.GetName().Name;
            System.Runtime.CompilerServices.ContractHelper.TriggerFailure(kind, SR.Format(SR.MustUseCCRewrite, contractKind, simpleName), null, null, null);

            _assertingMustUseRewriter = false;
        }
    }
}

namespace System.Runtime.CompilerServices
{
    public static partial class ContractHelper
    {
        /// Rewriter will call this method on a contract failure to allow listeners to be notified.
        /// The method should not perform any failure (assert/throw) itself.
        /// <returns>null if the event was handled and should not trigger a failure.
        ///          Otherwise, returns the localized failure message</returns>
        public static string RaiseContractFailedEvent(ContractFailureKind failureKind, String userMessage, String conditionText, Exception innerException)
        {
            var resultFailureMessage = "Contract failed"; // default in case implementation does not assign anything.
            RaiseContractFailedEventImplementation(failureKind, userMessage, conditionText, innerException, ref resultFailureMessage);
            return resultFailureMessage;
        }

        /// Rewriter calls this method to get the default failure behavior.
        public static void TriggerFailure(ContractFailureKind kind, String displayMessage, String userMessage, String conditionText, Exception innerException)
        {
            TriggerFailureImplementation(kind, displayMessage, userMessage, conditionText, innerException);
        }

        /// Rewriter will call this method on a contract failure to allow listeners to be notified.
        /// The method should not perform any failure (assert/throw) itself.
        /// This method has 3 functions:
        /// 1. Call any contract hooks (such as listeners to Contract failed events)
        /// 2. Determine if the listeneres deem the failure as handled (then resultFailureMessage should be set to null)
        /// 3. Produce a localized resultFailureMessage used in advertising the failure subsequently.
        /// <param name="resultFailureMessage">Should really be out (or the return value), but partial methods are not flexible enough.
        /// On exit: null if the event was handled and should not trigger a failure.
        ///          Otherwise, returns the localized failure message</param>
        static partial void RaiseContractFailedEventImplementation(ContractFailureKind failureKind, String userMessage, String conditionText, Exception innerException, ref string resultFailureMessage);
        {
            if (failureKind < ContractFailureKind.Precondition || failureKind > ContractFailureKind.Assume)
                throw new ArgumentException(SR.Format(SR.Arg_EnumIllegalVal, failureKind), nameof(failureKind));
            Contract.EndContractBlock();

            string returnValue;
            String displayMessage = "contract failed.";  // Incomplete, but in case of OOM during resource lookup...
            ContractFailedEventArgs eventArgs = null;  // In case of OOM.
            System.Runtime.CompilerServices.RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                displayMessage = GetDisplayMessage(failureKind, userMessage, conditionText);
                EventHandler<ContractFailedEventArgs> contractFailedEventLocal = contractFailedEvent;
                if (contractFailedEventLocal != null)
                {
                    eventArgs = new ContractFailedEventArgs(failureKind, displayMessage, conditionText, innerException);
                    foreach (EventHandler<ContractFailedEventArgs> handler in contractFailedEventLocal.GetInvocationList())
                    {
                        try
                        {
                            handler(null, eventArgs);
                        }
                        catch (Exception e)
                        {
                            eventArgs.thrownDuringHandler = e;
                            eventArgs.SetUnwind();
                        }
                    }
                    if (eventArgs.Unwind)
                    {
                        // unwind
                        if (innerException == null) { innerException = eventArgs.thrownDuringHandler; }
                        throw new ContractException(failureKind, displayMessage, userMessage, conditionText, innerException);
                    }
                }
            }
            finally
            {
                if (eventArgs != null && eventArgs.Handled)
                {
                    returnValue = null; // handled
                }
                else
                {
                    returnValue = displayMessage;
                }
            }
            resultFailureMessage = returnValue;
        }

        /// Implements the default failure behavior of the platform. Under the BCL, it triggers an Assert box.
        static partial void TriggerFailureImplementation(ContractFailureKind kind, String displayMessage, String userMessage, String conditionText, Exception innerException)
        {
            // If we're here, our intent is to pop up a dialog box (if we can).  For developers 
            // interacting live with a debugger, this is a good experience.  For Silverlight 
            // hosted in Internet Explorer, the assert window is great.  If we cannot
            // pop up a dialog box, throw an exception (consider a library compiled with 
            // "Assert On Failure" but used in a process that can't pop up asserts, like an 
            // NT Service).

            if (!Environment.UserInteractive)
            {
                throw new ContractException(kind, displayMessage, userMessage, conditionText, innerException);
            }

            // May need to rethink Assert.Fail w/ TaskDialogIndirect as a model.  Window title.  Main instruction.  Content.  Expanded info.
            // Optional info like string for collapsed text vs. expanded text.
            String windowTitle = SR.GetResourceString(GetResourceNameForFailure(kind));
            const int numStackFramesToSkip = 2;  // To make stack traces easier to read
            System.Diagnostics.Assert.Fail(conditionText, displayMessage, windowTitle, COR_E_CODECONTRACTFAILED, StackTrace.TraceFormat.Normal, numStackFramesToSkip);
            // If we got here, the user selected Ignore.  Continue.
        }
    }
}