using System;
using System.Linq;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Parsers;
using Laan.Sql.Parser.Exceptions;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser
{
    public class SetStatementParser : StatementParser<SetStatement>
    {
        private string[] _options;

        public SetStatementParser( ITokenizer tokenizer ) : base( tokenizer )
        {
            _options = new[]
            {
                "CONCAT_NULL_YIELDS_NULL",
                "CURSOR_CLOSE_ON_COMMIT",
                "QUOTED_IDENTIFIER",
                "ARITHABORT",
                "ARITHIGNORE",
                "FMTONLY",
                "NOCOUNT",
                "NOEXEC",
                "NUMERIC_ROUNDABORT",
                "PARSEONLY",
                "ANSI_DEFAULTS",
                "ANSI_NULL_DFLT_OFF",
                "ANSI_NULL_DFLT_ON",
                "ANSI_NULLS",
                "ANSI_PADDING",
                "ANSI_WARNINGS",
                "FORCEPLAN",
                "SHOWPLAN_ALL",
                "SHOWPLAN_TEXT",
                "SHOWPLAN_XML",
                "STATISTICS",
                "IMPLICIT_TRANSACTIONS",
                "REMOTE_PROC_TRANSACTIONS",
                "XACT_ABORT"
            };        
        }

        private void ReadAssignment( SetStatement statement )
        {
            var parser = new ExpressionParser( Tokenizer );
            statement.Assignment = parser.Execute();
        }

        public override SetStatement Execute()
        {
            string identifier = GetIdentifier();

            if ( identifier.StartsWith( "@" ) )
            {
                var variableStatement = new SetVariableStatement();
                variableStatement.Variable = identifier;
                Tokenizer.ExpectToken( Constants.Assignment );
                variableStatement.Assignment = new ExpressionParser( Tokenizer ).Execute();

                return variableStatement;
            }
            else if ( _options.Contains( identifier ) )
            {
                var optionStatement = new SetOptionStatement();

                if ( String.Compare( identifier, Constants.Statistics, true ) == 0 )
                {
                    if ( !Tokenizer.IsNextToken( Constants.Io, Constants.Xml, Constants.Profile, Constants.Time ) )
                        throw new SyntaxException( "STATISTIC expects IO, XML, PROFILE, or TIME" );

                    identifier += " " + Tokenizer.Current.Value;
                    Tokenizer.ReadNextToken();
                }
                optionStatement.Option = identifier;

                if ( !Tokenizer.IsNextToken( Constants.On, Constants.Off ) )
                    throw new SyntaxException( "SET [option] expects ON or OFF" );

                optionStatement.Assignment = new StringExpression( Tokenizer.Current.Value, null );
                Tokenizer.ReadNextToken();

                return optionStatement;
            }
            else
            {
                switch ( identifier )
                {
                    case Constants.DateFirst:
                    {
                        SetDateFirstStatement statement = new SetDateFirstStatement();
                        ReadAssignment( statement );
                        return statement;
                    }

                    case Constants.DateFormat:
                    {
                        SetDateFormatStatement statement = new SetDateFormatStatement();
                        ReadAssignment( statement );
                        return statement;
                    }

                    case Constants.DeadlockPriority:
                    {
                        SetDeadlockPriorityStatement statement = new SetDeadlockPriorityStatement();
                        ReadAssignment( statement );
                        return statement;
                    }
                }
            }

            throw new ParserNotImplementedException( String.Format( "SET {0} is not supported", identifier ) );
        }
    }
}
