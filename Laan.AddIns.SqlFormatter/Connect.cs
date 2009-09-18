using EnvDTE;
using Extensibility;
using Microsoft.Office.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Laan.SQL.Formatter;

namespace Laan.AddIns.SqlFormatter
{

    #region Read me for Add-in installation and setup information.
    // When run, the Add-in wizard prepared the registry for the Add-in.
    // At a later time, if the Add-in becomes unavailable for reasons such as:
    //   1) You moved this project to a computer other than which is was originally created on.
    //   2) You chose 'Yes' when presented with a message asking if you wish to remove the Add-in.
    //   3) Registry corruption.
    // you will need to re-register the Add-in by building the MyAddin21Setup project 
    // by right clicking the project in the Solution Explorer, then choosing install.
    #endregion

    /// <summary>
    ///   The object for implementing an Add-in.
    /// </summary>
    /// <seealso class='IDTExtensibility2' />
    [GuidAttribute( "CC5866B2-5695-4557-AA26-A7AFB5072533" ), ProgId( LaanSQLFormatter )]
    public class Connect : Object, IDTExtensibility2, IDTCommandTarget
    {
        private const string LaanSQLFormatter = "Laan.Addins.SqlFormatter";
        
        private _DTE _application;
        private AddIn _addIn;

        public Connect()
        {
        }

        #region Not Used

        public void OnDisconnection( ext_DisconnectMode disconnectMode, ref System.Array custom )
        {
        }

        public void OnAddInsUpdate( ref System.Array custom )
        {
        }

        public void OnStartupComplete( ref System.Array custom )
        {
        }

        public void OnBeginShutdown( ref System.Array custom )
        {
        }

        #endregion

        public void OnConnection( object application, ext_ConnectMode connectMode, object addInInst, ref System.Array custom )
        {
            _application = (_DTE) application;
            _addIn = (AddIn) addInInst;

            if ( connectMode == ext_ConnectMode.ext_cm_UISetup )
            {
                object[] contextGUIDS = new object[] {};
                Commands commands = _application.Commands;
                _CommandBars commandBars = _application.CommandBars as _CommandBars;

                try
                {
                    Command command = commands.AddNamedCommand(
                        _addIn,
                        LaanSQLFormatter,
                        "SQL Format",
                        "Applies formatting to SQL code",
                        true,
                        59,
                        ref contextGUIDS,
                        (int) vsCommandStatus.vsCommandStatusSupported + (int) vsCommandStatus.vsCommandStatusEnabled
                    );
                    CommandBar commandBar = (CommandBar) commandBars[ "Tools" ];
                    //commandBar.
                    command.AddControl( commandBar, 1 );
                }
                catch ( System.Exception e )
                {
                    Trace.WriteLine( e.Message.ToString() );
                }
            }
        }

        public void FormatSQL()
        {
            TextDocument textDocument = (TextDocument) _application.ActiveDocument.Object( "TextDocument" );
            EditPoint editPoint = (EditPoint) textDocument.StartPoint.CreateEditPoint();
            
            string output = "";

            var engine = new FormattingEngine();
            try
            {
                output = engine.Execute( textDocument.Selection.Text );
                textDocument.Selection.Text = output;
            }
            catch ( Exception ex )
            {
                output = textDocument.Selection.Text;
                Trace.WriteLine( ex );
            }
       }

        public void QueryStatus( string commandName, EnvDTE.vsCommandStatusTextWanted neededText, ref EnvDTE.vsCommandStatus status, ref object commandText )
        {
            if ( neededText == EnvDTE.vsCommandStatusTextWanted.vsCommandStatusTextWantedNone )
            {
                if ( commandName == LaanSQLFormatter )
                {
                    status = (vsCommandStatus) vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                }
            }
        }

        public void Exec( string commandName, EnvDTE.vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled )
        {
            handled = false;
            if ( executeOption == EnvDTE.vsCommandExecOption.vsCommandExecOptionDoDefault )
            {
                if ( commandName == LaanSQLFormatter )
                {
                    FormatSQL();
                    handled = true;
                    return;
                }
            }
        }
    }
}