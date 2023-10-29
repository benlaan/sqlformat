using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestSelectStatementParser
    {
        [Test]
        public void TestNoParserException()
        {
            // Exercise
            Assert.Catch<ParserNotImplementedException>(
                () => ParserFactory.Execute<SelectStatement>("merge from table").First(),
                "No parser exists for statement type: merge"
            );
        }

        [Test]
        public void Select_StarField_Only()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select * from dbo.table").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.Fields.Count);
            Assert.AreEqual("*", statement.Fields[0].Expression.Value);
            Assert.IsNull(statement.Top);
            Assert.AreEqual("dbo.table", statement.From[0].Name);
        }

        [Test]
        public void TestExpectedError()
        {
            // Exercise
            Assert.Catch<ExpectedTokenNotFoundException>(
                () => ParserFactory.Execute<SelectStatement>("select * from table order field").First(),
                "Expected: 'BY' but found: 'field' at Row: 1, Col: 27"
            );
        }

        [Test]
        public void Should_Fail_With_ExpectedSyntaxError_When_Using_Reserved_Word_As_Alias()
        {
            // Exercise
            Assert.Catch<SyntaxException>(
                () => ParserFactory.Execute<SelectStatement>("select * from table select").First(),
                "Identifier expected"
            );
        }

        [Test]
        public void Select_Top_10_StarField()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select top 10 * from table").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.Fields.Count);
            Assert.AreEqual("*", statement.Fields[0].Expression.Value);
            Assert.AreEqual("10", statement.Top.Expression.Value);
            Assert.AreEqual("table", statement.From[0].Name);
        }

        [Test]
        public void Select_Top_50_Percent()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select top 50 percent * from table").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.Fields.Count);
            Assert.AreEqual("*", statement.Fields[0].Expression.Value);
            Assert.AreEqual("50", statement.Top.Expression.Value);
            Assert.IsTrue(statement.Top.Percent);
            Assert.AreEqual("table", statement.From[0].Name);
        }

        [Test]
        public void Select_Top_50_Percent_With_Brackets()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select top (50) percent * from table").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.Fields.Count);
            Assert.AreEqual("*", statement.Fields[0].Expression.Value);
            Assert.AreEqual("(50)", statement.Top.Expression.Value);
            Assert.IsTrue(statement.Top.Percent);
            Assert.AreEqual("table", statement.From[0].Name);
        }

        [Test]
        public void Select_Top_Missing_Top_Param_StarField()
        {
            // Exercise
            Assert.Catch<SyntaxException>(
                () => ParserFactory.Execute<SelectStatement>("select top * from table").First(),
                "Expected integer but found: '*'"
            );
        }

        [Test]
        public void Select_Without_Alias_And_Where_Clause()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select fielda from table where a > 1").First();
            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.Fields.Count);
            Assert.AreEqual("fielda", statement.Fields[0].Expression.Value);
            Assert.AreEqual("table", statement.From[0].Name);
            Assert.AreEqual("a > 1", statement.Where.Value);
        }

        [Test]
        public void Select_Distinct_Top_10_StarField()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select distinct top 10 * from table").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.Fields.Count);
            Assert.AreEqual("*", statement.Fields[0].Expression.Value);
            Assert.IsNotNull(statement.Top);
            Assert.AreEqual("10", statement.Top.Expression.Value);
            Assert.IsTrue(statement.Distinct);
            Assert.AreEqual("table", statement.From[0].Name);
        }

        [Test]
        public void Select_Multiple_Fields()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select fielda, field2, fie3ld from table").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(3, statement.Fields.Count);

            var expectedFields = new string[] { "fielda", "field2", "fie3ld" };
            int index = 0;
            foreach (var field in expectedFields)
                Assert.AreEqual(field, statement.Fields[index++].Expression.Value);

            Assert.AreEqual("table", statement.From[0].Name);
        }

        [Test]
        public void Select_With_Aliased_Table_With_As()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select * from table as t").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.From.Count);
            Assert.AreEqual("table", statement.From[0].Name);
            Assert.AreEqual("t", statement.From[0].Alias.Name);
            Assert.AreEqual(AliasType.As, statement.From[0].Alias.Type);
        }

        [Test]
        public void Select_With_Aliased_Table_Without_As()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select * from table t").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.From.Count);
            Assert.AreEqual("table", statement.From[0].Name);
            Assert.AreEqual("t", statement.From[0].Alias.Name);
            Assert.AreEqual(AliasType.Implicit, statement.From[0].Alias.Type);
        }

        [Test]
        public void Select_With_Two_Aliased_Table_With_As()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select * from table1 as t1, table2 as t2").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(2, statement.From.Count);
            Assert.AreEqual("table1", statement.From[0].Name);
            Assert.AreEqual("t1", statement.From[0].Alias.Name);
            Assert.AreEqual("table2", statement.From[1].Name);
            Assert.AreEqual("t2", statement.From[1].Alias.Name);
        }

        [Test]
        public void Select_With_Two_Aliased_Table_Without_As()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select * from table1 t1, table2 t2 ").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(2, statement.From.Count);
            Assert.AreEqual("table1", statement.From[0].Name);
            Assert.AreEqual("t1", statement.From[0].Alias.Name);
            Assert.AreEqual("table2", statement.From[1].Name);
            Assert.AreEqual("t2", statement.From[1].Alias.Name);
        }

        [Test]
        public void Select_Multiple_Fields_With_Aliases()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select field, fielda a, field2 as b, alias = fie3ld from table").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(4, statement.Fields.Count);

            Assert.AreEqual("field", statement.Fields[0].Expression.Value);
            Assert.IsNull(statement.Fields[0].Alias.Name);

            Assert.AreEqual("fielda", statement.Fields[1].Expression.Value);
            Assert.AreEqual("a", statement.Fields[1].Alias.Name);
            Assert.AreEqual(AliasType.Implicit, statement.Fields[1].Alias.Type);

            Assert.AreEqual("field2", statement.Fields[2].Expression.Value);
            Assert.AreEqual("b", statement.Fields[2].Alias.Name);
            Assert.AreEqual(AliasType.As, statement.Fields[2].Alias.Type);

            Assert.AreEqual("fie3ld", statement.Fields[3].Expression.Value);
            Assert.AreEqual("alias", statement.Fields[3].Alias.Name);
            Assert.AreEqual(AliasType.Equals, statement.Fields[3].Alias.Type);
        }

        [Test]
        public void Select_Field_With_Alias_As_String()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select field as 'a string' from table").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.Fields.Count);

            Assert.AreEqual("field", statement.Fields[0].Expression.Value);
            Assert.AreEqual("'a string'", statement.Fields[0].Alias.Name);
            Assert.AreEqual(AliasType.As, statement.Fields[0].Alias.Type);
        }

        [Test]
        public void Select_Field_With_Alias_As_Identifier_With_Spaces()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select field as [an identifier with spaces] from table").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.Fields.Count);

            Assert.AreEqual("field", statement.Fields[0].Expression.Value);
            Assert.AreEqual("[an identifier with spaces]", statement.Fields[0].Alias.Name);
            Assert.AreEqual(AliasType.As, statement.Fields[0].Alias.Type);
        }

        [Test]
        public void Select_Multiple_Fields_With_Table_Alias_Prefix()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>("select t1.fielda, t1.field2, SomeDb.dbo.fie3ld from table as t1").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(3, statement.Fields.Count);

            var expectedFields = new string[] { "t1.fielda", "t1.field2", "SomeDb.dbo.fie3ld" };
            int index = 0;
            foreach (var field in expectedFields)
            {
                Field column = statement.Fields[index++];
                Assert.IsTrue(column.Expression is Expression);
                Assert.AreEqual(field, column.Expression.Value);
            }

            Assert.AreEqual("table", statement.From[0].Name);
        }

        [Test]
        public void Select_With_Inner_Join_Condition()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(@"

                select fielda

                from table1 t1

                join table2 as t2
                  on t1.field1 = t2.field2
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);

            // Test From
            Assert.AreEqual(1, statement.From.Count);
            Assert.AreEqual("table1", statement.From[0].Name);
            Assert.AreEqual("t1", statement.From[0].Alias.Name);
            Assert.AreEqual(AliasType.Implicit, statement.From[0].Alias.Type);

            // Test Join
            Assert.AreEqual(1, statement.From[0].Joins.Count);

            Join join = statement.From[0].Joins[0];

            Assert.AreEqual("table2", join.Name);
            Assert.AreEqual("t2", join.Alias.Name);
            Assert.AreEqual(AliasType.As, join.Alias.Type);

            Assert.AreEqual(JoinType.Join, join.Type);

            CriteriaExpression expr = join.Condition as CriteriaExpression;

            Assert.AreEqual("=", expr.Operator);
            Assert.IsTrue(expr.Left is IdentifierExpression);

            var leftExpression = (IdentifierExpression)expr.Left;

            Assert.AreEqual("t1", leftExpression.Parts[0]);
            Assert.AreEqual("field1", leftExpression.Parts[1]);
            Assert.AreEqual("t1.field1", expr.Left.Value);

            var rightExpression = (IdentifierExpression)expr.Right;

            Assert.AreEqual("t2", rightExpression.Parts[0]);
            Assert.AreEqual("field2", rightExpression.Parts[1]);
            Assert.AreEqual("t2.field2", expr.Right.Value);

            Assert.AreEqual("t1.field1 = t2.field2", expr.Value);
        }

        [Test]
        public void Select_With_Inner_Join_Condition_With_Complex_Expressions()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(@"

                select fielda 

                from table1 t1 

                join table2 as t2 
                  on (t1.field1 + 150) / 12 = ( t2.field2 + t1.fielda )
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);

            // Test From
            Assert.AreEqual(1, statement.From.Count);
            Assert.AreEqual("table1", statement.From[0].Name);
            Assert.AreEqual("t1", statement.From[0].Alias.Name);

            // Test Join
            Assert.AreEqual(1, statement.From[0].Joins.Count);

            Join join = statement.From[0].Joins[0];

            Assert.AreEqual("table2", join.Name);
            Assert.AreEqual("t2", join.Alias.Name);

            Assert.AreEqual(JoinType.Join, join.Type);

            CriteriaExpression expr = join.Condition as CriteriaExpression;
            Assert.AreEqual("=", expr.Operator);
            Assert.IsTrue(expr.Left is OperatorExpression);

            var leftExpression = (OperatorExpression)expr.Left;
            Assert.AreEqual("/", leftExpression.Operator);

            Assert.IsTrue(leftExpression.Left is NestedExpression);
            var leftLeftExpression = (NestedExpression)leftExpression.Left;

            Assert.IsTrue(leftLeftExpression.Expression is OperatorExpression);

            OperatorExpression leftOperatorExpression = (OperatorExpression)leftLeftExpression.Expression;

            Assert.AreEqual("t1.field1", leftOperatorExpression.Left.Value);
            Assert.AreEqual("+", leftOperatorExpression.Operator);
            Assert.AreEqual("150", leftOperatorExpression.Right.Value);

            IdentifierExpression rightIdentifierExpression = (IdentifierExpression)leftExpression.Right;

            Assert.AreEqual("12", rightIdentifierExpression.Value);

            //var rightExpression = ( NestedExpression )expr.Right;
            //Assert.AreEqual( "t2", rightExpression.Parts[ 0 ] );
            //Assert.AreEqual( "field2", rightExpression.Parts[ 1 ] );
            //Assert.AreEqual( "t2.field2", expr.Right.Value );
        }

        [Test]
        public void Select_With_Inner_Join_Condition_Within_Brackets()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(@"

                select fielda 

                from table1 t1 

                join table2 as t2 
                  on (t1.field1 = t2.fielda )
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);

            // Test From
            Assert.AreEqual(1, statement.From.Count);
            Assert.AreEqual("table1", statement.From[0].Name);
            Assert.AreEqual("t1", statement.From[0].Alias.Name);

            // Test Join
            Assert.AreEqual(1, statement.From[0].Joins.Count);

            Join join = statement.From[0].Joins[0];

            Assert.AreEqual("table2", join.Name);
            Assert.AreEqual("t2", join.Alias.Name);

            Assert.AreEqual(JoinType.Join, join.Type);

            Assert.IsTrue(join.Condition is NestedExpression);
            NestedExpression expr = join.Condition as NestedExpression;

            Assert.IsTrue(expr.Expression is CriteriaExpression);
            CriteriaExpression nestedExpression = expr.Expression as CriteriaExpression;
            Assert.AreEqual("=", nestedExpression.Operator);
            Assert.IsTrue(nestedExpression.Left is IdentifierExpression);
            Assert.AreEqual("t1.field1", nestedExpression.Left.Value);
            Assert.IsTrue(nestedExpression.Right is IdentifierExpression);
            Assert.AreEqual("t2.fielda", nestedExpression.Right.Value);
        }

        [Test]
        [TestCase("JOIN", JoinType.Join)]
        [TestCase("INNER JOIN", JoinType.InnerJoin)]
        [TestCase("LEFT JOIN", JoinType.LeftJoin)]
        [TestCase("LEFT OUTER JOIN", JoinType.LeftOuterJoin)]
        [TestCase("RIGHT JOIN", JoinType.RightJoin)]
        [TestCase("RIGHT OUTER JOIN", JoinType.RightOuterJoin)]
        [TestCase("FULL JOIN", JoinType.FullJoin)]
        [TestCase("FULL OUTER JOIN", JoinType.FullOuterJoin)]
        [TestCase("CROSS JOIN", JoinType.CrossJoin, false)]
        public void Can_Parse_All_Join_Types(string joinText, JoinType type, bool includeCriteria = true)
        {
            var sql = String.Format("select * from table1 t1 {0} table2 as t2 {1}", joinText, includeCriteria ? "on t1.field1 = t2.fielda" : String.Empty);
            var statement = ParserFactory.Execute<SelectStatement>(sql).First();

            // Verify outcome
            Assert.IsNotNull(statement);

            // Test From
            Assert.AreEqual(1, statement.From.Count);
            Assert.AreEqual("table1", statement.From[0].Name);
            Assert.AreEqual("t1", statement.From[0].Alias.Name);

            // Test Join
            Assert.AreEqual(1, statement.From[0].Joins.Count);

            var join = statement.From[0].Joins[0];
            Assert.AreEqual("table2", join.Name);
            Assert.AreEqual("t2", join.Alias.Name);

            Assert.AreEqual(type, join.Type);
        }

        [Test]
        [TestCase("CROSS", JoinType.CrossApply)]
        [TestCase("OUTER", JoinType.OuterApply)]
        public void Can_Parse_Apply_Joins(string joinText, JoinType type)
        {
            var sql = String.Format("SELECT P.ProductId, P.Name, SP.TotalSales FROM Production.Product P {0} APPLY Sales.GetSalesByProduct (P.ProductId) AS SP", joinText);
            var statement = ParserFactory.Execute<SelectStatement>(sql).First();

            // Verify outcome
            Assert.IsNotNull(statement);

            // Test From
            Assert.AreEqual(1, statement.From.Count);
            Assert.AreEqual("Production.Product", statement.From[0].Name);
            Assert.AreEqual("P", statement.From[0].Alias.Name);

            // Test Join
            Assert.AreEqual(1, statement.From[0].Joins.Count);

            var join = statement.From[0].Joins[0] as ApplyJoin;
            var expression = (FunctionExpression)join.Expression;

            Assert.AreEqual(type, join.Type);
            Assert.AreEqual("Sales.GetSalesByProduct", expression.Name);
            Assert.AreEqual(1, expression.Arguments.Count);
            Assert.AreEqual("P.ProductId", expression.Arguments[0].Value);
            Assert.AreEqual("SP", join.Alias.Name);
        }

        [Test]
        [TestCase("CROSS", JoinType.CrossApply)]
        [TestCase("OUTER", JoinType.OuterApply)]
        public void Can_Apply_Derived_Apply_Joins(string joinText, JoinType type)
        {
            var sql = String.Format(@"

                SELECT soh.SalesOrderID, soh.OrderDate, sub.*
                FROM Sales.SalesOrderHeader soh {0} APPLY (

                    SELECT
                        SUM (sod.UnitPrice) AS UnitPrice,
                        SUM (sod.OrderQty) AS Quantity
                    FROM Sales.SalesOrderDetail sod
                    WHERE sod.SalesOrderID = soh.SalesOrderId

                ) AS sub",
                joinText
            );

            var statement = ParserFactory.Execute<SelectStatement>(sql).First();

            // Verify outcome
            Assert.IsNotNull(statement);

            // Test From
            Assert.AreEqual(1, statement.From.Count);
            Assert.AreEqual("Sales.SalesOrderHeader", statement.From[0].Name);
            Assert.AreEqual("soh", statement.From[0].Alias.Name);

            // Test Join
            Assert.AreEqual(1, statement.From[0].Joins.Count);

            var join = statement.From[0].Joins[0] as ApplyJoin;
            var expression = (SelectExpression)join.Expression;

            Assert.AreEqual(type, join.Type);
            Assert.AreEqual("Sales.SalesOrderDetail", expression.Statement.From[0].Name);
            Assert.AreEqual(2, expression.Statement.Fields.Count);
            Assert.AreEqual("sub", join.Alias.Name);
        }

        [Test]
        public void When_Join_Is_Missing_On_Clause_Throw_SyntaxError()
        {
            var sql = @"
                select * from Table Outer
                join (
                    select * from Table
                ) Inner --on inner.Value = Outer.Value
            ";

            Assert.Catch<ExpectedTokenNotFoundException>(
                () => ParserFactory.Execute<SelectStatement>(sql),
                "Expected: 'ON' but found: 'EOF' at Row: 6, Col: 13"
            );
        }

        [Test]
        public void Select_With_Where_Condition()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(@"

                select fielda 
                from table1 t1
                where t1.fieldb = (select top 1 Name from table)
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            CriteriaExpression expr = statement.Where as CriteriaExpression;
            Assert.AreEqual("=", expr.Operator);

            Assert.IsTrue(expr.Left is IdentifierExpression);

            Assert.IsTrue(expr.Right is NestedExpression);
            NestedExpression nestedExpression = (NestedExpression)expr.Right;
            Assert.IsTrue(nestedExpression.Expression is SelectExpression);
        }

        [Test]
        public void Select_With_Multi_Part_Where_Condition()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(@"

                select fielda 
                from table1 t1
                where t1.fieldb = t1.fieldc
                  and t1.fieldd = 0
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.IsTrue(statement.Where is CriteriaExpression);

            CriteriaExpression criteriaExpression = (CriteriaExpression)statement.Where;
            Assert.AreEqual("and", criteriaExpression.Operator.ToLower());
            Assert.IsTrue(criteriaExpression.Left is CriteriaExpression);

            CriteriaExpression leftCriteriaExpression = (CriteriaExpression)criteriaExpression.Left;
            Assert.AreEqual("t1.fieldb", leftCriteriaExpression.Left.Value);
            Assert.AreEqual("t1.fieldc", leftCriteriaExpression.Right.Value);

            CriteriaExpression rightCriteriaExpression = (CriteriaExpression)criteriaExpression.Right;
            Assert.AreEqual("t1.fieldd", rightCriteriaExpression.Left.Value);
            Assert.AreEqual("0", rightCriteriaExpression.Right.Value);
        }

        [Test]
        public void Select_With_Multi_Part_Where_Condition_With_Nested_Conditions()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(@"

                select fielda 
                from table1 t1
                where ( t1.fieldd = 0 or t1.fieldc < 10 )
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.IsTrue(statement.Where is NestedExpression);

            NestedExpression nestedExpression = (NestedExpression)statement.Where;

            CriteriaExpression criteriaExpression = (CriteriaExpression)nestedExpression.Expression;
            Assert.AreEqual("or", criteriaExpression.Operator.ToLower());
            Assert.IsTrue(criteriaExpression.Left is CriteriaExpression);

            CriteriaExpression leftCriteriaExpression = (CriteriaExpression)criteriaExpression.Left;
            Assert.AreEqual("t1.fieldd", leftCriteriaExpression.Left.Value);
            Assert.AreEqual("=", leftCriteriaExpression.Operator);
            Assert.AreEqual("0", leftCriteriaExpression.Right.Value);

            CriteriaExpression rightCriteriaExpression = (CriteriaExpression)criteriaExpression.Right;
            Assert.AreEqual("t1.fieldc", rightCriteriaExpression.Left.Value);
            Assert.AreEqual("<", rightCriteriaExpression.Operator);
            Assert.AreEqual("10", rightCriteriaExpression.Right.Value);
        }

        [Test]
        public void Select_With_Multi_Part_Where_Condition_With_Several_Nested_Conditions()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(@"

                select fielda 
                from table1 t1
                where ( t1.fieldd = 0 or t1.fieldc < 10 )
                  and ( ( select top 1 field from table1 ) = ( select top 1 fieldb from table2 ) )
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
        }

        [Test]
        public void Select_From_Derived_View()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(@"
                select * from (select field from table x) as t
            ").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.From.Count);
            Assert.AreEqual("t", statement.From[0].Alias.Name);
            Assert.IsTrue(statement.From[0] is DerivedTable);

            DerivedTable derivedTable = (DerivedTable)statement.From[0];
            Assert.AreEqual("field", derivedTable.SelectStatement.Fields[0].Expression.Value);
            Assert.AreEqual("table", derivedTable.SelectStatement.From[0].Name);
            Assert.AreEqual("x", derivedTable.SelectStatement.From[0].Alias.Name);
        }

        [Test]
        public void Select_With_Order_By()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(@"
                select * from table order by field1 desc, field2 asc
            ").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.From.Count);
            Assert.AreEqual("table", statement.From[0].Name);
            Assert.AreEqual(2, statement.OrderBy.Count);
            Assert.AreEqual(SortOrder.Descending, (statement.OrderBy[0] as SortedField).SortOrder);
            Assert.AreEqual(SortOrder.Ascending, (statement.OrderBy[1] as SortedField).SortOrder);
        }

        [Test]
        public void Select_With_Group_By()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(@"
                select * from table group by field1, field2
            ").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.From.Count);
            Assert.AreEqual("table", statement.From[0].Name);
            Assert.AreEqual(2, statement.GroupBy.Count);
        }

        [Test]
        public void Select_With_Group_By_With_Having()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(@"
                select * from table group by field1, field2 having sum( field2 ) > 0
            ").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.From.Count);
            Assert.AreEqual("table", statement.From[0].Name);
            Assert.AreEqual(2, statement.GroupBy.Count);
            Assert.IsTrue(statement.Having is CriteriaExpression);

            CriteriaExpression operatorExpression = (CriteriaExpression)statement.Having;
            Assert.AreEqual(">", operatorExpression.Operator);

            // verify that the left part of the criteria expression is a function expression
            Assert.IsTrue(operatorExpression.Left is FunctionExpression);
            FunctionExpression functionExpression = (FunctionExpression)operatorExpression.Left;
            Assert.AreEqual("sum", functionExpression.Name);
            Assert.AreEqual(1, functionExpression.Arguments.Count);
            Assert.AreEqual("field2", functionExpression.Arguments[0].Value);

            // the right branch is a simple IdentifierExpression
            Assert.IsTrue(operatorExpression.Right is IdentifierExpression);
            Assert.AreEqual("0", operatorExpression.Right.Value);
        }

        [Test]
        public void Select_With_Derived_Join()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(@"
                select * from table t join ( select field from other o ) as o1 on o.a = t.a
            ").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.From.Count);
            Assert.AreEqual("t", statement.From[0].Alias.Name);
            Assert.IsTrue(statement.From[0].Joins[0] is DerivedJoin);

            DerivedJoin derivedJoin = (DerivedJoin)statement.From[0].Joins[0];
            Assert.AreEqual("field", derivedJoin.SelectStatement.Fields[0].Expression.Value);
            Assert.AreEqual("other", derivedJoin.SelectStatement.From[0].Name);
            Assert.AreEqual("o", derivedJoin.SelectStatement.From[0].Alias.Name);
            Assert.AreEqual("o1", derivedJoin.Alias.Name);
        }

        [Test]
        [TestCase("union", SetType.Union)]
        [TestCase("union all", SetType.UnionAll)]
        [TestCase("except", SetType.Except)]
        [TestCase("intersect", SetType.Intersect)]
        public void Select_With_Union(string setOperator, SetType type)
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(String.Format(@"
                select id, name from table1 {0} select id, name from table2 order by id", setOperator)).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.From.Count);
            Assert.IsTrue(statement.SetOperation != null);
            Assert.AreEqual(type, statement.SetOperation.Type);
            Assert.IsTrue(statement.SetOperation.Statement != null);
            Assert.IsTrue(statement.SetOperation.Statement is SelectStatement);
            var select = statement.SetOperation.Statement as SelectStatement;
            Assert.AreEqual("id", select.OrderBy.First().Expression.Value);
        }

        [Test]
        public void Derived_View_With_Select_Unions_Without_From()
        {
            var sql = @"

                SELECT S.Field
                FROM Source S
                JOIN (
                    SELECT Field = 'AA'
                    UNION
                    SELECT 'BB'
                    UNION
                    SELECT 'CC'
                ) D ON D.Field = S.Field
            ";

            SelectStatement statement = ParserFactory.Execute<SelectStatement>(sql).First();
        }

        [Test]
        public void Select_With_Into_Clause()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(
                "select id, name into #T from table"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(2, statement.Fields.Count);
            Assert.AreEqual("#T", statement.Into);
        }

        [Test]
        public void Select_With_Multiple_From_Tables()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(
                "select id from table1, table2"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(2, statement.From.Count);
        }

        [Test]
        public void Select_With_Multiple_From_Tables_With_Joins()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(
                "select id from table1 t1 join other1 o1 on t1.id = o1.id, table2 t2 join other2 o2 on o2.id = j2.id"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(2, statement.From.Count);
        }

        [Test]
        public void Select_Count_Star()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(
                "select count(*) from table"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.IsInstanceOf<CountExpression>(statement.Fields.First().Expression);
            Assert.AreEqual("COUNT(*)", statement.Fields.First().Expression.Value);
            Assert.AreEqual("table", statement.From.First().Name);
        }

        [Test]
        public void Select_Count_Field()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(
                "select count(id) from table"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.IsInstanceOf<CountExpression>(statement.Fields.First().Expression);
            Assert.AreEqual("COUNT(id)", statement.Fields.First().Expression.Value);
            Assert.AreEqual("table", statement.From.First().Name);
        }

        [Test]
        public void Select_Count_Distinct_Field()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>(
                "select count(distinct Name) from table"
            ).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.IsInstanceOf<CountExpression>(statement.Fields.First().Expression);
            Assert.IsTrue(((CountExpression)statement.Fields.First().Expression).Distinct);
            Assert.AreEqual("COUNT(DISTINCT Name)", statement.Fields.First().Expression.Value);
            Assert.AreEqual("table", statement.From.First().Name);
        }

        [Test]
        [TestCase("ROW_NUMBER()")]
        [TestCase("RANK()")]
        [TestCase("DENSE_RANK()")]
        [TestCase("NTILE(4)")]
        public void Select_Ranking_Functions_Over_With_Order_By(string functionName)
        {
            // Exercise
            var statement = ParserFactory.Execute<SelectStatement>(String.Format(
                @"
                    SELECT *
                    FROM (

                        SELECT 
                            RowIndex = {0} OVER (ORDER BY SomeNumber, OtherNumber DESC), 
                            *
                        FROM [PagedTable]
                    ) T

                    WHERE RowIndex BETWEEN 2 AND 3
                ",
                functionName
            )).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.From.Count);

            Assert.AreEqual("T", statement.From[0].Alias.Name);
            Assert.IsTrue(statement.From[0] is DerivedTable);
            Assert.AreEqual("RowIndex BETWEEN 2 AND 3", statement.Where.Value);

            DerivedTable derivedTable = (DerivedTable)statement.From[0];
            Assert.AreEqual("RowIndex", derivedTable.SelectStatement.Fields[0].Alias.Name);
            Assert.IsTrue(derivedTable.SelectStatement.Fields[0].Expression is RankingFunctionExpression);

            var rankingFunctionExpression = (RankingFunctionExpression)derivedTable.SelectStatement.Fields[0].Expression;

            Assert.AreEqual(functionName, rankingFunctionExpression.Name);
            Assert.AreEqual(functionName + " OVER (ORDER BY SomeNumber, OtherNumber DESC)", rankingFunctionExpression.Value);
            Assert.AreEqual(2, rankingFunctionExpression.OrderBy.Count);
            Assert.AreEqual("SomeNumber", rankingFunctionExpression.OrderBy[0].Expression.Value);
            Assert.AreEqual("OtherNumber", rankingFunctionExpression.OrderBy[1].Expression.Value);
        }

        [Test]
        [TestCase("ROW_NUMBER()")]
        [TestCase("RANK()")]
        [TestCase("DENSE_RANK()")]
        [TestCase("NTILE(4)")]
        public void Select_Ranking_Functions_Over_With_Order_By_And_Partition_By(string functionName)
        {
            // Exercise
            var statement = ParserFactory.Execute<SelectStatement>(String.Format(
                @"
                    SELECT *
                    FROM (

                        SELECT 
                            RowIndex = {0} OVER (PARTITION BY Code ORDER BY SomeNumber, OtherNumber DESC),
                            *
                        FROM [PagedTable]
                    ) T

                    WHERE RowIndex BETWEEN 2 AND 3
                ",
                functionName
            )).First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(1, statement.From.Count);

            Assert.AreEqual("T", statement.From[0].Alias.Name);
            Assert.IsTrue(statement.From[0] is DerivedTable);
            Assert.AreEqual("RowIndex BETWEEN 2 AND 3", statement.Where.Value);

            DerivedTable derivedTable = (DerivedTable)statement.From[0];
            Assert.AreEqual("RowIndex", derivedTable.SelectStatement.Fields[0].Alias.Name);
            Assert.IsTrue(derivedTable.SelectStatement.Fields[0].Expression is RankingFunctionExpression);

            var rankingFunctionExpression = (RankingFunctionExpression)derivedTable.SelectStatement.Fields[0].Expression;

            Assert.AreEqual(functionName, rankingFunctionExpression.Name);
            Assert.AreEqual(functionName + " OVER (PARTITION BY Code ORDER BY SomeNumber, OtherNumber DESC)", rankingFunctionExpression.Value);

            Assert.AreEqual(2, rankingFunctionExpression.OrderBy.Count);
            Assert.AreEqual("SomeNumber", rankingFunctionExpression.OrderBy[0].Expression.Value);
            Assert.AreEqual("OtherNumber", rankingFunctionExpression.OrderBy[1].Expression.Value);

            Assert.AreEqual(1, rankingFunctionExpression.PartitionBy.Count);
            Assert.AreEqual("Code", rankingFunctionExpression.PartitionBy[0].Expression.Value);
        }

        [Test]
        public void Can_Parse_Select_With_Pivot()
        {
            // Exercise
            var statement = ParserFactory.Execute<SelectStatement>(@"
                SELECT 'AverageCost' AS Cost_Sorted_By_Production_Days, [0], [1], [2], [3], [4] 
                FROM Production.Product T
                PIVOT (
                    AVG(StandardCost)
                    FOR DaysToManufacture IN ([0], [1], [2], [3], [4])  
                ) AS PT

                WHERE T.Data BETWEEN 2 AND 3

            ").First();

            // Verify outcome
            Assert.IsNotNull(statement);
            Assert.AreEqual(6, statement.Fields.Count);
            Assert.AreEqual(1, statement.From.Count);
            Assert.AreEqual("Production.Product", statement.From[0].Name);
            Assert.AreEqual("T", statement.From[0].Alias.Name);

            Assert.IsNotNull(statement.Pivot);
            Assert.IsNotNull(statement.Pivot.Alias);
            Assert.AreEqual("PT", statement.Pivot.Alias.Name);
            Assert.AreEqual(1, statement.Pivot.Fields.Count);
            Assert.AreEqual("DaysToManufacture", statement.Pivot.For);
            Assert.AreEqual(5, statement.Pivot.In.Count);
        }
    }
}
