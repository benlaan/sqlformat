using System;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser.Parsers
{
    public class GrantStatementParser : StatementParser<GrantStatement>
    {
        public GrantStatementParser( ITokenizer tokenizer ) : base( tokenizer )
        {
        }

        public override GrantStatement Execute()
        {
            GrantStatement statement = new GrantStatement();

            if ( Tokenizer.IsNextToken( Constants.Select, Constants.Insert, Constants.Update, Constants.Delete, Constants.All ) )
            {
                string token = Tokenizer.Current.Value;
                ReadNextToken();
                statement.Operation = token;
            }
            else
                throw new ExpectedTokenNotFoundException( 
                    "'SELECT', 'INSERT', 'UPDATE', 'DELETE', 'ALL'",
                    CurrentToken, 
                    Tokenizer.Position 
                );

            Tokenizer.ExpectToken( Constants.On );
            statement.TableName = GetTableName();
            Tokenizer.ExpectToken( Constants.To );
            statement.Grantee = GetDotNotationIdentifier();

            return statement;
        }
    }
}
