using System;
using System.Diagnostics.Contracts;

namespace DotNet.Core.Contracts
{
    public class ContractException : Exception
    {
		private const int COR_E_CODECONTRACTFAILED = unchecked((int)0x80131542);
        public ContractFailureKind Kind { get; private set; }
        public string UserMessage { get; private set; }
        public string ConditionText { get; private set; }

        public ContractException(ContractFailureKind kind, string conditionText, string userMessage = null)
            : base($"{kind} failed: {userMessage ?? conditionText}")
        {
            HResult = COR_E_CODECONTRACTFAILED;
            Kind = kind;
            UserMessage = userMessage;
            ConditionText = conditionText;
        }
    }
}