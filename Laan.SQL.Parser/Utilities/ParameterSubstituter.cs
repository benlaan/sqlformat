using System;
using System.Linq;
using System.Collections.Generic;

using Laan.Sql.Parser;

namespace Laan.NHibernate.Appender
{
    public class ParameterSubstituter
    {
        private string ProcessParameter(ITokenizer tokenizer, string parameterName)
        {
            // the following code produces a parameter that supports Guids. For unknown reasons, NH
            // is supplying the parameter value as an unquoted set of alpha numerics, so, here they are processed 
            // until the next token is NOT a dash
            int tokenCount = 0;
            string token = "";
            //string regexDateFormat = GetDateFormatRegex();
            tokenizer.SkipWhiteSpace = false;
            do
            {
                if (tokenizer.Current.Type != TokenType.BlockedText)
                {
                    token +=
                        // TODO: the code below will not work until the parser can retain embedded comments
                        //String.Format("/* {0} */ ", parameterName) +
                        tokenizer.Current.Value;

                    if (tokenizer.Current.Type != TokenType.WhiteSpace)
                        tokenCount++;
                }
                tokenizer.ReadNextToken();
            }
            while (tokenizer.HasMoreTokens && !tokenizer.IsNextToken(Constants.Comma));

            tokenizer.SkipWhiteSpace = true;

            return tokenCount > 1 && !token.StartsWith("'") ? String.Format("'{0}'", token.Trim().ToUpper()) : token;
        }

        public string UpdateParamsWithValues(string originalSql)
        {
            string[] parts = originalSql.Replace("N'", "'").Replace("''", "'").Split(';');
            if (parts.Length <= 1)
                return originalSql;

            string sql = String.Join(";", parts.Take(parts.Length - 1).ToArray());
            string paramList = parts.Last();

            var parameters = new Dictionary<string, string>();
            ITokenizer tokenizer = new SqlTokenizer(paramList);
            tokenizer.ReadNextToken();
            do
            {
                string parameter = tokenizer.Current.Value;

                tokenizer.ReadNextToken();
                tokenizer.ExpectToken(Constants.Assignment);

                parameters.Add(parameter, ProcessParameter(tokenizer, parameter));
            }
            while (tokenizer.TokenEquals(Constants.Comma));

            foreach (var item in parameters.Reverse())
                sql = sql.Replace(item.Key, item.Value);

            return sql;
        }

    }
}
