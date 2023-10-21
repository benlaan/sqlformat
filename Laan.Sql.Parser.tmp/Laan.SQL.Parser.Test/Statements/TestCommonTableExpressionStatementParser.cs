using System;
using System.Collections.Generic;
using System.Linq;

using Laan.Sql.Parser.Entities;

using NUnit.Framework;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestCommonTableExpressionStatementParser
    {
        [Test]
        public void Can_Select_With_Simple_Cte()
        {
            var sql = @"
            
                WITH A AS (

                    SELECT Num = 1, Bob AS Fred
                )
                SELECT * FROM A
            
            ";

            // Exercise
            var statement = ParserFactory.Execute<CommonTableExpressionStatement>(sql).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.CommonTableExpressions.Count);

            var cte = statement.CommonTableExpressions[0];
            Assert.AreEqual("A", cte.Name);
            Assert.AreEqual(0, cte.ColumnNames.Count);

            Assert.AreEqual(2, cte.Fields.Count);
            Assert.AreEqual("Num", cte.Fields[0].Alias.Name);
            Assert.AreEqual("1", cte.Fields[0].Expression.Value);

            Assert.AreEqual("Fred", cte.Fields[1].Alias.Name);
            Assert.AreEqual("Bob", cte.Fields[1].Expression.Value);

            Assert.AreEqual("*", statement.Statement.Fields[0].Expression.Value);
            Assert.AreEqual(1, statement.Statement.From.Count);
        }

        [Test]
        public void Can_Select_Cte_With_Explicit_Columns()
        {
            var sql = @"
            
                WITH A (Number, Name) AS (

                    SELECT Num = 1, Name = '!'
                )
                SELECT * FROM A
            
            ";

            // Exercise
            var statement = ParserFactory.Execute<CommonTableExpressionStatement>(sql).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.CommonTableExpressions.Count);
            Assert.AreEqual("A", statement.CommonTableExpressions[0].Name);

            Assert.AreEqual(2, statement.CommonTableExpressions[0].ColumnNames.Count);
            Assert.AreEqual("Number", statement.CommonTableExpressions[0].ColumnNames[0]);
            Assert.AreEqual("Name", statement.CommonTableExpressions[0].ColumnNames[1]);

            Assert.AreEqual("*", statement.Statement.Fields[0].Expression.Value);
            Assert.AreEqual(1, statement.Statement.From.Count);
        }

        [Test]
        public void Can_Select_With_Multiple_Ctes()
        {
            var sql = @"
            
                WITH A (Number, Name) AS (

                    SELECT Num = 1, Name = '!'
                ),
                B AS (

                    SELECT Num = 1
                )
                SELECT * FROM A, B, C
            
            ";

            // Exercise
            var statement = ParserFactory.Execute<CommonTableExpressionStatement>(sql).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(2, statement.CommonTableExpressions.Count);
            Assert.AreEqual("A", statement.CommonTableExpressions[0].Name);
            Assert.AreEqual("B", statement.CommonTableExpressions[1].Name);

            Assert.AreEqual(2, statement.CommonTableExpressions[0].ColumnNames.Count);
            Assert.AreEqual("Number", statement.CommonTableExpressions[0].ColumnNames[0]);
            Assert.AreEqual("Name", statement.CommonTableExpressions[0].ColumnNames[1]);

            Assert.AreEqual(0, statement.CommonTableExpressions[1].ColumnNames.Count);

            Assert.AreEqual("*", statement.Statement.Fields[0].Expression.Value);
            Assert.AreEqual(3, statement.Statement.From.Count);
        }

        [Test]
        public void Can_Define_View_With_Cte()
        {
            var sql = @"
            
                CREATE VIEW V 
                AS
                    WITH A (Number, Name) AS (

                        SELECT Num = 1, Name = '!'
                    ),
                    B AS (

                        SELECT Num = 1
                    )
                    SELECT * FROM A, B, C
            
            ";

            // Exercise
            var createViewStatement = ParserFactory.Execute<CreateViewStatement>(sql).First();
            Assert.IsNotNull(createViewStatement);

            var statement = createViewStatement.Definition as CommonTableExpressionStatement;

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(2, statement.CommonTableExpressions.Count);
            Assert.AreEqual("A", statement.CommonTableExpressions[0].Name);
            Assert.AreEqual("B", statement.CommonTableExpressions[1].Name);

            Assert.AreEqual(2, statement.CommonTableExpressions[0].ColumnNames.Count);
            Assert.AreEqual("Number", statement.CommonTableExpressions[0].ColumnNames[0]);
            Assert.AreEqual("Name", statement.CommonTableExpressions[0].ColumnNames[1]);

            Assert.AreEqual(0, statement.CommonTableExpressions[1].ColumnNames.Count);

            Assert.AreEqual("*", statement.Statement.Fields[0].Expression.Value);
            Assert.AreEqual(3, statement.Statement.From.Count);
        }
    }
}
