using System;

using Laan.AddIns.Actions;
using Laan.AddIns.Core;
using Laan.Sql.Formatter;

namespace Laan.AddIns.Ssms.Actions
{
    [Menu("Laan")]
    public class SqlFormatterAction : Core.BaseAction
    {
        private FormattingEngine _engine;

        // Experimental (not working!)
        //private static int GetStartIndent( TextDocument textDocument )
        //{
        //    var text = textDocument.Selection.Text;
        //    int index = 0;
        //    while ( ( text[ index ] == ' ' ) && ( text[ index ] != '\n' ) )
        //        index++;

        //    return index + textDocument.Selection.AnchorColumn;
        //}

        public SqlFormatterAction(AddIn addIn) : base(addIn)
        {
            _engine = new FormattingEngine();
            _engine.IndentStep = 0; // experimental: GetStartIndent( textDocument ) / engine.TabSize;

            KeyName = "LaanSqlFormat";
            DisplayName = "Format Sql";
            DescriptivePhrase = "Formatting SQL";

            ButtonText = "Format S&ql";
            ToolTip = "Formats the current file";
            ImageIndex = 1;
            KeyboardBinding = "Text Editor::Ctrl+`";
            ShowWaitCursor = true;
        }

        public override bool CanExecute()
        {
            return AddIn.IsCurrentDocumentExtension("sql")
                && AddIn.AllText.Length > 0;
        }

        public override void Execute()
        {
            var textDocument = AddIn.TextDocument;

            if (textDocument.Selection.IsEmpty)
                textDocument.Selection.SelectAll();

            try
            {
                AddIn.InsertText(_engine.Execute(textDocument.Selection.Text + Environment.NewLine));
            }
            finally
            {
                textDocument.Selection.Cancel();
            }
        }
    }
}
