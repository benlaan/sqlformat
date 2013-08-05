using System;
using System.Collections.Generic;
using System.Linq;

using Laan.Sql.Parser.Entities;

using NUnit.Framework;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestExecStatementParser
    {
        /// <summary>
        /// This sql is captured from a sql profiler, and has escaped strings and the 'triple' string
        /// input to sp_executesql
        /// </summary>
        [Test]
        public void Can_Execute_Simple_Sql_String()
        {
            var sql = @"exec sp_executesql 

                  N'select Id from [Transaction] T 
                    where Id = @p0 and OtherId = @p1 
                    and Name <> @p2',
                  N'@p0 int,@p1 int,@p2 nvarchar(max)',
                  @p0=100,@p1=44,@p2=N'WOO'
            ";

            // Exercise
            var statement = ParserFactory.Execute<ExecuteSqlStatement>(sql).First();

            // Verify outcome
            Assert.IsNotNull(statement);

            SelectStatement selectStatement = statement.InnerStatement as SelectStatement;
            Assert.IsNotNull(selectStatement);

            Assert.AreEqual("[Transaction]", selectStatement.From.First().Name);
            Assert.AreEqual(3, statement.Arguments.Count);
            Assert.AreEqual(new string[] { "@p0", "@p1", "@p2" }, statement.Arguments.Select(a => a.Name).ToArray());
            Assert.AreEqual(new string[] { "100", "44", "N'WOO'" }, statement.Arguments.Select(a => a.Value).ToArray());
        }

        /// <summary>
        /// This sql is captured from a sql profiler, and has escaped strings and the 'triple' string
        /// input to sp_executesql
        /// </summary>
        [Test]
        public void Can_Execute_Sql_String_Containing_Quotes_Within_Sql()
        {
            var sql = @"exec sp_executesql 
                  N'select T.Id, T.Name from [Transaction] T where T.Type in (''Process'', ''TransferFrom'') and Name <> @p0',
                  N'@p0 nvarchar(4000)',
                  @p0=N'WOO'
            ";

            // Exercise
            var statement = ParserFactory.Execute<ExecuteSqlStatement>(sql).First();

            // Verify outcome
            Assert.IsNotNull(statement);

            SelectStatement selectStatement = statement.InnerStatement as SelectStatement;
            Assert.IsNotNull(selectStatement);

            Assert.AreEqual("[Transaction]", selectStatement.From.First().Name);
            Assert.AreEqual(1, statement.Arguments.Count);
            Assert.AreEqual(new string[] { "@p0" }, statement.Arguments.Select(a => a.Name).ToArray());
            Assert.AreEqual(new string[] { "N'WOO'" }, statement.Arguments.Select(a => a.Value).ToArray());
        }

        /// <summary>
        /// This sql is captured from a sql profiler, and has escaped strings and the 'triple' string
        /// input to sp_executesql
        /// </summary>
        [Test]
        public void Can_Execute_Multiple_Sql_Strings()
        {
            var sql = @"

                exec sp_executesql 
                  N'select Id from [Transaction] T where Id = @p0 and OtherId = @p1 and Name <> @p2',
                  N'@p0 int,@p1 int,@p2 nvarchar(max)',
                  @p0=100,@p1=44,@p2=N'WOO'
                
                go

                exec sp_executesql 
                  N'select Id from [Transaction] T where Id = @p0 and OtherId = @p1 and Name <> @p2',
                  N'@p0 int,@p1 int,@p2 nvarchar(max)',
                  @p0=100,@p1=44,@p2=N'WOO'

            ";

            // Exercise
            var statements = ParserFactory.Execute(sql);

            // Verify outcome
            Assert.AreEqual(3, statements.Count);

            foreach (var statement in statements.OfType<ExecuteSqlStatement>())
            {
                SelectStatement selectStatement = statement.InnerStatement as SelectStatement;
                Assert.IsNotNull(selectStatement);

                Assert.AreEqual("[Transaction]", selectStatement.From.First().Name);
                Assert.AreEqual(3, statement.Arguments.Count);
                Assert.AreEqual(new string[] { "@p0", "@p1", "@p2" }, statement.Arguments.Select(a => a.Name).ToArray());
                Assert.AreEqual(new string[] { "100", "44", "N'WOO'" }, statement.Arguments.Select(a => a.Value).ToArray());
            }
        }

        [Test]
        public void Can_Execute_Stored_Proc_Without_Arguments()
        {
            var sql = @"exec dbo.SomeStoredProc";

            // Exercise
            var statement = ParserFactory.Execute<ExecStatement>(sql).First();

            // Verify outcome
            Assert.IsNotNull(statement);

            Assert.AreEqual("dbo.SomeStoredProc", statement.FunctionName);
            Assert.AreEqual(0, statement.Arguments.Count);
        }

        [Test]
        public void Can_Execute_Stored_Proc_With_One_Unnamed_Argument()
        {
            var sql = @"exec dbo.SomeStoredProc 15";

            // Exercise
            var statement = ParserFactory.Execute<ExecStatement>(sql).First();

            // Verify outcome
            Assert.IsNotNull(statement);

            Assert.AreEqual("dbo.SomeStoredProc", statement.FunctionName);
            Assert.AreEqual(1, statement.Arguments.Count);
            Assert.AreEqual("15", statement.Arguments[0].Value);
        }

        [Test]
        public void Can_Execute_Stored_Proc_With_One_Named_Argument()
        {
            var sql = @"exec dbo.SomeStoredProc @Arg = 15";

            // Exercise
            var statement = ParserFactory.Execute<ExecStatement>(sql).First();

            // Verify outcome
            Assert.IsNotNull(statement);

            Assert.AreEqual("dbo.SomeStoredProc", statement.FunctionName);
            Assert.AreEqual(1, statement.Arguments.Count);
            Assert.AreEqual("15", statement.Arguments[0].Value);
            Assert.AreEqual("@Arg", statement.Arguments[0].Name);
        }

        [Test]
        public void Can_Execute_Stored_Proc_With_Two_Named_Arguments()
        {
            var sql = @"exec dbo.SomeStoredProc @Id = 15, @Name = 'Ben'";

            // Exercise
            var statement = ParserFactory.Execute<ExecStatement>(sql).First();

            // Verify outcome
            Assert.IsNotNull(statement);

            Assert.AreEqual("dbo.SomeStoredProc", statement.FunctionName);
            Assert.AreEqual(2, statement.Arguments.Count);

            Assert.AreEqual("15", statement.Arguments[0].Value);
            Assert.AreEqual("@Id", statement.Arguments[0].Name);

            Assert.AreEqual("'Ben'", statement.Arguments[1].Value);
            Assert.AreEqual("@Name", statement.Arguments[1].Name);
        }

        [Test]
        public void Can_Execute_Stored_Proc_With_Two_Unnamed_Arguments()
        {
            var sql = @"exec dbo.SomeStoredProc 15, 'Ben'";

            // Exercise
            var statement = ParserFactory.Execute<ExecStatement>(sql).First();

            // Verify outcome
            Assert.IsNotNull(statement);

            Assert.AreEqual("dbo.SomeStoredProc", statement.FunctionName);
            Assert.AreEqual(2, statement.Arguments.Count);

            Assert.AreEqual("15", statement.Arguments[0].Value);
            Assert.AreEqual("", statement.Arguments[0].Name);

            Assert.AreEqual("'Ben'", statement.Arguments[1].Value);
            Assert.AreEqual("", statement.Arguments[1].Name);
        }
    }
}
