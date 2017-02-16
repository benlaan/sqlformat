using System;
using System.Linq;

using Laan.AddIns.Actions;
using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public class SurroundWithBlockBracketAction : Core.BaseAction
    {
        public SurroundWithBlockBracketAction(AddIn addIn) : base(addIn)
        {
            KeyName = "LaanSqlSurroundWithBlockBracketAction";
            DisplayName = "Surround Selection With Block Bracket";
            DescriptivePhrase = "Surrounds Selection With Block Bracket";

            ButtonText = "Surround Selection With Block Bracket";
            ToolTip = "Surround Selection With Block Bracket";
            KeyboardBinding = "Text Editor::[";
            ImageIndex = 6;
        }

        public override void Execute()
        {
            var textDocument = AddIn.TextDocument;

            if (AddIn.CurrentSelection.Length == 0)
                textDocument.Selection.Insert("[");
            else
            {
                var text = textDocument.Selection.Text;
                AddIn.InsertText(String.Format("[{0}]", text), true);
                textDocument.Selection.CharRight(false, text.Length);
            }
        }

        public override bool CanExecute()
        {
            return AddIn.IsCurrentDocumentExtension("sql");
        }
    }
}
