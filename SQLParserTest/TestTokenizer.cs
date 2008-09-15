using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using MbUnit.Framework;

namespace Laan.SQLParser.Test
{
    [TestFixture]
    public class TestTokenizer
    {
        [Test]
        public void Ensure_Empty_String_Returns_No_Tokens()
        {
            var r = new StringReader( "" );
            var sut = new Tokenizer( r );
            sut.ReadNextToken();
            Assert.AreEqual( "", sut.Current );
            Assert.IsFalse( sut.HasMoreTokens );
        }

        [Test]
        [Row( "Hello", new[] { "Hello" } )]
        [Row( "Hello World", new[] { "Hello", "World" } )]
        public void Tokenize_Alpha_Strings( string input, string[] tokens )
        {
            var sut = new Tokenizer( input );

            foreach ( string token in tokens )
            {
                Assert.IsTrue( sut.HasMoreTokens );
                sut.ReadNextToken();
                Assert.AreEqual( token, sut.Current );
            }
            Assert.IsFalse( sut.HasMoreTokens );
        }

        [Test]
        [Row( "1234", new[] { "1234" } )]
        [Row( "1234 9876", new[] { "1234", "9876" } )]
        [Row( "12349876 123312 12312", new[] { "12349876", "123312", "12312" } )]
        public void Tokenize_Numeric_Strings( string input, string[] tokens )
        {
            var sut = new Tokenizer( input );

            foreach ( string token in tokens )
            {
                Assert.IsTrue( sut.HasMoreTokens );
                sut.ReadNextToken();
                Assert.AreEqual( token, sut.Current );
            }
            Assert.IsFalse( sut.HasMoreTokens );
        }

        [Test]
        [Row( "Ben 1234", new[] { "Ben", "1234" } )]
        [Row( "Ben1234 Laan9876 12Hello 34World", new[] { "Ben1234", "Laan9876", "12", "Hello", "34", "World" } )]
        public void Tokenize_Alpha_Numeric_Strings( string input, string[] tokens )
        {
            var sut = new Tokenizer( input );

            foreach ( string token in tokens )
            {
                Assert.IsTrue( sut.HasMoreTokens );
                sut.ReadNextToken();
                Assert.AreEqual( token, sut.Current );
            }
            Assert.IsFalse( sut.HasMoreTokens );
        }

        [Test]
        [Row( 
            "Ben 1234", 
            new[] { "Ben", "1234" } )
        ]
        [Row( 
            "Ben1234 Laan9876 12Hello 34World", 
            new[] { "Ben1234", "Laan9876", "12", "Hello", "34", "World" } )
        ]
        [Row( 
            @"Ben(Test1234), Laan(9876) ""12"" [Hello, 'World']",
            new[] { "Ben", "(", "Test1234", ")", ",", "Laan", "(", "9876", ")", "\"", "12", "\"", "[", "Hello", ",", "'", "World", "'", "]" } )
        ]
        [Row(
            "A = 1, 2 <= 5; A != B; 33 >= 12", 
            new[] { "A", "=", "1", ",", "2", "<=", "5", ";", "A", "!=", "B", ";", "33", ">=", "12" } )
        ]
        [Row( "@Ben_Laan", new[] { "@Ben_Laan" } )]
        public void Tokenize_Alpha_Numeric_And_Special_Strings( string input, string[] tokens )
        {
            var sut = new Tokenizer( input );

            foreach ( string token in tokens )
            {
                Assert.IsTrue( sut.HasMoreTokens );
                sut.ReadNextToken();
                Assert.AreEqual( token, sut.Current );
            }
            Assert.IsFalse( sut.HasMoreTokens );
        }
    }
}
