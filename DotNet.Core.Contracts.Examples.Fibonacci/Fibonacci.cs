using System;
using Xunit;
using System.Diagnostics.Contracts;
using System.Linq;

using static System.Math;

namespace DotNet.Core.Contractss.Examples
{
    [ContractClass(typeof(FibonacciContract))]
    public class Fibonacci
    {
        public bool IsFibonacci(int n)
        {
            Func<int, bool> isPerfectSquare = (int m) => Sqrt(m) == Math.Round(Sqrt(m));
            return isPerfectSquare(5*n*n + 4) || isPerfectSquare(5*n*n - 4);
        }

        public int GetNthFibonacci(int n)
        {
            return (n < 2) ? n : GetNthFibonacci(n - 1) + GetNthFibonacci(n - 2);
        }

        public int GetNextFibonacci(int n)
        {
            return Enumerable.Range(0, n).Select(i => GetNthFibonacci(i)).First(m => m > n);
        }
    }

    [ContractClassFor(typeof(Fibonacci))]
    abstract class FibonacciContract : Fibonacci
    {
        new bool IsFibonacci(int n)
        {
            Contract.Ensures(n >= 0 || Contract.Result<bool>() == false);
            return default(bool);
        }

        new int GetNthFibonacci(int n)
        {
            Contract.Requires(n >= 0);
            Contract.Ensures(((Fibonacci)this).IsFibonacci(Contract.Result<int>()));
            return default(int);
        }

        new int GetNextFibonacci(int n)
        {
            Contract.Requires(((Fibonacci)this).IsFibonacci(n));
            Contract.Ensures(((Fibonacci)this).IsFibonacci(Contract.Result<int>()));
            Contract.Ensures(Contract.ForAll(n+1, Contract.Result<int>(), m => !((Fibonacci)this).IsFibonacci(m)));
            return default(int);
        }
    }

    public class FibonacciTest
    {

        [Fact]
        public void Test1()
        {
            Console.WriteLine("TEST");
            Assert.True(true);
        }
    }
}
