// Source: https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Diagnostics/Contracts/ContractsBCL.cs

using System.Diagnostics.CodeAnalysis;

namespace System.Diagnostics.Contracts
{
    [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
    public sealed class ContractException : Exception
    {
        private readonly ContractFailureKind _Kind;
        private readonly string _UserMessage;
        private readonly string _Condition;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ContractFailureKind Kind { get { return _Kind; } }
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public string Failure { get { return this.Message; } }
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public string UserMessage { get { return _UserMessage; } }
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public string Condition { get { return _Condition; } }

        // Called by COM Interop, if we see COR_E_CODECONTRACTFAILED as an HRESULT.
        private ContractException()
        {
            HResult = System.Runtime.CompilerServices.ContractHelper.COR_E_CODECONTRACTFAILED;
        }

        public ContractException(ContractFailureKind kind, string failure, string userMessage, string condition, Exception innerException)
            : base(failure, innerException)
        {
            HResult = System.Runtime.CompilerServices.ContractHelper.COR_E_CODECONTRACTFAILED;
            _Kind = kind;
            _UserMessage = userMessage;
            _Condition = condition;
        }

        // public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        // {
        //     base.GetObjectData(info, context);
        // }
    }
}