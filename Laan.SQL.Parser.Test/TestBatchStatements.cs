using System;
using System.Collections.Generic;
using System.Linq;

using MbUnit.Framework;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestBatchStatements
    {
        [Test]
        public void Test_Implicit_Termination_Of_Statement()
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
