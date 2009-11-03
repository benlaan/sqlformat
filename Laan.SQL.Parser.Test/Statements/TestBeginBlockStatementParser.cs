using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestBeginBlockStatementParser
    {
        [Test]
        [Row( "begin select id from t end", 1 )]
        [Row( "begin select id from t select id from x end", 2 )]
        public void Test_Begin_End_Block( string sql, int statementCount )
        {
            // Exercise
            var statement = ParserFactory.Execute<BlockStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( statementCount, statement.Statements.Count );
        }
    }
}
