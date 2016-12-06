using System;
using System.Linq;

using NUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestCreateViewStatementFormatting : BaseFormattingTest
    {
        [Test]
        public void Can_Format_Simple_Create_Index_Statement()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute(@"CREATE VIEW ben.MyView AS SELECT * FROM [Table]");

            // Verify outcome
            var expected = new[]
            {
                "CREATE VIEW ben.MyView",
                "AS",
                "SELECT *", 
                "FROM [Table]"
            };

            Compare(actual, expected);
        }
    }
}
