using System.Collections.Generic;

namespace SQLParser
{
    public class CreateTableStatement : IStatement
    {
        public CreateTableStatement()
        {
            Fields = new List<FieldDefinition>();
        }

        public List<FieldDefinition> Fields { get; set; }        
    }
}
