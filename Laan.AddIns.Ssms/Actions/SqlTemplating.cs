using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using Laan.AddIns.Actions;
using Laan.AddIns.Core;
using System.Diagnostics;

namespace Laan.AddIns.Ssms.Actions
{
    [MenuBarToolsMenu]
    public class SqlTemplating : DropDownList
    {
        public static List<Template> Templates { get; set; }

        public SqlTemplating(AddIn addIn) : base(addIn)
        {
            Templates = LoadTemplates();

            KeyName = "LaanSqlTemplating";
            DisplayName = "Insert Template";
            DescriptivePhrase = "Inserting Template";

            ButtonText = "Insert &Template";
            ToolTip = "Inserts a template at the cursor";
            ImageIndex = 59;
            KeyboardBinding = "Text Editor::`";
        }

        public List<Template> LoadTemplates()
        {
            try
            {
                return TemplateDocument.Load();
            }
            catch (Exception ex)
            {
                AddIn.Error(ex); 
                return new List<Template>();
            }
        }
        
        private Cursor DetermineCursorFromBar(IList<string> lines)
        {
            int column = 0;
            int row = 0;

            foreach (string line in lines)
            {
                column = line.IndexOf('|');
                if (column >= 0)
                    break;
                row++;
            }

            // if no bar was found, just put the cursor at the last row and column of the replaced text
            if (column == -1)
            {
                column = lines.Last().Length;
                row = lines.Count() - 1;
            }
            return new Cursor(column, row);
        }

        public override bool CanExecute()
        {
            try
            {
                return AddIn.IsCurrentDocumentExtension("sql")
                    && AddIn.CurrentSelection == "";
            }
            catch (Exception ex)
            {
                AddIn.Error(ex);
                return false;
            }
        }

        public void Expand(string word)
        {
            try
            {
                AddIn.SelectCurrentWord();
                string padding = new string(' ', 4);
                Template foundTemplate = Templates.First(t => String.Compare(t.Code, word, true) == 0);

                if (foundTemplate == null)
                    return;

                var raw = foundTemplate
                    .Body
                    .Split('\n')
                    .Select(line => line.Replace("\t", padding))
                    .ToList();

                string offset = new string(' ', AddIn.Cursor.Column - 1);

                // add lines, indenting as required, except the first
                var lines = new List<String>();
                lines.Add(raw.FirstOrDefault());
                for (int index = 1; index < raw.Count; index++)
                    lines.Add((raw[index] == "" ? "" : offset) + raw[index]);

                Cursor cursor = DetermineCursorFromBar(raw);
                if (cursor.Row < lines.Count)
                    lines[cursor.Row] = lines[cursor.Row].Replace("|", "");

                AddIn.InsertText(String.Join("\n", lines.ToArray()));
                AddIn.Cursor = cursor;
            }
            finally
            {
                AddIn.CancelSelection();
            }
        }

        protected override void ExecuteItem(Item item)
        {
            Expand(item.Code);
        }

        protected override IEnumerable<Item> GetItems()
        {
            var word = AddIn.CurrentWord;

            foreach (var template in Templates.OrderBy(k => k.Code))
            {
                if (template.Code.StartsWith(word))
                    yield return new Item() { Code = template.Code, Name = template.Name };
            }
        }
    }
}
