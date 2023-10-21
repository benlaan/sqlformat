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

        [Test]
        public void Can_Format_Insert_Within_spExecuteSql()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var sql = @"exec sp_executesql N'-- Query
                INSERT INTO dbo.Table (Field1, Field2, Field3, Field4, Field5, Field6, Field7, Field8)
                VALUES (@p0, @p1, @p2, @p3, @P4, @p5, @p6, @P7)',N'@p0 int,@p1 int,@p2 nvarchar(4000),@p3 nvarchar(4000),@P4 datetime,@p5 datetime,@p6 bit,@P7 uniqueidentifier',@p0=1,@p1=1,@p2=NULL,@p3=N'A-User',@P4='2019-01-23 22:41:57.023',@p5='2019-01-23 22:41:57.023',@p6=0,@P7='87EE43C3-1589-41FA-8C17-A9DF00979904'
            ";
            
            var actual = sut.Execute(sql);

            // Verify outcome
            var expected = new[]
            {
                "DECLARE",
                "    @p0 int,",
                "    @p1 int,",
                "    @p2 nvarchar(4000),",
                "    @p3 nvarchar(4000),",
                "    @P4 datetime,",
                "    @p5 datetime,",
                "    @p6 bit,",
                "    @P7 uniqueidentifier",
                "",
                "SELECT",
                "    @p0 = 1,",
                "    @p1 = 1,",
                "    @p2 = NULL,",
                "    @p3 = N'A-User',",
                "    @P4 = '2019-01-23 22:41:57.023',",
                "    @p5 = '2019-01-23 22:41:57.023',",
                "    @p6 = 0,",
                "    @P7 = '87EE43C3-1589-41FA-8C17-A9DF00979904'",
                "",
                "INSERT INTO dbo.Table (",
                "    Field1, Field2, Field3, Field4, Field5, Field6, Field7, Field8",
                "    )",
                "    VALUES (@p0, @p1, @p2, @p3, @P4, @p5, @p6, @P7)",
                };

            Compare(actual, expected);
        }

        [Test]
        [Ignore("Running slowly - needs investigation")]
        public void Can_Format_Very_Long_Statement()
        {
            var sql = @"
exec sp_executesql N'--
SELECT count(*) as y0_ FROM dbo.SomeView this_ WHERE (((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((((this_.Code = @p0 or this_.Code = @p1) or this_.Code = @p2) or this_.Code = @p3) or this_.Code = @P4) or this_.Code = @p5) or this_.Code = @p6) or this_.Code = @P7) or this_.Code = @p8) or this_.Code = @p9) or this_.Code = @p10) or this_.Code = @p11) or this_.Code = @p12) or this_.Code = @P13) or this_.Code = @p14) or this_.Code = @P15) or this_.Code = @p16) or this_.Code = @p17) or this_.Code = @p18) or this_.Code = @p19) or this_.Code = @p20) or this_.Code = @p21) or this_.Code = @p22) or this_.Code = @p23) or this_.Code = @p24) or this_.Code = @p25) or this_.Code = @P26) or this_.Code = @p27) or this_.Code = @p28) or this_.Code = @p29) or this_.Code = @p30) or this_.Code = @p31) or this_.Code = @P32) or this_.Code = @p33) or this_.Code = @p34) or this_.Code = @p35) or this_.Code = @p36) or this_.Code = @P37) or this_.Code = @p38) or this_.Code = @p39) or this_.Code = @p40) or this_.Code = @p41) or this_.Code = @p42) or this_.Code = @P43) or this_.Code = @p44) or this_.Code = @p45) or this_.Code = @p46) or this_.Code = @P47) or this_.Code = @p48) or this_.Code = @p49) or this_.Code = @p50) or this_.Code = @p51) or this_.Code = @p52) or this_.Code = @p53) or this_.Code = @p54) or this_.Code = @p55) or this_.Code = @p56) or this_.Code = @p57) or this_.Code = @p58) or this_.Code = @p59) or this_.Code = @p60) or this_.Code = @p61) or this_.Code = @p62) or this_.Code = @p63) or this_.Code = @p64) or this_.Code = @p65) or this_.Code = @p66) or this_.Code = @p67) or this_.Code = @p68) or this_.Code = @p69) or this_.Code = @p70) or this_.Code = @p71) or this_.Code = @p72) or this_.Code = @p73) or this_.Code = @p74) or this_.Code = @p75) or this_.Code = @p76) or this_.Code = @p77) or this_.Code = @p78) or this_.Code = @p79) or this_.Code = @p80) or this_.Code = @p81) or this_.Code = @p82) or this_.Code = @p83) or this_.Code = @p84) or this_.Code = @p85) or this_.Code = @p86) or this_.Code = @p87) or this_.Code = @p88) or
this_.Code = @p89) or this_.Code = @p90) or this_.Code = @p91) or this_.Code = @p92) or this_.Code = @p93) or this_.Code = @p94) or this_.Code = @p95) or this_.Code = @p96) or this_.Code = @p97) or this_.Code = @p98) or this_.Code = @p99) or this_.Code = @p100) or
this_.Code = @p101) or this_.Code = @p102) or this_.Code = @p103)',
N'@p0 nvarchar(4000)',
@p0=N'CODE'
";

            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute(sql);
            Assert.IsNotNull(actual);
        }
   }
}