// Source: https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Diagnostics/Contracts/Contracts.cs

using System.Diagnostics.CodeAnalysis;

namespace System.Diagnostics.Contracts
{
    /// <summary>
    /// Methods and classes marked with this attribute can be used within calls to Contract methods. Such methods not make any visible state changes.
    /// </summary>
    [Conditional("CONTRACTS_FULL")]
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Delegate | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class PureAttribute : Attribute
    {
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
    /// This attribute is used to mark a method as being the invariant
    /// method for a class. The method can have any name, but it must
    /// return "void" and take no parameters. The body of the method
    /// must consist solely of one or more calls to the method
    /// Contract.Invariant. A suggested name for the method is 
    /// "ObjectInvariant".
    /// </summary>
    [Conditional("CONTRACTS_FULL")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ContractInvariantMethodAttribute : Attribute
    {
    }

    /// <summary>
    /// Attribute that specifies that an assembly is a reference assembly with contracts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class ContractReferenceAssemblyAttribute : Attribute
    {
    }

    /// <summary>
    /// Methods (and properties) marked with this attribute can be used within calls to Contract methods, but have no runtime behavior associated with them.
    /// </summary>
    [Conditional("CONTRACTS_FULL")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ContractRuntimeIgnoredAttribute : Attribute
    {
    }

    /// <summary>
    /// Instructs downstream tools whether to assume the correctness of this assembly, type or member without performing any verification or not.
    /// Can use [ContractVerification(false)] to explicitly mark assembly, type or member as one to *not* have verification performed on it.
    /// Most specific element found (member, type, then assembly) takes precedence.
    /// (That is useful if downstream tools allow a user to decide which polarity is the default, unmarked case.)
    /// </summary>
    /// <remarks>
    /// Apply this attribute to a type to apply to all members of the type, including nested types.
    /// Apply this attribute to an assembly to apply to all types and members of the assembly.
    /// Apply this attribute to a property to apply to both the getter and setter.
    /// </remarks>
    [Conditional("CONTRACTS_FULL")]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property)]
    public sealed class ContractVerificationAttribute : Attribute
    {
        private bool _value;

        public ContractVerificationAttribute(bool value) { _value = value; }

        public bool Value
        {
            get { return _value; }
        }
    }

    /// <summary>
    /// Allows a field f to be used in the method contracts for a method m when f has less visibility than m.
    /// For instance, if the method is public, but the field is private.
    /// </summary>
    [Conditional("CONTRACTS_FULL")]
    [AttributeUsage(AttributeTargets.Field)]
    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "Thank you very much, but we like the names we've defined for the accessors")]
    public sealed class ContractPublicPropertyNameAttribute : Attribute
    {
        private String _publicName;

        public ContractPublicPropertyNameAttribute(String name)
        {
            _publicName = name;
        }

        public String Name
        {
            get { return _publicName; }
        }
    }

    /// <summary>
    /// Enables factoring legacy if-then-throw into separate methods for reuse and full control over
    /// thrown exception and arguments
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [Conditional("CONTRACTS_FULL")]
    public sealed class ContractArgumentValidatorAttribute : Attribute
    {
    }

    /// <summary>
    /// Enables writing abbreviations for contracts that get copied to other methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [Conditional("CONTRACTS_FULL")]
    public sealed class ContractAbbreviatorAttribute : Attribute
    {
    }

    /// <summary>
    /// Allows setting contract and tool options at assembly, type, or method granularity.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    [Conditional("CONTRACTS_FULL")]
    public sealed class ContractOptionAttribute : Attribute
    {
        private String _category;
        private String _setting;
        private bool _enabled;
        private String _value;

        public ContractOptionAttribute(String category, String setting, bool enabled)
        {
            _category = category;
            _setting = setting;
            _enabled = enabled;
        }

        public ContractOptionAttribute(String category, String setting, String value)
        {
            _category = category;
            _setting = setting;
            _value = value;
        }

        public String Category
        {
            get { return _category; }
        }

        public String Setting
        {
            get { return _setting; }
        }

        public bool Enabled
        {
            get { return _enabled; }
        }

        public String Value
        {
            get { return _value; }
        }
    }
}