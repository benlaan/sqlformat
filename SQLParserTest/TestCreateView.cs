using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MbUnit.Framework;

namespace Laan.SQL.Parser.Test
{
    [TestFixture]
    public class TestCreateView
    {
        [Test]
        [ExpectedException( typeof( NotImplementedException ), Message = "No parser exists for that statement type: merge" )]
        public void TestNoParserException()
        {
            //TODO: needs to be moved into a ParserFactory specific unit test
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "merge from table" );
        }

        [Test]
        public void Select_StarField_Only()
        {
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select * from table" );
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );
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
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select * table" );
        }

        [Test]
        public void Select_Top_10_StarField()
        {
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select top 10 * from table" );
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );
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
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select top * from table" );
        }

        [Test]
        public void Select_Distinct_Top_10_StarField()
        {
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select distinct top 10 * from table" );
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );
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
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select fielda, field2, fie3ld from table" );
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );
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
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select * from table as t" );
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias );
        }

        [Test]
        public void Select_With_Aliased_Table_Without_As()
        {
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select * from table t" );
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias );
        }

        [Test]
        public void Select_With_Two_Aliased_Table_With_As()
        {
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select * from table1 as t1, table2 as t2" );
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );
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
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select * from table1 t1, table2 t2 " );
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );

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
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( 
                "create view v1 as select field, fielda a, field2 as b, alias = fie3ld from table" 
            );
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );
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
            //CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view something as select t1.fielda, t1.field2, t1.fie3ld from table as t1" );
            //SelectStatement statement = sut.SelectBlock;

            //// Verify outcome
            //Assert.IsNotNull( statement );
            //Assert.AreEqual( 3, statement.Fields.Count );

            //var expectedFields = new string[] { "fielda", "field2", "fie3ld" };
            //int index = 0;
            //foreach (var field in expectedFields)
            //    Assert.AreEqual( field, statement.Fields[ index++ ].Name );

            //Assert.AreEqual( "table", statement.From );
        }

        [Test]
        public void Select_With_Inner_Join_Condition()
        {
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( @"

                CREATE VIEW SomeView
                AS 
                    SELECT FieldA
                    FROM Table1 T1 
                    JOIN Table2 T2 
                      ON T1.Field1 = T2.Field2"
            );
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "SomeView", sut.Name );

            // Test From
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "Table1", statement.From[ 0 ].Name );
            Assert.AreEqual( "T1", statement.From[ 0 ].Alias );

            // Test Join
            Assert.AreEqual( 1, statement.Joins.Count );

            Join join = statement.Joins[ 0 ];

            Assert.AreEqual( "Table2", join.Name );
            Assert.AreEqual( "T2", join.Alias );

            Assert.AreEqual( JoinType.InnerJoin, join.Type );
            Assert.AreEqual( "=", join.Condition.Operator );
            Assert.AreEqual( "T1.Field1", join.Condition.Left.Value );

            Assert.AreEqual( "T2.Field2", join.Condition.Right.Value );

            Assert.AreEqual( "T1.Field1 = T2.Field2", join.Condition.Value );
        }
    }

}
