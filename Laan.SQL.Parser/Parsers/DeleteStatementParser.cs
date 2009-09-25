using System;

namespace Laan.SQL.Parser
{
    public class DeleteStatementParser : CriteriaStatementParser<DeleteStatement>
    {
        public DeleteStatementParser( ITokenizer tokenizer ) : base( tokenizer ) { }

        public override DeleteStatement Execute()
        {
            _statement = new DeleteStatement();

            if ( Tokenizer.IsNextToken( Constants.Top ) )
                _statement.Top = GetTop();

            if ( !Tokenizer.IsNextToken( Constants.From ) )
                _statement.TableName = GetTableName();

            ProcessFrom();
            ProcessWhere();
            ProcessTerminator();

            return _statement;
        }
    }
}
