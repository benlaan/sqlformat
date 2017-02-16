using System;
using System.Linq;

using Laan.AddIns.Actions;
using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public class SurroundWithQuoteAction : Core.BaseAction
    {
        public SurroundWithQuoteAction(AddIn addIn) : base(addIn)
        {
            KeyName = "LaanSqlSurroundWithQuoteAction";
            DisplayName = "Surround Selection With Quotes";
            DescriptivePhrase = "Surrounds Selection With Quotes";

            ButtonText = "Surround Selection With Quotes";
            ToolTip = "Surround Selection With Quotes";
            KeyboardBinding = "Text Editor::'";
            ImageIndex = 6;
        }

        public override void Execute()
        {
            var textDocument = AddIn.TextDocument;

            if (AddIn.CurrentSelection.Length == 0)
                textDocument.Selection.Insert("'");
            else
            {
                var text = textDocument.Selection.Text;
                AddIn.InsertText(String.Format("'{0}'", text.Replace("'", "''")), true);
                textDocument.Selection.CharRight(false, text.Length);
            }
        }

        public override bool CanExecute()
        {
            return AddIn.IsCurrentDocumentExtension("sql");
        }
    }
}
