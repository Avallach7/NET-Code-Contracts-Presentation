using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Reflection;

namespace NewVoiceMedia.CoreCodeContracts
{
	class ContractRuntime 
	{
		public void Check(bool condition, string userMessage)
		{
            if (!condition)
            {
                
            }
		}

		public void RequireRewriter()
		{
			Debug.Fail("This contract requires use of Contract Syntax Rewriter");
		}
	}



    public static class Contract
    {
		private static ContractRuntime _runtime = new ContractRuntime();

        [Conditional("DEBUG")]
        public static void Assume(bool condition, string userMessage = null)
        {
            _runtime.Check(condition, userMessage);
        }

        [Conditional("DEBUG")]
        public static void Assert(bool condition, string userMessage = null)
        {
            _runtime.Check(condition, userMessage);
        }

        [Conditional("DEBUG")]
        public static void Requires(bool condition, string userMessage = null)
        {
            _runtime.Check(condition, userMessage);
        }

		// purposefully unconditional
        public static void Requires<TException>(bool condition, string userMessage = null) 
			where TException : Exception
        {
            _runtime.Check(condition, userMessage);
        }

		[Conditional("CONTRACTS_FULL"), Conditional("DEBUG")]
        public static void Ensures(bool condition, String userMessage)
        {
            _runtime.RequireRewriter();
        }

		[Conditional("CONTRACTS_FULL"), Conditional("DEBUG")]
        public static void EnsuresOnThrow<TException>(bool condition, string userMessage = null) 
			where TException : Exception
        {
            _runtime.RequireRewriter();
        }

        #region Old, Result, and Out Parameters
        public static T Result<T>() 
		{
            _runtime.RequireRewriter();
			return default(T); 
		}

        public static T ValueAtReturn<T>(out T value) 
		{
            _runtime.RequireRewriter();
			value = default(T); 
			return value; 
		}

        /// <summary>
        /// Represents the value of <paramref name="value"/> as it was at the start of the method or property.
        /// </summary>
        /// <typeparam name="T">Type of <paramref name="value"/>.  This can be inferred.</typeparam>
        /// <param name="value">Value to represent.  This must be a field or parameter.</param>
        /// <returns>Value of <paramref name="value"/> at the start of the method or property.</returns>
        /// <remarks>
        /// This method can only be used within the argument to the <seealso cref="Ensures(bool)"/> contract.
        /// </remarks>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
        [Pure]
#if FEATURE_RELIABILITY_CONTRACTS
#endif
        public static T OldValue<T>(T value) { return default(T); }

        #endregion Old, Result, and Out Parameters

        #endregion Ensures

        #region Invariant

        /// <summary>
        /// Specifies a contract such that the expression <paramref name="condition"/> will be true after every method or property on the enclosing class.
        /// </summary>
        /// <param name="condition">Boolean expression representing the contract.</param>
        /// <remarks>
        /// This contact can only be specified in a dedicated invariant method declared on a class.
        /// This contract is not exposed to clients so may reference members less visible as the enclosing method.
        /// The contract rewriter must be used for runtime enforcement of this invariant.
        /// </remarks>
        [Pure]
        [Conditional("CONTRACTS_FULL")]
#if FEATURE_RELIABILITY_CONTRACTS
#endif
        public static void Invariant(bool condition)
        {
            AssertMustUseRewriter(ContractFailureKind.Invariant, "Invariant");
        }

        /// <summary>
        /// Specifies a contract such that the expression <paramref name="condition"/> will be true after every method or property on the enclosing class.
        /// </summary>
        /// <param name="condition">Boolean expression representing the contract.</param>
        /// <param name="userMessage">If it is not a constant string literal, then the contract may not be understood by tools.</param>
        /// <remarks>
        /// This contact can only be specified in a dedicated invariant method declared on a class.
        /// This contract is not exposed to clients so may reference members less visible as the enclosing method.
        /// The contract rewriter must be used for runtime enforcement of this invariant.
        /// </remarks>
        [Pure]
        [Conditional("CONTRACTS_FULL")]
#if FEATURE_RELIABILITY_CONTRACTS
#endif
        public static void Invariant(bool condition, String userMessage)
        {
            AssertMustUseRewriter(ContractFailureKind.Invariant, "Invariant");
        }

        #endregion Invariant

        #region Quantifiers

        #region ForAll

