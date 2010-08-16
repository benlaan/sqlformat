using System;
using System.Linq;
using MbUnit.Framework;
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
            var statement = ParserFactory.Execute<SetStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "@A", statement.Variable );
            Assert.AreEqual( "1", statement.Expression.Value );
        }
        
        [Test]
        public void Set_Variable_To_Complex_Expression()
        {
            // Setup
            var sql = "SET @A = (@B + 5) / 2";

            // Exercise
            var statement = ParserFactory.Execute<SetStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "@A", statement.Variable );
            Assert.AreEqual( "(@B + 5) / 2", statement.Expression.Value );
            Assert.IsTrue( statement.Expression is OperatorExpression );
        }
 
        [Test]
        public void Set_Variable_To_Select_Expression()
        {
            // Setup
            var sql = "SET @A = (SELECT TOP 1 Name FROM dbo.Names)";

            // Exercise
            var statement = ParserFactory.Execute<SetStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "@A", statement.Variable );
            Assert.IsTrue( statement.Expression is NestedExpression );
            NestedExpression nestedExpression = ( NestedExpression )statement.Expression;
            Assert.IsTrue( nestedExpression.Expression is SelectExpression );
        }
    }
}
