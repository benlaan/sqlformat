using Laan.Sql.Parser.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laan.Sql.Parser.Expressions
{
    public class RankingFunctionExpression : Expression, IInlineFormattable
    {
        /// <summary>
        /// Initializes a new instance of the RankingFunctionExpression class.
        /// </summary>
        public RankingFunctionExpression(Expression parent) : base(parent)
        {
            OrderBy = new List<Field>();
            PartitionBy = new List<Field>();
        }

        public List<Field> OrderBy { get; set; }
        public List<Field> PartitionBy { get; set; }

        public string Name { get; set; }

        public override string Value
        {
            get
            {
                List<string> parts = new List<string>();
                if (OrderBy.Any())
                {
                    parts.Add(String.Format(
                        "{0} {1} {2}",
                        Constants.Order,
                        Constants.By,
                        OrderBy.Select(arg => arg.Value).ToCsv()
                    ));
                }

                if (PartitionBy.Any())
                {
                    parts.Add(String.Format(
                        "{0} {1} {2}",
                        Constants.Partition,
                        Constants.By,
                        PartitionBy.Select(arg => arg.Value).ToCsv()
                    ));
                }

                return String.Format("{0} OVER ({1})", Name, parts.Join(" "));
            }
        }
    }
}
