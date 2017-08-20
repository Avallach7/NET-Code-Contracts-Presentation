using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.FindSymbols;
using System.IO;
using System.Reflection;

namespace DotNet.Core.Contracts
{
    class Program
    {
		static readonly string exampleCode = @"
			using System.Diagnostics.Contracts;
			public class Program
			{
				static void Main(string[] args)
				{
					System.Diagnostics.Contracts.Contract.Requires(args.Length > 1);
					Contract.Requires(args.Length > 2);
					Console.WriteLine();
				}
			}";
        static void Main(string[] args)
        {
			var syntaxTree = CSharpSyntaxTree.ParseText(exampleCode);
			var syntaxTrees = new SyntaxTree[] { syntaxTree };
			var contractsAssembly = MetadataReference.CreateFromFile(typeof(Contract).GetTypeInfo().Assembly.Location);
			var compilation = CSharpCompilation.Create("DotNet.Core.Contracts", syntaxTrees, new[] { contractsAssembly });
			var semanticModel = compilation.GetSemanticModel(syntaxTree);
			var rewriter = new ContractRewriter(
				semanticModel, 
				typeof(Contract),
				typeof(ContractsRuntime));
			var result = rewriter.Visit(syntaxTree.GetRoot());
			Console.WriteLine(result.ToFullString());
        }
    }
}
