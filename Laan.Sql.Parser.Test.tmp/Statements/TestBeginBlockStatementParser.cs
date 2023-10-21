using System;
using System.Linq;

using Laan.Sql.Parser.Entities;

using NUnit.Framework;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestBeginBlockStatementParser
    {
        [Test]
        [TestCase("begin select id from t end", 1)]
        [TestCase("begin select id from t select id from x end", 2)]
        public void Begin_End_Block(string sql, int statementCount)
        {
            // Exercise
            var statement = ParserFactory.Execute<BlockStatement>(sql).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(statementCount, statement.Statements.Count);
        }
    }
}
