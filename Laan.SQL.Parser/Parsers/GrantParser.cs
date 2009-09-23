using System;

namespace Laan.SQL.Parser
{
    public class GrantParser : StatementParser
    {
        public GrantParser( ITokenizer tokenizer ) : base( tokenizer )
        {
        }

        public override IStatement Execute()
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
