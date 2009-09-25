using System;

namespace Laan.SQL.Parser
{
    public class UpdateStatementParser : CriteriaStatementParser<UpdateStatement>
    {
        public UpdateStatementParser( ITokenizer tokenizer ) : base( tokenizer ) { }

        public override UpdateStatement Execute()
        {
            _statement = new UpdateStatement();

            if ( Tokenizer.IsNextToken( Constants.Top ) )
                _statement.Top = GetTop();

            _statement.TableName = GetTableName();

            Tokenizer.ExpectToken( Constants.Set );

            ProcessFields( FieldType.Update, _statement.Fields );
            ProcessFrom();
            ProcessWhere();
            ProcessTerminator();

            return _statement;
        }
    }
}
