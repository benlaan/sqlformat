using System;
using System.Collections.Generic;
using System.Linq;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;
using Laan.Sql.Parser.Expressions;

using NUnit.Framework;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestCreateProcedureStatement
    {
        [Test]
        [TestCase("create")]
        [TestCase("alter")]
        public void Select_StarField_Only(string modificationType)
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateProcedureStatement>(string.Format("{0} proc v1 as select * from table", modificationType)).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual("v1", statement.Name);
            Assert.IsInstanceOf<SelectStatement>(statement.Definition);
        }

        [Test]
        public void Create_Procedure_With_Schema()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateProcedureStatement>("create procedure dbo.v1 as select * from table").First();

            Assert.IsInstanceOf<SelectStatement>(statement.Definition);

            SelectStatement selectStatement = (SelectStatement)statement.Definition;

            // Verify outcome
            Assert.IsNotNull(selectStatement);
            Assert.AreEqual("dbo.v1", statement.Name);
            Assert.AreEqual(1, selectStatement.Fields.Count);
            Assert.AreEqual("*", selectStatement.Fields[0].Expression.Value);
            Assert.IsNull(selectStatement.Top);
            Assert.AreEqual("table", selectStatement.From[0].Name);
        }

        [Test]
        public void Create_Procedure_With_Block_Definition()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateProcedureStatement>("create procedure dbo.v1 as begin select * from table end").First();

            Assert.IsInstanceOf<BlockStatement>(statement.Definition);
            var blockStatement = (BlockStatement)statement.Definition;

            Assert.AreEqual(1, blockStatement.Statements.Count);
            Assert.IsInstanceOf<SelectStatement>(blockStatement.Statements[0]);
            var selectStatement = (SelectStatement)blockStatement.Statements[0];

            // Verify outcome
            Assert.IsNotNull(selectStatement);
            Assert.AreEqual("dbo.v1", statement.Name);
            Assert.AreEqual(1, selectStatement.Fields.Count);
            Assert.AreEqual("*", selectStatement.Fields[0].Expression.Value);
            Assert.IsNull(selectStatement.Top);
            Assert.AreEqual("table", selectStatement.From[0].Name);
        }

        [Test]
        public void Create_Procedure_With_Arguments_Without_Brackets()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateProcedureStatement>("create procedure dbo.v1 @Id INT, @Name VARCHAR(MAX) as select * from table").First();

            Assert.AreEqual(2, statement.Arguments.Count);
            Assert.AreEqual("@Id", statement.Arguments[0].Name);
            Assert.AreEqual("INT", statement.Arguments[0].Type);
        }

        [Test]
        public void Create_Procedure_With_Arguments_With_Brackets()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateProcedureStatement>("create procedure dbo.v1 (@Id INT, @Name VARCHAR(MAX)) as select * from table").First();

            Assert.AreEqual(2, statement.Arguments.Count);
            Assert.IsTrue(statement.HasBracketedArguments);
            Assert.AreEqual("@Id", statement.Arguments[0].Name);
            Assert.AreEqual("INT", statement.Arguments[0].Type);
        }
    }
}
