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
        private const string DOT = ".";
        private const string OPEN_SQUARE_BRACE = "[";
        private const string CLOSE_SQUARE_BRACE = "]";
        private const string CONSTRAINT = "CONSTRAINT";
        private const string CLUSTERED = "CLUSTERED";
        private const string ASC = "ASC";
        private const string DESC = "DESC";

        CreateTableStatement _statement;

        internal CreateTableStatementParser( Tokenizer tokenizer )
            : base( tokenizer )
        {
        }

        public override IStatement Execute()
        {
            _statement = new CreateTableStatement();

            _statement.TableName = GetTableName();

            ExpectToken( OPEN_BRACKET );
            do
            {
                if ( Tokenizer.TokenEquals( CONSTRAINT ) )
                    ProcessPrimaryKeyConstraint();
                else
                    ProcessFieldDefinition(); 

            } while ( Tokenizer.TokenEquals( COMMA ) );

            ExpectToken( CLOSE_BRACKET );

            return _statement;
        }

        private string GetTableName()
        {
            string tableName = "";
            do
            {
                string identifier = ProcessSquareBracketedIdentifier();
                tableName += ( tableName != "" ? DOT : "" ) + identifier;
            }
            while ( Tokenizer.TokenEquals( DOT ) );

            return tableName;
        }

        private SqlType ProcessType()
        {
            string identifier = ProcessSquareBracketedIdentifier();
            SqlType result = new SqlType( identifier );

            if ( Tokenizer.TokenEquals( OPEN_BRACKET ) )
            {
                string token = CurrentToken;
                ReadNextToken();
                result.Length = Int32.Parse( token );

                if ( Tokenizer.TokenEquals( COMMA ) )
                {
                    result.Scale = Int32.Parse( CurrentToken );
                    ReadNextToken();
                }

                ExpectToken( CLOSE_BRACKET );
            }
            return result;
        }

        private string ProcessSquareBracketedIdentifier()
        {
            string identifier = "";
            if ( Tokenizer.TokenEquals( OPEN_SQUARE_BRACE ) )
            {
                identifier += OPEN_SQUARE_BRACE + CurrentToken + CLOSE_SQUARE_BRACE;
                ReadNextToken();
                ExpectToken( CLOSE_SQUARE_BRACE );
            }
            else
            {
                identifier = CurrentToken;
                ReadNextToken();
            }
            return identifier;
        }

        private void ProcessPrimaryKeyConstraint()
        {
            // this is the name of the constraint - not currenly used!
            string identifier = ProcessSquareBracketedIdentifier();
            string orderBy = "";

            ExpectToken( PRIMARY );
            ExpectToken( KEY );
            ExpectToken( CLUSTERED );

            ExpectToken( OPEN_BRACKET );
            string keyFieldName = ProcessSquareBracketedIdentifier();

            FieldDefinition keyField = _statement.Fields.FindByName( keyFieldName );
            if ( keyField != null )
                keyField.IsPrimaryKey = true;

            string token = CurrentToken;
            if ( Tokenizer.TokenEquals( ASC ) || Tokenizer.TokenEquals( DESC ) )
                orderBy = token;

            ExpectToken( CLOSE_BRACKET );
        }

        private void ProcessFieldDefinition()
        {
            Nullability nullability = Nullability.Nullable;
            bool isPrimaryKey = false;

            string fieldName = ProcessSquareBracketedIdentifier();
            SqlType type = ProcessType();

            if ( Tokenizer.TokenEquals( NULL ) )
            {
                nullability = Nullability.Nullable;
            }
            else if ( Tokenizer.TokenEquals( NOT ) )
            {
                ExpectToken( NULL );
                nullability = Nullability.NotNullable;
            }
            else if ( Tokenizer.TokenEquals( PRIMARY ) )
            {
                ExpectToken( KEY );
                nullability = Nullability.NotNullable;
                isPrimaryKey = true;
            }

            _statement.Fields.Add(
                new FieldDefinition()
                {
                    Name = fieldName,
                    Type = type,
                    Nullability = nullability,
                    IsPrimaryKey = isPrimaryKey
                }
            );
        }
    }
}
