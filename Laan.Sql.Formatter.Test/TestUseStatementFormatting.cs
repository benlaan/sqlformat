using System;
using NUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestUseStatementFormatting : BaseFormattingTest
    {
        [Test]
        public void Can_Format_Use_Statement_With_Simple_Database_Name()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "USE MyDatabase" );

            // Verify outcome
            var expected = new[]
            {
                "USE MyDatabase"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Use_Statement_With_Bracketed_Database_Name()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "USE [MyDatabase]" );

            // Verify outcome
            var expected = new[]
            {
                "USE [MyDatabase]"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Use_Statement_With_Schema_Qualified_Name()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "use master" );

            // Verify outcome
            var expected = new[]
            {
                "USE master"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Use_Statement_Followed_By_Go()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "USE MyDatabase\nGO" );

            // Verify outcome
            var expected = new[]
            {
                "USE MyDatabase",
                "",
                "GO"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Use_Statement_Followed_By_Select()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "USE MyDatabase\nSELECT * FROM Users" );

            // Verify outcome
            var expected = new[]
            {
                "USE MyDatabase",
                "",
                "SELECT *",
                "FROM Users"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Multiple_Use_Statements()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "USE Database1\nGO\nUSE Database2\nGO" );

            // Verify outcome
            var expected = new[]
            {
                "USE Database1",
                "",
                "GO",
                "",
                "USE Database2",
                "",
                "GO"
            };

            Compare( actual, expected );
        }
    }
}
