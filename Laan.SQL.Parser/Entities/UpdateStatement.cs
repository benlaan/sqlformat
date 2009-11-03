using System;

namespace Laan.Sql.Parser.Entities
{
    public class UpdateStatement : ProjectionStatement
    {
        public Top Top { get; set; }
        public string TableName { get; set; }
    }
}