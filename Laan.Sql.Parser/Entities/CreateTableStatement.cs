using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Laan.Sql.Parser.Entities
{
    [DebuggerDisplay("{Value}")]
    public class CreateTableStatement : Statement
    {
        public CreateTableStatement()
        {
            Fields = new FieldDefinitions();
        }

        public string TableName { get; set; }
        public FieldDefinitions Fields { get; set; }

        public override string Identifier
        {
            get
            {
                return String.Format(
                    "CREATE TABLE {0}\n(\n{1}\n)",
                    TableName,
                    Fields.Select(fld => String.Format("\t{0}", fld)).Join(",\n")
                );
            }
        }
    }
}
