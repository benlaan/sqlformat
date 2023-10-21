using System;
using System.Collections.Generic;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser.Parsers
{
    public class VariableDefinitionParser : CustomParser
    {
        private List<VariableDefinition> _statement;

        public VariableDefinitionParser(ITokenizer tokenizer) : base(tokenizer)
        {
            _statement = new List<VariableDefinition>();
        }

        private string GetSqlType()
        {
            string typeName = Tokenizer.Current.Value;
            Tokenizer.ReadNextToken();

            if (Tokenizer.IsNextToken(Constants.OpenBracket))
            {
                using (new BracketStructure(Tokenizer))
                {
                    string number = Tokenizer.Current.Value;
                    Tokenizer.ReadNextToken();

                    if (Tokenizer.TokenEquals(Constants.Comma))
                    {
                        string precision = Tokenizer.Current.Value;
                        Tokenizer.ReadNextToken();
                        return String.Format("{0}({1}, {2})", typeName, number, precision);
                    }
                    else
                        return String.Format("{0}({1})", typeName, number);
                }
            }

            return typeName;
        }

        public List<VariableDefinition> Execute()
        {
            do
            {
                string name = Tokenizer.Current.Value;
                Tokenizer.ReadNextToken();

                if (!Tokenizer.HasMoreTokens)
                    throw new SyntaxException(String.Format("type missing for declaration of variable '{0}'", name));

                string type = GetSqlType();
                var definition = new VariableDefinition(name, type);

                if (Tokenizer.TokenEquals(Constants.Assignment))
                {
                    var parser = new ExpressionParser(Tokenizer);
                    definition.DefaultValue = parser.Execute();
                }

                _statement.Add(definition);
            }
            while (Tokenizer.TokenEquals(Constants.Comma));

            return _statement;
        }
    }
}