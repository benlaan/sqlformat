using System;

namespace SQLParser
{
    public class CreateTableStatementParser : StatementParser
    {
        private const string OPEN_BRACE = "{";
        private const string CLOSE_BRACE = "}";

        CreateTableStatement _statement;

        internal CreateTableStatementParser( Tokenizer tokenizer ) : base( tokenizer )
        {
        }

        public override IStatement Execute()
        {
            _statement = new CreateTableStatement();

            ExpectToken( OPEN_BRACE );

            do
            {
                ProcessField();

            } while ( Tokenizer.TokenEquals( COMMA ) );

            ExpectToken( CLOSE_BRACE );

            return _statement;
        }

        private void ProcessField()
        {
            string fieldName = CurrentToken;
            Tokenizer.ReadNextToken();

            string typeName = CurrentToken;
            Tokenizer.ReadNextToken();

            _statement.Fields.Add( new FieldDefinition() { Name = fieldName, Type = typeName } );
        }
    }
}
