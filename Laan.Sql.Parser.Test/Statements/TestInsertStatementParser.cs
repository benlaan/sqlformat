﻿using System;
using System.Collections.Generic;
using System.Linq;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;

using NUnit.Framework;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestInsertStatementParser
    {
        [Test]
        public void Basic_Insert_Statement()
        {
            // Exercise
            InsertStatement statement = ParserFactory.Execute<InsertStatement>("insert into table values (1, 'A')").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("table", statement.TableName);
            Assert.AreEqual(1, statement.Values.Count);

            var row = statement.Values[0];
            Assert.AreEqual(2, row.Count);
            Assert.AreEqual("1", row[0].Value);
            Assert.AreEqual("'A'", row[1].Value);
        }

        [Test]
        public void Basic_Insert_Statement_With_Columns()
        {
            // Exercise
            InsertStatement statement = ParserFactory.Execute<InsertStatement>(@"

                insert into table ( field1, field2 ) values (1, 'A')"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("table", statement.TableName);
            Assert.AreEqual(1, statement.Values.Count);

            var row = statement.Values[0];
            Assert.AreEqual(2, row.Count);
            Assert.AreEqual("1", row[0].Value);
            Assert.AreEqual("'A'", row[1].Value);

            Assert.AreEqual(2, statement.Columns.Count);
            Assert.AreEqual("field1", statement.Columns[0]);
            Assert.AreEqual("field2", statement.Columns[1]);
        }


        [Test]
        public void Insert_Statement_With_Select_Statement()
        {
            // Exercise
            InsertStatement statement = ParserFactory.Execute<InsertStatement>("insert into table select a from source").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("table", statement.TableName);
            Assert.AreEqual(0, statement.Values.Count);

            Assert.IsNotNull(statement.SourceStatement);
            Assert.AreEqual("source", statement.SourceStatement.From[0].Name);

        }

        [Test]
        public void Insert_Statement_With_Multiple_Value_Sets()
        {
            // Exercise
            InsertStatement statement = ParserFactory.Execute<InsertStatement>(@"

                insert into table values (1, 'A'), (2, 'B'), (3, 'C')"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("table", statement.TableName);
            Assert.AreEqual(3, statement.Values.Count);

            var expected = new[] 
            {   
                new { Id = "1", Name = "'A'" }, 
                new { Id = "2", Name = "'B'" }, 
                new { Id = "3", Name = "'C'" } 
            };

            Assert.AreEqual(expected.Length, statement.Values.Count);

            for (int index = 0; index < expected.Length; index++)
            {
                var row = statement.Values[index];
                Assert.AreEqual(expected[index].Id, row[0].Value);
                Assert.AreEqual(expected[index].Name, row[1].Value);
            }
        }

        // This supports oracle syntax that allows functions within VALUES
        [Test]
        public void Insert_Statement_With_Complex_Values()
        {
            // Exercise
            InsertStatement statement = ParserFactory.Execute<InsertStatement>(@"

                insert into table values (1, 'A', to_date('16/JAN/14 05:55:24','DD/MON/RR HH24:MI:SS'))"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("table", statement.TableName);
            Assert.AreEqual(1, statement.Values.Count);

            var row = statement.Values[0];
            Assert.AreEqual(3, row.Count);
            Assert.AreEqual("1", row[0].Value);
            Assert.AreEqual("'A'", row[1].Value);
            Assert.AreEqual("to_date('16/JAN/14 05:55:24','DD/MON/RR HH24:MI:SS')", row[2].Value);
        }
    }
}
