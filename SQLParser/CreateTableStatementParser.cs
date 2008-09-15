using System;

namespace Laan.SQLParser
{
    public class CreateTableStatementParser : StatementParser
    {
        private const string OPEN_BRACKET = "(";
        private const string CLOSE_BRACKET = ")";
        private const string NOT = "NOT";
        private const string NULL = "NULL";
        private const string PRIMARY = "PRIMARY";
        private const string KEY = "KEY";

        CreateTableStatement _statement;

        internal CreateTableStatementParser( Tokenizer tokenizer ) : base( tokenizer )
        {
        }

        public override IStatement Execute()
        {
            _statement = new CreateTableStatement();

            _statement.TableName = Tokenizer.Current;
            ReadNextToken();

            ExpectToken( OPEN_BRACKET );

            do
            {
                ProcessField();

            } while ( Tokenizer.TokenEquals( COMMA ) );

            ExpectToken( CLOSE_BRACKET );

            return _statement;
        }

        
        private string GetTypeName()
        {
            string typeName = CurrentToken;
            ReadNextToken();

            if (Tokenizer.TokenEquals( OPEN_BRACKET ) )
            {
                typeName += OPEN_BRACKET;
                do
                {
                    typeName += CurrentToken;
                    ReadNextToken();
                }
                while ( CurrentToken != CLOSE_BRACKET );
                typeName += CLOSE_BRACKET;
                ReadNextToken();
            }

            return typeName;
        }

        private void ProcessField()
        {
            Nullability nullability = Nullability.Nullable;
            bool isPrimaryKey = false;

            string fieldName = CurrentToken;
            ReadNextToken();

            string typeName = GetTypeName();
            if ( Tokenizer.TokenEquals( NULL ) )
            {
                nullability = Nullability.Nullable;
            }
            else
                if ( Tokenizer.TokenEquals( NOT ) )
                {
                    ExpectToken( NULL );
                    nullability = Nullability.NotNullable;
                }
                else
                    if ( Tokenizer.TokenEquals( PRIMARY ) )
                    {
                        ExpectToken( KEY );
                        nullability = Nullability.NotNullable;
                        isPrimaryKey = true;
                    }

            _statement.Fields.Add( 
                new FieldDefinition() 
                {
                    Name = fieldName,
                    Type = typeName,
                    Nullability = nullability,
                    IsPrimaryKey = isPrimaryKey
                } 
            );
        }
    }
}
