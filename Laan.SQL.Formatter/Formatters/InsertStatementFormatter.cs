using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class InsertStatementFormatter : CustomFormatter<InsertStatement>, IStatementFormatter
    {
        private const int MaxOneLineColumnCount = 4;

        public InsertStatementFormatter( string indent, int indentStep, StringBuilder sql, InsertStatement statement )
            : base( indent, indentStep, sql, statement )
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
            _sql.Append(
                String.Format( "{0} {1} {2}", Constants.Insert, Constants.Into, _statement.TableName ) );
        }

        private void FormatColumns()
        {
            if ( _statement.Columns.Count > 0 )
            {
                string text = String.Join( ", ", _statement.Columns.ToArray() );

                if ( _statement.Columns.Count <= MaxOneLineColumnCount && FitsOnRow( text ) )
                    AppendFormat( " {0}", FormatBrackets( text ) );
                else
                {
                    _sql.Append( " (" );
                    NewLine();

                    using ( new IndentScope( this ) )
                    {
                        Append( text );
                        NewLine();
                        Append( ")" );
                    }
                }
            }
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
                        AppendFormat(
                            " {0} {1}{2}",
                            values == _statement.Values.First() ? Constants.Values : new string( ' ', Constants.Values.Length ),
                            FormatBrackets( String.Join( ", ", values.ToArray() ) ),
                            values == _statement.Values.Last() ? "" : ",\n"
                        );
                    }
                }

            }
            else if ( _statement.SourceStatement != null )
            {
                NewLine();
                var formatter = new SelectStatementFormatter( _indent, _indentStep + 1, _sql, _statement.SourceStatement, true );
                formatter.Execute();
            }
        }
    }
}
