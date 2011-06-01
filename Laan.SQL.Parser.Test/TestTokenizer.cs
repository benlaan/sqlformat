using System;
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
        private static ITokenizer GetTokenizer( string input )
        {
            return new SqlTokenizer( input );
        }

        private static void Verify( string input, string[] tokens )
        {
            var tokenizer = GetTokenizer( input );

            foreach ( string token in tokens )
            {
                Assert.IsTrue( tokenizer.HasMoreTokens );
                tokenizer.ReadNextToken();

                Assert.AreEqual( token, tokenizer.Current.Value );
            }
            tokenizer.ReadNextToken();
            Assert.IsFalse( tokenizer.HasMoreTokens );
        }

        [Test]
        [TestCase( "", new string[] {}  )]
        public void Ensure_Empty_String_Returns_No_Tokens( string input, string[] tokens )
        {
            Verify( input, tokens );
        }

        [Test]
        [TestCase( "Hello", new[] { "Hello" } )]
        [TestCase( "Hello World", new[] { "Hello", "World" } )]
        public void Tokenize_Alpha_Strings( string input, string[] tokens )
        {
            Verify( input, tokens );
        }

        [Test]
        [TestCase( "SELECT @var, 1, @a12, :hello123", new[] { "SELECT", "@var", ",", "1", ",", "@a12", ",", ":hello123" } )]
        [TestCase( "SELECT @@ROWCOUNT", new[] { "SELECT", "@@ROWCOUNT" } )]
        [TestCase( "@ROW @COUNT", new[] { "@ROW", "@COUNT" } )]
        public void Can_Tokenize_Variables( string input, string[] tokens )
        {
            Verify( input, tokens );
        }

        [Test]
        [TestCase( "1234", new[] { "1234" } )]
        [TestCase( "1234 9876", new[] { "1234", "9876" } )]
        [TestCase( "12349876 123312 12312", new[] { "12349876", "123312", "12312" } )]
        public void Tokenize_Numeric_Strings( string input, string[] tokens )
        {
            Verify( input, tokens );
        }

        [Test]
        [TestCase( "Ben 1234", new[] { "Ben", "1234" } )]
        [TestCase( "Ben1234 Laan9876 12Hello 34World", new[] { "Ben1234", "Laan9876", "12", "Hello", "34", "World" } )]
        public void Tokenize_Alpha_Numeric_Strings( string input, string[] tokens )
        {
            Verify( input, tokens );
        }

        [Test]
        [TestCase( "Ben 1234", new[] { "Ben", "1234" } )]
        [TestCase(
            "Ben1234 Laan9876 12Hello 34World",
            new[] { "Ben1234", "Laan9876", "12", "Hello", "34", "World" } )
        ]
        [TestCase(
            @"Ben(Test1234), Laan(9876) ""12"" [Hello, 'World']",
            new[] { "Ben", "(", "Test1234", ")", ",", "Laan", "(", "9876", ")", "\"", "12", "\"", "[Hello, 'World']" } )
        ]
        [TestCase( "A = 1", new[] { "A", "=", "1" } )]
        [TestCase( "2 <= 5", new[] { "2", "<=", "5" } )]
        [TestCase( "A != B", new[] { "A", "!=", "B" } )]
        [TestCase( "@Ben_Laan", new[] { "@Ben_Laan" } )]
        [TestCase( "Database.[Some Owner].Table", new[] { "Database", ".", "[Some Owner]", ".", "Table" } )]
        [TestCase( "#table ##global", new[] { "#table", "##global" } )]
        public void Tokenize_Alpha_Numeric_And_Special_Strings( string input, string[] tokens )
        {
            Verify( input, tokens );
        }

        [Test]
        //[TestCase("CONVERT(INT, '10')", new[] { "CONVERT", "(", "INT", ",", "'", "10", "'", ")" } )]
        [TestCase("'Hello World'", new[] { "'", "Hello", " ", "World", "'" } )]
        public void Can_Tokenize_Strings_As_One_Token( string input, string[] tokens )
        {
            Verify( input, tokens );
        }

        [Test]
        [TestCase( "SELECT * -- Get All Fields", new[] { "SELECT", "*", "-- Get All Fields" } )]
        [TestCase( "SELECT * -- Get All Fields\r\nFROM dbo.Table", new[] { "SELECT", "*", "-- Get All Fields", "FROM", "dbo", ".", "Table" } )]
        [TestCase( "SELECT * /* Get All\r\nFields */\r\nFROM dbo.Table", new[] { "SELECT", "*", "/* Get All\r\nFields */", "FROM", "dbo", ".", "Table" } )]
        [TestCase( "/* A\r\nB */\r\nC", new[] { "/* A\r\nB */", "C" } )]
        public void Can_Tokenize_Strings_With_An_Inline_Comment( string input, string[] tokens )
        {
            // Hack: Change the Skip flag for comments so they can be tested..
            //       Eventually, when comments are processed correctly, this can be removed!
            var tokenizer = GetTokenizer( input );
            SqlTokenizer sqlTokenizer = tokenizer as SqlTokenizer;
            sqlTokenizer.SkipComments = false;
            foreach ( string token in tokens )
            {
                Assert.IsTrue( tokenizer.HasMoreTokens );
                tokenizer.ReadNextToken();

                Assert.AreEqual( token, tokenizer.Current.Value );
            }
            tokenizer.ReadNextToken();
            Assert.IsFalse( tokenizer.HasMoreTokens );
        }
    }
}
