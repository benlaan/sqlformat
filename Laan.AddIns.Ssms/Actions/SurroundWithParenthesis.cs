using System;
using System.Linq;

using Laan.AddIns.Actions;
using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public class SurroundWithParenthesisAction : Core.BaseAction
    {
        public SurroundWithParenthesisAction(AddIn addIn) : base(addIn)
        {
            KeyName = "LaanSqlSurroundWithParenthesisAction";
            DisplayName = "Surround Selection With Parenthesis";
            DescriptivePhrase = "Surrounds Selection With Parenthesis";

            ButtonText = "Surround Selection With Parenthesis";
            ToolTip = "Surround Selection With Parenthesis";
            KeyboardBinding = "Text Editor::Ctrl+9"; // These does not work.. as 9, or (, or Shift+9
            ImageIndex = 6;
        }

        public override void Execute()
        {
            var textDocument = AddIn.TextDocument;

            if (AddIn.CurrentSelection.Length == 0)
                textDocument.Selection.Insert("(");
            else
            {
                var text = textDocument.Selection.Text;
                AddIn.InsertText(String.Format("({0})", text), true);
                textDocument.Selection.CharRight(false, text.Length);
            }
        }

        public override bool CanExecute()
        {
            return AddIn.IsCurrentDocumentExtension("sql");
        }
    }
}
