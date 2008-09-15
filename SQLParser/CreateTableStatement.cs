using System.Collections.Generic;
using System;

namespace Laan.SQLParser
{
    public class CreateTableStatement : IStatement
    {
        public CreateTableStatement()
        {
            Fields = new List<FieldDefinition>();
        }

        public string TableName { get; set; }
        public List<FieldDefinition> Fields { get; set; }        
    }
}
