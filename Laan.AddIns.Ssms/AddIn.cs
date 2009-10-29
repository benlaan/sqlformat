using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

using EnvDTE;
using EnvDTE80;
using Extensibility;

using Microsoft.VisualStudio.CommandBars;

namespace Laan.AddIns.Core
{
    public class AddIn : IDTExtensibility2, IDTCommandTarget
    {
        private Commands2 _commands;
        private string _uniqueKey;
        private EnvDTE.AddIn _addIn;
        private List<Action> _actions;
        private DTE _application;

        public AddIn()
        {
            _uniqueKey = GetType().FullName;
            _actions = new List<Action>();
            BuildActions();
        }

        #region Utilities

        private void BuildActions()
        {
            try
            {
                var actions = Assembly.GetExecutingAssembly().GetTypes()
                    .Where( type => !type.IsAbstract && typeof( Action ).IsAssignableFrom( type ) )
                    .Select( type => (Action) Activator.CreateInstance( type, this ) ).ToList<Action>();

                _actions.AddRange( actions );
            }
            catch ( Exception ex )
            {
                Error( ex );
            }
        }

        private void Initialise( object instance )
        {
            _addIn = (EnvDTE.AddIn) instance;
            _application = ( (EnvDTE.AddIn) instance ).DTE;
            _commands = (Commands2) _application.Commands;
        }

        private bool CommandIsInstalled( Action action )
        {
            bool found = true;
            try
            {
                var findCommand = _commands.Item( action.FullName, 1 );
            }
            catch ( Exception ex )
            {
                found = false;
                Error( action.DisplayName + " not found: ", ex );
            }
            return found;
        }

        private string GetToolsMenuName()
        {
            string toolsMenuName;

            try
            {
                //If you would like to move the command to a different menu, change the word "Tools" to the
                //  English version of the menu. This code will take the culture, append on the name of the menu
                //  then add the command to that menu. You can find a list of all the top-level menus in the file
                //  CommandBar.resx.
                string resourceName;
                var assembly = Assembly.GetExecutingAssembly();
                var resourceManager = new ResourceManager( assembly.GetName().Name + ".CommandBar", assembly );
                var cultureInfo = new CultureInfo( _application.LocaleID );

                if ( cultureInfo.TwoLetterISOLanguageName == "zh" )
                {
                    System.Globalization.CultureInfo parentCultureInfo = cultureInfo.Parent;
                    resourceName = String.Concat( parentCultureInfo.Name, "Tools" );
                }
                else
                    resourceName = String.Concat( cultureInfo.TwoLetterISOLanguageName, "Tools" );

                toolsMenuName = resourceManager.GetString( resourceName );
            }
            catch
            {
                //We tried to find a localized version of the word Tools, but one was not found.
                //  Default to the en-US word, which may work for the current culture.
                toolsMenuName = "Tools";
            }
            return toolsMenuName;
        }

        private Action FindAction( string commandName )
        {
            return _actions.Single( c => ( _uniqueKey + "." + c.KeyName ).Equals( commandName, StringComparison.CurrentCultureIgnoreCase ) );
        }

        private void PlaceCommandOnToolsMenu()
        {
            //Place the command on the tools menu.
            //Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
            var menuBarCommandBar = ( (CommandBars) _application.CommandBars )[ "MenuBar" ];
            var toolsControl = menuBarCommandBar.Controls[ GetToolsMenuName() ];
            var toolsPopup = (CommandBarPopup) toolsControl;

            if ( _actions.Count == 0 )
                throw new Exception( String.Format( "No actions defined for this AddIn: {0}", GetType().Name ) );

            foreach ( var action in _actions )
            {
                if ( CommandIsInstalled( action ) )
                    continue;

                try
                {
                    object[] contextGUIDS = new object[] { };

                    var command = _commands.AddNamedCommand2(
                        _addIn,
                        action.KeyName,
                        action.ButtonText,
                        action.ToolTip,
                        true,
                        action.ImageIndex,
                        ref contextGUIDS,
                        (int) vsCommandStatus.vsCommandStatusSupported + (int) vsCommandStatus.vsCommandStatusEnabled,
                        (int) vsCommandStyle.vsCommandStylePictAndText,
                        vsCommandControlType.vsCommandControlTypeButton
                    );

                    if ( command != null && toolsPopup != null )
                        command.AddControl( toolsPopup.CommandBar, 1 );

                    command.Bindings = action.KeyboardBinding;
                }
                catch ( System.ArgumentException ex )
                {
                    Error( ex );
                }
            }
        }

        private void RemoveCommandFromToolsMenu()
        {
            foreach ( var action in _actions )
            {
                try
                {
                    var command = _commands.Item( action.FullName, 1 );
                    command.Delete();
                }
                catch ( Exception ex )
                {
                    Error( action.DisplayName + " not found: ", ex );
                }
            }
        }

