using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using Laan.AddIns.Core;

namespace Laan.AddIns.Actions
{
    [ResultsMenu]
    public class SaveResultAsWithCopy : SaveResultsAs
    {
        public SaveResultAsWithCopy( AddIn addIn ) : base( addIn )
        {
            KeyName = "LannSqlSaveResultsAsWithCopy";
            DisplayName = "Save Results As and Copy";
            DescriptivePhrase = "Save Results to log file and copy file to clipboard";
            ButtonText = "Save Results to .log and Copy";
            ToolTip = "hi there";
        }

        protected override void SaveLogFile( string textBuffer, string logFilename )
        {
            base.SaveLogFile( textBuffer, logFilename );
            var filePaths = new StringCollection();
            filePaths.Add( logFilename );

            Clipboard.SetFileDropList(filePaths);
        }
    }
}