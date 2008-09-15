using System.Collections.Generic;
using System;

namespace Laan.SQLParser
{
    public class CreateTableStatement : IStatement
    {
        public CreateTableStatement()
        {
            Fields = new FieldDefinitions();
        }

        public string TableName { get; set; }
        public FieldDefinitions Fields { get; set; }        
    }
}
