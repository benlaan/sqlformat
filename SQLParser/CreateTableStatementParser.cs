using System;

namespace Laan.SQL.Parser
{
    public class CreateTableStatementParser : TableStatementParser
    {
        private const string NOT = "NOT";
        private const string NULL = "NULL";
        private const string ASC = "ASC";
        private const string DESC = "DESC";
        private const string IDENTITY = "IDENTITY";
        private const string COLLATE = "COLLATE";
        private const string DEFAULT = "DEFAULT";

        CreateTableStatement _statement;

        internal CreateTableStatementParser( Tokenizer tokenizer ) : base( tokenizer )
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

        private SqlType ProcessType()
        {
            string identifier = GetIdentifier();
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

        private void ProcessPrimaryKeyConstraint()
        {
            // this is the name of the constraint - not currenly used!
            string identifier = GetIdentifier();
            string orderBy = "";

            ExpectTokens( new[] { PRIMARY, KEY, CLUSTERED, OPEN_BRACKET } );

            string keyFieldName = GetIdentifier();

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
            Identity identity = null;

            string fieldName = GetIdentifier();
            SqlType type = ProcessType();

            if ( Tokenizer.TokenEquals( IDENTITY ) )
            {
                identity = ProcessIdentity();
            }

            if ( Tokenizer.TokenEquals( COLLATE ) )
            {
                type.Collation = CurrentToken;
                ReadNextToken();
            }

            if ( Tokenizer.TokenEquals( NULL ) )
            {
                nullability = Nullability.Nullable;
            }
            
            if ( Tokenizer.TokenEquals( NOT ) )
            {
                ExpectToken( NULL );
                nullability = Nullability.NotNullable;
            }

            if ( Tokenizer.TokenEquals( IDENTITY ) )
            {
                identity = ProcessIdentity();
            }

            if ( Tokenizer.TokenEquals( PRIMARY ) )
            {
                ExpectToken( KEY );
                nullability = Nullability.NotNullable;
                isPrimaryKey = true;
            }
            
            if ( Tokenizer.TokenEquals( CONSTRAINT ))
            {
                // TODO: process column constraint
                string name = GetIdentifier();
                ExpectToken( DEFAULT );
                string defaultValue = GetExpression();
            }
            _statement.Fields.Add(
                new FieldDefinition()
                {
                    Name = fieldName, Type = type, Nullability = nullability, IsPrimaryKey = isPrimaryKey, Identity = identity
                }
            );
        }

        private Identity ProcessIdentity()
        {
            Identity result = new Identity();

            ExpectToken( OPEN_BRACKET );

            result.Start = Int32.Parse( CurrentToken );
            ReadNextToken();

            ExpectToken( COMMA );

            result.Increment = Int32.Parse( CurrentToken );
            ReadNextToken();

            ExpectToken( CLOSE_BRACKET );

            return result;
        }
    }
}
