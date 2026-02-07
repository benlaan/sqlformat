using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Formatter
{
    public class CreateIndexStatementFormatter : CustomStatementFormatter<CreateIndexStatement>, IStatementFormatter
    {
        public CreateIndexStatementFormatter( IIndentable indentable, StringBuilder sql, CreateIndexStatement statement )
            : base( indentable, sql, statement )
        {
        }

        #region IStatementFormatter Members

        public void Execute()
        {
            AppendKeyword(Constants.Create);
            _sql.Append(" ");
            
            if (_statement.Unique)
            {
                AppendKeyword(Constants.Unique);
                _sql.Append(" ");
            }

            if (!_statement.Clustered)
            {
                AppendKeyword(Constants.NonClustered);
                _sql.Append(" ");
            }

            AppendKeyword(Constants.Index);
            _sql.Append( " " );
            _sql.Append( _statement.IndexName );
            _sql.Append( " " );
            AppendKeyword(Constants.On);
            _sql.Append( " " );
            _sql.Append( _statement.TableName );

            if ( _statement.Columns.Count > 1 )
            {
                _sql.AppendLine();
                _sql.AppendLine( "(" );
                using ( new IndentScope( this ) )
                {

                    int columnCount = _statement.Columns.Count;

                    for ( int i = 0; i < columnCount; i++)
                    {
                        var column = _statement.Columns[ i ];

                        IndentAppendFormat( "{0}", FormatColumn( column ) );

                        if ( i < columnCount - 1 )
                            _sql.Append( "," );

                        _sql.AppendLine();
                    }

                }
                _sql.AppendLine( ")" );
            }
            else
            {
                _sql.AppendFormat( " ( {0} )", FormatColumn( _statement.Columns[ 0 ] ) );
            }

            int optionsCount = _statement.RelationalIndexOptions.Count;
            if ( optionsCount > 0 )
            {

                if ( optionsCount == 1 )
                    _sql.Append( " " );

                AppendKeyword(Constants.With);

                _sql.AppendFormat( " ( {0} )", string.Join( ", ", _statement.RelationalIndexOptions.Select( x => FormatIndexOptions( x ) ).ToArray() ) );
            }

            if ( !string.IsNullOrEmpty( _statement.FileGroupName ) )
            {
                _sql.AppendFormat( " {0} {1}", Constants.On, _statement.FileGroupName );
            }
            FormatTerminator();
        }

        private Dictionary<IndexWithOption, string> _indexOptionMap = new Dictionary<IndexWithOption, string>()
            {
                { IndexWithOption.PadIndex, Constants.PadIndex },
                { IndexWithOption.SortInTempDb, Constants.SortInTempDb },
                { IndexWithOption.IgnoreDupKey, Constants.IgnoreDupKey },
                { IndexWithOption.StatisticsNorecompute, Constants.StatisticsNorecompute},
                { IndexWithOption.DropExisting, Constants.DropExisting },
                { IndexWithOption.Online, Constants.Online },
                { IndexWithOption.AllowRowLocks, Constants.AllowRowLocks },
                { IndexWithOption.AllowPageLocks, Constants.AllowPageLocks }
            };

        private string FormatIndexOptions( RelationalIndexOption option )
        {
            return string.Format( "{0} = {1}", _indexOptionMap[ option.Option ], option.Assignment.Value );
        }

        private string FormatColumn(IndexedColumn column)
        {
            string s = column.Name;

            if ( column.Order == Order.Descending )
                s += " " + Constants.Descending;

            return s;
        }

        #endregion
    }
}
