using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MbUnit.Framework;

using Laan.SQL.Parser;

namespace Laan.SQL.Parser.Test
{
    [TestFixture]
    public class TestCreateNonClusteredIndex
    {
        [Test]
        public void Test_Can_Read_Non_Clustered_Index()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateIndexStatement>( @"

                CREATE NONCLUSTERED INDEX [_TransactionID] ON [dbo].[Transactions] (ID1, ID2, ID3)
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.IsFalse( statement.Clustered );
            Assert.IsFalse( statement.Unique );
            Assert.AreEqual( "[dbo].[Transactions]", statement.TableName );
            Assert.AreEqual( "[_TransactionID]", statement.IndexName );
            Assert.AreEqual( 3, statement.Columns.Count );

            for ( int index = 0; index < 3; index++ )
                Assert.AreEqual( String.Format( "ID{0}", index + 1 ), statement.Columns[ index ].Name );
        }

        [Test]
        public void Test_Can_Read_Non_Clustered_Index_In_Descending_Order()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateIndexStatement>( @"

                CREATE NONCLUSTERED INDEX [_TransactionID] ON [dbo].[Transactions] (ID1 DESC, ID2, ID3 DESC)
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.IsFalse( statement.Clustered );
            Assert.IsFalse( statement.Unique );
            Assert.AreEqual( "[dbo].[Transactions]", statement.TableName );
            Assert.AreEqual( "[_TransactionID]", statement.IndexName );
            Assert.AreEqual( 3, statement.Columns.Count );

            var orders = new[] { Order.Descending, Order.Ascending, Order.Descending };

            for ( int index = 0; index < 3; index++ )
            {
                Assert.AreEqual( String.Format( "ID{0}", index + 1 ), statement.Columns[ index ].Name );
                Assert.AreEqual( orders[ index ], statement.Columns[ index ].Order );
            }
        }

        [Test]
        public void Test_Can_Read_Non_Clustered_Index_Again()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateIndexStatement>( @"

                CREATE NONCLUSTERED INDEX [IX_Weights_ByTransaction] 
                                       ON [dbo].[Weights] ([Type],[TransactionID],[IsCancelled])
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.IsFalse( statement.Clustered );
            Assert.IsFalse( statement.Unique );
            Assert.AreEqual( "[dbo].[Weights]", statement.TableName );
            Assert.AreEqual( "[IX_Weights_ByTransaction]", statement.IndexName );
            Assert.AreEqual( 3, statement.Columns.Count );

            var columns = new[] { "[Type]", "[TransactionID]", "[IsCancelled]" };
            int index = 0;
            foreach ( var column in columns )
                Assert.AreEqual( column, statement.Columns[ index++ ].Name );
        }


        [Test]
        public void Test_Can_Read_Clustered_Index()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateIndexStatement>( @"

                CREATE CLUSTERED INDEX IX_TransactionID ON dbo.Transactions ([ID1])
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.IsTrue( statement.Clustered );
            Assert.IsFalse( statement.Unique );
            Assert.AreEqual( "dbo.Transactions", statement.TableName );
            Assert.AreEqual( "IX_TransactionID", statement.IndexName );
            Assert.AreEqual( 1, statement.Columns.Count );
            Assert.AreEqual( "[ID1]", statement.Columns[ 0 ].Name );
        }

        [Test]
        public void Test_Can_Read_Unique_Clustered_Index()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateIndexStatement>( @"

                CREATE UNIQUE CLUSTERED INDEX IX_TransactionID ON dbo.Transactions ([ID1])
                "
            ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.IsTrue( statement.Clustered );
            Assert.IsTrue( statement.Unique );
            Assert.AreEqual( "dbo.Transactions", statement.TableName );
            Assert.AreEqual( "IX_TransactionID", statement.IndexName );
            Assert.AreEqual( 1, statement.Columns.Count );
            Assert.AreEqual( "[ID1]", statement.Columns[ 0 ].Name );
        }
    }
}
