using System;

namespace Laan.SQL.Parser
{
    public class UpdateStatementParser : CriteriaStatementParser<UpdateStatement>
    {
        public UpdateStatementParser( ITokenizer tokenizer ) : base( tokenizer ) { }

        public override IStatement Execute()
        {
            _statement = new UpdateStatement();

            if ( Tokenizer.IsNextToken( Constants.Top ) )
            {
                Tokenizer.ReadNextToken();
                using ( Tokenizer.ExpectBrackets() )
                {
                    int top;
                    if ( Int32.TryParse( Tokenizer.Current.Value, out top ) )
                    {
                        _statement.Top = top;
                        Tokenizer.ReadNextToken();
                    }
                    else
                        throw new SyntaxException( "TOP clause requires an integer" );
                }
            }

            _statement.TableName = GetTableName();

            Tokenizer.ExpectToken( Constants.Set );

            ProcessFields( FieldType.Update, _statement.Fields );
            ProcessFrom();
            ProcessWhere();

            return _statement;
        }
    }
}
