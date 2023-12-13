using System;
using System.Linq;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Parsers
{
    public class CommonTableExpressionStatementParser : CriteriaStatementParser<CommonTableExpressionStatement>
    {
        public CommonTableExpressionStatementParser(ITokenizer tokenizer) : base(tokenizer)
        {

        }

        public override CommonTableExpressionStatement Execute()
        {
            _statement = new CommonTableExpressionStatement();
            do
            {
                var commonTableExpression = new CommonTableExpression();

                commonTableExpression.Name = GetDotNotationIdentifier();

                if (Tokenizer.IsNextToken(Constants.OpenBracket))
                {
                    using (Tokenizer.ExpectBrackets())
                    {
                        commonTableExpression.ColumnNames.AddRange(GetIdentifierList());
                    }
                }

                ExpectToken(Constants.As);

                using (Tokenizer.ExpectBrackets())
                {
                    ExpectToken(Constants.Select);

                    var parser = new SelectStatementParser(Tokenizer);
                    var statement = parser.Execute();

                    commonTableExpression.Fields = statement.Fields;
                    commonTableExpression.From = statement.From;
                    commonTableExpression.Top = statement.Top;
                    commonTableExpression.Distinct = statement.Distinct;
                    commonTableExpression.GroupBy = statement.GroupBy;
                    commonTableExpression.OrderBy = statement.OrderBy;
                    commonTableExpression.Having = statement.Having;
                    commonTableExpression.Where = statement.Where;
                    commonTableExpression.SetOperation = statement.SetOperation;
                }

                _statement.CommonTableExpressions.Add(commonTableExpression);
            }
            while (Tokenizer.HasMoreTokens && Tokenizer.TokenEquals(Constants.Comma));

            ExpectToken(Constants.Select);

            var statementParser = new SelectStatementParser(Tokenizer);
            _statement.Statement = statementParser.Execute();

            return _statement;
        }
    }
}
