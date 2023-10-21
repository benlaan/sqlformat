using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestGoTerminatorStatementFormatting : BaseFormattingTest
    {
        [Test]
        public void Can_Create_Formatting_Engine()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise

            // Verify outcome
            Assert.IsNotNull( sut );
        }

        [Test]
        public void Can_Format_Simple_Select_Statement()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "SELECT TOP 20 Field1, Field2 FROM dbo.Table T" );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT TOP 20",
                "    Field1,",
                "    Field2",
                "",
                "FROM dbo.Table T",
            };

            Compare( actual, expected );
        }
    }
}