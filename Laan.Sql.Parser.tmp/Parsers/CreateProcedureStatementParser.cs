using System;
using System.Linq;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser.Parsers
{
    public class CreateProcedureStatementParser : StatementParser<CreateProcedureStatement>
    {
        internal CreateProcedureStatementParser(ITokenizer tokenizer) : base(tokenizer)
        {
        }

        public override CreateProcedureStatement Execute()
        {
            _statement = new CreateProcedureStatement() { Name = GetDotNotationIdentifier(), IsAlter = IsAlter, IsShortForm = IsShortForm };

            if (!Tokenizer.IsNextToken(Constants.As))
            {
                var parser = new VariableDefinitionParser(Tokenizer);

                if (Tokenizer.IsNextToken(Constants.OpenBracket))
                {
                    using (Tokenizer.ExpectBrackets())
                    {
                        _statement.Arguments = parser.Execute();

                        if (_statement.Arguments.Count == 0)
                            throw new SyntaxException("expected 1 or more arguments");
                    }
                    _statement.HasBracketedArguments = true;
                }
                else
                    _statement.Arguments = parser.Execute();
            }

            ExpectToken(Constants.As);

            _statement.Definition = ParserFactory.Execute(Tokenizer, true).Single();

            return _statement;
        }

        public bool IsAlter { get; set; }
        public bool IsShortForm { get; set; }
    }
}
