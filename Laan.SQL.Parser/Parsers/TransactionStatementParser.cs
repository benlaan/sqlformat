using System;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser.Parsers
{
    public class TransactionStatementParser<T> : StatementParser<T> where T : TransactionStatement, new()
    {
        public TransactionStatementParser( ITokenizer tokenizer ) : base( tokenizer )
        {
        }

        private void ProcessDescriptor( T statement )
        {
            if ( Tokenizer.TokenEquals( Constants.Tran ) )
                statement.Descriptor = TransactionDescriptor.Tran;

            else if ( Tokenizer.TokenEquals( Constants.Transaction ) )
                statement.Descriptor = TransactionDescriptor.Transaction;

            else if ( Tokenizer.TokenEquals( Constants.Work ) )
                statement.Descriptor = TransactionDescriptor.Work;
}

        public override T Execute()
        {
            _statement = new T();
            ProcessDescriptor( _statement );

            if ( _statement.Descriptor != TransactionDescriptor.None && Tokenizer.HasMoreTokens && !Tokenizer.IsNextToken( 
                    Constants.Select, Constants.Update, Constants.Declare, Constants.Insert, Constants.Create, 
                    Constants.Alter, Constants.Go, Constants.Print, Constants.Exec, Constants.Grant, Constants.Begin
                )
            )
            {
                _statement.Name = GetIdentifier();
            }

            ProcessTerminator();
            return _statement;
        }
    }

    public class BeginTransactionStatementParser : TransactionStatementParser<BeginTransactionStatement>
    {
        public BeginTransactionStatementParser( ITokenizer tokenizer ) : base( tokenizer )
        {
        }

        public override BeginTransactionStatement Execute()
        {
            bool distributed = false;
            if ( Tokenizer.TokenEquals( Constants.Distributed ) )
                distributed = true;

            var statement = base.Execute();
            statement.Distributed = distributed;
            return statement;
        }
    }

    public class CommitStatementParser : TransactionStatementParser<CommitTransactionStatement>
    {
        public CommitStatementParser( ITokenizer tokenizer ) : base( tokenizer )
        {
        }
    }

    public class RollbackStatementParser : TransactionStatementParser<RollbackTransactionStatement>
    {
        public RollbackStatementParser( ITokenizer tokenizer ) : base( tokenizer )
        {
        }
    }
}
