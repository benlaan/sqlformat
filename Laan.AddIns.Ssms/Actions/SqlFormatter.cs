using System;

using Laan.AddIns.Core;
using Laan.SQL.Formatter;

namespace Laan.AddIns.Ssms.Actions
{
    public class SqlFormatter : Core.Action
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

        public SqlFormatter( AddIn addIn ) : base( addIn )
        {
            _engine = new FormattingEngine();
            _engine.IndentStep = 0; // experimental: GetStartIndent( textDocument ) / engine.TabSize;

            KeyName = "LaanSqlFormat";
            DisplayName = "Format SQL";
            DescriptivePhrase = "Formatting SQL";

            ButtonText = "Format S&QL"; 
            ToolTip = "Formats the current file"; 
            ImageIndex = 59;
            KeyboardBinding = "Text Editor::Ctrl+`";
        }

        public override bool CanExecute()
        {
            return ( _addIn.IsCurrentDocumentExtension( "sql" ) );
        }

        public override void Execute()
        {
            var textDocument = _addIn.TextDocument;

            if ( textDocument.Selection.IsEmpty )
                textDocument.Selection.SelectAll();

            try
            {
                _addIn.InsertText( _engine.Execute( textDocument.Selection.Text ) );
            }
            finally
            {
                textDocument.Selection.Cancel();
            }
        }
    }
}
