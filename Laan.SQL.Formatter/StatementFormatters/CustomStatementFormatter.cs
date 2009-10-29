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

        protected void IndentAppend( string text )
        {
            for ( int count = 0; count < IndentLevel; count++ )
                _sql.Append( Indent );
            _sql.Append( text );
        }

        protected void IndentAppendFormat( string text, params object[] args )
        {
            IndentAppend( String.Format( text, args ) );
        }

        protected void IndentAppendLine( string text )
        {
            IndentAppend( text );
            NewLine();
        }

        protected void IndentAppendLineFormat( string text, params object[] args )
        {
            IndentAppendLine( String.Format( text, args ) );
        }

        protected void Append( string text )
        {
            _sql.Append( text );
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

        #endregion
        
        protected void FormatTop( Top top )
        {
            if ( top == null )
                return;

            string format = top.Brackets ? " TOP ({0}){1}" : " TOP {0}{1}";

            Append(
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
                        IndentAppend( "FROM (" );
                        NewLine( CanCompactFormat() ? 1 : 2 );

                        using ( new IndentScope( this ) )
                        {
                            var formatter = new SelectStatementFormatter( this, _sql, derivedTable.SelectStatement );
                            formatter.Execute();
                        }
                        NewLine( CanCompactFormat() ? 1 : 2 );
                        IndentAppend( String.Format( "){0}", from.Alias.Value ) );
                    }
                    else
                    {
                        NewLine();
                        IndentAppendFormat(
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
            IndentAppend( derivedJoin.Value );
            NewLine( 2 );
            using ( new IndentScope( this ) )
            {
                var formatter = new SelectStatementFormatter( this, _sql, derivedJoin.SelectStatement );
                formatter.Execute();
            } 
            NewLine( 2 );
            IndentAppend( String.Format( "){0}", derivedJoin.Alias.Value ) );
            NewLine();
            IndentAppendFormat(
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
                        IndentAppend( join.Value );
                        NewLine();
                        IndentAppendFormat(
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
                IndentAppendFormat(
                    "{0} {1}",
                    Constants.Where,
                    _statement.Where.FormattedValue( WhereLength, this )
                );
            }
        }

        protected void FormatTerminator()
        {
            if ( _statement.Terminated )
                Append( Constants.SemiColon );
        }

        protected void FormatStatement( IStatement statement )
        {
            var formatter = StatementFormatterFactory.GetFormatter( this, _sql, statement );
            formatter.Execute();
        }

        protected string FormatBrackets( string text )
        {
            return String.Format( _bracketFormats[ bracketSpaceOption ], text );
        }

        protected void FormatBlock( IStatement statement )
        {
            // do not increase indent when the child statement is a block.. 
            // this results in IF <> BEGIN END working as expected
            if ( statement is BlockStatement )
                FormatStatement( statement );
            else
                using ( new IndentScope( this ) )
                    FormatStatement( statement );
        }

        protected bool FitsOnRow( string text )
        {
            return text.Length <= ( WrapMarginColumn - ( IndentLevel * Indent.Length + CurrentColumn ) );
        }

        protected int CurrentColumn
        {
            get { return _sql.ToString().Split( '\n' ).Last().Length; }
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
