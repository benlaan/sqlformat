using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laan.Sql.Parser.Entities
{
    public class DeleteStatement : CustomStatement
    {
        public Top Top { get; set; }
        public string TableName { get; set; }
    }
}
