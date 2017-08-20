using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;
using System.Linq;

// TODO: namespace and package name NetCore.Contracts or DotNet.Core.Contracts or Nvm.NetCore.Contracts or NetCore.System.Diagnostics.Contracts or System.Diagnostics.Contracts.Core?
namespace DotNet.Core.Contracts
{
	static class ContractInterfaceMetaData
	{
		public static readonly ISet<string> ContractMethods = new HashSet<string>(new [] { 
			"Requires", 
			"Ensures", 
			"EnsuresOnThrow", 
			"Invariant", 
			"Assert", 
			"Assume"
		});
	}

	class ContractRewriter : CSharpSyntaxRewriter
	{
		private SemanticModel semanticModel;
		private Type sourceType;
		private Type targetType;
		private readonly ExpressionSyntax targetTypeReference;

		public ContractRewriter(SemanticModel semanticModel, Type sourceType, Type targetType)
		{
			this.semanticModel = semanticModel;
			this.sourceType = sourceType;
			this.targetType = targetType;
			this.targetTypeReference = SyntaxFactory.ParseExpression(targetType.FullName);
		}

		private bool InvokesSourceTypeMember(InvocationExpressionSyntax node)
		{
			var invocationSymbol = semanticModel.GetSymbolInfo(node).Symbol;
			if (invocationSymbol == null)
				return false;
			var containingType = invocationSymbol.ContainingType;
			var containingTypeName = containingType.ToDisplayString();
			return containingTypeName == sourceType.FullName;
		}

		public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			return InvokesSourceTypeMember(node) ? Rewrite(node) : node;
		}

		private SyntaxNode Rewrite(InvocationExpressionSyntax node)
		{
			if (!(node.Expression is MemberAccessExpressionSyntax))
				return node; //TODO: ?
			var oldNode = ((MemberAccessExpressionSyntax)node.Expression).Expression;
			var arguments = node.ArgumentList;
			var conditionText = arguments.Arguments.First().GetText().ToString();
			var conditionTextArgument = SyntaxFactory.Argument(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal(conditionText)));
			return node
				.ReplaceNode(oldNode, targetTypeReference)
				.WithArgumentList(arguments.AddArguments(conditionTextArgument))
				.WithTriviaFrom(node);
		}
	}
}