using System;
using System.Collections.Generic;
using System.Linq;

using Laan.Sql.Parser.Parsers;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser
{
    public class SqlTypeParser : CustomParser
    {
        public SqlTypeParser(ITokenizer tokenizer) : base(tokenizer)
        {
        }

        public SqlType Execute()
        {
            string identifier = GetIdentifier();
            if (String.Equals(identifier, Constants.As, StringComparison.InvariantCultureIgnoreCase))
                return null;

            var result = new SqlType(identifier);

            if (Tokenizer.IsNextToken(Constants.Precision))
            {
                result.Name = String.Format("{0} {1}", result.Name, Constants.Precision);
                ReadNextToken();
            }

            if (!Tokenizer.IsNextToken(Constants.OpenBracket))
                return result;

            using (Tokenizer.ExpectBrackets())
            {
                string token = CurrentToken;
                ReadNextToken();
                result.Max = (String.Compare(token, Constants.Max, true) == 0);

                if (!result.Max)
                {
                    result.Length = Int32.Parse(token);

                    if (Tokenizer.TokenEquals(Constants.Comma))
                    {
                        result.Scale = Int32.Parse(CurrentToken);
                        ReadNextToken();
                    }
                }
            }

            return result;
        }
    }
}
