using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using Laan.SQL.Parser;
using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Formatter
{
    public enum BracketFormatOption
    {
        NoSpaces,
        SpacesWithinBrackets
    }

    public class CustomStatementFormatter<T> : IIndentable where T : CustomStatement
    {
        BracketFormatOption bracketSpaceOption = BracketFormatOption.NoSpaces;
        private IIndentable _indentable;
        private const int WrapMarginColumn = 80;

        private static Dictionary<BracketFormatOption, string> _bracketFormats;

        static CustomStatementFormatter()
        {
            _bracketFormats = new Dictionary<BracketFormatOption, string>()
            {
                { BracketFormatOption.NoSpaces, "({0})" },
                { BracketFormatOption.SpacesWithinBrackets, "( {0} )" },
            };
        }

        public CustomStatementFormatter( IIndentable indentable, StringBuilder sql, T statement )
        {
            _indentable = indentable;
            _sql = sql;
            _statement = statement;
        }

        private const int WhereLength = 5;
        protected StringBuilder _sql;
        protected T _statement;

        #region Rendering Utilities

        protected void Append( string text )
        {
            for ( int count = 0; count < IndentLevel; count++ )
                _sql.Append( Indent );
            _sql.Append( text );
        }

        protected void AppendFormat( string text, params object[] args )
        {
            Append( String.Format( text, args ) );
        }

        protected void NewLine( int times )
        {
            for ( int index = 0; index < times; index++ )
                _sql.AppendLine();
        }

        protected void NewLine()
        {
            NewLine( 1 );
        }

        protected void AppendLine( string text )
        {
            Append( text );
            NewLine();
        }

        protected void AppendLineFormat( string text, params object[] args )
        {
            AppendLine( String.Format( text, args ) );
        }

        #endregion
        
        protected void FormatTop( Top top )
        {
            if ( top == null )
                return;

            string format = top.Brackets ? " TOP ({0}){1}" : " TOP {0}{1}";

            _sql.Append(
                String.Format(
                    format,
                    top.Expression.FormattedValue( 0, this ),
                    top.Percent ? " PERCENT" : ""
                )
            );
        }

        protected void FormatFrom()
        {
            if ( _statement.From != null && _statement.From.Any() )
            {
                NewLine( CanCompactFormat() ? 0 : 1 );
                foreach ( var from in _statement.From )
                {
                    if ( from is DerivedTable )
                    {
                        DerivedTable derivedTable = (DerivedTable) from;
                        NewLine();
                        Append( "FROM (" );
                        NewLine( CanCompactFormat() ? 1 : 2 );

                        using ( new IndentScope( this ) )
                        {
                            var formatter = new SelectStatementFormatter( this, _sql, derivedTable.SelectStatement );
                            formatter.Execute();
                        }
                        NewLine( CanCompactFormat() ? 1 : 2 );
                        Append( String.Format( "){0}", from.Alias.Value ) );
                    }
                    else
                    {
                        NewLine();
                        AppendFormat(
                            "FROM {0}{1}",
                            from.Name, from.Alias.Value
                        );
                    }
                }
            }
        }

        private void FormatDerivedJoin( DerivedJoin derivedJoin )
        {
            NewLine( 2 );
            Append( derivedJoin.Value );
            NewLine( 2 );
            using ( new IndentScope( this ) )
            {
                var formatter = new SelectStatementFormatter( this, _sql, derivedJoin.SelectStatement );
                formatter.Execute();
            } 
            NewLine( 2 );
            Append( String.Format( "){0}", derivedJoin.Alias.Value ) );
            NewLine();
            AppendFormat(
                "  ON {0}",
                derivedJoin.Condition.FormattedValue( 4, this )
            );
        }

        protected void FormatJoins()
        {
            if ( _statement.Joins != null && _statement.Joins.Any() )
            {
                foreach ( var join in _statement.Joins )
                {
                    if ( join is DerivedJoin )
                        FormatDerivedJoin( (DerivedJoin) join );
                    else
                    {
                        NewLine( 2 );
                        Append( join.Value );
                        NewLine();
                        AppendFormat(
                            "{0}ON {1}",
                            new string( ' ', join.Length - Constants.On.Length ),
                            join.Condition.FormattedValue( join.Length, this )
                        );
                    }
                }
            }
        }

        protected void FormatWhere()
        {
            if ( _statement.Where != null )
            {
                NewLine( CanCompactFormat() ? 1 : 2 );
                AppendFormat(
                    "{0} {1}",
                    Constants.Where,
                    _statement.Where.FormattedValue( WhereLength, this )
                );
            }
        }

        protected void FormatTerminator()
        {
            if ( _statement.Terminated )
                _sql.Append( Constants.SemiColon );
        }

        protected void FormatStatement( IStatement statement )
        {
            using ( new IndentScope( this ) )
            {
                var formatter = StatementFormatterFactory.GetFormatter( this, _sql, statement );
                formatter.Execute();
            }
        }

        protected bool FitsOnRow( string text )
        {
            return text.Length <= ( WrapMarginColumn - ( IndentLevel * Indent.Length + CurrentColumn ) );
        }

        protected int CurrentColumn
        {
            get { return _sql.ToString().Split( '\n' ).Last().Length; }
        }

        protected string FormatBrackets( string text )
        {
            return String.Format( _bracketFormats[ bracketSpaceOption ], text );
        }

        protected bool IsExpressionOperatorAndOr( Expression expression )
        {
            CriteriaExpression where = expression as CriteriaExpression;
            return where == null || ( where.Operator != Constants.And && where.Operator != Constants.Or );
        }

        protected virtual bool CanCompactFormat()
        {
            return IsExpressionOperatorAndOr( _statement.Where );
        }


        public void IncreaseIndent()
        {
            IndentLevel++;
        }

        public void DecreaseIndent()
        {
            IndentLevel--;
        }

        public virtual bool CanInline
        {
            get { return false; }
        }

        public string Indent
        {
            get { return _indentable.Indent; }
            set { _indentable.Indent = value; }
        }

        public int IndentLevel
        {
            get { return _indentable.IndentLevel; }
            set { _indentable.IndentLevel = value; }
        }

    }
}