        private void Execute( Action action )
        {
            Stopwatch timer = Stopwatch.StartNew();

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            try
            {
                SetStatus( "{0}...", action.DescriptivePhrase );
                using ( new ScopedUndoContext( this, action.KeyName ) )
                {
                    action.Execute();
                }
                SetStatus( "{0} completed in {1} seconds", action.DescriptivePhrase, timer.Elapsed.TotalSeconds );
            }
            catch ( Exception ex )
            {
                Error( ex );
                SetStatus( "Error {0}: {1}", action.DescriptivePhrase, ex.Message );
            }
            finally
            {
                timer.Stop();
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
        }

        internal string DocumentFullName
        {
            get { return _application.ActiveDocument != null ? _application.ActiveDocument.FullName : ""; }
        }

        internal bool IsCurrentDocumentExtension( string extension )
        {
            return Path.GetExtension( DocumentFullName ).Equals(
                String.Format( ".{0}", extension ),
                StringComparison.CurrentCultureIgnoreCase
            );
        }

        internal void SetStatus( string message, params object[] args )
        {
            _application.StatusBar.Text = String.Format( message, args );
        }

        internal void Error( Exception ex )
        {
            Trace.WriteLine( ex );
        }

        internal void Error( string message, Exception ex )
        {
            Trace.WriteLine( message + "\n\t" + ex );
        }

        internal void OpenUndoContext( string name, bool strict )
        {
            _application.UndoContext.Open( name, strict );
        }

        internal void CloseUndoContext()
        {
            _application.UndoContext.Close();
        }

        internal void CancelSelection()
        {
            TextDocument.Selection.Cancel();
        }

        internal void SelectCurrentWord()
        {
            var cursor = TextDocument.Selection.ActivePoint;
            var point = cursor.CreateEditPoint();
            point.WordLeft( 1 );
            TextDocument.Selection.MoveToPoint( point, false );
            TextDocument.Selection.WordRight( true, 1 );
        }

        internal void ClearSelection()
        {
            TextDocument.Selection.Text = "";
        }

        internal void InsertText( string message )
        {
            InsertText( message, true );
        }

        internal void InsertText( string message, bool replace )
        {
            if ( replace )
                ClearSelection();
            TextDocument.Selection.Insert( message, (int) vsInsertFlags.vsInsertFlagsContainNewText );
        }

        internal string CurrentLine
        {
            get
            {
                var cursor = TextDocument.Selection.ActivePoint;
                var point = cursor.CreateEditPoint();
                point.StartOfLine();
                return point.GetText( cursor );
            }
        }

        internal string CurrentWord
        {
            get
            {
                var cursor = TextDocument.Selection.ActivePoint;
                var point = cursor.CreateEditPoint();
                point.WordLeft( 1 );
                return point.GetText( cursor ).Trim();
            }
        }

        internal string CurrentSelection
        {
            get { return TextDocument.Selection.Text; }
        }

        internal TextDocument TextDocument
        {
            get { return (TextDocument) _application.ActiveDocument.Object( "TextDocument" ); }
        }

        internal string AllText
        {
            get
            {
                var cursor = TextDocument.StartPoint;
                var point = cursor.CreateEditPoint();
                point.StartOfDocument();
                return point.GetText( TextDocument.EndPoint );
            }
        }

        internal Cursor Cursor
        {
            get
            {
                var point = TextDocument.Selection.TopPoint;
                return new Cursor( point.DisplayColumn, point.Line );
            }
            set
            {
                var point = TextDocument.Selection.TopPoint.CreateEditPoint();
                point.LineDown( value.Row );
                point.CharRight( value.Column );
                TextDocument.Selection.MoveToPoint( point, false );
            }
        }

        internal Cursor VirtualCursor
        {
            get
            {
                var start = TextDocument.Selection.TextPane.StartPoint;
                var top = TextDocument.Selection.TopPoint;
                return new Cursor( top.DisplayColumn - start.DisplayColumn + 1 , top.Line - start.Line + 1 );
            }
            set
            {
                var point = TextDocument.Selection.TopPoint.CreateEditPoint();
                point.LineDown( value.Row );
                point.CharRight( value.Column );
                TextDocument.Selection.MoveToPoint( point, false );
            }
        }

        private void DumpCommands()
        {
            foreach ( Command c in _commands )
            {
                Trace.WriteLine(
                    String.Format(
                        "{0} {1}: {2}",
                        c.ID, c.Name, ( (object[]) c.Bindings ).FirstOrDefault() )
                );
            }
        }

        #endregion

        #region IDTExtensibility2 Members

        public void OnConnection( object application, ext_ConnectMode connectMode, object instance, ref Array custom )
        {
            Initialise( instance );
            PlaceCommandOnToolsMenu();
        }

        public void OnDisconnection( ext_DisconnectMode disconnectMode, ref Array custom )
        {
            RemoveCommandFromToolsMenu();
        }

        public void OnAddInsUpdate( ref Array custom )
        {
        }

        public void OnStartupComplete( ref Array custom )
        {
        }

        public void OnBeginShutdown( ref Array custom )
        {
        }

        #endregion

        #region IDTCommandTarget Members

        public void Exec(
            string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled
        )
        {
            handled = false;
            if ( executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault )
            {
                var action = FindAction( commandName );
                Execute( action );
                handled = true;
                return;
            }
        }

        public void QueryStatus(
            string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText
        )
        {
            var action = FindAction( commandName );

            if ( action.CanExecute()
                && neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone )
                status = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
        }

        #endregion

    }
}
