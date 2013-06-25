using System;
using System.Linq;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Parsers;

using NUnit.Framework;

namespace Laan.Sql.Parser.Test
{
    
    [TestFixture]
    public class TestCastFunction : TestExpressionParser
    {
        [Test]
        public void CanCastInt()
        {
            // setup
            var tokenizer = NewTokenizer( "CAST (@Age AS INT )" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            var function = expression as CastExpression;
            Assert.IsNotNull( function );

            Assert.AreEqual( 1, function.Arguments.Count );
            Assert.AreEqual( "CAST", function.Name );
            Assert.AreEqual( "@Age", function.Arguments.First().Value );
            Assert.AreEqual( "INT", function.OutputType.ToString() );
            Assert.AreEqual( "CAST(@Age AS INT)", function.Value );
        }

        [Test]
        public void CanCastVarchar()
        {
            // setup
            var tokenizer = NewTokenizer( "CAST (@Age AS VARCHAR(50) )" );
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser( tokenizer );

            // exercise
            Expression expression = parser.Execute();

            // verify
            var function = expression as CastExpression;
            Assert.IsNotNull( function );

            Assert.AreEqual( 1, function.Arguments.Count );
            Assert.AreEqual( "CAST", function.Name );
            Assert.AreEqual( "@Age", function.Arguments.First().Value );
            Assert.AreEqual( "VARCHAR(50)", function.OutputType.ToString() );
            Assert.AreEqual( "CAST(@Age AS VARCHAR(50))", function.Value );
        }

        [Test]
        public void LowerCaseCast()
        {
            // setup
            var tokenizer = NewTokenizer("cast(@Age as varchar(50) )");
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser(tokenizer);

            // exercise
            Expression expression = parser.Execute();

            // verify
            var function = expression as CastExpression;
            Assert.IsNotNull(function);

            Assert.AreEqual(1, function.Arguments.Count);
            Assert.AreEqual("CAST", function.Name);
            Assert.AreEqual("@Age", function.Arguments.First().Value);
            Assert.AreEqual("varchar(50)", function.OutputType.ToString());
            Assert.AreEqual("CAST(@Age AS varchar(50))", function.Value);
        }

        [Test]
        public void ExpressionWithLowerCast()
        {
            // setup
            var tokenizer = NewTokenizer("CASE WHEN CostCenter > 0 THEN  cast(CostCenter as varchar(10)) ELSE CostCenterDesc END ");
            tokenizer.ReadNextToken();

            ExpressionParser parser = new ExpressionParser(tokenizer);

            // exercise
            Expression expression = parser.Execute();

            // verify
            var function = expression as CaseWhenExpression;
            Assert.IsNotNull(function, "not a case statement");

            var cast = function.Cases[0].Then as CastExpression;
            Assert.IsNotNull(cast, "first case is not a cast");

            Assert.AreEqual("CAST", cast.Name);
            Assert.AreEqual("CostCenter", cast.Arguments.First().Value);
            Assert.AreEqual("varchar(10)", cast.OutputType.ToString());
            Assert.AreEqual("CAST(CostCenter AS varchar(10))", cast.Value);
        }
        // TODO: test lower-case cast, lower-case cast inside another expression (such as CASE)
    }
}
