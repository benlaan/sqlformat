using System;
using System.Linq;

using NUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestCreateViewStatementFormatting : BaseFormattingTest
    {
        [Test]
        [TestCase("create")]
        [TestCase("alter")]
        public void Can_Format_Simple_Create_View_Statement(string modificationType)
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute(string.Format(@"{0} VIEW ben.MyView AS SELECT * FROM [Table]", modificationType));

            // Verify outcome
            var expected = new[]
            {
                modificationType.ToUpper() + " VIEW ben.MyView",
                "AS",
                "SELECT *", 
                "FROM [Table]"
            };

            Compare(actual, expected);
        }
    }
}
