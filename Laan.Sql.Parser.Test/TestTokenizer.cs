﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NUnit.Framework;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestTokenizer
    {
        private static void Verify(string input, string[] tokens, bool skipComments = true)
        {
            var tokenizer = new SqlTokenizer(input);
            tokenizer.SkipComments = skipComments;

            foreach (string token in tokens)
            {
                Assert.IsTrue(tokenizer.HasMoreTokens);

                tokenizer.ReadNextToken();

                Assert.AreEqual(token, tokenizer.Current.Value);
            }
            tokenizer.ReadNextToken();
            Assert.IsFalse(tokenizer.HasMoreTokens);
        }

        [Test]
        [TestCase("", new string[] { })]
        public void Ensure_Empty_String_Returns_No_Tokens(string input, string[] tokens)
        {
            Verify(input, tokens);
        }

        [Test]
        [TestCase("Hello", new[] { "Hello" })]
        [TestCase("Hello World", new[] { "Hello", "World" })]
        public void Tokenize_Alpha_Strings(string input, string[] tokens)
        {
            Verify(input, tokens);
        }

        [Test]
        [TestCase("SELECT @var, 1, @a12, :hello123", new[] { "SELECT", "@var", ",", "1", ",", "@a12", ",", ":hello123" })]
        [TestCase("SELECT @@ROWCOUNT", new[] { "SELECT", "@@ROWCOUNT" })]
        [TestCase("@ROW @COUNT", new[] { "@ROW", "@COUNT" })]
        public void Can_Tokenize_Variables(string input, string[] tokens)
        {
            Verify(input, tokens);
        }

        [Test]
        [TestCase("1234", new[] { "1234" })]
        [TestCase("1234 9876", new[] { "1234", "9876" })]
        [TestCase("12349876 123312 12312", new[] { "12349876", "123312", "12312" })]
        public void Tokenize_Numeric_Strings(string input, string[] tokens)
        {
            Verify(input, tokens);
        }

        [Test]
        [TestCase("Ben 1234", new[] { "Ben", "1234" })]
        [TestCase("Ben1234 Laan9876 12Hello 34World", new[] { "Ben1234", "Laan9876", "12", "Hello", "34", "World" })]
        public void Tokenize_Alpha_Numeric_Strings(string input, string[] tokens)
        {
            Verify(input, tokens);
        }

        [Test]
        [TestCase("Ben 1234", new[] { "Ben", "1234" })]
        [TestCase("Ben1234 Laan9876 12Hello 34World", new[] { "Ben1234", "Laan9876", "12", "Hello", "34", "World" })]
        [TestCase(@"Ben(Test1234)", new[] { "Ben", "(", "Test1234", ")" })]
        [TestCase(@"""12"" [Hello, 'World']", new[] { @"""12""", "[Hello, 'World']" })]
        [TestCase(@"""A"" [Hello, World]", new[] { @"""A""", "[Hello, World]" })]
        [TestCase(@"[Hello, 'World']", new[] { "[Hello, 'World']" })]
        [TestCase("A = 1", new[] { "A", "=", "1" })]
        [TestCase("2 <= 5", new[] { "2", "<=", "5" })]
        [TestCase("A != B", new[] { "A", "!=", "B" })]
        [TestCase("@Ben_Laan", new[] { "@Ben_Laan" })]
        [TestCase("Database.[Some Owner].Table", new[] { "Database", ".", "[Some Owner]", ".", "Table" })]
        [TestCase("Database.[日本].Table", new[] { "Database", ".", "[日本]", ".", "Table" })]
        [TestCase("#table ##global", new[] { "#table", "##global" })]
        public void Tokenize_Alpha_Numeric_And_Special_Strings(string input, string[] tokens)
        {
            Verify(input, tokens);
        }

        [Test]
        [TestCase("''", new[] { "''" })]
        [TestCase("'Hello World'", new[] { "'Hello World'" })]
        [TestCase("'A&R'", new[] { "'A&R'" })]
        [TestCase("'!@#$%^&*('", new[] { "'!@#$%^&*('" })]
        [TestCase("'☀☂☭'", new[] { "'☀☂☭'" })]
        [TestCase("'日本の漢字'", new[] { "'日本の漢字'" })]
        [TestCase("'中国字符'", new[] { "'中国字符'" })]
        [TestCase("CASE A WHEN 'N' THEN 1 WHEN 'Y' THEN 2 END", new[] { "CASE", "A", "WHEN", "'N'", "THEN", "1", "WHEN", "'Y'", "THEN", "2", "END" })]
        public void Can_Tokenize_Strings_As_One_Token(string input, string[] tokens)
        {
            Verify(input, tokens);
        }

        [Test]
        [TestCase("N'XX'", new[] { "N'XX'" })]
        [TestCase("N'Hello World'", new[] { "N'Hello World'" })]
        [TestCase("N'A&R'", new[] { "N'A&R'" })]
        [TestCase("N'!@#$%^&*('", new[] { "N'!@#$%^&*('" })]
        [TestCase("N'☀☂☭'", new[] { "N'☀☂☭'" })]
        [TestCase("N'日本の漢字'", new[] { "N'日本の漢字'" })]
        [TestCase("N'中国字符'", new[] { "N'中国字符'" })]
        [TestCase("'N'", new[] { "'N'" })]
        public void Can_Tokenize_Strings_With_N_Prefix(string input, string[] tokens)
        {
            Verify(input, tokens);
        }

        [Test]
        [TestCase("N'1\r\n2\r\n'", new[] { "N'1\r\n2\r\n'" })]
        public void Can_Tokenize_MultiLine_Strings_With_N_Prefix(string input, string[] tokens)
        {
            Verify(input, tokens);
        }

        [Test]
        [TestCase("[PK_Test] primary key clustered ( [id] )", new[] { "[PK_Test]", "primary", "key", "clustered", "(", "[id]", ")" })]
        public void Can_Tokenize_Block_Test(string input, string[] tokens)
        {
            Verify(input, tokens);
        }

        [Test]
        [TestCase("'WHERE Name = ''Laan'''", new[] { "'WHERE Name = '", "'Laan'", "''" })]
        [TestCase("'''The Coder'''", new[] { "''", "'The Coder'", "''" })]
        [TestCase("N'Ben ''The Coder'' Laan'", new[] { "N'Ben '", "'The Coder'", "' Laan'" })]
        public void Can_Tokenize_Strings_With_Escaped_Inner_Quotes(string input, string[] tokens)
        {
            Verify(input, tokens);
        }

        [Test]
        [TestCase("         CREATE TABLE", new[] { "CREATE", "TABLE" })]
        [TestCase("         \n\rALTER TABLE", new[] { "ALTER", "TABLE" })]
        public void Tokenize_With_Preceding_Leading_Whitespace(string input, string[] tokens)
        {
            Verify(input, tokens);
        }

        [TestCase("ID < 10;", new[] { "ID", "<", "10", ";" })]
        [TestCase("ID <= 10", new[] { "ID", "<=", "10" })]
        [TestCase("ID > 10;", new[] { "ID", ">", "10", ";" })]
        [TestCase("ID >= 10", new[] { "ID", ">=", "10" })]
        [TestCase("ID <> 10", new[] { "ID", "<>", "10" })]
        [TestCase("ID * 10", new[] { "ID", "*", "10" })]
        [TestCase("ID / 10", new[] { "ID", "/", "10" })]
        public void Tokenize_Comparator_Operators(string input, string[] tokens)
        {
            Verify(input, tokens);
        }

        [Test]
        [TestCase( "SELECT * -- Get All Fields", new[] { "SELECT", "*", "-- Get All Fields" } )]
        [TestCase( "SELECT * -- Get All Fields\r\nFROM dbo.Table", new[] { "SELECT", "*", "-- Get All Fields", "FROM", "dbo", ".", "Table" } )]
        public void Can_Tokenize_Strings_With_An_Inline_Comment(string input, string[] tokens)
        {
            // Hack: Change the Skip flag for comments so they can be tested..
            //       Eventually, when comments are processed correctly, this can be removed!
            Verify(input, tokens, false);
        }

        [Test]
        //[TestCase("/* * */", new[] { "/* * */" })]
        //[TestCase("/* / */", new[] { "/* / */" })]
        [TestCase("/* !@#$%^ */", new[] { "/* !@#$%^ */" })]
        [TestCase("/* A\r\nB */\r\nC", new[] { "/* A\r\nB */", "C" })]
        [TestCase("select * from /* dbo.table t */ dbo.otherTable t", new[] { "select", "*", "from", "/* dbo.table t */", "dbo", ".", "otherTable", "t" })]
        public void Can_Tokenize_Strings_With_Block_Comment_With_Symbols_In_Comments(string input, string[] tokens)
        {
            // Hack: Change the Skip flag for comments so they can be tested..
            //       Eventually, when comments are processed correctly, this can be removed!
            Verify(input, tokens, false);
        }

        [Test]
        [TestCase( "SELECT * /* Get All\r\nFields */\r\nFROM dbo.Table", new[] { "SELECT", "*", "/* Get All\r\nFields */", "FROM", "dbo", ".", "Table" } )]
        [TestCase("/* A\r\nB */\r\nC", new[] { "/* A\r\nB */", "C" })]
        [TestCase("select * from /* dbo.table t */ dbo.otherTable t", new[] { "select", "*", "from", "/* dbo.table t */", "dbo", ".", "otherTable", "t" })]
        public void Can_Tokenize_Strings_With_Block_Comment(string input, string[] tokens)
        {
            // Hack: Change the Skip flag for comments so they can be tested..
            //       Eventually, when comments are processed correctly, this can be removed!
            Verify(input, tokens, false);
        }
    }
}
