using System;

namespace Laan.SQL.Parser
{
    public class UpdateStatementParser : CriteriaStatementParser<UpdateStatement>
    {
        public UpdateStatementParser( Tokenizer tokenizer ) : base( tokenizer ) { }

        public override IStatement Execute()
        {
            _statement = new UpdateStatement();

            _statement.TableName = GetTableName();

            Tokenizer.ExpectToken( "SET" );

            ProcessFields( FieldType.Update, _statement.Fields );
            ProcessFrom();
            ProcessWhere();

            return _statement;
        }
    }
}
