using System;

namespace Laan.SQL.Parser
{
    public class BeginStatementParser : CustomParser, IParser
    {
        public BeginStatementParser( ITokenizer tokenizer ) : base( tokenizer )
        {
        }

        #region IParser Members

        public IStatement Execute()
        {
            if ( Tokenizer.IsNextToken( Constants.Tran, Constants.Transaction, Constants.Distributed ) )
                return new BeginTransactionStatementParser( Tokenizer ).Execute();

            BlockStatement statement = new BlockStatement();
            statement.Statements.AddRange( ParserFactory.Execute( Tokenizer, false ) );
            Tokenizer.ExpectToken( Constants.End );
            return statement;
        }

        #endregion
    }
}
