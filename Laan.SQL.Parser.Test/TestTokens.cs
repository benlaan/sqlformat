using System;

using NUnit.Framework;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestTokens
    {
        [Test]
        public void Empty_Tokens_Are_The_Same()
        {
            // Setup
            Token empty1 = new Token();
            Token empty2 = new Token();

            // Verify outcome
            Assert.IsTrue( empty1 == empty2 );
        }

        [Test]
        public void One_Empty_And_One_Non_Empty_Tokens_Are_Different()
        {
            // Setup
            Token nonEmpty = new Token( "SELECT", TokenType.Alpha );
            Token empty = new Token();

            // Verify outcome
            Assert.IsTrue( nonEmpty != empty );
        }

        [Test]
        public void Two_Non_Empty_Tokens_Are_Same()
        {
            // Setup
            Token nonEmpty1 = new Token( "SELECT", TokenType.Alpha );
            Token nonEmpty2 = new Token( "SELECT", TokenType.Alpha );

            // Verify outcome
            Assert.IsTrue( nonEmpty1 == nonEmpty2 );
        }

        [Test]
        public void Two_Non_Empty_Tokens_Are_Different()
        {
            // Setup
            Token nonEmpty1 = new Token( "SELECT", TokenType.Alpha );
            Token nonEmpty2 = new Token( "UPDATE", TokenType.Alpha );

            // Verify outcome
            Assert.IsTrue( nonEmpty1 != nonEmpty2 );
        }

        [Test]
        public void One_Non_Empty_Tokens_And_One_String_Are_Same()
        {
            // Setup
            Token nonEmpty = new Token( "SELECT", TokenType.Alpha );
            string value = "SELECT";

            // Verify outcome
            Assert.IsTrue( nonEmpty == value );
        }

        [Test]
        public void One_Non_Empty_Tokens_And_One_String_Are_Different()
        {
            // Setup
            Token nonEmpty = new Token( "SELECT", TokenType.Alpha );
            string value = "UPDATE";

            // Verify outcome
            Assert.IsTrue( nonEmpty != value );
        }
    }
}
