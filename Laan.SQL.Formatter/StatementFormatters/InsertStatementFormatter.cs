using System;
using System.Linq;
using System.Text;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
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

        private void FormatColumns()
        {
            if ( _statement.Columns.Count > 0 )
            {
                string text = String.Join( ", ", _statement.Columns.ToArray() );

                if ( _statement.Columns.Count <= MaxOneLineColumnCount && FitsOnRow( text ) )
                    _sql.AppendFormat( " {0}", FormatBrackets( text ) );
                else
                {
                    _sql.Append( " (" );
                    NewLine();

                    using ( new IndentScope( this ) )
                    {
                        IndentAppendLine( text );
                        IndentAppendLine( ")" );
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
