using System;

using NUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestUpdateStatementFormatting : BaseFormattingTest
    {
        [Test]
        public void Can_Format_Simple_Update_Statement()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "UPDATE dbo.Table SET Field1 = 'Value'" );

            // Verify outcome
            var expected = new[]
            {
               @"UPDATE dbo.Table",
                "   SET Field1 = 'Value'"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Update_Statement_With_Top_N()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "UPDATE TOP (10) dbo.Table SET Field1 = 'Value'" );

            // Verify outcome
            var expected = new[]
            {
               @"UPDATE TOP (10) dbo.Table",
                "   SET Field1 = 'Value'"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Update_Statement_With_Where_Clause()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "Update T SET T.Name = 'New' FROM dbo.Table T WHERE T.TableID = 10" );

            // Verify outcome
            var expected = new[]
            {
               @"UPDATE T",
                "   SET T.Name = 'New'",
                "",
                "FROM dbo.Table T",
                "",
                "WHERE T.TableID = 10"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Update_Statement_With_Multiple_Condition_Where_Clause()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "UPDATE T SET ID = 1 FROM dbo.Table T WHERE T.TableID = 10 AND T.Data IS NULL" );

            // Verify outcome
            var expected = new[]
            {
               @"UPDATE T",
                "   SET ID = 1",
                "",
                "FROM dbo.Table T",
                "",
                "WHERE T.TableID = 10",
                "  AND T.Data IS NULL"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Update_Statement_With_Joins()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute(
                @"UPDATE T SET Name = O.Name FROM dbo.Table T JOIN dbo.OtherJoin AS O ON O.ID = T.ID 
                  AND O.Num = T.Num AND O.Field = 0 LEFT JOIN dbo.Joined J ON T.ID = J.ID AND J.Field = 1"
            );

            // Verify outcome
            var expected = new[]
            {
                "UPDATE T",
                "   SET Name = O.Name",
                "",
                "FROM dbo.Table T",
                "",
                "JOIN dbo.OtherJoin AS O",
                "  ON O.ID = T.ID",
                " AND O.Num = T.Num",
                " AND O.Field = 0",
                "",
                "LEFT JOIN dbo.Joined J",
                "       ON T.ID = J.ID",
                "      AND J.Field = 1"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Update_Statement_With_Multiple_Fields_With_Aligned_Assignments()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "UPDATE dbo.Table SET Field = 'Value', SomeOtherLongField = 'Some Really Long Field'" );

            // Verify outcome
            var expected = new[]
            {
                "UPDATE dbo.Table",
                "   SET Field              = 'Value',",
                "       SomeOtherLongField = 'Some Really Long Field'"
            };

            Compare( actual, expected );
        }
    }
}