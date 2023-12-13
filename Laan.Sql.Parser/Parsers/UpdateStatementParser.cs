using System;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Parsers
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

            ProcessTableHints( _statement );
            Tokenizer.ExpectToken( Constants.Set );

            ProcessFields( FieldType.Update, _statement.Fields );
            ProcessFrom();
            ProcessWhere();
            ProcessTerminator();

            return _statement;
        }
    }
}
