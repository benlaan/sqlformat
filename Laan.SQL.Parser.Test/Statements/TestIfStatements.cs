using System;
using System.Linq;

using MbUnit.Framework;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestIfStatements
    {
        [Test]
        [ExpectedException( typeof( SyntaxException ), "missing condition for IF" )]
        public void Declare_Statement_With_No_Variables_Should_Fail()
        {
            // Setup
            var sql = "IF";

            // Exercise
            ParserFactory.Execute<IfStatement>( sql );
        }

        [Test]
        [ExpectedException( typeof( SyntaxException ), "missing success block for IF" )]
        public void If_Statement_Without_Statement_Should_Fail()
        {
            // Setup
            var sql = "IF @A = 1";

            // Exercise
            ParserFactory.Execute<IfStatement>( sql );
        }

        [Test]
        public void If_Statement_With_Simple_Statement()
        {
            // Setup
            var sql = "IF @A = 1 SELECT 1";

            // Exercise
            var statement = ParserFactory.Execute<IfStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( typeof( CriteriaExpression ), statement.Condition.GetType() );
            Assert.AreEqual( typeof( SelectStatement ), statement.If.GetType() );
        }

        [Test]
        public void If_Statement_With_Else_Statement()
        {
            // Setup
            var sql = @"
                IF @A = 1 
                    SELECT Field 
                    FROM dbo.ATable 
                ELSE 
                    UPDATE dbo.ATable 
                       SET Field = 1 

                    WHERE ID = 10
            ";

            // Exercise
            var statement = ParserFactory.Execute<IfStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( typeof( CriteriaExpression ), statement.Condition.GetType() );
            Assert.AreEqual( typeof( SelectStatement ), statement.If.GetType() );
            Assert.AreEqual( typeof( UpdateStatement ), statement.Else.GetType() );
        }
    }
}
