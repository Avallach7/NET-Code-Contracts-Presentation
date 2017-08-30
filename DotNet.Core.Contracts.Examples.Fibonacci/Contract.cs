namespace System.Diagnostics.Contracts
{
    public static partial class Contract
    {
        // public static event System.EventHandler<System.Diagnostics.Contracts.ContractFailedEventArgs> ContractFailed { add { } remove { } }
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Assert(bool condition) { }
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Assert(bool condition, string userMessage) { }
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Assume(bool condition) { }
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void Assume(bool condition, string userMessage) { }
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void EndContractBlock() { }
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void Ensures(bool condition) { }
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void Ensures(bool condition, string userMessage) { }
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void EnsuresOnThrow<TException>(bool condition) where TException : System.Exception { }
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void EnsuresOnThrow<TException>(bool condition, string userMessage) where TException : System.Exception { }
        public static bool Exists(int fromInclusive, int toExclusive, System.Predicate<int> predicate) { throw null; }
        public static bool Exists<T>(System.Collections.Generic.IEnumerable<T> collection, System.Predicate<T> predicate) { throw null; }
        public static bool ForAll(int fromInclusive, int toExclusive, System.Predicate<int> predicate) { throw null; }
        public static bool ForAll<T>(System.Collections.Generic.IEnumerable<T> collection, System.Predicate<T> predicate) { throw null; }
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void Invariant(bool condition) { }
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void Invariant(bool condition, string userMessage) { }
        public static T OldValue<T>(T value) { throw null; }
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void Requires(bool condition) { }
        [System.Diagnostics.ConditionalAttribute("CONTRACTS_FULL")]
        public static void Requires(bool condition, string userMessage) { }
        public static void Requires<TException>(bool condition) where TException : System.Exception { }
        public static void Requires<TException>(bool condition, string userMessage) where TException : System.Exception { }
        public static T Result<T>() { throw null; }
        public static T ValueAtReturn<T>(out T value) { throw null; }
    }

	/// <summary>
    /// Types marked with this attribute specify that they are a contract for the type that is the argument of the constructor.
    /// </summary>
    [Conditional("CONTRACTS_FULL")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ContractClassForAttribute : Attribute
    {
        private Type _typeIAmAContractFor;

        public ContractClassForAttribute(Type typeContractsAreFor)
        {
            _typeIAmAContractFor = typeContractsAreFor;
        }

        public Type TypeContractsAreFor
        {
            get { return _typeIAmAContractFor; }
        }
    }

    /// <summary>
    /// Types marked with this attribute specify that a separate type contains the contracts for this type.
    /// </summary>
    [Conditional("CONTRACTS_FULL")]
    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    public sealed class ContractClassAttribute : Attribute
    {
        private Type _typeWithContracts;

        public ContractClassAttribute(Type typeContainingContracts)
        {
            _typeWithContracts = typeContainingContracts;
        }

        public Type TypeContainingContracts
        {
            get { return _typeWithContracts; }
        }
    }
}
