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
                if ( Tokenizer.TokenEquals( OPEN_SQUARE_BRACE ) )
                {
                    string seperator = tableName != "" ? DOT : "";
                    tableName += seperator + OPEN_SQUARE_BRACE + CurrentToken + CLOSE_SQUARE_BRACE;
                    ReadNextToken();
                    ExpectToken( CLOSE_SQUARE_BRACE );
                }
            }
            while ( Tokenizer.TokenEquals( DOT ) );

            if ( tableName == "" )
            {
                tableName = CurrentToken;
                ReadNextToken();
            }

            return tableName;
        }

        private string GetTypeName()
        {
            string typeName = CurrentToken;
            ReadNextToken();

            if ( Tokenizer.TokenEquals( OPEN_BRACKET ) )
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


        private string GetFieldName()
        {
            string fieldName = "";
            if ( Tokenizer.TokenEquals( OPEN_SQUARE_BRACE ) )
            {
                fieldName += OPEN_SQUARE_BRACE + CurrentToken + CLOSE_SQUARE_BRACE;
                ReadNextToken();
                ExpectToken( CLOSE_SQUARE_BRACE );
            }
            else
            {
                fieldName = CurrentToken;
                ReadNextToken();
            }

            return fieldName;
        }

        private void ProcessFieldDefinition()
        {
            Nullability nullability = Nullability.Nullable;
            bool isPrimaryKey = false;

            string orderBy = "";
            if ( Tokenizer.TokenEquals( CONSTRAINT ) )
            {
                string keyName = GetFieldName();
                ExpectToken( PRIMARY );
                ExpectToken( KEY );
                ExpectToken( CLUSTERED );

                ExpectToken( OPEN_BRACKET );
                string keyFieldName = GetFieldName();

                FieldDefinition keyField = _statement.Fields.FindByName( keyFieldName );
                if ( keyField != null )
                    keyField.IsPrimaryKey = true;

                string token = CurrentToken;
                if ( Tokenizer.TokenEquals( ASC ) || Tokenizer.TokenEquals( DESC ) )
                    orderBy = token;

                ExpectToken( CLOSE_BRACKET );
            }
            else
            {
                string fieldName = GetFieldName();
                string typeName = GetTypeName();

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
                        Type = typeName,
                        Nullability = nullability,
                        IsPrimaryKey = isPrimaryKey
                    }
                );
            } 
        }
    }
}
