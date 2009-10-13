using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MbUnit.Framework;

namespace Laan.SQL.Formatter.Test
{
    [TestFixture]
    public class TestDeleteStatementFormatting : BaseFormattingTest
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
        public void Can_Format_Simple_Delete_Statement()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "DELETE FROM dbo.Table" );

            // Verify outcome
            var expected = new[]
            {
               @"DELETE",
                "FROM dbo.Table"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Delete_Statement_With_Top_N()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "DELETE TOP (10) FROM dbo.Table" );

            // Verify outcome
            var expected = new[]
            {
               @"DELETE TOP (10)",
                "FROM dbo.Table"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Delete_Statement_With_Top_N_And_Table_Alias()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "DELETE TOP (10) T FROM dbo.Table T" );

            // Verify outcome
            var expected = new[]
            {
               @"DELETE TOP (10) T",
                "FROM dbo.Table T"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Delete_Statement_With_Where_Clause()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "DELETE T FROM dbo.Table T WHERE T.TableID = 10" );

            // Verify outcome
            var expected = new[]
            {
               @"DELETE T",
                "FROM dbo.Table T",
                "WHERE T.TableID = 10"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Delete_Statement_With_Multiple_Condition_Where_Clause()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "Delete T FROM dbo.Table T WHERE T.TableID = 10 AND T.Data IS NULL" );

            // Verify outcome
            var expected = new[]
            {
               @"DELETE T",
                "",
                "FROM dbo.Table T",
                "",
                "WHERE T.TableID = 10",
                "  AND T.Data IS NULL"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Delete_Statement_With_Joins()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute(
                @"Delete T FROM dbo.Table T JOIN dbo.OtherJoin AS O ON O.ID = T.ID 
                  AND O.Num = T.Num AND O.Field = 0 LEFT JOIN dbo.Joined J ON T.ID = J.ID AND J.Field = 1"
            );

            // Verify outcome
            var expected = new[]
            {
               @"DELETE T",
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
    }
}