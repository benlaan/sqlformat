using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using Laan.AddIns.Forms;
using Microsoft.SqlServer.Management.UI.VSIntegration;
using Microsoft.VisualStudio.TextManager.Interop;
using Action = Laan.AddIns.Core.Action;
using AddIn = Laan.AddIns.Core.AddIn;

namespace Laan.AddIns.Actions
{
    [ResultsMenu]
    public class SaveResultsAs : Action
    {
        #region Delegates

        public delegate bool EnumWindowProc( IntPtr hWnd, IntPtr parameter );

        #endregion

        public SaveResultsAs( AddIn addIn ) : base( addIn )
        {
            KeyName = "LannSqlSaveResultsAs";
            DisplayName = "Save Results As";
            DescriptivePhrase = "Save Results to log file";
            ButtonText = "Save Results to .log";
            ToolTip = "hi there";
            SaveResultsAsPatternName = Settings.Constants.SaveResultsAsPattern1;
            SaveResultsAsPatternDefault = Settings.Defaults.SaveResultsAsPattern1;
            SaveResultsAsCopyToClipboardName = Settings.Constants.SaveResultsCopyToClipboard1;
            SaveResultsAsCopyToClipboardDefault = Settings.Defaults.SaveResultsCopyToClipboard1;
        }

        protected string SaveResultsAsPatternName { get; set; }
        protected string SaveResultsAsPatternDefault { get; set; }
        protected string SaveResultsAsCopyToClipboardName { get; set; }
        protected bool SaveResultsAsCopyToClipboardDefault { get; set; }

        /// <summary>
        /// </summary>
        /// <param name = "window"></param>
        /// <param name = "callback"></param>
        /// <param name = "i"></param>
        /// <returns></returns>
        [DllImport( "user32" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool EnumChildWindows( IntPtr window, EnumWindowProc callback, IntPtr i );

        [DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        private static extern int GetWindowTextLength( IntPtr hWnd );

        [DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        private static extern int GetWindowText( IntPtr hWnd, StringBuilder lpString, int nMaxCount );

        [DllImport( "user32.dll", CharSet = CharSet.Auto )]
        private static extern int GetClassName( IntPtr hWnd, StringBuilder lpClassName, int nMaxCount );

        private static bool EnumWindow( IntPtr handle, IntPtr pointer )
        {
            GCHandle gch = GCHandle.FromIntPtr( pointer );
            var list = gch.Target as List<IntPtr>;
            if ( list == null )
            {
                throw new InvalidCastException( "GCHandle Target could not be cast as List<IntPtr>" );
            }

            list.Add( handle );

            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name = "parent"></param>
        /// <returns></returns>
        private static IEnumerable<IntPtr> GetChildWindows( IntPtr parent )
        {
            var windowPointers = new List<IntPtr>();

            GCHandle listHandle = GCHandle.Alloc( windowPointers );

            try
            {
                var childProc = new EnumWindowProc( EnumWindow );

                EnumChildWindows( parent, childProc, GCHandle.ToIntPtr( listHandle ) );
            }
            finally
            {
                if ( listHandle.IsAllocated )
                {
                    listHandle.Free();
                }
            }

            return windowPointers;
        }

        /// <summary>
        /// </summary>
        /// <param name = "hWnd"></param>
        /// <returns></returns>
        private static string GetText( IntPtr hWnd )
        {
            int length = GetWindowTextLength( hWnd );
            var sb = new StringBuilder( length + 1 );
            GetWindowText( hWnd, sb, sb.Capacity );

            return sb.ToString();
        }

        /// <summary>
        /// </summary>
        /// <param name = "hWnd"></param>
        /// <returns></returns>
        private static string ClassName( IntPtr hWnd )
        {
            var sb = new StringBuilder( 100 );
            GetClassName( hWnd, sb, sb.Capacity );
            return sb.ToString();
        }

        public override void Execute()
        {
            Window w = AddIn.Application.DTE.ActiveDocument.Windows.Item( 1 );

            // Get all window pointers for the app
            IEnumerable<IntPtr> pointers = GetChildWindows( ServiceCache.MainShellWindow.Handle );

            bool found = false;

            if ( w.Document != null )
            {
                foreach ( IntPtr ptr in pointers )
                {
                    if ( found )
                        break;

                    // Try and match the windows based on the caption
                    if ( w.Caption.StartsWith( GetText( ptr ) ) )
                    {
                        // Enumerate through the window pointers to find the tab control
                        foreach ( IntPtr controlPtr in GetChildWindows( ptr ) )
                        {
                            if ( found )
                                break;

                            if ( ClassName( controlPtr ).StartsWith( "WindowsForms10.SysTabControl32.app.0." ) )
                            {
                                var tabControl = (TabControl) Control.FromHandle( controlPtr );

                                // find 'Results'

                                var resultsTab = tabControl.TabPages[ 0 ];

                                if ( resultsTab.Text != "Results" )
                                    continue;

                                found = true;

                                var ctl = resultsTab.Controls[ 0 ] as ShellTextViewControl;

                                IVsTextView text = ctl.TextView;

                                IVsTextLines lines;
                                text.GetBuffer( out lines );

                                int lineCount;
                                lines.GetLineCount( out lineCount );

                                int lastLine;
                                int lastLineIndex;
                                lines.GetLastLineIndex( out lastLine, out lastLineIndex );

                                int lineLength;
                                lines.GetLengthOfLine( 0, out lineLength );

                                string textBuffer;
                                lines.GetLineText( 0, 0, lastLine, lastLineIndex, out textBuffer );

                                string sourceFilename = w.Document.FullName;

                                string pattern =
                                    ReadConfigValue( SaveResultsAsPatternName, SaveResultsAsPatternDefault );


                                string logFilename = string.Format(pattern, 
                                    Path.Combine( Path.GetDirectoryName( sourceFilename ),
                                                Path.GetFileNameWithoutExtension( sourceFilename ) 
                                                )
                                            );

                                SaveLogFile( textBuffer, logFilename );

                                bool doCopy = ReadConfigValue( SaveResultsAsCopyToClipboardName, SaveResultsAsCopyToClipboardDefault );

                                if ( doCopy )
                                {
                                    var filePaths = new StringCollection {logFilename};

                                    Clipboard.SetFileDropList( filePaths );

                                }
                            }
                        }
                    }
                }
            }
        }


        protected virtual void SaveLogFile( string textBuffer, string logFilename )
        {
            // append text to filename
 
            if ( !File.Exists( logFilename ) ||
                 MessageBox.Show( "File exists. Do you wish to overwrite?", "Warning",
                                  MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                  MessageBoxDefaultButton.Button2 ) == DialogResult.Yes )
                File.WriteAllLines( logFilename, new[] {textBuffer} );
        }

        public override bool CanExecute()
        {
            // update button text with filename
            Window w = GetWindow();

            if ( w != null )
            {
                string sourceFilename = w.Document.FullName;

                string pattern = ReadConfigValue( SaveResultsAsPatternName, SaveResultsAsPatternDefault );

                bool copyToClipboard = ReadConfigValue( SaveResultsAsCopyToClipboardName, SaveResultsAsCopyToClipboardDefault );

                ButtonText = string.Format("Save Results to '{0}'{1}", 
                    string.Format( pattern, Path.GetFileNameWithoutExtension( sourceFilename )),
                    copyToClipboard ? " and Copy" : string.Empty
                    );
            }

            return true;
        }

        private Window GetWindow()
        {
            return AddIn.Application.DTE.ActiveDocument.Windows.Item( 1 );
        }
    }
}