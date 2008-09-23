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
        }
    }
}
