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
            var actual = sut.Execute( "SELECT TOP 20 Field1, Field2 FROM dbo.Table T" );

            // Verify outcome
            var expected = new[]
            {
                @"SELECT TOP 20",
                "    Field1,",
                "    Field2",
                "",
                "FROM dbo.Table T",
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_Statement_With_Where_Clause()
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

        [Test]
        public void Can_Format_Select_Statement_With_Multiple_Condition_Where_Clause()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "SELECT * FROM dbo.Table T WHERE T.TableID = 10 AND T.Data IS NULL" );

            // Verify outcome
            var expected = new[]
            {
                @"SELECT *",
                "",
                "FROM dbo.Table T",
                "",
                "WHERE T.TableID = 10",
                "  AND T.Data IS NULL"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_Statement_With_Joins()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute(
                @"SELECT * FROM dbo.Table T JOIN dbo.OtherJoin AS O ON O.ID = T.ID 
                  AND O.Num = T.Num AND O.Field = 0 LEFT JOIN dbo.Joined J ON T.ID = J.ID AND J.Field = 1"
            );

            // Verify outcome
            var expected = new[]
            {
                @"SELECT *",
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
        public void Can_Format_Select_Statement_With_Order_By_One_Column()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "SELECT * FROM dbo.Table T ORDER BY A.FieldID" );

            // Verify outcome
            var expected = new[]
            {
                @"SELECT *",
                "",
                "FROM dbo.Table T",
                "",
                "ORDER BY A.FieldID"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_Statement_With_Order_By_Multiple_Columns()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "SELECT * FROM dbo.Table T ORDER BY A.FieldID, B.FieldID, C.FieldID DESC" );

            // Verify outcome
            var expected = new[]
            {
                @"SELECT *",
                "",
                "FROM dbo.Table T",
                "",
                "ORDER BY",
                "    A.FieldID,",
                "    B.FieldID,",
                "    C.FieldID DESC"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_Statement_With_Group_By_Multiple_Columns()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "SELECT * FROM dbo.Table T GROUP BY A.FieldID, B.FieldID, C.FieldID" );

            // Verify outcome
            var expected = new[]
            {
                @"SELECT *",
                "",
                "FROM dbo.Table T",
                "",
                "GROUP BY",
                "    A.FieldID,",
                "    B.FieldID,",
                "    C.FieldID"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_Statement_With_Group_By_Multiple_Columns_With_Having_Clause()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "SELECT COUNT(*) FROM dbo.Table T GROUP BY A.FieldID, B.FieldID, C.FieldID HAVING COUNT(*) > 1" );

            // Verify outcome
            var expected = new[]
            {
                @"SELECT COUNT(*)",
                "",
                "FROM dbo.Table T",
                "",
                "GROUP BY",
                "    A.FieldID,",
                "    B.FieldID,",
                "    C.FieldID",
                "",
                "HAVING COUNT(*) > 1"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_Statement_With_Group_By_Multiple_Columns_With_Multiple_Having_Clauses()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "SELECT COUNT(*) FROM dbo.Table T GROUP BY A.FieldID HAVING COUNT(*) > 1 AND SUM(A.Total)<10 " );

            // Verify outcome
            var expected = new[]
            {
                @"SELECT COUNT(*)",
                "",
                "FROM dbo.Table T",
                "",
                "GROUP BY A.FieldID",
                "",
                "HAVING COUNT(*) > 1",
                "   AND SUM(A.Total) < 10"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_Statement_With_Nested_Select_In_From()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "SELECT A.ID, COUNT(*) FROM ( SELECT Name FROM Server.db.owner.Tables T JOIN OtherTable O ON O.ID = T.ID WHERE T.ID = 'ben' ) AS X " );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT",
                "    A.ID,",
                "    COUNT(*)",
                "",
                "FROM (",
                "",
                "    SELECT Name",
                "",
                "    FROM Server.db.owner.Tables T",
                "",
                "    JOIN OtherTable O",
                "      ON O.ID = T.ID",
                "",
                "    WHERE T.ID = 'ben'",
                "",
                ") AS X",
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_Statement_With_Nested_Select_In_Join()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT A.ID, COUNT(*) FROM dbo.Table T LEFT JOIN ( SELECT Name FROM Server.db.owner.Tables T 
                JOIN OtherTable O ON O.ID = T.ID WHERE T.ID = 'ben' ) AS X ON X.Name = T.Name AND X.ID <> Y.ID" 
            );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT",
                "    A.ID,",
                "    COUNT(*)",
                "",
                "FROM dbo.Table T",
                "",
                "LEFT JOIN (",
                "",
                "    SELECT Name",
                "",
                "    FROM Server.db.owner.Tables T",
                "",
                "    JOIN OtherTable O",
                "      ON O.ID = T.ID",
                "",
                "    WHERE T.ID = 'ben'",
                "",
                ") AS X",
                "  ON X.Name = T.Name",
                " AND X.ID <> Y.ID"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Sub_Select()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT * FROM dbo.Events E WHERE Date=(SELECT MAX(Date) FROM dbo.Events )"
            );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT *",
                "",
                "FROM dbo.Events E",
                "",
                "WHERE Date = (",
                "",
                "    SELECT MAX(Date)",
                "",
                "    FROM dbo.Events",
                "",
                ")"
            };

            // 

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Only_One_Field_As_Case()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT CASE A.field WHEN 1 THEN 'Y' WHEN @A + 2 THEN 'N' WHEN @A / 4 THEN 'X' ELSE 'U' END"
            );

            // Verify outcome
            var expected = new[]
            {
                @"SELECT CASE A.field",
                "            WHEN 1 THEN 'Y'",
                "            WHEN @A + 2 THEN 'N'",
                "            WHEN @A / 4 THEN 'X'",
                "        ELSE",
                "            'U'",
                "        END"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Simple_CaseSwitch()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT A.Field1, CASE A.field WHEN 1 THEN 'Y' WHEN @A + 2 THEN 'N' WHEN @A / 4 THEN 'X' ELSE 'U' END"
            );

            // Verify outcome
            var expected = new[]
            {
                @"SELECT",
                "    A.Field1,",
                "    CASE A.field",
                "        WHEN 1 THEN 'Y'",
                "        WHEN @A + 2 THEN 'N'",
                "        WHEN @A / 4 THEN 'X'",
                "    ELSE",
                "        'U'",
                "    END"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Simple_Case()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT A.Field1, CASE WHEN A.Field1 = 1 THEN 'Y' WHEN A.Field2 = 2 THEN 'N' ELSE 'U'   END"
            );

            // Verify outcome
            var expected = new[]
            {
                @"SELECT",
                "    A.Field1,",
                "    CASE",
                "        WHEN A.Field1 = 1 THEN 'Y'",
                "        WHEN A.Field2 = 2 THEN 'N'",
                "    ELSE",
                "        'U'",
                "    END"
            };

            // SELECT * FROM dbo.Events WHERE Date = (SELECT MAX(Date) FROM dbo.Events )

            Compare( actual, expected );
        }
    }
}