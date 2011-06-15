using System.Collections.Generic;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Entities
{
    /// <summary>
    /// Entity that can have table hints
    /// </summary>
    public interface ITableHints
    {
        List<TableHint> TableHints { get; set; }
    }
}