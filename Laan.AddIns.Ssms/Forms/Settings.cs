using System;
using System.Windows.Forms;

namespace Laan.AddIns.Forms
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        public string SaveResultAsPattern1 { get; set; }
        public string SaveResultAsPattern2 { get; set; }
        public bool SaveResultsCopyToClipboard1 { get; set; }
        public bool SaveResultsCopyToClipboard2 { get; set; }

        private void Settings_Load( object sender, EventArgs e )
        {
            SaveResultsFilenamePattern1.Text = SaveResultAsPattern1;
            SaveResultsFilenamePattern2.Text = SaveResultAsPattern2;
            SaveResultsCopy1.Checked = SaveResultsCopyToClipboard1;
            SaveResultsCopy2.Checked = SaveResultsCopyToClipboard2;
        }

        private void OK_Click( object sender, EventArgs e )
        {
            SaveResultAsPattern1 = SaveResultsFilenamePattern1.Text;
            SaveResultAsPattern2 = SaveResultsFilenamePattern2.Text;
            SaveResultsCopyToClipboard1 = SaveResultsCopy1.Checked;
            SaveResultsCopyToClipboard2 = SaveResultsCopy2.Checked;
        }

        public class Constants
        {
            public const string SaveResultsAsPattern1 = "SaveResultsAsPattern1";
            public const string SaveResultsAsPattern2 = "SaveResultsAsPattern2";
            public const string SaveResultsCopyToClipboard1 = "SaveResultsCopyToClipboard1";
            public const string SaveResultsCopyToClipboard2 = "SaveResultsCopyToClipboard2";

        }

        public class Defaults
        {
            public const string SaveResultsAsPattern1 = "{0} - Rollback.log";
            public const string SaveResultsAsPattern2 = "{0} - Commit.log";
            public const bool SaveResultsCopyToClipboard1 = true;
            public const bool SaveResultsCopyToClipboard2 = true;
        }
    }
}
