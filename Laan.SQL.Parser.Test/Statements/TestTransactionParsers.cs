using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Test
{
    [TestFixture]
    public class TestTransactionParsers
    {
        [Test]
        [Row( "begin tran select id from t commit" )]
        [Row( "begin tran select id from t commit tran" )]
        [Row( "begin tran select id from t commit transaction" )]
        [Row( "begin transaction select id from t commit" )]
        public void Test_Basic_Begin_Tran_With_Commit( string sql )
        {
            // Exercise
            List<IStatement> statements = ParserFactory.Execute( sql );

            // Verify outcome
            Assert.IsNotNull( statements );
            Assert.AreEqual( 3, statements.Count );
        }

        [Test]
        [Row( "begin tran select id from t rollback" )]
        [Row( "begin tran a select id from t rollback tran a" )]
        [Row( "begin tran a select id from t rollback transaction a" )]
        public void Test_Basic_Begin_Tran_With_RollBack( string sql )
        {
            // Exercise
            List<IStatement> statements = ParserFactory.Execute( sql );

            // Verify outcome
            Assert.IsNotNull( statements );
            Assert.AreEqual( 3, statements.Count );
        }

        [Test]
        [Row( "begin tran 't1' ",                    "'t1'", TransactionDescriptor.Tran,        false )]
        [Row( "begin transaction 't1' ",             "'t1'", TransactionDescriptor.Transaction, false )]
        [Row( "begin distributed transaction 't1' ", "'t1'", TransactionDescriptor.Transaction,  true )]
        public void Test_Begin_Tran_With_Name_And_Distribution( string sql, string name, TransactionDescriptor descriptor, bool distributed )
        {
            // Exercise
            BeginTransactionStatement statement = ParserFactory.Execute<BeginTransactionStatement>( sql ).First();

            // Verify outcome
            Assert.IsNotNull( statement );
            Assert.AreEqual( name, statement.Name );
            Assert.AreEqual(descriptor, statement.Descriptor );
            Assert.AreEqual( distributed, statement.Distributed );
        }
    }
}
