using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MbUnit.Framework;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestSelectStatementParser
    {
        [Test]
        [ExpectedException( typeof( ParserNotImplementedException ), Message = "No parser exists for statement type: merge" )]
        public void TestNoParserException()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "merge from table" ).First();
        }

        [Test]
        public void Select_StarField_Only()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select * from dbo.table" ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "*", statement.Fields[ 0 ].Expression.Value );
            Assert.IsNull( statement.Top );
            Assert.AreEqual( "dbo.table", statement.From[ 0 ].Name );
        }

        [Test]
        [ExpectedException( typeof( ExpectedTokenNotFoundException ), Message = "Expected: 'BY' but found: 'field' at Row: 1, Col: 27" )]
        public void TestExpectedError()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select * from table order field" ).First();
        }

        [Test]
        [ExpectedException( typeof( SyntaxException ), Message = "Identifier expected" )]
        public void Should_Fail_With_ExpectedSyntaxError_When_Using_Reserved_Word_As_Alias()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select * from table select" ).First();
        }

        [Test]
        public void Select_Top_10_StarField()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select top 10 * from table" ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "*", statement.Fields[ 0 ].Expression.Value );
            Assert.AreEqual( "10", statement.Top.Expression.Value );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
        }

        [Test]
        public void Select_Top_50_Percent()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select top 50 percent * from table" ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "*", statement.Fields[ 0 ].Expression.Value );
            Assert.AreEqual( "50", statement.Top.Expression.Value );
            Assert.IsTrue( statement.Top.Percent );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
        }

        [Test]
        [ExpectedException( typeof( SyntaxException ), Message = "Expected integer but found: '*'" )]
        public void Select_Top_Missing_Top_Param_StarField()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select top * from table" ).First();
        }


        [Test]
        public void Select_Without_Alias_And_Where_Clause()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select fielda from table where a > 1" ).First();
            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "fielda", statement.Fields[ 0 ].Expression.Value );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
            Assert.AreEqual( "a > 1", statement.Where.Value );
        }

        [Test]
        public void Select_Distinct_Top_10_StarField()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select distinct top 10 * from table" ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "*", statement.Fields[ 0 ].Expression.Value );
            Assert.IsNotNull( statement.Top );
            Assert.AreEqual( "10", statement.Top.Expression.Value );
            Assert.IsTrue( statement.Distinct );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
        }

        [Test]
        public void Select_Multiple_Fields()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select fielda, field2, fie3ld from table" ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 3, statement.Fields.Count );

            var expectedFields = new string[] { "fielda", "field2", "fie3ld" };
            int index = 0;
            foreach ( var field in expectedFields )
                Assert.AreEqual( field, statement.Fields[ index++ ].Expression.Value );

            Assert.AreEqual( "table", statement.From[ 0 ].Name );
        }

        [Test]
        public void Select_With_Aliased_Table_With_As()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select * from table as t" ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias.Name );
            Assert.AreEqual( AliasType.As, statement.From[ 0 ].Alias.Type );
        }

        [Test]
        public void Select_With_Aliased_Table_Without_As()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select * from table t" ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias.Name );
            Assert.AreEqual( AliasType.Implicit, statement.From[ 0 ].Alias.Type );
        }

        [Test]
        public void Select_With_Two_Aliased_Table_With_As()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select * from table1 as t1, table2 as t2" ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 2, statement.From.Count );
            Assert.AreEqual( "table1", statement.From[ 0 ].Name );
            Assert.AreEqual( "t1", statement.From[ 0 ].Alias.Name );
            Assert.AreEqual( "table2", statement.From[ 1 ].Name );
            Assert.AreEqual( "t2", statement.From[ 1 ].Alias.Name );
        }

        [Test]
        public void Select_With_Two_Aliased_Table_Without_As()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select * from table1 t1, table2 t2 " ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 2, statement.From.Count );
            Assert.AreEqual( "table1", statement.From[ 0 ].Name );
            Assert.AreEqual( "t1", statement.From[ 0 ].Alias.Name );
            Assert.AreEqual( "table2", statement.From[ 1 ].Name );
            Assert.AreEqual( "t2", statement.From[ 1 ].Alias.Name );
        }

        [Test]
        public void Select_Multiple_Fields_With_Aliases()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select field, fielda a, field2 as b, alias = fie3ld from table" ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 4, statement.Fields.Count );

            Assert.AreEqual( "field", statement.Fields[ 0 ].Expression.Value );
            Assert.IsNull( statement.Fields[ 0 ].Alias.Name );

            Assert.AreEqual( "fielda", statement.Fields[ 1 ].Expression.Value );
            Assert.AreEqual( "a", statement.Fields[ 1 ].Alias.Name );
            Assert.AreEqual( AliasType.Implicit, statement.Fields[ 1 ].Alias.Type );

            Assert.AreEqual( "field2", statement.Fields[ 2 ].Expression.Value );
            Assert.AreEqual( "b", statement.Fields[ 2 ].Alias.Name );
            Assert.AreEqual( AliasType.As, statement.Fields[ 2 ].Alias.Type );

            Assert.AreEqual( "fie3ld", statement.Fields[ 3 ].Expression.Value );
            Assert.AreEqual( "alias", statement.Fields[ 3 ].Alias.Name );
            Assert.AreEqual( AliasType.Equals, statement.Fields[ 3 ].Alias.Type );
        }

        [Test]
        public void Select_Multiple_Fields_With_Table_Alias_Prefix()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select t1.fielda, t1.field2, SomeDb.dbo.fie3ld from table as t1" ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 3, statement.Fields.Count );

            var expectedFields = new string[] { "t1.fielda", "t1.field2", "SomeDb.dbo.fie3ld" };
            int index = 0;
            foreach ( var field in expectedFields )
            {
                Field column = statement.Fields[ index++ ];
                Assert.IsTrue( column.Expression is Expression );
                Assert.AreEqual( field, column.Expression.Value );
            }

            Assert.AreEqual( "table", statement.From[ 0 ].Name );
        }

        [Test]
        public void Select_With_Inner_Join_Condition()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( @"

                select fielda 

                from table1 t1 

                join table2 as t2
                  on t1.field1 = t2.field2
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );

            // Test From
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table1", statement.From[ 0 ].Name );
            Assert.AreEqual( "t1", statement.From[ 0 ].Alias.Name );
            Assert.AreEqual( AliasType.Implicit, statement.From[ 0 ].Alias.Type );

            // Test Join
            Assert.AreEqual( 1, statement.From[ 0 ].Joins.Count );

            Join join = statement.From[ 0 ].Joins[ 0 ];

            Assert.AreEqual( "table2", join.Name );
            Assert.AreEqual( "t2", join.Alias.Name );
            Assert.AreEqual( AliasType.As, join.Alias.Type );

            Assert.AreEqual( JoinType.Join, join.Type );

            CriteriaExpression expr = join.Condition as CriteriaExpression;

            Assert.AreEqual( "=", expr.Operator );
            Assert.IsTrue( expr.Left is IdentifierExpression );

            var leftExpression = ( IdentifierExpression )expr.Left;

            Assert.AreEqual( "t1", leftExpression.Parts[ 0 ] );
            Assert.AreEqual( "field1", leftExpression.Parts[ 1 ] );
            Assert.AreEqual( "t1.field1", expr.Left.Value );

            var rightExpression = ( IdentifierExpression )expr.Right;

            Assert.AreEqual( "t2", rightExpression.Parts[ 0 ] );
            Assert.AreEqual( "field2", rightExpression.Parts[ 1 ] );
            Assert.AreEqual( "t2.field2", expr.Right.Value );

            Assert.AreEqual( "t1.field1 = t2.field2", expr.Value );
        }

        [Test]
        public void Select_With_Inner_Join_Condition_With_Complex_Expressions()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( @"

                select fielda 

                from table1 t1 

                join table2 as t2 
                  on (t1.field1 + 150) / 12 = ( t2.field2 + t1.fielda )
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );

            // Test From
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table1", statement.From[ 0 ].Name );
            Assert.AreEqual( "t1", statement.From[ 0 ].Alias.Name );

            // Test Join
            Assert.AreEqual( 1, statement.From[ 0 ].Joins.Count );

            Join join = statement.From[ 0 ].Joins[ 0 ];

            Assert.AreEqual( "table2", join.Name );
            Assert.AreEqual( "t2", join.Alias.Name );

            Assert.AreEqual( JoinType.Join, join.Type );

            CriteriaExpression expr = join.Condition as CriteriaExpression;
            Assert.AreEqual( "=", expr.Operator );
            Assert.IsTrue( expr.Left is OperatorExpression );

            var leftExpression = ( OperatorExpression )expr.Left;
            Assert.AreEqual( "/", leftExpression.Operator );

            Assert.IsTrue( leftExpression.Left is NestedExpression );
            var leftLeftExpression = ( NestedExpression )leftExpression.Left;

            Assert.IsTrue( leftLeftExpression.Expression is OperatorExpression );

            OperatorExpression leftOperatorExpression = ( OperatorExpression )leftLeftExpression.Expression;

            Assert.AreEqual( "t1.field1", leftOperatorExpression.Left.Value );
            Assert.AreEqual( "+", leftOperatorExpression.Operator );
            Assert.AreEqual( "150", leftOperatorExpression.Right.Value );

            IdentifierExpression rightIdentifierExpression = ( IdentifierExpression )leftExpression.Right;

            Assert.AreEqual( "12", rightIdentifierExpression.Value );

            //var rightExpression = ( NestedExpression )expr.Right;
            //Assert.AreEqual( "t2", rightExpression.Parts[ 0 ] );
            //Assert.AreEqual( "field2", rightExpression.Parts[ 1 ] );
            //Assert.AreEqual( "t2.field2", expr.Right.Value );
        }

        [Test]
        public void Select_With_Inner_Join_Condition_Within_Brackets()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( @"

                select fielda 

                from table1 t1 

                join table2 as t2 
                  on (t1.field1 = t2.fielda )
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );

            // Test From
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table1", statement.From[ 0 ].Name );
            Assert.AreEqual( "t1", statement.From[ 0 ].Alias.Name );

            // Test Join
            Assert.AreEqual( 1, statement.From[ 0 ].Joins.Count );

            Join join = statement.From[ 0 ].Joins[ 0 ];

            Assert.AreEqual( "table2", join.Name );
            Assert.AreEqual( "t2", join.Alias.Name );

            Assert.AreEqual( JoinType.Join, join.Type );

            Assert.IsTrue( join.Condition is NestedExpression );
            NestedExpression expr = join.Condition as NestedExpression;

            Assert.IsTrue( expr.Expression is CriteriaExpression );
            CriteriaExpression nestedExpression = expr.Expression as CriteriaExpression;
            Assert.AreEqual( "=", nestedExpression.Operator );
            Assert.IsTrue( nestedExpression.Left is IdentifierExpression );
            Assert.AreEqual( "t1.field1", nestedExpression.Left.Value );
            Assert.IsTrue( nestedExpression.Right is IdentifierExpression );
            Assert.AreEqual( "t2.fielda", nestedExpression.Right.Value );
        }

        [Test]
        public void Select_With_Where_Condition()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( @"

                select fielda 
                from table1 t1
                where t1.fieldb = (select top 1 Name from table)
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            CriteriaExpression expr = statement.Where as CriteriaExpression;
            Assert.AreEqual( "=", expr.Operator );

            Assert.IsTrue( expr.Left is IdentifierExpression );

            Assert.IsTrue( expr.Right is NestedExpression );
            NestedExpression nestedExpression = ( NestedExpression )expr.Right;
            Assert.IsTrue( nestedExpression.Expression is SelectExpression );
        }

        [Test]
        public void Select_With_Multi_Part_Where_Condition()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( @"

                select fielda 
                from table1 t1
                where t1.fieldb = t1.fieldc
                  and t1.fieldd = 0
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.IsTrue( statement.Where is CriteriaExpression );

            CriteriaExpression criteriaExpression = ( CriteriaExpression )statement.Where;
            Assert.AreEqual( "and", criteriaExpression.Operator, StringComparison.CurrentCultureIgnoreCase );
            Assert.IsTrue( criteriaExpression.Left is CriteriaExpression );

            CriteriaExpression leftCriteriaExpression = ( CriteriaExpression )criteriaExpression.Left;
            Assert.AreEqual( "t1.fieldb", leftCriteriaExpression.Left.Value );
            Assert.AreEqual( "t1.fieldc", leftCriteriaExpression.Right.Value );

            CriteriaExpression rightCriteriaExpression = ( CriteriaExpression )criteriaExpression.Right;
            Assert.AreEqual( "t1.fieldd", rightCriteriaExpression.Left.Value );
            Assert.AreEqual( "0", rightCriteriaExpression.Right.Value );
        }

        [Test]
        public void Select_With_Multi_Part_Where_Condition_With_Nested_Conditions()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( @"

                select fielda 
                from table1 t1
                where ( t1.fieldd = 0 or t1.fieldc < 10 )
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.IsTrue( statement.Where is NestedExpression );

            NestedExpression nestedExpression = ( NestedExpression )statement.Where;

            CriteriaExpression criteriaExpression = ( CriteriaExpression )nestedExpression.Expression;
            Assert.AreEqual( "or", criteriaExpression.Operator, StringComparison.CurrentCultureIgnoreCase );
            Assert.IsTrue( criteriaExpression.Left is CriteriaExpression );

            CriteriaExpression leftCriteriaExpression = ( CriteriaExpression )criteriaExpression.Left;
            Assert.AreEqual( "t1.fieldd", leftCriteriaExpression.Left.Value );
            Assert.AreEqual( "=", leftCriteriaExpression.Operator );
            Assert.AreEqual( "0", leftCriteriaExpression.Right.Value );

            CriteriaExpression rightCriteriaExpression = ( CriteriaExpression )criteriaExpression.Right;
            Assert.AreEqual( "t1.fieldc", rightCriteriaExpression.Left.Value );
            Assert.AreEqual( "<", rightCriteriaExpression.Operator );
            Assert.AreEqual( "10", rightCriteriaExpression.Right.Value );
        }

        [Test]
        public void Select_With_Multi_Part_Where_Condition_With_Several_Nested_Conditions()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( @"

                select fielda 
                from table1 t1
                where ( t1.fieldd = 0 or t1.fieldc < 10 )
                  and ( ( select top 1 field from table1 ) = ( select top 1 fieldb from table2 ) )
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
        }

        [Test]
        public void Select_From_Derived_View()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( @"
                select * from (select field from table x) as t
            " ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias.Name );
            Assert.IsTrue( statement.From[ 0 ] is DerivedTable );

            DerivedTable derivedTable = ( DerivedTable )statement.From[ 0 ];
            Assert.AreEqual( "field", derivedTable.SelectStatement.Fields[ 0 ].Expression.Value );
            Assert.AreEqual( "table", derivedTable.SelectStatement.From[ 0 ].Name );
            Assert.AreEqual( "x", derivedTable.SelectStatement.From[ 0 ].Alias.Name );
        }

        [Test]
        public void Select_With_Order_By()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( @"
                select * from table order by field1 desc, field2 asc
            " ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
            Assert.AreEqual( 2, statement.OrderBy.Count );
            Assert.AreEqual( Laan.Sql.Parser.Entities.SortOrder.Descending, ( statement.OrderBy[ 0 ] as SortedField ).SortOrder );
            Assert.AreEqual( Laan.Sql.Parser.Entities.SortOrder.Ascending, ( statement.OrderBy[ 1 ] as SortedField ).SortOrder );
        }

        [Test]
        public void Select_With_Group_By()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( @"
                select * from table group by field1, field2
            " ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
            Assert.AreEqual( 2, statement.GroupBy.Count );
        }

        [Test]
        public void Select_With_Group_By_With_Having()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( @"
                select * from table group by field1, field2 having sum( field2 ) > 0
            " ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
            Assert.AreEqual( 2, statement.GroupBy.Count );
            Assert.IsTrue( statement.Having is CriteriaExpression );

            CriteriaExpression operatorExpression = ( CriteriaExpression )statement.Having;
            Assert.AreEqual( ">", operatorExpression.Operator );

            // verify that the left part of the criteria expression is a function expression
            Assert.IsTrue( operatorExpression.Left is FunctionExpression );
            FunctionExpression functionExpression = ( FunctionExpression )operatorExpression.Left;
            Assert.AreEqual( "sum", functionExpression.Name );
            Assert.AreEqual( 1, functionExpression.Arguments.Count );
            Assert.AreEqual( "field2", functionExpression.Arguments[ 0 ].Value );

            // the right branch is a simple IdentifierExpression
            Assert.IsTrue( operatorExpression.Right is IdentifierExpression );
            Assert.AreEqual( "0", operatorExpression.Right.Value );
        }

        [Test]
        public void Select_With_Derived_Join()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( @"
                select * from table t join ( select field from other o ) as o1 on o.a = t.a
            " ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias.Name );
            Assert.IsTrue( statement.From[ 0 ].Joins[ 0 ] is DerivedJoin );

            DerivedJoin derivedJoin = ( DerivedJoin )statement.From[ 0 ].Joins[ 0 ];
            Assert.AreEqual( "field", derivedJoin.SelectStatement.Fields[ 0 ].Expression.Value );
            Assert.AreEqual( "other", derivedJoin.SelectStatement.From[ 0 ].Name );
            Assert.AreEqual( "o", derivedJoin.SelectStatement.From[ 0 ].Alias.Name );
            Assert.AreEqual( "o1", derivedJoin.Alias.Name );
        }

        [Test]
        [Row( "union", SetType.Union )]
        [Row( "union all", SetType.UnionAll )]
        [Row( "except", SetType.Except )]
        [Row( "intersect", SetType.Intersect )]
        public void Select_With_Union( string setOperator, SetType type )
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( String.Format( @"
                select id, name from table1 {0} select id, name from table2 order by id", setOperator ) ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.From.Count );
            Assert.IsTrue( statement.SetOperation != null );
            Assert.AreEqual( type, statement.SetOperation.Type );
            Assert.IsTrue( statement.SetOperation.Statement != null );
            Assert.IsTrue( statement.SetOperation.Statement is SelectStatement );
            var select = statement.SetOperation.Statement as SelectStatement;
            Assert.AreEqual( "id", select.OrderBy.First().Expression.Value );
        }

        [Test]
        public void Select_With_Into_Clause()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( 
                "select id, name into #T from table" 
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 2, statement.Fields.Count );
            Assert.AreEqual( "#T", statement.Into );
        }
    
        [Test]
        public void Select_With_Multiple_From_Tables()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( 
                "select id from table1, table2" 
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 2, statement.From.Count );
        }

        [Test]
        public void Select_With_Multiple_From_Tables_With_Joins()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( 
                "select id from table1 t1 join other1 o1 on t1.id = o1.id, table2 t2 join other2 o2 on o2.id = j2.id" 
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 2, statement.From.Count );
        }
    }
}
