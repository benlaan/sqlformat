using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestBatchStatements
    {
        [Test]
        public void Implicit_Termination_Of_Statement()
        {
            // Setup
            var sql = "SELECT * FROM Table SELECT * FROM OtherTable";

            // Exercise
            var statements = ParserFactory.Execute<SelectStatement>(sql);

            // Verify outcome
            Assert.AreEqual(2, statements.Count);
        }

        [Test]
        public void Explicit_Termination_Of_Statement()
        {
            // Setup
            var sql = "SELECT * FROM Table; SELECT * FROM OtherTable";

            // Exercise
            var statements = ParserFactory.Execute<SelectStatement>(sql);

            // Verify outcome
            Assert.AreEqual(2, statements.Count);
            Assert.IsTrue(statements[0].Terminated);
            Assert.IsFalse(statements[1].Terminated);
        }

        [Test]
        public void Explicit_Termination_Of_Create_Table_Statement()
        {
            // Setup
            var sql = "CREATE TABLE T ( ID INT);";

            // Exercise
            var statements = ParserFactory.Execute<CreateTableStatement>(sql);

            // Verify outcome
            Assert.AreEqual(1, statements.Count);
            Assert.IsTrue(statements[0].Terminated);
        }
    }
}
