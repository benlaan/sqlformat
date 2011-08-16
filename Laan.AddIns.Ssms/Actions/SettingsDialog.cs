using System;
using System.Windows.Forms;
using Laan.AddIns.Core;
using Laan.AddIns.Forms;
using Action = Laan.AddIns.Core.Action;

namespace Laan.AddIns.Actions
{
    [MenuBarToolsMenu]
    public class SettingsDialog : Action
    {
        public SettingsDialog( AddIn addIn ) : base( addIn )
        {
            KeyName = "LannSettings";
            DisplayName = "Laan Settings";
            DescriptivePhrase = "Set preferences for Laan SMSS Addin";

            ButtonText = "Laan Settings";
            //ToolTip = "Formats the current file";
        }

        public override void Execute()
        {
            using ( var frm = new Settings
                                  {
                                      SaveResultAsPattern1 =
                                          ReadConfigValue( Settings.Constants.SaveResultsAsPattern1,
                                                           Settings.Defaults.SaveResultsAsPattern1 ),
                                      SaveResultAsPattern2 =
                                          ReadConfigValue( Settings.Constants.SaveResultsAsPattern2,
                                                           Settings.Defaults.SaveResultsAsPattern2 ),
                                      SaveResultsCopyToClipboard1 =
                                          ReadConfigValue( Settings.Constants.SaveResultsCopyToClipboard1,
                                                           Settings.Defaults.SaveResultsCopyToClipboard1 ),
                                      SaveResultsCopyToClipboard2 =
                                          ReadConfigValue( Settings.Constants.SaveResultsCopyToClipboard2,
                                                           Settings.Defaults.SaveResultsCopyToClipboard2 )
                                  } )
            {
                DialogResult result = frm.ShowDialog();

                if ( result == DialogResult.OK )
                {
                    // save settings
                    WriteConfigValue( Settings.Constants.SaveResultsAsPattern1, frm.SaveResultAsPattern1 );
                    WriteConfigValue( Settings.Constants.SaveResultsAsPattern2, frm.SaveResultAsPattern2 );
                    WriteConfigValue( Settings.Constants.SaveResultsCopyToClipboard1, frm.SaveResultsCopyToClipboard1 );
                    WriteConfigValue( Settings.Constants.SaveResultsCopyToClipboard2, frm.SaveResultsCopyToClipboard2 );
                }
            }
        }

        public override bool CanExecute()
        {
            return true;
        }
    }
}