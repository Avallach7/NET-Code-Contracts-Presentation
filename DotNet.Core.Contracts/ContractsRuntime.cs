using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Diagnostics;

namespace DotNet.Core.Contracts
{
	public class RewriterError : Exception {}

    public static class ContractsRuntime
    {
        public static event EventHandler<ContractFailedEventArgs> ContractFailed;

        public static void Requires(bool condition, string conditionText)
		{
			if(!condition)
				ContractHelper.TriggerFailure(
					new ContractException(ContractFailureKind.Precondition, conditionText));
		}

        public static void Requires(bool condition, string userMessage, string conditionText)
		{
			if(!condition)
				ContractHelper.TriggerFailure(new ContractException(
					ContractFailureKind.Precondition, conditionText, userMessage));
		}

        public static void Requires<TException>(bool condition, string conditionText) 
			where TException : Exception, new()
		{
			// TODO: works without rewriting on core?
			if(!condition)
				ContractHelper.TriggerFailure<TException>(
					new ContractException(ContractFailureKind.Precondition, conditionText));
		}

        public static void Requires<TException>(bool condition, string userMessage, string conditionText) 
			where TException : Exception, new()
		{
			if(!condition)
				ContractHelper.TriggerFailure<TException>(
					new ContractException(ContractFailureKind.Precondition, conditionText, userMessage));
		}

        public static void Ensures(bool condition, string conditionText)
		{
			if(!condition)
				ContractHelper.TriggerFailure(
					new ContractException(ContractFailureKind.Postcondition, conditionText));
		}

        public static void Ensures(bool condition, string userMessage, string conditionText)
		{
			if(!condition)
				ContractHelper.TriggerFailure(
					new ContractException(ContractFailureKind.Postcondition, conditionText, userMessage));
		}

        public static void EnsuresOnThrow<TException>(bool condition, string conditionText) where TException : Exception
		{
			if(!condition)
				ContractHelper.TriggerFailure(
					new ContractException(ContractFailureKind.PostconditionOnException, conditionText));
		}

        public static void EnsuresOnThrow<TException>(bool condition, string userMessage, string conditionText) where TException : Exception
		{
			if(!condition)
				ContractHelper.TriggerFailure(
					new ContractException(ContractFailureKind.PostconditionOnException, conditionText));
		}

        public static void Invariant(bool condition, string conditionText)
		{
			if(!condition)
				ContractHelper.TriggerFailure(
					new ContractException(ContractFailureKind.Invariant, conditionText));
		}

        public static void Invariant(bool condition, string userMessage, string conditionText)
		{
			if(!condition)
				ContractHelper.TriggerFailure(
					new ContractException(ContractFailureKind.Invariant, conditionText, userMessage));
		}

        public static void Assert(bool condition, string conditionText)
		{
			if(!condition)
				ContractHelper.TriggerFailure(
					new ContractException(ContractFailureKind.Assert, conditionText));
		}

        public static void Assert(bool condition, string userMessage, string conditionText)
		{
			if(!condition)
				ContractHelper.TriggerFailure(
					new ContractException(ContractFailureKind.Assert, conditionText, userMessage));
		}

        public static void Assume(bool condition, string conditionText)
		{
			if(!condition)
				ContractHelper.TriggerFailure(
					new ContractException(ContractFailureKind.Assume, conditionText));
		}

        public static void Assume(bool condition, string userMessage, string conditionText)
		{
			if(!condition)
				ContractHelper.TriggerFailure(
					new ContractException(ContractFailureKind.Assume, conditionText, userMessage));
		}

        public static void EndContractBlock()
		{
			// ContractHelper.TriggerFailure(new RewriterError();
		}

        public static T OldValue<T>(T value)
		{
			// this call has to be replaced by reference to generated variable
			throw new RewriterError();
		}

        public static T Result<T>()
		{
			// this call has to be replaced by reference to generated variable
			throw new RewriterError();
		}

        public static T ValueAtReturn<T>(out T value)
		{
			// this call has to be replaced by reference to variable passed as parameter
			throw new RewriterError();
		}

        public static bool Exists(int fromInclusive, int toExclusive, Predicate<int> predicate)
		{
			return Enumerable
				.Range(fromInclusive, toExclusive-fromInclusive)
				.Any(i => predicate(i));
		}

        public static bool Exists<T>(IEnumerable<T> collection, Predicate<T> predicate)
		{
			return collection.Any(i => predicate(i));
		}

        public static bool ForAll(int fromInclusive, int toExclusive, Predicate<int> predicate)
		{
			return Enumerable
				.Range(fromInclusive, toExclusive-fromInclusive)
				.All(i => predicate(i));
		}

        public static bool ForAll<T>(IEnumerable<T> collection, Predicate<T> predicate)
		{
			return collection.All(i => predicate(i));
		}
    }
}
