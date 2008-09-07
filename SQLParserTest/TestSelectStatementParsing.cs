using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SQLParser;

using MbUnit.Framework;

namespace SQLParserTest
{
    [TestFixture]
    public class TestSelectStatementParsing
    {
        [Test]
        public void Select_StarField_Only()
        {
            // Exercise
            IStatement sut = ParserFactory.Execute( "select * from table" );
            SelectStatement statement = sut as SelectStatement;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "*", statement.Fields[ 0 ].Name );
            Assert.IsNull( statement.Top );
            Assert.AreEqual( "table", statement.From );
        }

        [Test]
        public void Select_Top_10_StarField()
        {
            // Exercise
            IStatement sut = ParserFactory.Execute( "select top 10 * from table" );
            SelectStatement statement = sut as SelectStatement;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "*", statement.Fields[ 0 ].Name );
            Assert.AreEqual( 10, statement.Top );
            Assert.AreEqual( "table", statement.From );
        }

        [Test]
        public void Select_Distinct_Top_10_StarField()
        {
            // Exercise
            IStatement sut = ParserFactory.Execute( "select distinct top 10 * from table" );
            SelectStatement statement = sut as SelectStatement;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 1, statement.Fields.Count );
            Assert.AreEqual( "*", statement.Fields[ 0 ].Name );
            Assert.AreEqual( 10, statement.Top );
            Assert.IsTrue( statement.Distinct );
            Assert.AreEqual( "table", statement.From );
        }

        [Test]
        public void Select_Multiple_Fields()
        {
            // Exercise
            IStatement sut = ParserFactory.Execute( "select fielda, field2, fie3ld from table" );
            SelectStatement statement = sut as SelectStatement;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 3, statement.Fields.Count );

            var expectedFields = new string[] { "fielda", "field2", "fie3ld" };
            int index = 0;
            foreach (var field in expectedFields)
                Assert.AreEqual( field, statement.Fields[ index++ ].Name );

            Assert.AreEqual( "table", statement.From );
        }

        [Test]
        public void Select_With_Aliased_Table_With_As()
        {
            // Exercise
            IStatement sut = ParserFactory.Execute( "select * from table as t" );
            SelectStatement statement = sut as SelectStatement;

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
            IStatement sut = ParserFactory.Execute( "select * from table t" );
            SelectStatement statement = sut as SelectStatement;

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
            IStatement sut = ParserFactory.Execute( "select * from table1 as t1, table2 as t2" );
            SelectStatement statement = sut as SelectStatement;

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
            IStatement sut = ParserFactory.Execute( "select * from table1 t1, table2 t2 " );
            SelectStatement statement = sut as SelectStatement;

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
            IStatement sut = ParserFactory.Execute( "select fielda a, field2 as b, 'alias' = t1.fie3ld from table" );
            SelectStatement statement = sut as SelectStatement;

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( 3, statement.Fields.Count );

            Assert.AreEqual( "fielda", statement.Fields[ 0 ].Name );
            Assert.AreEqual( "a", statement.Fields[ 0 ].Alias );
            
            Assert.AreEqual( "field2", statement.Fields[ 1 ].Name );
            Assert.AreEqual( "b", statement.Fields[ 1 ].Alias );

            Assert.AreEqual( "fie3ld", statement.Fields[ 2 ].Name );
            Assert.AreEqual( "alias", statement.Fields[ 2 ].Alias );
        }

        [Test]
        public void Select_Multiple_Fields_With_Table_Alias_Prefix()
        {
            // Exercise
            //IStatement sut = ParserFactory.Execute( "select t1.fielda, t1.field2, t1.fie3ld from table as t1" );
            //SelectStatement statement = sut as SelectStatement;

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
            //IStatement sut = ParserFactory.Execute( "select fielda from table1 t1 join table2 t2 on t1.field1 = t2.field2" );
            //SelectStatement statement = sut as SelectStatement;

            //// Verify outcome
            //Assert.IsNotNull( statement );

            //// Test From
            //Assert.AreEqual( 1, statement.From.Count );
            //Assert.AreEqual( "table1", statement.From[ 0 ].Name );
            //Assert.AreEqual( "t1", statement.From[ 0 ].Alias );

            //// Test Join
            //Assert.AreEqual( 1, statement.Joins.Count );

            //Join join = statement.Joins[ 0 ];

            //Assert.AreEqual( "table2", join.Name );
            //Assert.AreEqual( "t2", join.Alias );

            //Assert.AreEqual( JoinType.InnerJoin, join.Type );
            //Assert.AreEqual( "=", join.Condition.Operation );
            //Assert.AreEqual( "t1", join.Condition.LeftExpression.Alias );
            //Assert.AreEqual( "field1", join.Condition.LeftExpression.Field );
            //Assert.AreEqual( "t1.field1", join.Condition.LeftExpression );

            //Assert.AreEqual( "t2", join.Condition.RightExpression.Alias );
            //Assert.AreEqual( "field2", join.Condition.RightExpression.Field );
            //Assert.AreEqual( "t2.field2", join.Condition.RightExpression );

            //Assert.AreEqual( "t1.field1 = t2.field2", join.Condition );
        }
    }

}
