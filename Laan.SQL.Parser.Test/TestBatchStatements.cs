using System;
using MbUnit.Framework;
using System.Collections.Generic;

namespace Laan.SQL.Parser.Test
{
    [TestFixture]
    public class TestBatchStatements
    {
        [Test]
        public void Test()
        {
            // Setup
            var sql = "SELECT * FROM Table SELECT * FROM OtherTable";

            // Exercise
            List<SelectStatement> statements = ParserFactory.Execute<SelectStatement>( sql );

            // Verify outcome
            Assert.AreEqual( 2, statements.Count );

        }
    }
}
