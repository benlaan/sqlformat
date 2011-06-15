using System;
using System.Linq;
using NUnit.Framework;
using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestSetStatementParser
    {
        [Test]
        public void Set_Variable_To_Integer()
        {
            // Setup
            var sql = "SET @A = 1";

            // Exercise
            var statement = ParserFactory.Execute<SetVariableStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "@A", statement.Variable );
            Assert.AreEqual( "1", statement.Assignment.Value );
        }
        
        [Test]
        public void Set_Variable_To_Complex_Expression()
        {
            // Setup
            var sql = "SET @A = (@B + 5) / 2";

            // Exercise
            var statement = ParserFactory.Execute<SetVariableStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "@A", statement.Variable );
            Assert.AreEqual( "(@B + 5) / 2", statement.Assignment.Value );
            Assert.IsTrue( statement.Assignment is OperatorExpression );
        }
 
        [Test]
        public void Set_Variable_To_Select_Expression()
        {
            // Setup
            var sql = "SET @A = (SELECT TOP 1 Name FROM dbo.Names)";

            // Exercise
            var statement = ParserFactory.Execute<SetVariableStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "@A", statement.Variable );
            Assert.IsTrue( statement.Assignment is NestedExpression );
            NestedExpression nestedExpression = ( NestedExpression )statement.Assignment;
            Assert.IsTrue( nestedExpression.Expression is SelectExpression );
        }

        string[] Options = new[]
        {
            "CONCAT_NULL_YIELDS_NULL",
            "CURSOR_CLOSE_ON_COMMIT",
            "QUOTED_IDENTIFIER",
            "ARITHABORT",
            "ARITHIGNORE",
            "FMTONLY",
            "NOCOUNT",
            "NOEXEC",
            "NUMERIC_ROUNDABORT",
            "PARSEONLY",
            "ANSI_DEFAULTS",
            "ANSI_NULL_DFLT_OFF",
            "ANSI_NULL_DFLT_ON",
            "ANSI_NULLS",
            "ANSI_PADDING",
            "ANSI_WARNINGS",
            "FORCEPLAN",
            "SHOWPLAN_ALL",
            "SHOWPLAN_TEXT",
            "SHOWPLAN_XML",
            "IMPLICIT_TRANSACTIONS",
            "REMOTE_PROC_TRANSACTIONS",
            "XACT_ABORT",
            "STATISTICS IO",
            "STATISTICS XML",
            "STATISTICS PROFILE",
            "STATISTICS TIME",
        };

        [ Test ]
        [ TestCaseSource( "Options" ) ]
        public void On_Off_Set_Options( string option)
        {
            foreach ( bool value in new[] { false, true } )
            {
                // Setup
                var sql = String.Format( "SET {0} {1}", option, value ? "ON" : "OFF" );

                // Exercise
                var statement = ParserFactory.Execute<SetOptionStatement>( sql ).First();

                // Verify outcome
                Assert.IsNotNull( statement );
                Assert.AreEqual(option, statement.Option);
                Assert.AreEqual( value, statement.Assignment.Value == "ON" );
            }
        }

        [Test]
        public void Set_DateFirst_With_Constant()
        {
            // Setup
            var sql = String.Format( "SET DATEFIRST 10");

            // Exercise
            var statement = ParserFactory.Execute<SetDateFirstStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "10", statement.Assignment.Value );
        }

        [Test]
        public void Set_DateFirst_With_Variable()
        {
            // Setup
            var sql = String.Format( "SET DATEFIRST @dateFirst");

            // Exercise
            var statement = ParserFactory.Execute<SetDateFirstStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "@dateFirst", statement.Assignment.Value );
        }
        
        [Test]
        public void Set_DateFormat_With_Constant()
        {
            // Setup
            var sql = String.Format( "SET DATEFORMAT 'dd/mm/yyyy'");

            // Exercise
            var statement = ParserFactory.Execute<SetDateFormatStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "'dd/mm/yyyy'", statement.Assignment.Value );
        }

        [Test]
        public void Set_DateFormat_With_Variable()
        {
            // Setup
            var sql = String.Format( "SET DATEFORMAT @Dateformat");

            // Exercise
            var statement = ParserFactory.Execute<SetDateFormatStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "@Dateformat", statement.Assignment.Value );
        }

        [ Test ]
        [ TestCase( "LOW" ) ]
        [ TestCase( "NORMAL" ) ]
        [ TestCase( "HIGH" ) ]
        [ TestCase( "-10" ) ]
        [ TestCase( "10" ) ]
        [ TestCase( "@Priority" ) ]
        public void Set_Deadlock_Priority_With_Variable(string option)
        {
            // Setup
            var sql = String.Format("SET DEADLOCK_PRIORITY {0}", option);

            // Exercise
            var statement = ParserFactory.Execute<SetDeadlockPriorityStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( option, statement.Assignment.Value );
        }

        /*
            Still TODO:
        
        SET FIPS_FLAGGER ENTRY | FULL | INTERMEDIATE | OFF
        SET IDENTITY_INSERT  [ database_name . [ schema_name ] . ] table { ON | OFF }
        SET LANGUAGE { [ N ] 'language' | @language_var } 
        SET OFFSETS  keyword_list { ON | OFF }
        SET QUERY_GOVERNOR_COST_LIMIT value
        SET ROWCOUNT { number | @number_var } 
        SET TRANSACTION ISOLATION LEVEL { READ UNCOMMITTED | READ COMMITTED | REPEATABLE READ | SNAPSHOT | SERIALIZABLE }
        SET LOCK_TIMEOUT timeout_period
        SET TEXTSIZE { number } 
        */

        [Test]
        [ExpectedException(typeof(SyntaxException))]
        public void Set_Boolean_Option_With_Invalid_Value()
        {
            // Setup
            var sql = "SET XACT_ABORT BLAH";

            // Exercise
            ParserFactory.Execute<SetOptionStatement>( sql );
        }

        [Test]
        [ExpectedException(typeof(SyntaxException))]
        public void Set_Statistics_With_Invalid_Value()
        {
            // Setup
            var sql = "SET STATISTICS BLAH OFF";

            // Exercise
            ParserFactory.Execute<SetOptionStatement>( sql );
        }

        [Test]
        [ExpectedException( typeof( ParserNotImplementedException ) )]
        public void Set_Unknown_Option()
        {
            // Setup
            var sql = "SET BLAH OFF";

            // Exercise
            ParserFactory.Execute<SetOptionStatement>( sql );
        }
    }

}
