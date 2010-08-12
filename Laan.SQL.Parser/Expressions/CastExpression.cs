using System;
using System.Linq;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Expressions
{
    public class CastExpression : FunctionExpression
    {
        public CastExpression( Expression parent ) : base( parent )
        {
            Name = Constants.Cast;
        }

        public CastExpression( Expression parent, SqlType outputType) : this( parent )
        {
            OutputType = outputType;
        }

        public override bool CanInline
        {
            get { return true; }
        }

        public SqlType OutputType { get; set; }

        public override string Value
        {
            get
            {
                return String.Format( "CAST({0} AS {1})", Arguments.First().Value, OutputType );
            }
        }
    }
}
