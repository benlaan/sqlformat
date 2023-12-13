using System;

using NUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestExecuteSqlStatementFormatting : BaseFormattingTest
    {
        [Test]
        public void Can_Format_Select_Within_spExecuteSql()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var sql = @"exec sp_executesql 
                  N'select TOP (@p0) T.Id, T.Name from [Transaction] T where T.Type in (''Process'', ''TransferFrom'') and T.Code in (@p1) and T.Name <> @p2',
                  N'@p0 int,@p1 int,@p2 nvarchar(4000)',
                  @p0=100,@p1=44,@p2=N'WOO'
            ";
            
            var actual = sut.Execute(sql);

            // Verify outcome
            var expected = new[]
            {
               @"DECLARE",
                "    @p0 int,",
                "    @p1 int,",
                "    @p2 nvarchar(4000)",
                "",
                "SELECT",
                "    @p0 = 100,",
                "    @p1 = 44,",
                "    @p2 = N'WOO'",
                "",
                "SELECT TOP (@p0)",
                "    T.Id,",
                "    T.Name",
                "",
                "FROM [Transaction] T",
                "",
                "WHERE T.Type IN ('Process', 'TransferFrom')",
                "  AND T.Code IN (@p1)",
                "  AND T.Name <> @p2"
            };

            Compare(actual, expected);
        }
   }
}