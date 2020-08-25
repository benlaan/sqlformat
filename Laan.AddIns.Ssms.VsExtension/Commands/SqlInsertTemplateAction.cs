using System;
using System.Collections.Generic;
using System.Linq;

using Laan.AddIns.Ssms.VsExtension.Models;
using Laan.AddIns.Ssms.VsExtension.Utils;

using Microsoft.VisualStudio.Shell;

namespace Laan.AddIns.Ssms.VsExtension.Commands
{
    public class SqlInsertTemplateAction : BaseDropDownAction
    {
        public override int CommandId => 4000;

        public SqlInsertTemplateAction()
        {
           Templates = LoadTemplates();
        }

        public List<Template> LoadTemplates()
        {
            try
            {
                return TemplateDocument.Load();
            }
            catch (Exception ex)
            {
                Error(ex);
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

        protected override bool CanExecute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return IsCurrentDocumentExtension("sql")
                && CurrentSelection == String.Empty;
        }

        public void Expand(string word)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                SelectCurrentWord();
                string padding = new string(' ', 4);
                var foundTemplate = Templates.First(t => String.Compare(t.Code, word, true) == 0);

                if (foundTemplate == null)
                    return;

                var raw = foundTemplate
                    .Body
                    .Split('\n')
                    .Select(line => line.Replace("\t", padding))
                    .ToList();

                string offset = new string(' ', Cursor.Column - 1);

                // add lines, indenting as required, except the first
                var lines = new List<String>();
                lines.Add(raw.FirstOrDefault());
                for (int index = 1; index < raw.Count; index++)
                    lines.Add((raw[index] == String.Empty ? String.Empty : offset) + raw[index]);

                var cursor = DetermineCursorFromBar(raw);
                if (cursor.Row < lines.Count)
                    lines[cursor.Row] = lines[cursor.Row].Replace("|", "");

                InsertText(String.Join(Environment.NewLine, lines.ToArray()));
                Cursor = cursor;
            }
            finally
            {
                CancelSelection();
            }
        }

        protected override void ExecuteItem(Item item)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Expand(item.Code);
        }

        protected override IEnumerable<Item> GetItems()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var word = CurrentWord.ToLower();

            foreach (var template in Templates.OrderBy(k => k.Code))
            {
                if (template.Code.ToLower().StartsWith(word))
                    yield return new Item() { Code = template.Code, Name = template.Name };
            }
        }

        public static List<Template> Templates { get; set; }
    }
}