        /// <summary>
        /// Returns whether the <paramref name="predicate"/> returns <c>true</c> 
        /// for all integers starting from <paramref name="fromInclusive"/> to <paramref name="toExclusive"/> - 1.
        /// </summary>
        /// <param name="fromInclusive">First integer to pass to <paramref name="predicate"/>.</param>
        /// <param name="toExclusive">One greater than the last integer to pass to <paramref name="predicate"/>.</param>
        /// <param name="predicate">Function that is evaluated from <paramref name="fromInclusive"/> to <paramref name="toExclusive"/> - 1.</param>
        /// <returns><c>true</c> if <paramref name="predicate"/> returns <c>true</c> for all integers 
        /// starting from <paramref name="fromInclusive"/> to <paramref name="toExclusive"/> - 1.</returns>
        /// <seealso cref="System.Collections.Generic.List&lt;T&gt;.TrueForAll"/>
        [Pure]
#if FEATURE_RELIABILITY_CONTRACTS
#endif
        public static bool ForAll(int fromInclusive, int toExclusive, Predicate<int> predicate)
        {
            if (fromInclusive > toExclusive)
#if CORECLR
                throw new ArgumentException(SR.Argument_ToExclusiveLessThanFromExclusive);
#else
                throw new ArgumentException("fromInclusive must be less than or equal to toExclusive.");
#endif
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            Contract.EndContractBlock();

            for (int i = fromInclusive; i < toExclusive; i++)
                if (!predicate(i)) return false;
            return true;
        }


        /// <summary>
        /// Returns whether the <paramref name="predicate"/> returns <c>true</c> 
        /// for all elements in the <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">The collection from which elements will be drawn from to pass to <paramref name="predicate"/>.</param>
        /// <param name="predicate">Function that is evaluated on elements from <paramref name="collection"/>.</param>
        /// <returns><c>true</c> if and only if <paramref name="predicate"/> returns <c>true</c> for all elements in
        /// <paramref name="collection"/>.</returns>
        /// <seealso cref="System.Collections.Generic.List&lt;T&gt;.TrueForAll"/>
        [Pure]
#if FEATURE_RELIABILITY_CONTRACTS
#endif
        public static bool ForAll<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            Contract.EndContractBlock();

            foreach (T t in collection)
                if (!predicate(t)) return false;
            return true;
        }

        #endregion ForAll

        #region Exists

        /// <summary>
        /// Returns whether the <paramref name="predicate"/> returns <c>true</c> 
        /// for any integer starting from <paramref name="fromInclusive"/> to <paramref name="toExclusive"/> - 1.
        /// </summary>
        /// <param name="fromInclusive">First integer to pass to <paramref name="predicate"/>.</param>
        /// <param name="toExclusive">One greater than the last integer to pass to <paramref name="predicate"/>.</param>
        /// <param name="predicate">Function that is evaluated from <paramref name="fromInclusive"/> to <paramref name="toExclusive"/> - 1.</param>
        /// <returns><c>true</c> if <paramref name="predicate"/> returns <c>true</c> for any integer
        /// starting from <paramref name="fromInclusive"/> to <paramref name="toExclusive"/> - 1.</returns>
        /// <seealso cref="System.Collections.Generic.List&lt;T&gt;.Exists"/>
        [Pure]
#if FEATURE_RELIABILITY_CONTRACTS
#endif
        public static bool Exists(int fromInclusive, int toExclusive, Predicate<int> predicate)
        {
            if (fromInclusive > toExclusive)
#if CORECLR
                throw new ArgumentException(SR.Argument_ToExclusiveLessThanFromExclusive);
#else
                throw new ArgumentException("fromInclusive must be less than or equal to toExclusive.");
#endif
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            Contract.EndContractBlock();

            for (int i = fromInclusive; i < toExclusive; i++)
                if (predicate(i)) return true;
            return false;
        }

        /// <summary>
        /// Returns whether the <paramref name="predicate"/> returns <c>true</c> 
        /// for any element in the <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">The collection from which elements will be drawn from to pass to <paramref name="predicate"/>.</param>
        /// <param name="predicate">Function that is evaluated on elements from <paramref name="collection"/>.</param>
        /// <returns><c>true</c> if and only if <paramref name="predicate"/> returns <c>true</c> for an element in
        /// <paramref name="collection"/>.</returns>
        /// <seealso cref="System.Collections.Generic.List&lt;T&gt;.Exists"/>
        [Pure]
#if FEATURE_RELIABILITY_CONTRACTS
#endif
        public static bool Exists<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            Contract.EndContractBlock();

            foreach (T t in collection)
                if (predicate(t)) return true;
            return false;
        }

        #endregion Exists

        #endregion Quantifiers

        #region Pointers
        #endregion

        #region Misc.

        /// <summary>
        /// Marker to indicate the end of the contract section of a method.
        /// </summary>
        [Conditional("CONTRACTS_FULL")]
