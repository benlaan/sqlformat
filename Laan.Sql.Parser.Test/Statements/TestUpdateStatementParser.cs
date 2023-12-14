﻿using System;
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
    public class TestUpdateStatementParser
    {
        [Test]
        public void Basic_Update_Statement()
        {
            // Exercise
            UpdateStatement statement = ParserFactory.Execute<UpdateStatement>( "update dbo.table set field = 1" ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "dbo.table", statement.TableName );
            Assert.AreEqual( 1, statement.Fields.Count );

            Assert.AreEqual( "1", statement.Fields[ 0 ].Expression.Value );
        }

        [Test]
        public void Update_Statement_With_Where_Clause()
        {
            // Exercise
            UpdateStatement statement = ParserFactory.Execute<UpdateStatement>( "update dbo.table set field = 1 where field <> 2" ).First();

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
        public void Update_Statement_With_From_And_Where_Clause()
        {
            // Exercise
            UpdateStatement statement = ParserFactory.Execute<UpdateStatement>( 
                @"update t set field = 1 from dbo.table as t where field <> 2"
            ).First();

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
        public void Update_Statement_With_From_Join_And_Where_Clauses()
        {
            // Exercise
            UpdateStatement statement = ParserFactory.Execute<UpdateStatement>(
                @"update t set field = 1 from dbo.table as t join dbo.other o on o.id = a.id where field <> 2"
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "t", statement.TableName );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "1", statement.Fields[0].Expression.Value );

            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "dbo.table", statement.From[0].Name );
            Assert.AreEqual( "t", statement.From[0].Alias.Name );
            Assert.AreEqual( AliasType.As, statement.From[0].Alias.Type );

            Assert.AreEqual( 1, statement.From[0].Joins.Count );
            Join join = statement.From[0].Joins[0];
            Assert.AreEqual( "dbo.other", join.Name );
            Assert.AreEqual( "o", join.Alias.Name );
            Assert.AreEqual( AliasType.Implicit, join.Alias.Type );

            Assert.IsTrue( statement.Where is CriteriaExpression );
            CriteriaExpression criteriaExpression = ( CriteriaExpression )statement.Where;

            Assert.AreEqual( "field <> 2", statement.Where.Value );
            Assert.AreEqual( "field", criteriaExpression.Left.Value );
            Assert.AreEqual( "<>", criteriaExpression.Operator );
            Assert.AreEqual( "2", criteriaExpression.Right.Value );
        }

        [Test]
        public void Update_Statement_With_Top_N_Clause()
        {
            // Exercise
            UpdateStatement statement = ParserFactory.Execute<UpdateStatement>(
                @"update top (10) t set field = 1 from dbo.table as t"
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "t", statement.TableName );
            Assert.IsNotNull( statement.Top );
            Assert.AreEqual( "(10)", statement.Top.Expression.Value );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "1", statement.Fields[ 0 ].Expression.Value );

            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "dbo.table", statement.From[ 0 ].Name );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias.Name );
            Assert.AreEqual( AliasType.As, statement.From[ 0 ].Alias.Type );
        }

        [Test]
        public void Update_Statement_With_Top_N_Clause_With_Missing_Value()
        {
            // Exercise
            try
            {
                UpdateStatement statement = ParserFactory.Execute<UpdateStatement>(
                    @"update top () t set field = 1 from dbo.table as t join dbo.other o on o.id = a.id where field <> 2"
                ).First();
            }
            catch (SyntaxException ex)
            {
                Assert.AreEqual("expected alpha, numeric, or variable, found )", ex.Message);
            }
        }
    }
}
