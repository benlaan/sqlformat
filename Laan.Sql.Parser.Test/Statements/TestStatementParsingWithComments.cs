using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestStatementParsingWithComments
    {
        [Test]
        public void Select_Statement_With_Inline_Comment()
        {
            // Exercise
            var statement = ParserFactory.Execute<SelectStatement>(
                "select * -- all fields\n\r from dbo.table"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.Fields.Count);
            Assert.AreEqual("*", statement.Fields[0].Expression.Value);
            Assert.IsNull(statement.Top);
            Assert.AreEqual("dbo.table", statement.From[0].Name);
        }

        [Test]
        public void Select_Statement_With_Block_Comment()
        {
            var statement = ParserFactory.Execute<SelectStatement>(
                @"select * from /* dbo.table t */ dbo.otherTable t"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.Fields.Count);
            Assert.AreEqual("*", statement.Fields[0].Expression.Value);
            Assert.IsNull(statement.Top);
            Assert.AreEqual("dbo.otherTable", statement.From[0].Name);
        }
    }
}