#if FEATURE_RELIABILITY_CONTRACTS
#endif
        public static void EndContractBlock() { }

        #endregion

        #endregion User Methods

        #region Failure Behavior

        /// <summary>
        /// Without contract rewriting, failing Assert/Assumes end up calling this method.
        /// Code going through the contract rewriter never calls this method. Instead, the rewriter produced failures call
        /// System.Runtime.CompilerServices.ContractHelper.RaiseContractFailedEvent, followed by 
        /// System.Runtime.CompilerServices.ContractHelper.TriggerFailure.
        /// </summary>
        static partial void ReportFailure(ContractFailureKind failureKind, String userMessage, String conditionText, Exception innerException);

        /// <summary>
        /// This method is used internally to trigger a failure indicating to the "programmer" that he is using the interface incorrectly.
        /// It is NEVER used to indicate failure of actual contracts at runtime.
        /// </summary>
        static partial void AssertMustUseRewriter(ContractFailureKind kind, String contractKind);

        #endregion
    }

    public static partial class Contract
    {
        #region Private Methods

        [ThreadStatic]
        private static bool _assertingMustUseRewriter;

        /// <summary>
        /// This method is used internally to trigger a failure indicating to the "programmer" that he is using the interface incorrectly.
        /// It is NEVER used to indicate failure of actual contracts at runtime.
        /// </summary>
        static partial void AssertMustUseRewriter(ContractFailureKind kind, String contractKind)
        {
            if (_assertingMustUseRewriter)
                //System.Diagnostics.Assert.Fail("Asserting that we must use the rewriter went reentrant.", "Didn't rewrite this mscorlib?");
                System.Diagnostics.Debug.Fail("Asserting that we must use the rewriter went reentrant.", "Didn't rewrite this mscorlib?");
            _assertingMustUseRewriter = true;

            // For better diagnostics, report which assembly is at fault.  Walk up stack and
            // find the first non-mscorlib assembly.
            Assembly thisAssembly = typeof(Contract).GetTypeInfo().Assembly;  // In case we refactor mscorlib, use Contract class instead of Object.
            // StackTrace stack = new StackTrace();
            // Assembly probablyNotRewritten = null;
            // for (int i = 0; i < stack.FrameCount; i++)
            // {
            //     Assembly caller = stack.GetFrame(i).GetMethod().DeclaringType.Assembly;
            //     if (caller != thisAssembly)
            //     {
            //         probablyNotRewritten = caller;
            //         break;
            //     }
            // }

            // if (probablyNotRewritten == null)
            //     probablyNotRewritten = thisAssembly;
            // String simpleName = probablyNotRewritten.GetName().Name;
            // System.Runtime.CompilerServices.ContractHelper.TriggerFailure(kind, SR.Format(SR.MustUseCCRewrite, contractKind, simpleName), null, null, null);

			// Source: https://github.com/Microsoft/CodeContracts/blob/master/Foxtrot/Contracts/ContractsMSR.cs
            System.Runtime.CompilerServices.ContractHelper.TriggerFailure(kind, "Must use the rewriter when using Contract." + contractKind, null, null, null);

            _assertingMustUseRewriter = false;
        }

        #endregion Private Methods

        #region Failure Behavior

        /// <summary>
        /// Without contract rewriting, failing Assert/Assumes end up calling this method.
        /// Code going through the contract rewriter never calls this method. Instead, the rewriter produced failures call
        /// System.Runtime.CompilerServices.ContractHelper.RaiseContractFailedEvent, followed by 
        /// System.Runtime.CompilerServices.ContractHelper.TriggerFailure.
        /// </summary>
        [SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Security.SecuritySafeCriticalAttribute")]
        [System.Diagnostics.DebuggerNonUserCode]
#if FEATURE_RELIABILITY_CONTRACTS
#endif
        static partial void ReportFailure(ContractFailureKind failureKind, String userMessage, String conditionText, Exception innerException)
        {
            if (failureKind < ContractFailureKind.Precondition || failureKind > ContractFailureKind.Assume)
                //throw new ArgumentException(SR.Format(SR.Arg_EnumIllegalVal, failureKind), nameof(failureKind));
				// Source: https://github.com/Microsoft/CodeContracts/blob/master/Foxtrot/Contracts/ContractsMSR.cs
				throw new ArgumentException("failureKind is not in range", "failureKind");
            Contract.EndContractBlock();

            // displayMessage == null means: yes we handled it. Otherwise it is the localized failure message
            var displayMessage = System.Runtime.CompilerServices.ContractHelper.RaiseContractFailedEvent(failureKind, userMessage, conditionText, innerException);

            if (displayMessage == null) return;

            System.Runtime.CompilerServices.ContractHelper.TriggerFailure(failureKind, displayMessage, userMessage, conditionText, innerException);
        }

        /// <summary>
        /// Allows a managed application environment such as an interactive interpreter (IronPython)
        /// to be notified of contract failures and 
        /// potentially "handle" them, either by throwing a particular exception type, etc.  If any of the
        /// event handlers sets the Cancel flag in the ContractFailedEventArgs, then the Contract class will
        /// not pop up an assert dialog box or trigger escalation policy.  Hooking this event requires 
        /// full trust, because it will inform you of bugs in the appdomain and because the event handler
        /// could allow you to continue execution.
        /// </summary>
        public static event EventHandler<ContractFailedEventArgs> ContractFailed
        {
#if FEATURE_UNTRUSTED_CALLERS
#endif
            add
            {
                System.Runtime.CompilerServices.ContractHelper.InternalContractFailed += value;
            }
#if FEATURE_UNTRUSTED_CALLERS
#endif
            remove
            {
                System.Runtime.CompilerServices.ContractHelper.InternalContractFailed -= value;
            }
        }
        #endregion FailureBehavior
    }
}