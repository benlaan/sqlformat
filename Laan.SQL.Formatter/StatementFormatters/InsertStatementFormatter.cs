using System;
using System.Linq;
using System.Text;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;
using System.Collections.Generic;

namespace Laan.Sql.Formatter
{
    public class InsertStatementFormatter : CustomStatementFormatter<InsertStatement>, IStatementFormatter
    {
        private const int MaxOneLineColumnCount = 4;

        public InsertStatementFormatter( IIndentable indentable, StringBuilder sql, InsertStatement statement )
            : base( indentable, sql, statement )
        {
        }

        #region IStatementFormatter Members

        public void Execute()
        {
            FormatInsert();
            FormatColumns();
            FormatInputData();
            FormatTerminator();
        }

        #endregion

        private void FormatInsert()
        {
            IndentAppendFormat( "{0} {1} {2}", Constants.Insert, Constants.Into, _statement.TableName );
        }

        private string FormatColumnWithSeparator( int index )
        {
            return _statement.Columns[ index ] + ( index < _statement.Columns.Count - 1 ? ", " : "" );
        }

        private void FormatColumns()
        {
            if ( _statement.Columns.Count > 0 )
            {
                string text = String.Join( ", ", _statement.Columns.ToArray() );
                List<string> lines = new List<string>();
                if ( text.Length > WrapMarginColumn )
                {
                    string line = "";
                    for ( int index = 0; index < _statement.Columns.Count; index++ )
                    {
                        if ( line.Length + _statement.Columns[ index ].Length >= WrapMarginColumn )
                        {
                            lines.Add( line );
                            line = "";
                        }
                        line += FormatColumnWithSeparator( index );
                    }
                    lines.Add( line );
                }

                if ( _statement.Columns.Count <= MaxOneLineColumnCount && FitsOnRow( text ) )
                    _sql.AppendFormat( " {0}\n", FormatBrackets( text ) );
                else
                {
                    _sql.Append( " (" );
                    NewLine();

                    using ( new IndentScope( this ) )
                    {
                        foreach ( string line in lines )
                            IndentAppendLine( line );
                        
                        IndentAppendLine( ")" );
                    }
                }
            }
            else
                NewLine();
        }

        private void FormatInputData()
        {
            if ( _statement.Values.Any() )
            {
                NewLine();
                using ( new IndentScope( this ) )
                {
                    foreach ( var values in _statement.Values )
                    {
                        IndentAppendFormat(
                            "{0} {1}{2}",
                            values == _statement.Values.First() ? " " + Constants.Values : new string( ' ', Constants.Values.Length + 1 ),
                            FormatBrackets( String.Join( ", ", values.ToArray() ) ),
                            values == _statement.Values.Last() ? "" : ",\n"
                        );
                    }
                }
            }
            else if ( _statement.SourceStatement != null )
            {
                using ( new IndentScope( this ) )
                {
                    var formatter = new SelectStatementFormatter( this, _sql, _statement.SourceStatement );
                    formatter.Execute();
                }
            }
            //else if ( _statement.ExecuteProcudure != null )
            //{
            //    using ( new IndentScope( this ) )
            //    {
            //        var formatter = new ExecuteProcedureFormatter( this, _sql, _statement.ExecuteProcudure );
            //        formatter.Execute();
            //    }
            //}
        }
    }
}
