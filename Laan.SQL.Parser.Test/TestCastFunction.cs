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
            Assert.IsNotNull( expression );

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
            Assert.IsNotNull( expression );

            Assert.AreEqual( 1, function.Arguments.Count );
            Assert.AreEqual( "CAST", function.Name );
            Assert.AreEqual( "@Age", function.Arguments.First().Value );
            Assert.AreEqual( "VARCHAR(50)", function.OutputType.ToString() );
            Assert.AreEqual( "CAST(@Age AS VARCHAR(50))", function.Value );
        }
    }
}
