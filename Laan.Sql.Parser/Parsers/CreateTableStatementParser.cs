using System;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Parsers
{
    public class CreateTableStatementParser : TableStatementParser<CreateTableStatement>
    {
        internal CreateTableStatementParser(ITokenizer tokenizer) : base(tokenizer)
        {
        }

        public override CreateTableStatement Execute()
        {
            _statement = new CreateTableStatement();
            _statement.TableName = GetTableName();

            using (Tokenizer.ExpectBrackets())
            {
                do
                {
                    if (Tokenizer.TokenEquals(Constants.Constraint))
                        ProcessPrimaryKeyConstraint();
                    else
                        ProcessFieldDefinition();
                }
                while (Tokenizer.TokenEquals(Constants.Comma));
            }

            ProcessTerminator();
            return _statement;
        }

        private void ProcessPrimaryKeyConstraint()
        {
            // this is the name of the constraint - not currenly used!
            var identifier = GetIdentifier();
            var orderBy = String.Empty;

            Tokenizer.ExpectTokens(new[] { Constants.Primary, Constants.Key, Constants.Clustered });

            using (Tokenizer.ExpectBrackets())
            {
                var keyFieldName = GetIdentifier();

                var keyField = _statement.Fields.FindByName(keyFieldName);
                if (keyField != null)
                    keyField.IsPrimaryKey = true;

                var token = CurrentToken;
                if (Tokenizer.TokenEquals(Constants.Ascending) || Tokenizer.TokenEquals(Constants.Descending))
                    orderBy = token;
            }
        }

        private SqlType ProcessType()
        {
            var sqlTypeParser = new SqlTypeParser(Tokenizer);
            return sqlTypeParser.Execute();
        }

        private void ProcessFieldDefinition()
        {
            var nullability = Nullability.Nullable;
            var isPrimaryKey = false;
            Identity identity = null;

            var fieldName = GetIdentifier();
            var type = ProcessType();
            if (type == null)
            {
                var calcExpression = new FieldDefinition { Name = fieldName, Nullability = Nullability.Nullable, Type = null };
                var parser = new ExpressionParser(Tokenizer);
                calcExpression.CalculatedValue = parser.Execute();
                _statement.Fields.Add(calcExpression);
                return;
            }

            if (Tokenizer.TokenEquals(Constants.Identity))
            {
                identity = ProcessIdentity();
            }

            if (Tokenizer.TokenEquals(Constants.Collate))
            {
                type.Collation = CurrentToken;
                ReadNextToken();
            }

            if (Tokenizer.TokenEquals(Constants.Identity))
            {
                identity = ProcessIdentity();
            }

            if (Tokenizer.TokenEquals(Constants.Primary))
            {
                Tokenizer.ExpectToken(Constants.Key);
                nullability = Nullability.NotNullable;
                isPrimaryKey = true;
            }

            if (Tokenizer.TokenEquals(Constants.Constraint))
            {
                // TODO: process column constraint
                var name = GetIdentifier();
                Tokenizer.ExpectToken(Constants.Default);
                using (Tokenizer.ExpectBrackets())
                {
                    var expression = ProcessSimpleExpression();
                    var defaultValue = expression.Value;
                }
            }

            if (Tokenizer.TokenEquals(Constants.Default))
            {
                // TODO: process column constraint
                var expression = ProcessSimpleExpression();
                var defaultValue = expression.Value;
            }

            if (Tokenizer.TokenEquals(Constants.Null))
            {
                nullability = Nullability.Nullable;
            }

            if (Tokenizer.TokenEquals(Constants.Not))
            {
                Tokenizer.ExpectToken(Constants.Null);
                nullability = Nullability.NotNullable;
            }

            _statement.Fields.Add(new FieldDefinition
            {
                Name = fieldName,
                Type = type,
                Nullability = nullability,
                IsPrimaryKey = isPrimaryKey,
                Identity = identity
            });
        }

        private Identity ProcessIdentity()
        {
            var result = new Identity();

            using (Tokenizer.ExpectBrackets())
            {
                result.Start = Int32.Parse(CurrentToken);
                ReadNextToken();

                Tokenizer.ExpectToken(Constants.Comma);

                result.Increment = Int32.Parse(CurrentToken);
                ReadNextToken();
            }

            return result;
        }
    }
}
