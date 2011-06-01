using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Exceptions;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestCreateView
    {
        [Test]
        [ExpectedException( typeof( ParserNotImplementedException ), ExpectedMessage = "No parser exists for statement type: merge" )]
        public void TestNoParserException()
        {
            //TODO: needs to be moved into a ParserFactory specific unit test
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "merge from table" ).First(); ;
        }

        [Test]
        public void Select_StarField_Only()
        {
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select * from table" ).First(); ;
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
        public void Select_Top_10_StarField()
        {
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select top 10 * from table" ).First(); ;
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "*", statement.Fields[ 0 ].Expression.Value );
            Assert.AreEqual( "10", statement.Top.Expression.Value );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
        }

        [Test]
        [ExpectedException( typeof( SyntaxException ), ExpectedMessage = "Expected integer but found: '*'" )]
        public void Select_Top_Missing_Top_Param_StarField()
        {
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select top * from table" ).First();
        }

        [Test]
        public void Select_Distinct_Top_10_StarField()
        {
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select distinct top 10 * from table" ).First();
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "*", statement.Fields[ 0 ].Expression.Value );
            Assert.AreEqual( "10", statement.Top.Expression.Value );
            Assert.IsTrue( statement.Distinct );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
        }

        [Test]
        public void Select_Multiple_Fields()
        {
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select fielda, field2, fie3ld from table" ).First();
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
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select * from table as t" ).First();
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias.Name );
        }

        [Test]
        public void Select_With_Aliased_Table_Without_As()
        {
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select * from table t" ).First();
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "table", statement.From[ 0 ].Name );
            Assert.AreEqual( "t", statement.From[ 0 ].Alias.Name );
        }

        [Test]
        public void Select_With_Two_Aliased_Table_With_As()
        {
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select * from table1 as t1, table2 as t2" ).First();
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );
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
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( "create view v1 as select * from table1 t1, table2 t2 " ).First();
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );

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
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( 
                "create view v1 as select field, fielda a, field2 as b, alias = fie3ld from table"
            ).First();

            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "v1", sut.Name );
            Assert.AreEqual( 4, statement.Fields.Count );

            Assert.AreEqual( "field", statement.Fields[ 0 ].Expression.Value );
            Assert.IsNull( statement.Fields[ 0 ].Alias.Name );

            Assert.AreEqual( "fielda", statement.Fields[ 1 ].Expression.Value );
            Assert.AreEqual( "a", statement.Fields[ 1 ].Alias.Name );

            Assert.AreEqual( "field2", statement.Fields[ 2 ].Expression.Value );
            Assert.AreEqual( "b", statement.Fields[ 2 ].Alias.Name );

            Assert.AreEqual( "fie3ld", statement.Fields[ 3 ].Expression.Value );
            Assert.AreEqual( "alias", statement.Fields[ 3 ].Alias.Name );
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
        public void Select_With_Join_Condition()
        {
            // Exercise
            CreateViewStatement sut = ParserFactory.Execute<CreateViewStatement>( @"

                CREATE VIEW SomeView
                AS 
                    SELECT FieldA
                    FROM Table1 T1 
                    JOIN Table2 T2 
                      ON T1.Field1 = T2.Field2"
            ).First();
            SelectStatement statement = sut.SelectBlock;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( "SomeView", sut.Name );

            // Test From
            Assert.AreEqual( 1, statement.From.Count );
            Assert.AreEqual( "Table1", statement.From[ 0 ].Name );
            Assert.AreEqual( "T1", statement.From[ 0 ].Alias.Name );

            // Test Join
            Assert.AreEqual( 1, statement.From[ 0 ].Joins.Count );

            Join join = statement.From[ 0 ].Joins[ 0 ];

            Assert.AreEqual( "Table2", join.Name );
            Assert.AreEqual( "T2", join.Alias.Name );

            CriteriaExpression expr = join.Condition as CriteriaExpression;

            Assert.AreEqual( JoinType.Join, join.Type );
            Assert.AreEqual( "=", expr.Operator );
            Assert.AreEqual( "T1.Field1", expr.Left.Value );

            Assert.AreEqual( "T2.Field2", expr.Right.Value );

            Assert.AreEqual( "T1.Field1 = T2.Field2", expr.Value );
        }
    }

}
