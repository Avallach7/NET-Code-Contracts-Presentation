using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.Contracts
{
    public static class ContractsRuntime
    {
        public static void Requires(
            bool condition, string message, string conditionText)
        {
            // Simulates default behavior.
            if (!condition)
            ReportFailure(ContractFailureKind.Precondition,
                            message, conditionText, null);
        }
            
        // // Code skipped...
            
        [DebuggerStepThrough]
        public static void ReportFailure(
            ContractFailureKind kind, string message,
            string conditionText, Exception innerException)
        {
            // Simulates default behavior.
            string displayMessage = RaiseContractFailedEvent(
            kind, message, conditionText, innerException);
            if (displayMessage != null)
            TriggerFailure(kind, displayMessage, message,
                            conditionText, innerException);
        }

       public static string RaiseContractFailedEvent(
           ContractFailureKind kind, string message,
           string conditionText, Exception innerException)
       {
           // Simulates default behavior.
           return ContractHelper.RaiseContractFailedEvent(kind, message, conditionText, innerException);
       }

       public static void TriggerFailure(
           ContractFailureKind kind, string displayMessage,
           string userMessage, string conditionText,
           Exception innerException)
       {
           // this code originally used Environment.UserInteractive, ContractException and System.Diagnostics.Assert.Fail
           // Debug.Fail just prints stack trace to Debug and calls Debug.Break()
           if (Debugger.IsAttached)
               Debug.Fail(conditionText, displayMessage);
           else
               throw new ContractException(kind, displayMessage, userMessage, conditionText, innerException);
       }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // Debug.Fail("bye, world.");
            // Console.WriteLine("Hello World!");
            // Console.ReadKey();
        }
    }
}
