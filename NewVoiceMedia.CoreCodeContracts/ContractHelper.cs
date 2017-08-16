// Source: https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Diagnostics/Contracts/Contracts.cs
// Source: https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Diagnostics/Contracts/ContractsBCL.cs

using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.Runtime.CompilerServices
{
    public static partial class ContractHelper
    {
        #region Rewriter Failure Hooks

        /// <summary>
        /// Rewriter will call this method on a contract failure to allow listeners to be notified.
        /// The method should not perform any failure (assert/throw) itself.
        /// </summary>
        /// <returns>null if the event was handled and should not trigger a failure.
        ///          Otherwise, returns the localized failure message</returns>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [System.Diagnostics.DebuggerNonUserCode]
#if FEATURE_RELIABILITY_CONTRACTS
#endif
        public static string RaiseContractFailedEvent(ContractFailureKind failureKind, String userMessage, String conditionText, Exception innerException)
        {
            var resultFailureMessage = "Contract failed"; // default in case implementation does not assign anything.
            RaiseContractFailedEventImplementation(failureKind, userMessage, conditionText, innerException, ref resultFailureMessage);
            return resultFailureMessage;
        }


        /// <summary>
        /// Rewriter calls this method to get the default failure behavior.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
#if FEATURE_RELIABILITY_CONTRACTS
#endif
        public static void TriggerFailure(ContractFailureKind kind, String displayMessage, String userMessage, String conditionText, Exception innerException)
        {
            TriggerFailureImplementation(kind, displayMessage, userMessage, conditionText, innerException);
        }

        #endregion Rewriter Failure Hooks

        #region Implementation Stubs

        /// <summary>
        /// Rewriter will call this method on a contract failure to allow listeners to be notified.
        /// The method should not perform any failure (assert/throw) itself.
        /// This method has 3 functions:
        /// 1. Call any contract hooks (such as listeners to Contract failed events)
        /// 2. Determine if the listeneres deem the failure as handled (then resultFailureMessage should be set to null)
        /// 3. Produce a localized resultFailureMessage used in advertising the failure subsequently.
        /// </summary>
        /// <param name="resultFailureMessage">Should really be out (or the return value), but partial methods are not flexible enough.
        /// On exit: null if the event was handled and should not trigger a failure.
        ///          Otherwise, returns the localized failure message</param>
        static partial void RaiseContractFailedEventImplementation(ContractFailureKind failureKind, String userMessage, String conditionText, Exception innerException, ref string resultFailureMessage);

        /// <summary>
        /// Implements the default failure behavior of the platform. Under the BCL, it triggers an Assert box.
        /// </summary>
        static partial void TriggerFailureImplementation(ContractFailureKind kind, String displayMessage, String userMessage, String conditionText, Exception innerException);

        #endregion Implementation Stubs
    }

    public static partial class ContractHelper
    {
        #region Private fields

        private static volatile EventHandler<ContractFailedEventArgs> contractFailedEvent;
        private static readonly Object lockObject = new Object();

        internal const int COR_E_CODECONTRACTFAILED = unchecked((int)0x80131542);

        #endregion

        /// <summary>
        /// Allows a managed application environment such as an interactive interpreter (IronPython) or a
        /// web browser host (Jolt hosting Silverlight in IE) to be notified of contract failures and 
        /// potentially "handle" them, either by throwing a particular exception type, etc.  If any of the
        /// event handlers sets the Cancel flag in the ContractFailedEventArgs, then the Contract class will
        /// not pop up an assert dialog box or trigger escalation policy.  Hooking this event requires 
        /// full trust.
        /// </summary>
        internal static event EventHandler<ContractFailedEventArgs> InternalContractFailed
        {
#if FEATURE_UNTRUSTED_CALLERS
#endif
            add
            {
                // Eagerly prepare each event handler _marked with a reliability contract_, to 
                // attempt to reduce out of memory exceptions while reporting contract violations.
                // This only works if the new handler obeys the constraints placed on 
                // constrained execution regions.  Eagerly preparing non-reliable event handlers
                // would be a perf hit and wouldn't significantly improve reliability.
                // UE: Please mention reliable event handlers should also be marked with the 
                // PrePrepareMethodAttribute to avoid CER eager preparation work when ngen'ed.
                // System.Runtime.CompilerServices.RuntimeHelpers.PrepareContractedDelegate(value);
                lock (lockObject)
                {
                    contractFailedEvent += value;
                }
            }
#if FEATURE_UNTRUSTED_CALLERS
#endif
            remove
            {
                lock (lockObject)
                {
                    contractFailedEvent -= value;
                }
            }
        }

        /// <summary>
        /// Rewriter will call this method on a contract failure to allow listeners to be notified.
        /// The method should not perform any failure (assert/throw) itself.
        /// This method has 3 functions:
        /// 1. Call any contract hooks (such as listeners to Contract failed events)
        /// 2. Determine if the listeneres deem the failure as handled (then resultFailureMessage should be set to null)
        /// 3. Produce a localized resultFailureMessage used in advertising the failure subsequently.
        /// </summary>
        /// <param name="resultFailureMessage">Should really be out (or the return value), but partial methods are not flexible enough.
        /// On exit: null if the event was handled and should not trigger a failure.
        ///          Otherwise, returns the localized failure message</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [System.Diagnostics.DebuggerNonUserCode]
#if FEATURE_RELIABILITY_CONTRACTS
#endif
        static partial void RaiseContractFailedEventImplementation(ContractFailureKind failureKind, String userMessage, String conditionText, Exception innerException, ref string resultFailureMessage)
        {
            // if (failureKind < ContractFailureKind.Precondition || failureKind > ContractFailureKind.Assume)
            //     throw new ArgumentException(SR.Format(SR.Arg_EnumIllegalVal, failureKind), nameof(failureKind));
            // Contract.EndContractBlock();

            string returnValue;
            String displayMessage = "contract failed.";  // Incomplete, but in case of OOM during resource lookup...
            ContractFailedEventArgs eventArgs = null;  // In case of OOM.
#if FEATURE_RELIABILITY_CONTRACTS
            System.Runtime.CompilerServices.RuntimeHelpers.PrepareConstrainedRegions();
#endif
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

        /// <summary>
        /// Rewriter calls this method to get the default failure behavior.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "conditionText")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "userMessage")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "kind")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "innerException")]
        [System.Diagnostics.DebuggerNonUserCode]
        static partial void TriggerFailureImplementation(ContractFailureKind kind, String displayMessage, String userMessage, String conditionText, Exception innerException)
        {
            // If we're here, our intent is to pop up a dialog box (if we can).  For developers 
            // interacting live with a debugger, this is a good experience.  For Silverlight 
            // hosted in Internet Explorer, the assert window is great.  If we cannot
            // pop up a dialog box, throw an exception (consider a library compiled with 
            // "Assert On Failure" but used in a process that can't pop up asserts, like an 
            // NT Service).

            if (!Debugger.IsAttached)// if (!Environment.UserInteractive)
            {
                throw new ContractException(kind, displayMessage, userMessage, conditionText, innerException);
            }
			else
			{
				Debug.Fail(conditionText, displayMessage);
			}

            // May need to rethink Assert.Fail w/ TaskDialogIndirect as a model.  Window title.  Main instruction.  Content.  Expanded info.
            // Optional info like string for collapsed text vs. expanded text.
            // String windowTitle = SR.GetResourceString(GetResourceNameForFailure(kind));
            // const int numStackFramesToSkip = 2;  // To make stack traces easier to read
            // System.Diagnostics.Assert.Fail(conditionText, displayMessage, windowTitle, COR_E_CODECONTRACTFAILED, StackTrace.TraceFormat.Normal, numStackFramesToSkip);
            // If we got here, the user selected Ignore.  Continue.
        }

        private static String GetResourceNameForFailure(ContractFailureKind failureKind)
        {
            String resourceName = null;
            switch (failureKind)
            {
                case ContractFailureKind.Assert:
                    resourceName = "AssertionFailed";
                    break;

                case ContractFailureKind.Assume:
                    resourceName = "AssumptionFailed";
                    break;

                case ContractFailureKind.Precondition:
                    resourceName = "PreconditionFailed";
                    break;

                case ContractFailureKind.Postcondition:
                    resourceName = "PostconditionFailed";
                    break;

                case ContractFailureKind.Invariant:
                    resourceName = "InvariantFailed";
                    break;

                case ContractFailureKind.PostconditionOnException:
                    resourceName = "PostconditionOnExceptionFailed";
                    break;

                default:
                    // Contract.Assume(false, "Unreachable code");
                    resourceName = "AssumptionFailed";
                    break;
            }
            return resourceName;
        }

// #if FEATURE_RELIABILITY_CONTRACTS
// #endif
		private static System.Resources.ResourceManager myResourceManager = new System.Resources.ResourceManager("mscorlib", typeof(Object).GetTypeInfo().Assembly);

        private static String GetDisplayMessage(ContractFailureKind failureKind, String userMessage, String conditionText)
        {
            String resourceName = GetResourceNameForFailure(failureKind);
            // // Well-formatted English messages will take one of four forms.  A sentence ending in
            // // either a period or a colon, the condition string, then the message tacked 
            // // on to the end with two spaces in front.
            // // Note that both the conditionText and userMessage may be null.  Also, 
            // // on Silverlight we may not be able to look up a friendly string for the
            // // error message.  Let's leverage Silverlight's default error message there.
            // String failureMessage;
            // if (!String.IsNullOrEmpty(conditionText))
            // {
            //     resourceName += "_Cnd";
            //     failureMessage = SR.Format(SR.GetResourceString(resourceName), conditionText);
            // }
            // else
            // {
            //     failureMessage = SR.GetResourceString(resourceName);
            // }

            // // Now add in the user message, if present.
            // if (!String.IsNullOrEmpty(userMessage))
            // {
            //     return failureMessage + "  " + userMessage;
            // }
            // else
            // {
            //     return failureMessage;
            // }

			// Source: https://github.com/Microsoft/CodeContracts/blob/master/Foxtrot/Contracts/ContractsMSR.cs
			var failureMessage = myResourceManager.GetString(resourceName);
            if (failureMessage == null)
            { // Hack for pre-V4 CLR�s
                failureMessage = String.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0} failed", failureKind);
            }
            // Now format based on presence of condition/userProvidedMessage
            if (!String.IsNullOrEmpty(conditionText))
            {
                if (!String.IsNullOrEmpty(userMessage))
                {
                    // both != null
                    return String.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0}: {1} {2}", failureMessage, conditionText, userMessage);
                }
                else
                {
                    // condition != null, userProvidedMessage == null
                    return String.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0}: {1}", failureMessage, conditionText);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(userMessage))
                {
                    // condition null, userProvidedMessage != null
                    return String.Format(System.Globalization.CultureInfo.CurrentUICulture, "{0}: {1}", failureMessage, userMessage);
                }
                else
                {
                    // both null
                    return failureMessage;
                }
            }
        }
    }
}  // namespace System.Runtime.CompilerServices
