using System;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Parsers
{
    public class AlterTableStatementParser : TableStatementParser<AlterTableStatement>
    {
        internal AlterTableStatementParser( ITokenizer tokenizer ) : base( tokenizer ) { }

        public override AlterTableStatement Execute()
        {
            // TODO: This is not currently a general purpose ALTER TABLE parser.  
            //       It only allows ADD CONSTRAINT X PRIMARY KEY CLUSTERED and FOREIGN KEY (partial).  
            //       Further grammars will be supported on a need-to-do basis

            _statement = new AlterTableStatement();
            _statement.TableName = GetTableName();

            if ( Tokenizer.TokenEquals( Constants.With ) )
            {
                if ( Tokenizer.TokenEquals( Constants.NoCheck ) )
                {
                    string nocheck = CurrentToken;
                }
            }
                
            // TODO: future changes will include a ADD/DELETE operation field, and an entity COLUMN/CONSTRAINT/etc
            //       to affect, and various options to cater for the constraint syntax
            Tokenizer.ExpectTokens( new[] { Constants.Add, Constants.Constraint } );
            _statement.ConstraintName = GetIdentifier();

            if ( Tokenizer.TokenEquals( Constants.Primary ) )
            {
                Tokenizer.ExpectTokens( new[] { Constants.Key, Constants.Clustered } );
                using ( Tokenizer.ExpectBrackets() )
                {
                    _statement.PrimaryKeys = GetIdentifierList();
                }
            }
            else if ( Tokenizer.TokenEquals( Constants.Unique ) )
            {
                Tokenizer.ExpectTokens( new[] { Constants.NonClustered } );
                using ( Tokenizer.ExpectBrackets() )
                {
                    _statement.PrimaryKeys = GetIdentifierList();
                }
            }
            else if ( Tokenizer.TokenEquals( Constants.Foreign ) )
            {
                // TODO: these fields are being consumed, but not stored into a constrain object
                //       this is not required (for my current task) at this stage.
                Tokenizer.ExpectTokens( new[] { Constants.Key } );
                using ( Tokenizer.ExpectBrackets() )
                {
                    string keyID = GetIdentifier();
                }

                Tokenizer.ExpectTokens( new[] { Constants.References } );
                string refereringTable = GetTableName();

                using ( Tokenizer.ExpectBrackets() )
                {
                    string result = GetIdentifier();
                }
            }

            return _statement;
        }
    }
}
