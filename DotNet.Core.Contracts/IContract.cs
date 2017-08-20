using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;

namespace DotNet.Core.Contracts
{
	// based on https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.Contracts/ref/System.Diagnostics.Contracts.cs
	public interface IContract
    {
        event EventHandler<ContractFailedEventArgs> ContractFailed;

        void Requires(bool condition);
        void Requires(bool condition, string userMessage);
        void Requires<TException>(bool condition) where TException : Exception;
        void Requires<TException>(bool condition, string userMessage) where TException : Exception;

        void Ensures(bool condition);
        void Ensures(bool condition, string userMessage);
        void EnsuresOnThrow<TException>(bool condition) where TException : Exception;
        void EnsuresOnThrow<TException>(bool condition, string userMessage) where TException : Exception;

        void Invariant(bool condition);
        void Invariant(bool condition, string userMessage);
        void Assert(bool condition);
        void Assert(bool condition, string userMessage);
        void Assume(bool condition);
        void Assume(bool condition, string userMessage);

        void EndContractBlock();
        T OldValue<T>(T value);
        T Result<T>();
        T ValueAtReturn<T>(out T value);
        
        bool Exists(int fromInclusive, int toExclusive, Predicate<int> predicate);
        bool Exists<T>(IEnumerable<T> collection, Predicate<T> predicate);
        bool ForAll(int fromInclusive, int toExclusive, Predicate<int> predicate);
        bool ForAll<T>(IEnumerable<T> collection, Predicate<T> predicate);
    }

	public interface IContractRuntime
    {
        event EventHandler<ContractFailedEventArgs> ContractFailed;

        void Requires(bool condition, string conditionText);
        void Requires(bool condition, string userMessage, string conditionText);
        void Requires<TException>(bool condition, string conditionText) where TException : Exception;
        void Requires<TException>(bool condition, string userMessage, string conditionText) where TException : Exception;

        void Ensures(bool condition, string conditionText);
        void Ensures(bool condition, string userMessage, string conditionText);
        void EnsuresOnThrow<TException>(bool condition, string conditionText) where TException : Exception;
        void EnsuresOnThrow<TException>(bool condition, string userMessage, string conditionText) where TException : Exception;

        void Invariant(bool condition, string conditionText);
        void Invariant(bool condition, string userMessage, string conditionText);
        void Assert(bool condition, string conditionText);
        void Assert(bool condition, string userMessage, string conditionText);
        void Assume(bool condition, string conditionText);
        void Assume(bool condition, string userMessage, string conditionText);

        void EndContractBlock();
        T OldValue<T>(T value);
        T Result<T>();
        T ValueAtReturn<T>(out T value);
        
        bool Exists(int fromInclusive, int toExclusive, Predicate<int> predicate);
        bool Exists<T>(IEnumerable<T> collection, Predicate<T> predicate);
        bool ForAll(int fromInclusive, int toExclusive, Predicate<int> predicate);
        bool ForAll<T>(IEnumerable<T> collection, Predicate<T> predicate);
    }
}
