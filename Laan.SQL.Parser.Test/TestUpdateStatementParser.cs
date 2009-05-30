using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Laan.SQL.Parser;

namespace Laan.SQLParser.Test
{
    [TestFixture]
    public class TestUpdateStatementParser
    {
        [Test]
        public void Test_Basic_Update_Statement()
        {
            // Exercise
            UpdateStatement statement = ParserFactory.Execute<UpdateStatement>( "update dbo.table set field = 1" );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "dbo.table", statement.TableName );
            Assert.AreEqual( 1, statement.Fields.Count );

            Assert.AreEqual( "1", statement.Fields[ 0 ].Expression.Value );
        }

        [Test]
        public void Test_Update_Statement_With_Where_Clause()
        {
            // Exercise
            UpdateStatement statement = ParserFactory.Execute<UpdateStatement>( "update dbo.table set field = 1 where field <> 2" );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "dbo.table", statement.TableName );
            Assert.AreEqual( 1, statement.Fields.Count );

            Assert.AreEqual( "1", statement.Fields[ 0 ].Expression.Value );
            Assert.AreEqual( "field <> 2", statement.Where.Value );

            Assert.IsTrue( statement.Where is CriteriaExpression );
            CriteriaExpression criteriaExpression = ( CriteriaExpression )statement.Where;

            Assert.AreEqual( "field", criteriaExpression.Left.Value );
            Assert.AreEqual( "<>", criteriaExpression.Operator );
            Assert.AreEqual( "2", criteriaExpression.Right.Value );
        }

        [Test]
        public void Test_Update_Statement_With_From_Clause()
        {
            // Exercise
            UpdateStatement statement = ParserFactory.Execute<UpdateStatement>( 
                @"update t set field = 1 from dbo.table as t where field <> 2" 
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "t", statement.TableName );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "1", statement.Fields[ 0 ].Expression.Value );

            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "dbo.table", statement.From[ 0 ].Name );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias.Name );
            Assert.AreEqual( AliasType.As, statement.From[ 0 ].Alias.Type );

            Assert.IsTrue( statement.Where is CriteriaExpression );
            CriteriaExpression criteriaExpression = ( CriteriaExpression )statement.Where;

            Assert.AreEqual( "field <> 2", statement.Where.Value );
            Assert.AreEqual( "field", criteriaExpression.Left.Value );
            Assert.AreEqual( "<>", criteriaExpression.Operator );
            Assert.AreEqual( "2", criteriaExpression.Right.Value );
        }

        [Test]
        public void Test_Update_Statement_With_From_And_Join_Clauses()
        {
            // Exercise
            UpdateStatement statement = ParserFactory.Execute<UpdateStatement>(
                @"update t set field = 1 from dbo.table as t join dbo.other o on o.id = a.id where field <> 2"
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "t", statement.TableName );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "1", statement.Fields[ 0 ].Expression.Value );

            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "dbo.table", statement.From[ 0 ].Name );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias.Name );
            Assert.AreEqual( AliasType.As, statement.From[ 0 ].Alias.Type );

            Assert.AreEqual( 1, statement.Joins.Count );
            Assert.AreEqual( "dbo.other", statement.Joins[ 0 ].Name );
            Assert.AreEqual( "o", statement.Joins[ 0 ].Alias.Name );
            Assert.AreEqual( AliasType.Implicit, statement.Joins[ 0 ].Alias.Type );

            Assert.IsTrue( statement.Where is CriteriaExpression );
            CriteriaExpression criteriaExpression = ( CriteriaExpression )statement.Where;

            Assert.AreEqual( "field <> 2", statement.Where.Value );
            Assert.AreEqual( "field", criteriaExpression.Left.Value );
            Assert.AreEqual( "<>", criteriaExpression.Operator );
            Assert.AreEqual( "2", criteriaExpression.Right.Value );
        }
    }
}
