using System;

using Laan.SQL.Parser.Expressions;

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

        internal CreateTableStatementParser( ITokenizer tokenizer )
            : base( tokenizer )
        {
        }

        public override IStatement Execute()
        {
            _statement = new CreateTableStatement();

            _statement.TableName = GetTableName();

            using ( Tokenizer.ExpectBrackets() )
            {
                do
                {
                    if ( Tokenizer.TokenEquals( CONSTRAINT ) )
                        ProcessPrimaryKeyConstraint();
                    else
                        ProcessFieldDefinition();

                } while ( Tokenizer.TokenEquals( Constants.Comma ) );
            }
            return _statement;
        }

        private SqlType ProcessType()
        {
            string identifier = GetIdentifier();
            SqlType result;
            if (identifier == Constants.As )
            {
                result = null;
            }
            else
            {
                result = new SqlType( identifier );

                if ( Tokenizer.IsNextToken( Constants.OpenBracket ) )
                {
                    using ( Tokenizer.ExpectBrackets() )
                    {
                        string token = CurrentToken;
                        ReadNextToken();
                        result.Max = ( String.Compare( token, "MAX", true ) == 0 );

                        if ( !result.Max )
                        {
                            result.Length = Int32.Parse( token );

                            if ( Tokenizer.TokenEquals( Constants.Comma ) )
                            {
                                result.Scale = Int32.Parse( CurrentToken );
                                ReadNextToken();
                            }
                        }
                    }
                }
            }
            return result;
        }

        private void ProcessPrimaryKeyConstraint()
        {
            // this is the name of the constraint - not currenly used!
            string identifier = GetIdentifier();
            string orderBy = "";

            Tokenizer.ExpectTokens( new[] { PRIMARY, KEY, CLUSTERED } );

            using ( Tokenizer.ExpectBrackets() )
            {
                string keyFieldName = GetIdentifier();

                FieldDefinition keyField = _statement.Fields.FindByName( keyFieldName );
                if ( keyField != null )
                    keyField.IsPrimaryKey = true;

                string token = CurrentToken;
                if ( Tokenizer.TokenEquals( ASC ) || Tokenizer.TokenEquals( DESC ) )
                    orderBy = token;
            }
        }

        private void ProcessFieldDefinition()
        {
            Nullability nullability = Nullability.Nullable;
            bool isPrimaryKey = false;
            Identity identity = null;

            string fieldName = GetIdentifier();
            SqlType type = ProcessType();
            if ( type == null )
            {
                FieldDefinition calcExpression = new FieldDefinition() { Name = fieldName, Nullability = Nullability.Nullable, Type = null };
                ExpressionParser parser = new ExpressionParser( Tokenizer );
                calcExpression.CalculatedValue = parser.Execute();
                _statement.Fields.Add( calcExpression );
                return;
            }

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
                Tokenizer.ExpectToken( NULL );
                nullability = Nullability.NotNullable;
            }

            if ( Tokenizer.TokenEquals( IDENTITY ) )
            {
                identity = ProcessIdentity();
            }

            if ( Tokenizer.TokenEquals( PRIMARY ) )
            {
                Tokenizer.ExpectToken( KEY );
                nullability = Nullability.NotNullable;
                isPrimaryKey = true;
            }

            if ( Tokenizer.TokenEquals( CONSTRAINT ) )
            {
                // TODO: process column constraint
                string name = GetIdentifier();
                Tokenizer.ExpectToken( DEFAULT );
                using ( Tokenizer.ExpectBrackets() )
                {
                Expression expression = ProcessExpression();
                string defaultValue = expression.Value;
                }
            }

            if ( Tokenizer.TokenEquals( DEFAULT ) )
            {
                // TODO: process column constraint
                Expression expression = ProcessExpression();
                string defaultValue = expression.Value;
            }
            _statement.Fields.Add(
                new FieldDefinition()
                {
                    Name = fieldName,
                    Type = type,
                    Nullability = nullability,
                    IsPrimaryKey = isPrimaryKey,
                    Identity = identity
                }
            );
        }

        private Identity ProcessIdentity()
        {
            Identity result = new Identity();

            using ( Tokenizer.ExpectBrackets() )
            {
                result.Start = Int32.Parse( CurrentToken );
                ReadNextToken();

                Tokenizer.ExpectToken( Constants.Comma );

                result.Increment = Int32.Parse( CurrentToken );
                ReadNextToken();
            }

            return result;
        }
    }
}
