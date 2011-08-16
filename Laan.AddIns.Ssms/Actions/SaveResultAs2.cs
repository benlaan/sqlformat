using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using Laan.AddIns.Core;
using Laan.AddIns.Forms;

namespace Laan.AddIns.Actions
{
    [ResultsMenu]
    public class SaveResultAs2 : SaveResultsAs
    {
        public SaveResultAs2( AddIn addIn ) : base( addIn )
        {
            KeyName = "LannSqlSaveResultsAs2";
            DisplayName = "Save Results As";
            DescriptivePhrase = "Save Results to log file and optionally copy file to clipboard";
            ButtonText = "Save Results to .log";
            //ToolTip = "hi there";

            SaveResultsAsPatternName = Settings.Constants.SaveResultsAsPattern2;
            SaveResultsAsPatternDefault = Settings.Defaults.SaveResultsAsPattern2;
            SaveResultsAsCopyToClipboardName = Settings.Constants.SaveResultsCopyToClipboard2;
            SaveResultsAsCopyToClipboardDefault = Settings.Defaults.SaveResultsCopyToClipboard2;

        }

    }
}