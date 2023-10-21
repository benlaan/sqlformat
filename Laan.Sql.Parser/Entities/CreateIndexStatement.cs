using System;
using System.Collections.Generic;
using System.Linq;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Entities
{
    public enum IndexWithOption
    {
        PadIndex,
        SortInTempDb,
        IgnoreDupKey,
        StatisticsNorecompute,
        DropExisting,
        Online,
        AllowRowLocks,
        AllowPageLocks
    }

    public class RelationalIndexOption
    {

        /// <summary>
        /// Initializes a new instance of the SortedField class.
        /// </summary>
        /// <param name="map"></param>
        public RelationalIndexOption()
        {
        }

        public IndexWithOption Option { get; set; }

        public Expression Assignment { get; set; }

    }

    public class CreateIndexStatement : CustomStatement
    {
        public CreateIndexStatement()
        {
            Unique = false;
            Clustered = false;
            Columns = new List<IndexedColumn>();
            RelationalIndexOptions = new List<RelationalIndexOption>();
        }

        public bool Clustered { get; set; }
        public bool Unique { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public List<IndexedColumn> Columns { get; set; }
        public List<RelationalIndexOption> RelationalIndexOptions { get; set; }
        public string FileGroupName { get; set; }

        #region IStatement Members

        public override string Identifier
        {
            get
            {
                return String.Format(
                    "CREATE INDEX {0}ON {1}({2})",
                    IndexName,
                    TableName,
                    String.Join( ", ", Columns.Select( c => c.Name ).ToArray() )
                );
            }
        }

        #endregion
    }
}
