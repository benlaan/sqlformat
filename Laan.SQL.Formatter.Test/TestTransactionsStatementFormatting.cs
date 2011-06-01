using System;

using NUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestTransactionsStatementFormatting : BaseFormattingTest
    {
        [Test]
        public void Can_Format_Begin_Transaction_With_Rollback()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "BEGIN TRAN SELECT TOP 20 Field1, Field2 FROM dbo.Table T ROLLBACK" );

            // Verify outcome
            var expected = new[]
		            {
		               @"BEGIN TRAN",
		                "",
		                "    SELECT TOP 20",
		                "        Field1,",
		                "        Field2",
		                "",
		                "    FROM dbo.Table T",
		                "",
		                "ROLLBACK"
		            };

            Compare( actual, expected );
        }
    }
}
