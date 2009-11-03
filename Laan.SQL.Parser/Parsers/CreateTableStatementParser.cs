using System;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Parsers
{
    public class CreateTableStatementParser : TableStatementParser<CreateTableStatement>
    {
        internal CreateTableStatementParser( ITokenizer tokenizer ) : base( tokenizer )
        {
        }

        public override CreateTableStatement Execute()
        {
            _statement = new CreateTableStatement();

            _statement.TableName = GetTableName();

            using ( Tokenizer.ExpectBrackets() )
            {
                do
                {
                    if ( Tokenizer.TokenEquals( Constants.Constraint ) )
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

            Tokenizer.ExpectTokens( new[] { Constants.Primary, Constants.Key, Constants.Clustered } );

            using ( Tokenizer.ExpectBrackets() )
            {
                string keyFieldName = GetIdentifier();

                FieldDefinition keyField = _statement.Fields.FindByName( keyFieldName );
                if ( keyField != null )
                    keyField.IsPrimaryKey = true;

                string token = CurrentToken;
                if ( Tokenizer.TokenEquals( Constants.Ascending ) || Tokenizer.TokenEquals( Constants.Descending ) )
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

            if ( Tokenizer.TokenEquals( Constants.Identity ) )
            {
                identity = ProcessIdentity();
            }

            if ( Tokenizer.TokenEquals( Constants.Collate ) )
            {
                type.Collation = CurrentToken;
                ReadNextToken();
            }

            if ( Tokenizer.TokenEquals( Constants.Null ) )
            {
                nullability = Nullability.Nullable;
            }

            if ( Tokenizer.TokenEquals( Constants.Not ) )
            {
                Tokenizer.ExpectToken( Constants.Null );
                nullability = Nullability.NotNullable;
            }

            if ( Tokenizer.TokenEquals( Constants.Identity ) )
            {
                identity = ProcessIdentity();
            }

            if ( Tokenizer.TokenEquals( Constants.Primary ) )
            {
                Tokenizer.ExpectToken( Constants.Key );
                nullability = Nullability.NotNullable;
                isPrimaryKey = true;
            }

            if ( Tokenizer.TokenEquals( Constants.Constraint ) )
            {
                // TODO: process column constraint
                string name = GetIdentifier();
                Tokenizer.ExpectToken( Constants.Default );
                using ( Tokenizer.ExpectBrackets() )
                {
                Expression expression = ProcessExpression();
                string defaultValue = expression.Value;
                }
            }

            if ( Tokenizer.TokenEquals( Constants.Default ) )
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
