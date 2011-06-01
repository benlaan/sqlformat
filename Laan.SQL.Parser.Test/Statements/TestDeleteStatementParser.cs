using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestDeleteStatementParser
    {
        [Test]
        public void Basic_Delete_Statement()
        {
            // Exercise
            DeleteStatement statement = ParserFactory.Execute<DeleteStatement>( "delete from dbo.table" ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "dbo.table", statement.From.First().Name );
            Assert.AreEqual( null, statement.TableName );
        }

        [Test]
        public void Delete_Statement_With_Where_Clause()
        {
            // Exercise
            DeleteStatement statement = ParserFactory.Execute<DeleteStatement>( "delete from dbo.table where field <> 2" ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( null, statement.TableName );
            Assert.AreEqual( "dbo.table", statement.From.First().Name );

            Assert.IsTrue( statement.Where is CriteriaExpression );
            CriteriaExpression criteriaExpression = ( CriteriaExpression )statement.Where;

            Assert.AreEqual( "field", criteriaExpression.Left.Value );
            Assert.AreEqual( "<>", criteriaExpression.Operator );
            Assert.AreEqual( "2", criteriaExpression.Right.Value );
        }

        [Test]
        public void Delete_Statement_With_From_And_Where_Clause()
        {
            // Exercise
            DeleteStatement statement = ParserFactory.Execute<DeleteStatement>( 
                @"delete t from dbo.table as t where field <> 2"
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "t", statement.TableName );

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
        public void Delete_Statement_With_From_Join_And_Where_Clauses()
        {
            // Exercise
            DeleteStatement statement = ParserFactory.Execute<DeleteStatement>(
                @"delete t from dbo.table as t join dbo.other o on o.id = a.id where field <> 2"
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "t", statement.TableName );

            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "dbo.table", statement.From[ 0 ].Name );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias.Name );
            Assert.AreEqual( AliasType.As, statement.From[ 0 ].Alias.Type );

            Assert.AreEqual( 1, statement.From[ 0 ].Joins.Count );
            Assert.AreEqual( "dbo.other", statement.From[ 0 ].Joins[ 0 ].Name );
            Assert.AreEqual( "o", statement.From[ 0 ].Joins[ 0 ].Alias.Name );
            Assert.AreEqual( AliasType.Implicit, statement.From[ 0 ].Joins[ 0 ].Alias.Type );

            Assert.IsTrue( statement.Where is CriteriaExpression );
            CriteriaExpression criteriaExpression = ( CriteriaExpression )statement.Where;

            Assert.AreEqual( "field <> 2", statement.Where.Value );
            Assert.AreEqual( "field", criteriaExpression.Left.Value );
            Assert.AreEqual( "<>", criteriaExpression.Operator );
            Assert.AreEqual( "2", criteriaExpression.Right.Value );
        }

        [Test]
        public void Delete_Statement_With_Top_N_Clause()
        {
            // Exercise
            DeleteStatement statement = ParserFactory.Execute<DeleteStatement>(
                @"delete top (10) t from dbo.table as t"
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "t", statement.TableName );
            Assert.IsNotNull( statement.Top );
            Assert.AreEqual( "10", statement.Top.Expression.Value );

            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "dbo.table", statement.From[ 0 ].Name );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias.Name );
            Assert.AreEqual( AliasType.As, statement.From[ 0 ].Alias.Type );
        }

        [Test]
        [ExpectedException( typeof( SyntaxException ), ExpectedMessage = "expected alpha, numeric, or variable, found )" )]
        public void Delete_Statement_With_Top_N_Clause_With_Missing_Value()
        {
            // Exercise
            DeleteStatement statement = ParserFactory.Execute<DeleteStatement>(
                @"delete top () t from dbo.table as t join dbo.other o on o.id = a.id where field <> 2"
            ).First();
        }
    }
}
