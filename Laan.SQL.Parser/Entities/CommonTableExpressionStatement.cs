using System;
using System.Linq;
using System.Collections.Generic;

namespace Laan.Sql.Parser.Entities
{
    public class CommonTableExpression : SelectStatement
    {
        public CommonTableExpression()
        {
            ColumnNames = new List<string>();
        }

        public string Name { get; set; }
        public List<string> ColumnNames { get; set; }
    }

    public class CommonTableExpressionStatement : CustomStatement
    {
        public CommonTableExpressionStatement()
        {
            CommonTableExpressions = new List<CommonTableExpression>();
        }

        public List<CommonTableExpression> CommonTableExpressions { get; set; }
        public SelectStatement Statement { get; set; }
    }
}
