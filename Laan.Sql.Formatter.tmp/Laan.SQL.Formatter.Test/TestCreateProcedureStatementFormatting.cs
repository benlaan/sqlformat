using System;
using System.Linq;

using NUnit.Framework;

namespace Laan.Sql.Formatter.Test
{
    [TestFixture]
    public class TestCreateProcedureStatementFormatting : BaseFormattingTest
    {
        [Test]
        [TestCase("create")]
        [TestCase("alter")]
        public void Can_Format_Simple_Create_Procedure_Statement(string modificationType)
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute(String.Format(@"{0} PROCEDURE ben.MyProc AS SELECT * FROM [Table]", modificationType));

            // Verify outcome
            var expected = new[]
            {
                modificationType.ToUpper() + " PROCEDURE ben.MyProc",
                "AS",
                "SELECT *",
                "FROM [Table]"
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Create_Procedure_Statement_With_Block()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("CREATE PROCEDURE ben.MyProc AS BEGIN SELECT * FROM [Table] SELECT * FROM [Table] END");

            // Verify outcome
            var expected = new[]
            {
                "CREATE PROCEDURE ben.MyProc",
                "AS",
                "BEGIN",
                "",
                "    SELECT *",
                "    FROM [Table]",
                "",
                "    SELECT *",
                "    FROM [Table]",
                "",
                "END",
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Create_Procedure_Statement_With_Arguments_Without_Brackets()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("CREATE PROCEDURE ben.MyProc @Id INT, @Name VARCHAR(10) AS SELECT * FROM [Table]");

            // Verify outcome
            var expected = new[]
            {
                "CREATE PROCEDURE ben.MyProc",
                "    @Id   INT,",
                "    @Name VARCHAR(10)",
                "AS",
                "SELECT *",
                "FROM [Table]",
            };

            Compare(actual, expected);
        }

        [Test]
        public void Can_Format_Create_Procedure_Statement_With_Arguments_With_Brackets()
        {
            // Setup
            var sut = new FormattingEngine();

            // Exercise
            var actual = sut.Execute("CREATE PROCEDURE ben.MyProc (@Id INT, @Name VARCHAR(10)) AS SELECT * FROM [Table]");

            // Verify outcome
            var expected = new[]
            {
                "CREATE PROCEDURE ben.MyProc (",
                "    @Id   INT,",
                "    @Name VARCHAR(10)",
                ")",
                "AS",
                "SELECT *",
                "FROM [Table]",
            };

            Compare(actual, expected);
        }
    }
}
