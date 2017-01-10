using System;

using NUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestCteStatementFormatting : BaseFormattingTest
    {
        [Test]
        public void Can_Format_Simple_Cte()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("WITH A AS( SELECT Num  =1)  SELECT * FROM A");

            // Verify outcome
            var expected = new[]
            {
               @"WITH A AS (",
                "",
                "    SELECT Num = 1",
                ")",
                "SELECT *",
                "FROM A",
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Muliple_Ctes()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("WITH A AS( SELECT Num  =1), B AS (SELECT 1 AS P FROM X JOIN Y ON X.I = Y.I)  SELECT * FROM A");

            // Verify outcome
            var expected = new[]
            {
               @"WITH A AS (",
                "",
                "    SELECT Num = 1",
                "),",
                "B AS (",
                "",
                "    SELECT 1 AS P",
                "",
                "    FROM X",
                "",
                "    JOIN Y",
                "      ON X.I = Y.I",
                ")",
                "SELECT *",
                "FROM A",
            };

            Compare(actual, expected);
        }
    }
}