// Source: https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.Contracts/ref/System.Diagnostics.Contracts.cs

namespace System.Diagnostics.Contracts
{
    public enum ContractFailureKind
    {
        Assert = 4,
        Assume = 5,
        Invariant = 3,
        Postcondition = 1,
        PostconditionOnException = 2,
        Precondition = 0,
    }
}
