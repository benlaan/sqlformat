using System;
using System.Linq;

using NUnit.Framework;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Parsers;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestExpressionParser
    {
        protected ITokenizer NewTokenizer( string input )
        {
            return new SqlTokenizer( input );
        }

        [Test]
        public void Expression_Reads_Multi_Part_Identifier()
        {
            // setup
            var tokenizer = NewTokenizer( " Database.Owner.Table " );
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
        public void Expression_Reads_Multi_Part_Identifier_With_Square_Brackets()
        {
            // setup
            var tokenizer = NewTokenizer( " Database.[Owner].Table " );
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
        public void Expression_Reads_Multi_Part_Identifier_With_Square_Brackets_Around_Two_Part_Identifier()
        {
            // setup
            var tokenizer = NewTokenizer( " Database.[Some Owner].Table " );
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
        public void Expression_Reads_Quoted_String_Around_Two_Part_Identifier()
        {
            // setup
            var tokenizer = NewTokenizer( " 'Some Owner' " );
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
        [TestCase( "''", "''" )]
        [TestCase( " '' ", "''" )]
        [TestCase( " ' ' ", "' '" )]
        [TestCase( "' '", "' '" )]
        public void Expression_Reads_Empty_Quoted_String( string input, string output )
        {
            // setup
            var tokenizer = NewTokenizer( input );
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
        public void Expression_Reads_Function_Expression_Without_Params()
        {
            // setup
            var tokenizer = NewTokenizer( "SomeFunction()" );
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
        public void Expression_Reads_Function_Expression_With_Multiple_Params()
        {
            // setup
            var tokenizer = NewTokenizer( "Max(120, A)" );
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
        [TestCase( "+" ), TestCase( "-" ), TestCase( "*" ), TestCase( "/" ), TestCase( "%" ), TestCase( "^" )]
        public void Expression_With_Add_Operator( string op )
        {
            // setup
            var tokenizer = NewTokenizer( String.Format( "A.Field1 {0} 120", op ) );
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
        public void Expression_With_Multiple_Operators_With_Addition_First()
        {
            // setup
            var tokenizer = NewTokenizer( "A.Field1 + 120 * 50" );
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
        public void Expression_With_Multiple_Operators_With_Multiplication_First()
        {
            // setup
            var tokenizer = NewTokenizer( "A.Field1 * 120 + 50" );
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
        public void Expression_With_Multiple_Operators_With_Brackets_Around_Addition()
        {
            // setup
            var tokenizer = NewTokenizer( "(A.Field1 + 120) * 50" );
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
        public void Expression_With_Nested_Select_Statement()
        {
            // setup
            var tokenizer = NewTokenizer( "( SELECT A.Field1 FROM Table ) * ( 50 + ( 20 * F.ID ) )" );
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
        [TestCase( "=" ), TestCase( "<" ), TestCase( ">" ), TestCase( "<=" ), TestCase( ">=" ), TestCase( "IS" ), TestCase( "LIKE" )]
        public void Simple_Criteria_Expression( string op )
        {
            // setup
            var tokenizer = NewTokenizer( String.Format( "A.Field1 {0} B.Field2", op ) );
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
        public void Nested_Criteria_Expression()
        {
            // setup
            var tokenizer = NewTokenizer( "A.Field1 = (2 + B.Field2)" );
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
        public void Can_Read_Case_Switch_Expression()
        {
            // setup
            var tokenizer = NewTokenizer( "CASE A.Field1 WHEN 1 THEN 'Y' WHEN 2 THEN 'N' ELSE 'U' END" );
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
        public void Can_Read_Case_Expression_With_Nested_Case_Expression()
        {
            // setup
            var tokenizer = NewTokenizer( @"
                    CASE A.Field1 
                        WHEN 1 THEN 
                            CASE A.Field2 
                                WHEN 1 THEN 'Y'
                            ELSE 
                                'U' 
                            END 
                        WHEN 2 THEN 'N' 
                    ELSE 
                        'U' 
                    END" );
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
            Assert.IsTrue( caseSwitch.Cases[ 0 ].Then is CaseExpression );

            Assert.AreEqual( "2", caseSwitch.Cases[ 1 ].When.Value );
            Assert.AreEqual( "'N'", caseSwitch.Cases[ 1 ].Then.Value );

            Assert.AreEqual( "'U'", caseSwitch.Else.Value );
        }

        [Test]
        public void Can_Read_Case_When_Expression()
        {
            // setup
            var tokenizer = NewTokenizer( @"

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

        [Test]
        public void Can_Read_Negated_Expression()
        {
            var tokenizer = NewTokenizer( "NOT ( A.Field = 1 OR A.Other IS NULL )" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is NegationExpression );
            
            NegationExpression negationExpression = (NegationExpression) expression;
            Expression criteria = negationExpression.Expression;
            Assert.IsTrue( criteria is NestedExpression );

            NestedExpression nestedExpression = (NestedExpression) criteria;
            Assert.IsTrue( nestedExpression.Expression is CriteriaExpression );
            
            CriteriaExpression criteriaExpression = (CriteriaExpression) nestedExpression.Expression;
            Assert.AreEqual( "A.Field = 1", criteriaExpression.Left.Value );
            Assert.AreEqual( "OR", criteriaExpression.Operator );
            Assert.AreEqual( "A.Other IS NULL", criteriaExpression.Right.Value );
        }

        [Test]
        public void Can_Read_Negated_Expression_Without_Brackets()
        {
            var tokenizer = NewTokenizer( "NOT EXISTS(SELECT 1 FROM dbo.Table)" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is NegationExpression );

            NegationExpression negationExpression = (NegationExpression) expression;
            Expression criteria = negationExpression.Expression;
            Assert.IsTrue( criteria is FunctionExpression );
            FunctionExpression functionExpression = (FunctionExpression) criteria;
            Assert.AreEqual( 1, functionExpression.Arguments.Count );

            Expression arg = functionExpression.Arguments.First();
            Assert.IsTrue( arg is SelectExpression );
        }

        [Test]
        public void Can_Read_Not_Null_Expression()
        {
            var tokenizer = NewTokenizer( "A.Field IS NOT NULL" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is CriteriaExpression );
            CriteriaExpression criteria = (CriteriaExpression) expression;
            Assert.AreEqual( "A.Field", criteria.Left.Value );
            Assert.AreEqual( "IS", criteria.Operator );
            Assert.AreEqual( "NOT NULL", criteria.Right.Value );
        }

        [Test]
        public void Can_Read_Between_Expression_With_Identifiers()
        {
            var tokenizer = NewTokenizer( "A.Field BETWEEN 10 AND 20" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is BetweenExpression );

            BetweenExpression betweenExpression = (BetweenExpression) expression;
            Expression expr = betweenExpression.Expression;
            Assert.IsTrue( expr is IdentifierExpression );
            Assert.AreEqual( "A.Field", expr.Value );

            Expression from = betweenExpression.From;
            Assert.IsTrue( from is IdentifierExpression );
            Assert.AreEqual( "10", from.Value );

            Expression to = betweenExpression.To;
            Assert.IsTrue( to is IdentifierExpression );
            Assert.AreEqual( "20", to.Value );
        }

        [Test]
        public void Can_Read_Natural_Negated_In_Expression()
        {
            var tokenizer = NewTokenizer( "A.Field NOT IN (1, 2, 3)" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();
            Assert.IsTrue( expression is CriteriaExpression );
            CriteriaExpression criteria = (CriteriaExpression) expression;

            Assert.AreEqual( "A.Field", criteria.Left.Value );
            Assert.AreEqual( "NOT IN", criteria.Operator );
            Assert.AreEqual( "(1, 2, 3)", criteria.Right.Value );
        }

        [Test]
        public void Can_Read_Natural_Negated_Between_Expression()
        {
            var tokenizer = NewTokenizer( "A.Field NOT BETWEEN 10 AND 20" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            Assert.IsNotNull( expression );
            Assert.IsTrue( expression is BetweenExpression );

            BetweenExpression betweenExpression = (BetweenExpression) expression;
            Expression expr = betweenExpression.Expression;
            Assert.IsTrue( expr is IdentifierExpression );
            Assert.AreEqual( "A.Field", expr.Value );

            Expression from = betweenExpression.From;
            Assert.IsTrue( from is IdentifierExpression );
            Assert.AreEqual( "10", from.Value );

            Expression to = betweenExpression.To;
            Assert.IsTrue( to is IdentifierExpression );
            Assert.AreEqual( "20", to.Value );
        }
    }
}
