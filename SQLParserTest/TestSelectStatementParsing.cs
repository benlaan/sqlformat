using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MbUnit.Framework;

namespace Laan.SQL.Parser.Test
{
    [TestFixture]
    public class TestSelectStatementParsing
    {
        [Test]
        [ExpectedException( typeof( NotImplementedException ), Message = "No parser exists for that statement type: merge" )]
        public void TestNoParserException()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "merge from table" );
        }

        [Test]
        public void Select_StarField_Only()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select * from table" );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "*", statement.Fields[ 0 ].Expression.Value );
            Assert.IsNull( statement.Top );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
        }

        [Test]
        [ExpectedException( typeof( ExpectedTokenNotFoundException ), Message = "Expected: FROM, found: table" )]
        public void TestExpectedError()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select * table" );
        }

        [Test]
        public void Select_Top_10_StarField()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select top 10 * from table" );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "*", statement.Fields[ 0 ].Expression.Value );
            Assert.AreEqual( 10, statement.Top );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
        }

        [Test]
        [ExpectedException( typeof( SyntaxException ), Message = "Expected integer but found: '*'" )]
        public void Select_Top_Missing_Top_Param_StarField()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select top * from table" );
        }

        [Test]
        public void Select_Distinct_Top_10_StarField()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select distinct top 10 * from table" );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "*", statement.Fields[ 0 ].Expression.Value );
            Assert.AreEqual( 10, statement.Top );
            Assert.IsTrue( statement.Distinct );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
        }

        [Test]
        public void Select_Multiple_Fields()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select fielda, field2, fie3ld from table" );

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
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select * from table as t" );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias );
        }

        [Test]
        public void Select_With_Aliased_Table_Without_As()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select * from table t" );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias );
        }

        [Test]
        public void Select_With_Two_Aliased_Table_With_As()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select * from table1 as t1, table2 as t2" );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 2, statement.From.Count );
            Assert.AreEqual( "table1", statement.From[ 0 ].Name );
            Assert.AreEqual( "t1", statement.From[ 0 ].Alias );
            Assert.AreEqual( "table2", statement.From[ 1 ].Name );
            Assert.AreEqual( "t2", statement.From[ 1 ].Alias );
        }

        [Test]
        public void Select_With_Two_Aliased_Table_Without_As()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select * from table1 t1, table2 t2 " );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 2, statement.From.Count );
            Assert.AreEqual( "table1", statement.From[ 0 ].Name );
            Assert.AreEqual( "t1", statement.From[ 0 ].Alias );
            Assert.AreEqual( "table2", statement.From[ 1 ].Name );
            Assert.AreEqual( "t2", statement.From[ 1 ].Alias );
        }

        [Test]
        public void Select_Multiple_Fields_With_Aliases()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select field, fielda a, field2 as b, alias = fie3ld from table" );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 4, statement.Fields.Count );

            Assert.AreEqual( "field", statement.Fields[ 0 ].Expression.Value );
            Assert.IsNull( statement.Fields[ 0 ].Alias );

            Assert.AreEqual( "fielda", statement.Fields[ 1 ].Expression.Value );
            Assert.AreEqual( "a", statement.Fields[ 1 ].Alias );

            Assert.AreEqual( "field2", statement.Fields[ 2 ].Expression.Value );
            Assert.AreEqual( "b", statement.Fields[ 2 ].Alias );

            Assert.AreEqual( "fie3ld", statement.Fields[ 3 ].Expression.Value );
            Assert.AreEqual( "alias", statement.Fields[ 3 ].Alias );
        }

        [Test]
        public void Select_Multiple_Fields_With_Table_Alias_Prefix()
        {
            // Exercise
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( "select t1.fielda, t1.field2, SomeDb.dbo.fie3ld from table as t1" );

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
            SelectStatement statement = ParserFactory.Execute<SelectStatement>( 
                "select fielda from table1 t1 inner join table2 t2 on t1.field1 = t2.field2" 
            );

            // Verify outcome
            Assert.IsNotNull( statement );

            // Test From
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table1", statement.From[ 0 ].Name );
            Assert.AreEqual( "t1", statement.From[ 0 ].Alias );

            // Test Join
            Assert.AreEqual( 1, statement.Joins.Count );

            Join join = statement.Joins[ 0 ];

            Assert.AreEqual( "table2", join.Name );
            Assert.AreEqual( "t2", join.Alias );

            Assert.AreEqual( JoinType.InnerJoin, join.Type );
            Assert.AreEqual( "=", join.Condition.Operator );
//            Assert.IsTrue( join.Condition.Left is FieldExpression );
            //Assert.AreEqual( "t1", join.Condition.Left.Alias );
            //Assert.AreEqual( "field1", join.Condition.Left.Expression.Value );
            Assert.AreEqual( "t1.field1", join.Condition.Left.Value );

            //Assert.AreEqual( "t2", join.Condition.Right.Alias );
            //Assert.AreEqual( "field2", join.Condition.Right.Expression.Value );
            Assert.AreEqual( "t2.field2", join.Condition.Right.Value );

            Assert.AreEqual( "t1.field1 = t2.field2", join.Condition.Value );
        }
    }

}
