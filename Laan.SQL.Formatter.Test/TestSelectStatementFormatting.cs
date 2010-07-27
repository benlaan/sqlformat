using System;

using MbUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestSelectStatementFormatting : BaseFormattingTest
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
        public void Can_Format_Select_Statement_With_Top_N_Percent()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "SELECT TOP 50 PERCENT Field1, Field2 FROM dbo.Table T;" );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT TOP 50 PERCENT",
                "    Field1,",
                "    Field2",
                "",
                "FROM dbo.Table T;",
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Simple_Select_Statement_With_Terminator()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "SELECT Field1, Field2 FROM dbo.Table T;" );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT",
                "    Field1,",
                "    Field2",
                "",
                "FROM dbo.Table T;",
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
                "FROM dbo.Table T",
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
                "FROM dbo.Table T",
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
                "FROM dbo.Table T",
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
                SELECT * FROM dbo.Events E WHERE Date=(SELECT MAX(Date)
                FROM dbo.Events WHERE Date 
                > Now - 10 )"
            );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT *",
                "FROM dbo.Events E",
                "WHERE Date = (",
                "",
                "    SELECT MAX(Date)",
                "    FROM dbo.Events",
                "    WHERE Date > Now - 10",
                "",
                ")"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Nested_Criteria()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT * FROM dbo.Events E WHERE ( E.ID IS NULL OR E.Type = 'X')"
            );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT *",
                "FROM dbo.Events E",
                "WHERE (",
                "",
                "    E.ID IS NULL",
                "    OR",
                "    E.Type = 'X'",
                "",
                ")"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Two_Nested_Criteria()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT * FROM dbo.Events E WHERE ( E.ID IS NULL OR E.Type = 'X' AND E.ID = 20 )"
            );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT *",
                "FROM dbo.Events E",
                "WHERE (",
                "",
                "    E.ID IS NULL",
                "    OR",
                "    E.Type = 'X'",
                "    AND",
                "    E.ID = 20",
                "",
                ")"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Four_Nested_Criteria()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT * FROM dbo.Events E WHERE ( E.ID IS NULL OR E.Type = 'X' 
                AND E.ID = 20 AND E.Type != 'Y' 
                AND E.OtherID = 40)"
            );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT *",
                "FROM dbo.Events E",
                "WHERE (",
                "",
                "    E.ID IS NULL",
                "    OR",
                "    E.Type = 'X'",
                "    AND",
                "    E.ID = 20",
                "    AND",
                "    E.Type != 'Y'",
                "    AND",
                "    E.OtherID = 40",
                "",
                ")"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Nested_Criteria_Within_Nested_Criteria()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT * FROM dbo.Events E WHERE ( E.ID IS NULL OR E.Type = 'X' 
                AND (E.ID = 20 OR E.Type != 'Y' )
                AND E.OtherID = 40)"
            );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT *",
                "FROM dbo.Events E",
                "WHERE (",
                "",
                "    E.ID IS NULL",
                "    OR",
                "    E.Type = 'X'",
                "    AND",
                "    (",
                "",
                "        E.ID = 20",
                "        OR",
                "        E.Type != 'Y'",
                "",
                "    )",
                "    AND",
                "    E.OtherID = 40",
                "",
                ")"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Nested_Criteria_Within_Nested_Criteria_On_Where_Clause()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT * FROM dbo.States S JOIN dbo.Localities L 
                ON L.StateID = S.StateID AND (L.ID = S.ID OR (L.Key <> S.Key))"
            );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT *",
                "",
                "FROM dbo.States S",
                "",
                "JOIN dbo.Localities L",
                "  ON L.StateID = S.StateID",
                " AND (",
                "",
                "    L.ID = S.ID",
                "    OR",
                "    (L.Key <> S.Key)",
                "",
                ")"
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Implicit_Alias()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"SELECT A.ID SomeAlias FROM table ");

            // Verify outcome
            var expected = new[]
            {
               @"SELECT A.ID SomeAlias",
                "FROM table",
            };
            Compare( actual, expected );
       }

        [Test]
        public void Can_Format_Select_With_Alias_Using_As()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"SELECT A.ID AS SomeAlias FROM table " );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT A.ID AS SomeAlias",
                "FROM table",
            };
            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Union()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"SELECT A.ID FROM table UNION SELECT A.ID FROM table" );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT A.ID",
                "FROM table",
                "",
                "UNION",
                "",
                "SELECT A.ID",
                "FROM table",
            };
            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Into_Statement()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "SELECT ID, Name INTO #temp FROM dbo.Table T" );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT",
                "    ID,",
                "    Name",
                "",
                "INTO #temp",
                "",
                "FROM dbo.Table T",
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Multiple_Froms_With_Joins()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( "select id from table1 t1 join other1 o1 on t1.id = o1.id, table2 t2 join other2 o2 on t2.id = o2.id" );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT id",
                "",
                "FROM table1 t1",
                "",
                "    JOIN other1 o1",
                "      ON t1.id = o1.id,",
                "",
                "FROM table2 t2",
                "",
                "    JOIN other2 o2",
                "      ON t2.id = o2.id",
            };

            Compare( actual, expected );
        }

        [Test]
        public void Can_Format_Select_With_Simple_Sub_Select()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute( @"
                SELECT * FROM dbo.Events E WHERE Date=(SELECT TOP 1 Date FROM dbo.Events)"
            );

            // Verify outcome
            var expected = new[]
            {
               @"SELECT *",
                "FROM dbo.Events E",
                "WHERE Date = (SELECT TOP 1 Date FROM dbo.Events)"
            };

            Compare( actual, expected );
        }
    }
}