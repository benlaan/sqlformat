using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MbUnit.Framework;

namespace Laan.SQL.Formatter.Test
{
    [TestFixture]
    public class TestFormattingEngine
    {

        private static void Compare( string actual, string[] formatted )
        {
            Assert.AreElementsEqual( 
                formatted, 
                actual.Split( new string[] { "\r\n" }, StringSplitOptions.None ) 
            );
        }

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
            var actual = sut.Execute( "SELECT * FROM dbo.Table T WHERE T.TableID = 10" );

            // Verify outcome
            var expected = new[]
            {
                @"SELECT *",
                "",
                "FROM dbo.Table T",
                "",
                "WHERE T.TableID = 10"
            };

            Compare( actual, expected );
        }
    }
}
