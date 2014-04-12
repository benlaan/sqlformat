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
using Laan.AddIns.Actions;
using Microsoft.VisualStudio.CommandBars;

namespace Laan.AddIns.Core
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class AddIn : IDTExtensibility2, IDTCommandTarget
    {
        private Commands2 _commands;
        private string _uniqueKey;
        private EnvDTE.AddIn _addIn;
        private List<Action> _actions;
        public DTE Application { get; private set; }

        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public AddIn()
        {
            _uniqueKey = GetType().FullName;
            _actions = new List<Action>();
            BuildActions();
        }

        /// <summary>
        ///     Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in
        ///     is being loaded.
        /// </summary>
        /// <param name='application'>Root object of the host application.</param>
        /// <param name='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param name='instance'>Object representing this Add-in.</param>
        /// <param name="custom"></param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object instance, ref Array custom)
        {
            try
            {
                Initialise(instance);
                PlaceCommandsOnMenus();
            }
            catch (Exception ex)
            {
                Error("OnConnection", ex);
            }
        }

        /// <summary>
        ///     Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the
        ///     Add-in is being unloaded.
        /// </summary>
        /// <param name='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param name='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            try
            {
                RemoveCommandsFromMenus();
            }
            catch (Exception ex)
            {
                Error("OnDisconnection", ex);
            }
        }

        /// <summary>
        ///     Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the
        ///     collection of Add-ins has changed.
        /// </summary>
        /// <param name='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>
        ///     Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the
        ///     host application has completed loading.
        /// </summary>
        /// <param name='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom)
        {
        }

        /// <summary>
        ///     Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host
        ///     application is being unloaded.
        /// </summary>
        /// <param name='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
        {
        }

        /// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
        /// <param name='commandName'>The name of the command to execute.</param>
        /// <param name='executeOption'>Describes how the command should be run.</param>
        /// <param name='varIn'>Parameters passed from the caller to the command handler.</param>
        /// <param name='varOut'>Parameters passed from the command handler to the caller.</param>
        /// <param name='handled'>Informs the caller if the command was handled or not.</param>
        /// <seealso class='Exec' />
        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
        {
            try
            {
                handled = false;
                if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
                {
                    var action = FindAction(commandName);
                    Execute(action);
                    handled = true;
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        /// <summary>
        /// Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated
        /// </summary>
        /// <param name='commandName'>The name of the command to determine state for.</param>
        /// <param name='neededText'>Text that is needed for the command.</param>
        /// <param name='status'>The state of the command in the user interface.</param>
        /// <param name='commandText'>Text requested by the neededText parameter.</param>
        /// <seealso class='Exec' />
        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
        {
            try
            {
                var action = FindAction(commandName);

                string text = action.ButtonText;

                if (action.CanExecute()
                    && neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
                    status = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;

                string updatedText = action.ButtonText;

                if (text == updatedText)
                    return;

                var attr = (MenuAttribute)action.GetType().GetCustomAttributes(typeof(MenuAttribute), false).FirstOrDefault();

                if (attr != null)
                {
                    string commandBarName = attr.CommandBar;

                    var commandBar = ((CommandBars)Application.CommandBars)[commandBarName];

                    CommandBarControl control = commandBar.Controls[text];

                    if (control != null)
                        control.Caption = updatedText;
                }
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        private void BuildActions()
        {
            try
            {
                var actions = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => !type.IsAbstract && typeof(Action).IsAssignableFrom(type))
                    .Select(type => (Action)Activator.CreateInstance(type, this));

                _actions.AddRange(actions);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        private void Initialise(object instance)
        {
            _addIn = (EnvDTE.AddIn)instance;
            Application = ((EnvDTE.AddIn)instance).DTE;
            _commands = (Commands2)Application.Commands;
        }

        private bool CommandIsInstalled(Action action)
        {
            bool found = true;
            try
            {
                var findCommand = _commands.Item(action.FullName, 1);
            }
            catch (Exception)
            {
                found = false;
                // Error(action.DisplayName + " not found: ", ex);
            }
            return found;
        }

        private string GetMenuName(string menuName)
        {
            string localisedMenuName;

            try
            {
                //If you would like to move the command to a different menu, change the word "Tools" to the
                //  English version of the menu. This code will take the culture, append on the name of the menu
                //  then add the command to that menu. You can find a list of all the top-level menus in the file
                //  CommandBar.resx.
                string resourceName;
                var assembly = Assembly.GetExecutingAssembly();
                var resourceManager = new ResourceManager(assembly.GetName().Name + ".CommandBar", assembly);
                var cultureInfo = new CultureInfo(Application.LocaleID);

                if (cultureInfo.TwoLetterISOLanguageName == "zh")
                {
                    CultureInfo parentCultureInfo = cultureInfo.Parent;
                    resourceName = String.Concat(parentCultureInfo.Name, menuName);
                }
                else
                    resourceName = String.Concat(cultureInfo.TwoLetterISOLanguageName, menuName);

                localisedMenuName = resourceManager.GetString(resourceName);
            }
            catch
            {
                //We tried to find a localized version of the word Tools, but one was not found.
                //  Default to the en-US word, which may work for the current culture.
                localisedMenuName = menuName;
            }
            return localisedMenuName;
        }

        private Action FindAction(string commandName)
        {
            return _actions.Single(c => (_uniqueKey + "." + c.KeyName).Equals(commandName, StringComparison.CurrentCultureIgnoreCase));
        }

        private void PlaceCommandsOnMenus()
        {
            //Place the command on the tools menu.
            //Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:

            if (_actions.Count == 0)
                throw new Exception(String.Format("No actions defined for this AddIn: {0}", GetType().Name));

            foreach (var action in _actions)
            {
                if (CommandIsInstalled(action))
                    continue;

                var attr = (MenuAttribute)action.GetType().GetCustomAttributes(typeof(MenuAttribute), false).FirstOrDefault();

                if (attr != null)
                {
                    string commandBarName = attr.CommandBar;
                    string menuName = attr.Menu;


                    var commandBar = ((CommandBars)Application.CommandBars)[commandBarName];


                    CommandBarPopup toolsPopup = null;

                    if (menuName != null)
                    {
                        CommandBarControl toolsControl = commandBar.Controls[GetMenuName(menuName)];
                        toolsPopup = (CommandBarPopup)toolsControl;
                    }

                    try
                    {
                        var contextGUIDS = new object[] { };

                        //_commands.AddNamedCommand( _addInInstance, "MyCommand", "My Command", "blah", true, 0, ref contextGuids, (int) vsCommandStatus.vsCommandStatusEnabled + (int) vsCommandStatus.vsCommandStatusSupported );

                        var command = _commands.AddNamedCommand2(
                            _addIn,
                            action.KeyName,
                            action.ButtonText,
                            action.ToolTip,
                            true,
                            action.ImageIndex,
                            ref contextGUIDS,
                            (int)vsCommandStatus.vsCommandStatusSupported +
                            (int)vsCommandStatus.vsCommandStatusEnabled,
                            (int)vsCommandStyle.vsCommandStylePictAndText,
                            vsCommandControlType.vsCommandControlTypeButton
                            );

                        if (command != null)
                        {
                            command.AddControl(toolsPopup != null ? toolsPopup.CommandBar : commandBar, 1);

                            if (action.KeyboardBinding != null)
                                command.Bindings = action.KeyboardBinding;
                        }
                    }
                    catch (System.ArgumentException ex)
                    {
                        Error(String.Format("PlaceCommandsOnMenus({0})", action.DisplayName), ex);
                    }
                }
            }
        }

        private void RemoveCommandsFromMenus()
        {
            foreach (var action in _actions)
            {
                try
                {
                    var command = _commands.Item(action.FullName, 1);
                    command.Delete();
                }
                catch (Exception ex)
                {
                    Error(action.DisplayName + " not found: ", ex);
                }
            }
        }

        private void Execute(Action action)
        {
            Stopwatch timer = Stopwatch.StartNew();

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            try
            {
                SetStatus("{0}...", action.DescriptivePhrase);
                using (new ScopedUndoContext(this, action.KeyName))
                {
                    action.Execute();
                }
                SetStatus("{0} completed in {1} seconds", action.DescriptivePhrase, timer.Elapsed.TotalSeconds);
            }
            catch (Exception ex)
            {
                Error(ex);
                SetStatus("Error {0}: {1}", action.DescriptivePhrase, ex.Message);
            }
            finally
            {
                timer.Stop();
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
        }

        internal string DocumentFullName
        {
            get { return Application.ActiveDocument != null ? Application.ActiveDocument.FullName : ""; }
        }

        internal bool IsCurrentDocumentExtension(string extension)
        {
            return Path.GetExtension(DocumentFullName).Equals(
                String.Format(".{0}", extension),
                StringComparison.CurrentCultureIgnoreCase
            );
        }

        internal void SetStatus(string message, params object[] args)
        {
            Application.StatusBar.Text = String.Format(message, args);
        }

        private string GetDumpFile()
        {
            return Path.Combine(System.IO.Path.GetTempPath(), "Laan.Ssms.log");
        }

        [Conditional("DEBUG")]
        internal void Error(Exception ex)
        {
            try
            {
                System.IO.File.AppendAllText(GetDumpFile(), ex.ToString() + Environment.NewLine);
            }
            catch (Exception)
            {
            }

            Debug.WriteLine(ex);
        }

        [Conditional("DEBUG")]
        internal void Error(string message, Exception ex)
        {
            try
            {
                System.IO.File.AppendAllText(GetDumpFile(), message + " : " + ex.ToString() + Environment.NewLine);
            }
            catch (Exception)
            {
            }

            Debug.WriteLine(message + "\n\t" + ex);
        }

        internal void OpenUndoContext(string name, bool strict)
        {
            Application.UndoContext.Open(name, strict);
        }

        internal void CloseUndoContext()
        {
            Application.UndoContext.Close();
        }

        internal void CancelSelection()
        {
            TextDocument.Selection.Cancel();
        }

        internal void SelectCurrentWord()
        {
            var cursor = TextDocument.Selection.ActivePoint;
            var point = cursor.CreateEditPoint();
            point.WordLeft(1);
            TextDocument.Selection.MoveToPoint(point, false);
            TextDocument.Selection.WordRight(true, 1);
        }

        internal void ClearSelection()
        {
            TextDocument.Selection.Text = "";
        }

        internal void InsertText(string message)
        {
            InsertText(message, true);
        }

        internal void InsertText(string message, bool replace)
        {
            if (replace)
                ClearSelection();
            TextDocument.Selection.Insert(message, (int)vsInsertFlags.vsInsertFlagsContainNewText);
        }

        internal string CurrentLine
        {
            get
            {
                var cursor = TextDocument.Selection.ActivePoint;
                var point = cursor.CreateEditPoint();
                point.StartOfLine();
                return point.GetText(cursor);
            }
        }

        internal string CurrentWord
        {
            get
            {
                var cursor = TextDocument.Selection.ActivePoint;
                var point = cursor.CreateEditPoint();
                point.WordLeft(1);
                return point.GetText(cursor).Trim();
            }
        }

        internal string CurrentSelection
        {
            get { return TextDocument.Selection.Text; }
        }

        internal TextDocument TextDocument
        {
            get { return (TextDocument)Application.ActiveDocument.Object("TextDocument"); }
        }

        internal string AllText
        {
            get
            {
                var cursor = TextDocument.StartPoint;
                var point = cursor.CreateEditPoint();
                point.StartOfDocument();
                return point.GetText(TextDocument.EndPoint);
            }
        }

        internal Cursor Cursor
        {
            get
            {
                var point = TextDocument.Selection.TopPoint;
                return new Cursor(point.DisplayColumn, point.Line);
            }
            set
            {
                var point = TextDocument.Selection.TopPoint.CreateEditPoint();
                point.LineDown(value.Row);
                point.CharRight(value.Column);
                TextDocument.Selection.MoveToPoint(point, false);
            }
        }

        internal Cursor VirtualCursor
        {
            get
            {
                var start = TextDocument.Selection.TextPane.StartPoint;
                var top = TextDocument.Selection.TopPoint;
                return new Cursor(top.DisplayColumn - start.DisplayColumn + 1, top.Line - start.Line + 1);
            }
            set
            {
                var point = TextDocument.Selection.TopPoint.CreateEditPoint();
                point.LineDown(value.Row);
                point.CharRight(value.Column);
                TextDocument.Selection.MoveToPoint(point, false);
            }
        }
    }
}