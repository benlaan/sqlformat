using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Laan.SQL.Formatter;
using Laan.AddIns.Core;

namespace Laan.AddIns.Ssms.Actions
{
    public class SqlTemplating : Laan.AddIns.Core.Action
    {
        private static Dictionary<string, string> _templates;

        static SqlTemplating()
        {
            // temp template listing - this will change to being user-config, loaded from a file..
            _templates = new Dictionary<string, string>() 
            { 
                // Blocks, Transaction control
                { "br",     "BEGIN TRAN\n\n\t|\n\nROLLBACK" },
                { "bc",     "BEGIN TRAN\n\n\t|\n\nCOMMIT" },
                { "btr",    "BEGIN TRAN\n\n\t|\n\nROLLBACK\n--COMMIT" },
                { "btc",    "BEGIN TRAN\n\n\t|\n\nCOMMIT\n--ROLLBACK" },
                { "be",     "BEGIN\n\n\t|\n\nEND" },
                { "bt",     "BEGIN TRAN" },
                { "rt",     "ROLLBACK" },
                { "ct",     "COMMIT" },

                // Statements
                { "sfw",    "SELECT \nFROM |\nWHERE " },
                { "ufw",    "UPDATE \n   SET \nFROM |\nWHERE " },
                
                // Joins
                { "j",      "JOIN |\n  ON " },
                { "lj",     "LEFT JOIN |\n       ON " },
                { "rj",     "RIGHT JOIN |\n        ON " },

                // Helpers
                { "ob",    "ORDER BY |" },
                { "gb",    "GROUP BY |" },
                { "hv",    "HAVING |" },
                { "isn",   "IS NULL" },
                { "isnn",  "IS NOT NULL" },
                { "ex",    "EXISTS(|)" },
                { "nex",    "NOT EXISTS(|)" },
                { "pr",    "PRINT '|'" },

                // Declarations
                { "di",     "DECLARE @| INT" },
                { "dv",     "DECLARE @| VARCHAR(MAX)" },
                { "dn",     "DECLARE @| NUMERIC(12,4)" },
                { "dm",     "DECLARE @| MONEY" },
                { "ddt",    "DECLARE @| DATETIME" },
                { "dt",     "DECLARE @| TIME" },
                { "dd",     "DECLARE @| DATE" },

                // Declarations with assignment
                { "die",    "DECLARE @| INT = " },
                { "dve",    "DECLARE @| VARCHAR(MAX) = ''" },
                { "dne",    "DECLARE @| NUMERIC(12,4) = " },
                { "dme",    "DECLARE @| MONEY = " },
                { "ddte",   "DECLARE @| DATETIME = " },
                { "dte",    "DECLARE @| TIME = " },
                { "dde",    "DECLARE @| DATE = " },
                
                // Case
                { "ca",     "CASE | WHEN END" },
                { "cw",     "CASE WHEN | THEN  END" },
                { "cwe",    "CASE WHEN | THEN  ELSE  END" },

                // If
                { "ib",     "IF |\nBEGIN\n\n\t\n\nEND" },
                { "ibe",    "IF |\nBEGIN\n\n\t\n\nEND\nELSE\nBEGIN\n\n\t\n\nEND" }
            };
        }

        private Cursor DetermineCursorFromBar( IList<string> lines )
        {
            int column = 0;
            int row = 0;

            foreach ( string line in lines )
            {
                column = line.IndexOf( '|' );
                if ( column >= 0 )
                    break;
                row++;
            }

            // if no bar was found, just put the cursor at the last row and column of the replaced text
            if ( column == -1 )
            {
                column = lines.Last().Length;
                row = lines.Count() - 1;
            }
            return new Cursor( column, row );
        }

        public SqlTemplating( AddIn addIn ) : base( addIn )
        {
            KeyName = "LaanSqlTemplating";
            DisplayName = "Insert Template";
            DescriptivePhrase = "Inserting Template";

            ButtonText = "Insert &Template";
            ToolTip = "Inserts a template at the cursor";
            ImageIndex = 59;
            KeyboardBinding = "Text Editor::Ctrl+Shift+`";
        }

        public override bool CanExecute()
        {
            return ( _addIn.IsCurrentDocumentExtension( "sql" ) );
        }

        public override void Execute()
        {
            try
            {
                var word = _addIn.SelectCurrentWord();
                string padding = new string( ' ', 4 );
                var raw = _templates[ word ]
                    .Split( '\n' )
                    .Select( line => line.Replace( "\t", padding ) )
                    .ToList();

                string offset = new string( ' ', _addIn.Cursor.Column - 1 );
                var lines = new List<String>(
                    raw.Select( line => ( line == raw.First() ? "" : offset ) + line
                    )
                );

                Cursor cursor = DetermineCursorFromBar( raw );
                if ( lines.Count > cursor.Row )
                    lines[ cursor.Row ] = lines[ cursor.Row ].Replace( "|", "" );

                _addIn.InsertText( String.Join( "\n", lines.ToArray() ) );
                _addIn.Cursor = cursor;
            }
            finally
            {
                _addIn.CancelSelection();
            }
        }
    }
}
