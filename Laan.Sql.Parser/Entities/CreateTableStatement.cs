using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Laan.Sql.Parser.Entities
{
    [ DebuggerDisplay("{Value}") ]
    public class CreateTableStatement : IStatement
    {
        public bool Terminated { get; set; }
        public CreateTableStatement()
        {
            Fields = new FieldDefinitions();
        }

        public string TableName { get; set; }
        public FieldDefinitions Fields { get; set; }

        public string Identifier
        {
            get
            {
                return String.Format(
                    "CREATE TABLE {0}{2}}({2}{1}{2})",
                    TableName,
                    String.Join( "," + Environment.NewLine, Fields.Select( fld => "\t" + fld.ToString() ).ToArray() ),
                    Environment.NewLine
                );
            }
        }
    }
}
