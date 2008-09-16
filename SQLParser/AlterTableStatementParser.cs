using System;

namespace Laan.SQL.Parser
{
    public class AlterTableStatementParser : TableStatementParser
    {
        private const string ADD = "ADD";
        private const string FOREIGN = "FOREIGN";
        private const string REFERENCES = "REFERENCES";
        private const string WITH = "WITH";
        private const string NOCHECK = "NOCHECK";
        private AlterTableStatement _statement;

        internal AlterTableStatementParser( Tokenizer tokenizer ) : base( tokenizer ) { }

        public override IStatement Execute()
        {
            // TODO: This is not currently a general purpose ALTER TABLE parser.  
            //       It only allows ADD CONSTRAINT X PRIMARY KEY CLUSTERED and FOREIGN KEY (partial).  
            //       Further grammars will be supported on a need-to-do basis

            _statement = new AlterTableStatement();
            _statement.TableName = GetTableName();

            if ( Tokenizer.TokenEquals( WITH ) )
            {
                if ( Tokenizer.TokenEquals( NOCHECK ) )
                {
                    string nocheck = CurrentToken;
                }
            }
                
            // TODO: future changes will include a ADD/DELETE operation field, and an entity COLUMN/CONSTRAINT/etc
            //       to affect, and various options to cater for the constraint syntax
            ExpectTokens( new[] { ADD, CONSTRAINT } );
            _statement.ConstraintName = GetIdentifier();

            if (Tokenizer.TokenEquals( PRIMARY ) )
            {
                ExpectTokens( new[] { KEY, CLUSTERED, OPEN_BRACKET } );
                _statement.PrimaryKey = GetIdentifier();
                ExpectToken( CLOSE_BRACKET );
            }
            else if ( Tokenizer.TokenEquals( FOREIGN ) )
            {
                // TODO: these fields are being consumed, but not stored into a constrain object
                //       this is not required (for my current task) at this stage.
                ExpectTokens( new[] { KEY, OPEN_BRACKET } );
                string keyID = GetIdentifier();
                ExpectTokens( new[] { CLOSE_BRACKET, REFERENCES } );
                string refereringTable = GetTableName();

                ExpectToken( OPEN_BRACKET );
                string result = GetIdentifier();
                ExpectToken( CLOSE_BRACKET );
            }

            return _statement;
        }
    }
}
