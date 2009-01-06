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
            var sql = "SELECT * FROM dbo.Table T WHERE T.TableID = 10";
            var actual = sut.Format( sql );

            // Verify outcome
            const string formatted = 
@"SELECT *

FROM dbo.Table T

WHERE T.TableID = 10";
            Assert.AreEqual( formatted, actual );

        }
    }
}
