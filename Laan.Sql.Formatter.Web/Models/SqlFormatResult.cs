using System;
using System.Collections.Generic;

namespace Laan.Sql.Formatter.Web.Models
{
    public class SqlFormatResult
    {
        public IList<string> Sql { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
