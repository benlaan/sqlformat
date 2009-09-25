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

            if ( !Tokenizer.IsNextToken( Constants.From ) )
                _statement.TableName = GetTableName();

            ProcessFrom();
            ProcessWhere();
            ProcessTerminator();

            return _statement;
        }
    }
}
