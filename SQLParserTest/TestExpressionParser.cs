using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MbUnit.Framework;
using Laan.SQL.Parser;

namespace Laan.SQLParser.Test
{
    [TestFixture]
    public class TestExpressionParser
    {
        [Test]
        public void Test_Expression_Reads_Multi_Part_Identifier()
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( " Database.Owner.Table " );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.AreEqual( "Database.Owner.Table", expression.Value );
            Assert.IsTrue( expression is IdentifierExpression );
        }

        [Test]
        public void Test_Expression_Reads_Multi_Part_Identifier_With_Square_Brackets()
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( " Database.[Owner].Table " );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.AreEqual( "Database.[Owner].Table", expression.Value );
            Assert.IsTrue( expression is IdentifierExpression );
        }

        [Test]
        public void Test_Expression_Reads_Multi_Part_Identifier_With_Square_Brackets_Around_Two_Part_Identifier()
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( " Database.[Some Owner].Table " );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.AreEqual( "Database.[Some Owner].Table", expression.Value );
            Assert.IsTrue( expression is IdentifierExpression );
        }

        [Test]
        public void Test_Expression_Reads_Quoted_String_Around_Two_Part_Identifier()
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( " 'Some Owner' " );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.AreEqual( "'Some Owner'", expression.Value );
            Assert.IsTrue( expression is StringExpression );
        }

        [Test]
        [Row( "''", "''" )]
        [Row( " '' ", "''" )]
        [Row( " ' ' ", "' '" )]
        [Row( "' '", "' '" )]
        public void Test_Expression_Reads_Empty_Quoted_String( string input, string output )
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( input );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.AreEqual( output, expression.Value );
            Assert.IsTrue( expression is StringExpression );
        }

        [Test]
        public void Test_Expression_Reads_Function_Expression_Without_Params()
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( "SomeFunction()" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            var function = (FunctionExpression) expression;

            Assert.AreEqual( 0, function.Arguments.Count );
            Assert.AreEqual( "SomeFunction", function.Name );
        }

        [Test]
        public void Test_Expression_Reads_Function_Expression_With_Multiple_Params()
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( "Max(120, A)" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is FunctionExpression );

            var function = (FunctionExpression) expression;

            Assert.AreEqual( 2, function.Arguments.Count );
            Assert.AreEqual( "Max", function.Name );
            Assert.AreEqual( "120", function.Arguments[ 0 ].Value );
            Assert.AreEqual( "A", function.Arguments[ 1 ].Value );
        }

        [Test]
        [Row( "+" ), Row( "-" ), Row( "*" ), Row( "/" ), Row( "%" ), Row( "^" )]
        public void Test_Expression_With_Add_Operator( string op )
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( String.Format( "A.Field1 {0} 120", op ) );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is OperatorExpression );

            var operatorExpression = (OperatorExpression) expression;

            Assert.AreEqual( op, operatorExpression.Operator );
            Assert.IsTrue( operatorExpression.Left is IdentifierExpression );
            Assert.IsTrue( operatorExpression.Right is IdentifierExpression );

            Assert.AreEqual( "A.Field1", operatorExpression.Left.Value );
            Assert.AreEqual( "120", operatorExpression.Right.Value );
        }

        [Test]
        public void Test_Expression_With_Multiple_Operators_With_Addition_First()
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( "A.Field1 + 120 * 50" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is OperatorExpression );

            var operatorExpression = (OperatorExpression) expression;

            Assert.AreEqual( "+", operatorExpression.Operator );
            Assert.IsTrue( operatorExpression.Left is IdentifierExpression );
            Assert.IsTrue( operatorExpression.Right is OperatorExpression );

            Assert.AreEqual( "A.Field1", operatorExpression.Left.Value );

            var rightExpression = (OperatorExpression) operatorExpression.Right;
            Assert.AreEqual( "120", rightExpression.Left.Value );
            Assert.AreEqual( "*", rightExpression.Operator );
            Assert.AreEqual( "50", rightExpression.Right.Value );
        }

        [Test]
        public void Test_Expression_With_Multiple_Operators_With_Multiplication_First()
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( "A.Field1 * 120 + 50" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is OperatorExpression );

            var operatorExpression = (OperatorExpression) expression;

            Assert.AreEqual( "*", operatorExpression.Operator );
            Assert.IsTrue( operatorExpression.Left is IdentifierExpression );
            Assert.IsTrue( operatorExpression.Right is OperatorExpression );

            Assert.AreEqual( "A.Field1", operatorExpression.Left.Value );

            var rightExpression = (OperatorExpression) operatorExpression.Right;
            Assert.AreEqual( "120", rightExpression.Left.Value );
            Assert.AreEqual( "+", rightExpression.Operator );
            Assert.AreEqual( "50", rightExpression.Right.Value );
        }

        [Test]
        public void Test_Expression_With_Multiple_Operators_With_Brackets_Around_Addition()
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( "(A.Field1 + 120) * 50" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is OperatorExpression );

            var operatorExpression = (OperatorExpression) expression;

            Assert.AreEqual( "*", operatorExpression.Operator );
            Assert.IsTrue( operatorExpression.Left is NestedExpression );
            Assert.IsTrue( operatorExpression.Right is IdentifierExpression );

            NestedExpression leftExpression = (NestedExpression) operatorExpression.Left;
            Assert.IsTrue( leftExpression.Expression is OperatorExpression );

            OperatorExpression leftOperatorExpression = (OperatorExpression) leftExpression.Expression;

            Assert.AreEqual( "A.Field1", leftOperatorExpression.Left.Value );
            Assert.AreEqual( "+", leftOperatorExpression.Operator );
            Assert.AreEqual( "120", leftOperatorExpression.Right.Value );

            Assert.AreEqual( "50", operatorExpression.Right.Value );
        }

        [Test]
        public void Test_Expression_With_Nested_Select_Statement()
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( "( SELECT A.Field1 FROM Table ) * ( 50 + ( 20 * F.ID ) )" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is OperatorExpression );
            var operatorExpression = (OperatorExpression) expression;

            Assert.AreEqual( "*", operatorExpression.Operator );
            Assert.IsTrue( operatorExpression.Left is NestedExpression );

            NestedExpression leftExpression = (NestedExpression) operatorExpression.Left;
            Assert.IsTrue( leftExpression.Expression is SelectExpression );

            SelectExpression leftOperatorExpression = (SelectExpression) leftExpression.Expression;

            Assert.AreEqual( "A.Field1", leftOperatorExpression.Statement.Fields[ 0 ].Expression.Value );
            Assert.AreEqual( "Table", leftOperatorExpression.Statement.From[ 0 ].Name );
        }

        [Test]
        [Row( "=" ), Row( "<" ), Row( ">" ), Row( "<=" ), Row( ">=" ), Row( "IS" )]
        public void Test_Simple_Criteria_Expression( string op )
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( String.Format( "A.Field1 {0} B.Field2", op ) );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is CriteriaExpression );
            CriteriaExpression criteria = (CriteriaExpression) expression;
            Assert.AreEqual( "A.Field1", criteria.Left.Value );
            Assert.AreEqual( op, criteria.Operator );
            Assert.AreEqual( "B.Field2", criteria.Right.Value );
        }

        [Test]
        public void Test_Nested_Criteria_Expression()
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( "A.Field1 = (2 + B.Field2)" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is CriteriaExpression );
            CriteriaExpression criteria = (CriteriaExpression) expression;

            Assert.AreEqual( "A.Field1", criteria.Left.Value );
            Assert.AreEqual( "=", criteria.Operator );

            Assert.IsTrue( criteria.Right is NestedExpression );
            NestedExpression nestedCriteria = (NestedExpression) criteria.Right;

            Assert.IsTrue( nestedCriteria.Expression is OperatorExpression );
            OperatorExpression operatorExpression = (OperatorExpression) nestedCriteria.Expression;

            Assert.AreEqual( "2", operatorExpression.Left.Value );
            Assert.AreEqual( "B.Field2", operatorExpression.Right.Value );
        }

        [Test]
        public void Test_Can_Read_Case_Switch_Expression()
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( "CASE A.Field1 WHEN 1 THEN 'Y' WHEN 2 THEN 'N' ELSE 'U' END" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is CaseSwitchExpression );
            CaseSwitchExpression caseSwitch = (CaseSwitchExpression) expression;

            Assert.AreEqual( "A.Field1", caseSwitch.Switch.Value );
            Assert.AreEqual( 2, caseSwitch.Cases.Count );

            Assert.AreEqual( "1", caseSwitch.Cases[ 0 ].When.Value );
            Assert.AreEqual( "'Y'", caseSwitch.Cases[ 0 ].Then.Value );

            Assert.AreEqual( "2", caseSwitch.Cases[ 1 ].When.Value );
            Assert.AreEqual( "'N'", caseSwitch.Cases[ 1 ].Then.Value );

            Assert.AreEqual( "'U'", caseSwitch.Else.Value );
        }

        [Test]
        public void Test_Can_Read_Case_When_Expression()
        {
            // setup
            Tokenizer tokenizer = new Tokenizer( @"

                CASE 
                    WHEN A.Field1 < 1 THEN 'Y' 
                    WHEN A.Field2 >= 2 + A.Field3 THEN 15 / (A.Field4 * 2)
                ELSE 
                    'U' 
                END
                "
            );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is CaseWhenExpression );
            CaseWhenExpression caseWhen = (CaseWhenExpression) expression;

            Assert.AreEqual( 2, caseWhen.Cases.Count );

            Assert.AreEqual( "A.Field1 < 1", caseWhen.Cases[ 0 ].When.Value );
            Assert.AreEqual( "'Y'", caseWhen.Cases[ 0 ].Then.Value );

            Assert.AreEqual( "A.Field2 >= 2 + A.Field3", caseWhen.Cases[ 1 ].When.Value );
            Assert.AreEqual( "15 / (A.Field4 * 2)", caseWhen.Cases[ 1 ].Then.Value );

            Assert.AreEqual( "'U'", caseWhen.Else.Value );
        }
    }
}
