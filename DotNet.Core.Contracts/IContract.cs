using System;
using System.Diagnostics.Contracts;

namespace DotNet.Core.Contracts
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

	// based on https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.Contracts/ref/System.Diagnostics.Contracts.cs
	public interface IContract
    {
        event EventHandler<ContractFailedEventArgs> ContractFailed;
        void Assert(bool condition);
        void Assert(bool condition, string userMessage);
        void Assume(bool condition);
        void Assume(bool condition, string userMessage);
        void EndContractBlock();
        void Ensures(bool condition);
        void Ensures(bool condition, string userMessage);
        void EnsuresOnThrow<TException>(bool condition) where TException : System.Exception;
        void EnsuresOnThrow<TException>(bool condition, string userMessage) where TException : System.Exception;
        bool Exists(int fromInclusive, int toExclusive, System.Predicate<int> predicate);
        bool Exists<T>(System.Collections.Generic.IEnumerable<T> collection, System.Predicate<T> predicate);
        bool ForAll(int fromInclusive, int toExclusive, System.Predicate<int> predicate);
        bool ForAll<T>(System.Collections.Generic.IEnumerable<T> collection, System.Predicate<T> predicate);
        void Invariant(bool condition);
        void Invariant(bool condition, string userMessage);
        T OldValue<T>(T value);
        void Requires(bool condition);
        void Requires(bool condition, string userMessage);
        void Requires<TException>(bool condition) where TException : System.Exception;
        void Requires<TException>(bool condition, string userMessage) where TException : System.Exception;
        T Result<T>();
        T ValueAtReturn<T>(out T value);
    }
}
