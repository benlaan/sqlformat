using System;

namespace Laan.Sql.Parser.Entities
{
    public enum Order
    {
        Ascending, Descending
    }
    
    public class IndexedColumn
    {
        /// <summary>
        /// Initializes a new instance of the IndexedColumn class.
        /// </summary>
        public IndexedColumn()
        {
            Order = Order.Ascending;
        }

        public string Name { get; set; }
        public Order Order { get; set; }
    }
}
