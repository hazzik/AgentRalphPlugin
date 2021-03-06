﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3660 $</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class QueryExpressionTests
	{
		[Test]
		public void SimpleExpression()
		{
			QueryExpression qe = ParseUtilCSharp.ParseExpression<QueryExpression>(
				"from c in customers where c.City == \"London\" select c"
			);
			Assert.AreEqual("c", qe.FromClause.Identifier);
			Assert.AreEqual("customers", ((IdentifierExpression)qe.FromClause.InExpression).Identifier);
			Assert.AreEqual(1, qe.MiddleClauses.Count);
			Assert.IsInstanceOf<QueryExpressionWhereClause>(qe.MiddleClauses[0]);
			QueryExpressionWhereClause wc = (QueryExpressionWhereClause)qe.MiddleClauses[0];
			Assert.IsInstanceOf<BinaryOperatorExpression>(wc.Condition);
			Assert.IsInstanceOf<QueryExpressionSelectClause>(qe.SelectOrGroupClause);
		}
		
		[Test]
		public void ExpressionWithType1()
		{
			QueryExpression qe = ParseUtilCSharp.ParseExpression<QueryExpression>(
				"from Customer c in customers select c"
			);
			Assert.AreEqual("c", qe.FromClause.Identifier);
			Assert.AreEqual("Customer", qe.FromClause.Type.ToString());
			Assert.AreEqual("customers", ((IdentifierExpression)qe.FromClause.InExpression).Identifier);
			Assert.IsInstanceOf<QueryExpressionSelectClause>(qe.SelectOrGroupClause);
		}
		
		[Test]
		public void ExpressionWithType2()
		{
			QueryExpression qe = ParseUtilCSharp.ParseExpression<QueryExpression>(
				"from int c in customers select c"
			);
			Assert.AreEqual("c", qe.FromClause.Identifier);
			Assert.AreEqual("System.Int32", qe.FromClause.Type.Type);
			Assert.AreEqual("customers", ((IdentifierExpression)qe.FromClause.InExpression).Identifier);
			Assert.IsInstanceOf<QueryExpressionSelectClause>(qe.SelectOrGroupClause);
		}
		
		
		[Test]
		public void ExpressionWithType3()
		{
			QueryExpression qe = ParseUtilCSharp.ParseExpression<QueryExpression>(
				"from S<int[]>? c in customers select c"
			);
			Assert.AreEqual("c", qe.FromClause.Identifier);
			Assert.AreEqual("System.Nullable<S<System.Int32[]>>", qe.FromClause.Type.ToString());
			Assert.AreEqual("customers", ((IdentifierExpression)qe.FromClause.InExpression).Identifier);
			Assert.IsInstanceOf<QueryExpressionSelectClause>(qe.SelectOrGroupClause);
		}
		
		[Test]
		public void MultipleGenerators()
		{
			QueryExpression qe = ParseUtilCSharp.ParseExpression<QueryExpression>(@"
from c in customers
where c.City == ""London""
from o in c.Orders
where o.OrderDate.Year == 2005
select new { c.Name, o.OrderID, o.Total }");
			Assert.AreEqual(3, qe.MiddleClauses.Count);
			Assert.IsInstanceOf<QueryExpressionWhereClause>(qe.MiddleClauses[0]);
			Assert.IsInstanceOf<QueryExpressionFromClause>(qe.MiddleClauses[1]);
			Assert.IsInstanceOf<QueryExpressionWhereClause>(qe.MiddleClauses[2]);
			
			Assert.IsInstanceOf<QueryExpressionSelectClause>(qe.SelectOrGroupClause);
		}
		
		[Test]
		public void ExpressionWithOrderBy()
		{
			QueryExpression qe = ParseUtilCSharp.ParseExpression<QueryExpression>(
				"from c in customers orderby c.Name select c"
			);
			Assert.AreEqual("c", qe.FromClause.Identifier);
			Assert.AreEqual("customers", ((IdentifierExpression)qe.FromClause.InExpression).Identifier);
			Assert.IsInstanceOf<QueryExpressionOrderClause>(qe.MiddleClauses[0]);
			Assert.IsInstanceOf<QueryExpressionSelectClause>(qe.SelectOrGroupClause);
		}
		
		[Test]
		public void ExpressionWithOrderByAndLet()
		{
			QueryExpression qe = ParseUtilCSharp.ParseExpression<QueryExpression>(
				"from c in customers orderby c.Name let x = c select x"
			);
			Assert.AreEqual("c", qe.FromClause.Identifier);
			Assert.AreEqual("customers", ((IdentifierExpression)qe.FromClause.InExpression).Identifier);
			Assert.IsInstanceOf<QueryExpressionOrderClause>(qe.MiddleClauses[0]);
			Assert.IsInstanceOf<QueryExpressionLetClause>(qe.MiddleClauses[1]);
			Assert.IsInstanceOf<QueryExpressionSelectClause>(qe.SelectOrGroupClause);
		}
	}
}
