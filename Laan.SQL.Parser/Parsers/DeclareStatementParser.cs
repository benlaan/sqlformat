using System;

namespace Laan.SQL.Parser
{
    public class DeclareStatementParser : StatementParser<DeclareStatement>
    {
        public DeclareStatementParser( ITokenizer tokenizer )
            : base( tokenizer )
        {
            _statement = new DeclareStatement();
        }

        private string GetSqlType()
        {
            string typeName = Tokenizer.Current.Value;
            Tokenizer.ReadNextToken();
            if ( Tokenizer.IsNextToken( Constants.OpenBracket ) )
            {
                using ( new BracketStructure( Tokenizer ) )
                {
                    string number = Tokenizer.Current.Value;
                    Tokenizer.ReadNextToken();

                    if ( Tokenizer.TokenEquals( Constants.Comma ) )
                    {
                        string precision = Tokenizer.Current.Value;
                        Tokenizer.ReadNextToken();
                        return String.Format( "{0}({1}, {2})", typeName, number, precision );
                    }
                    else
                        return String.Format( "{0}({1})", typeName, number );
                }
            }
            return typeName;
        }

        public override DeclareStatement Execute()
        {
            if ( Tokenizer.HasMoreTokens )
                do
                {
                    string name = Tokenizer.Current.Value;
                    Tokenizer.ReadNextToken();

                    if ( !Tokenizer.HasMoreTokens )
                        throw new SyntaxException( String.Format( "type missing for declaration of variable '{0}'", name ) );

                    string type = GetSqlType();
                    var definition = new VariableDefinition( name, type );

                    if ( Tokenizer.TokenEquals( Constants.Assignment ) )
                    {
                        var parser = new ExpressionParser( Tokenizer );
                        definition.DefaultValue = parser.Execute();
                    }

                    _statement.Definitions.Add( definition );
                }
                while ( Tokenizer.TokenEquals( Constants.Comma ) );

            if ( _statement.Definitions.Count == 0 )
                throw new SyntaxException( "DECLARE requires at least one variable declaration" );

            return _statement;
        }
    }
}