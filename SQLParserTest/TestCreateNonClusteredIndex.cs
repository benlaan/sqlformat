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
            var statement = ParserFactory.Execute<CreateIndex>( @"

                CREATE NONCLUSTERED INDEX [_TransactionID] ON [dbo].[Transactions] (ID1, ID2, ID3)
                "
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.IsFalse( statement.Clustered );
            Assert.IsFalse( statement.Unique );
            Assert.AreEqual( "[dbo].[Transactions]", statement.TableName );
            Assert.AreEqual( "[_TransactionID]", statement.IndexName );
            Assert.AreEqual( 3, statement.Columns.Count );

            for ( int index = 0; index < 3; index++ )
                Assert.AreEqual( String.Format( "ID{0}", index + 1 ), statement.Columns[ index ] );
        }

        [Test]
        public void Test_Can_Read_Non_Clustered_Index_Again()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateIndex>( @"

                CREATE NONCLUSTERED INDEX [IX_Weights_ByTransaction] 
                                       ON [dbo].[Weights] ([Type],[TransactionID],[IsCancelled])
                "
            );

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
                Assert.AreEqual( column, statement.Columns[ index++ ] );
        }


        [Test]
        public void Test_Can_Read_Clustered_Index()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateIndex>( @"

                CREATE CLUSTERED INDEX IX_TransactionID ON dbo.Transactions ([ID1])
                "
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.IsTrue( statement.Clustered );
            Assert.IsFalse( statement.Unique );
            Assert.AreEqual( "dbo.Transactions", statement.TableName );
            Assert.AreEqual( "IX_TransactionID", statement.IndexName );
            Assert.AreEqual( 1, statement.Columns.Count );
            Assert.AreEqual( "[ID1]", statement.Columns[ 0 ] );
        }

        [Test]
        public void Test_Can_Read_Unique_Clustered_Index()
        {
            // Exercise
            var statement = ParserFactory.Execute<CreateIndex>( @"

                CREATE UNIQUE CLUSTERED INDEX IX_TransactionID ON dbo.Transactions ([ID1])
                "
            );

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.IsTrue( statement.Clustered );
            Assert.IsTrue( statement.Unique );
            Assert.AreEqual( "dbo.Transactions", statement.TableName );
            Assert.AreEqual( "IX_TransactionID", statement.IndexName );
            Assert.AreEqual( 1, statement.Columns.Count );
            Assert.AreEqual( "[ID1]", statement.Columns[ 0 ] );
        }
    }
}
