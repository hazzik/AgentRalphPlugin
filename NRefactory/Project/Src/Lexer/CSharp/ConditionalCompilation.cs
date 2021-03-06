// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;

namespace ICSharpCode.NRefactory.Parser.CSharp
{
	sealed class ConditionalCompilation : AbstractAstVisitor
	{
		static readonly object SymbolDefined = new object();
		Dictionary<string, object> symbols = new Dictionary<string, object>();
		
		public IDictionary<string, object> Symbols { 
			get { return symbols; }
		}
		
		public void Define(string symbol)
		{
			symbols[symbol] = SymbolDefined;
		}
		
		public void Undefine(string symbol)
		{
			symbols.Remove(symbol);
		}
		
		public bool Evaluate(Expression condition)
		{
			return condition.AcceptVisitor(this, null) == SymbolDefined;
		}
		
		public override object VisitPrimitiveExpression(PrimitiveExpression pe, object data)
		{
			if (pe.Value is bool)
				return (bool)pe.Value ? SymbolDefined : null;
			else
				return null;
		}
		
		public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
		{
			return symbols.ContainsKey(identifierExpression.Identifier) ? SymbolDefined : null;
		}
		
		public override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			if (unaryOperatorExpression.Op == UnaryOperatorType.Not) {
				return unaryOperatorExpression.Expression.AcceptVisitor(this, data) == SymbolDefined ? null : SymbolDefined;
			} else {
				return null;
			}
		}
		
		public override object VisitBinaryOperatorExpression(BinaryOperatorExpression bo, object data)
		{
			bool lhs = bo.Left.AcceptVisitor(this, data) == SymbolDefined;
			bool rhs = bo.Right.AcceptVisitor(this, data) == SymbolDefined;
			bool result;
			switch (bo.Op) {
				case BinaryOperatorType.LogicalAnd:
					result = lhs && rhs;
					break;
				case BinaryOperatorType.LogicalOr:
					result = lhs || rhs;
					break;
				case BinaryOperatorType.Equality:
					result = lhs == rhs;
					break;
				case BinaryOperatorType.InEquality:
					result = lhs != rhs;
					break;
				default:
					return null;
			}
			return result ? SymbolDefined : null;
		}
		
		public override object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
		{
			return parenthesizedExpression.Expression.AcceptVisitor(this, data);
		}
	}
}
