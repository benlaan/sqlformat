using System;

namespace SQLParser
{
    public class SelectStatementParser : StatementParser
    {
        private const string DISTINCT = "DISTINCT";
        private const string TOP = "TOP";
        private const string FROM = "FROM";
        private const string AS = "AS";
        private const string INNER = "INNER";
        private const string JOIN = "JOIN";
        private const string LEFT = "LEFT";
        private const string RIGHT = "RIGHT";
        private const string FULL = "FULL";
        private const string COMMA = ",";

        SelectStatement _statement;

        public SelectStatementParser( Tokenizer tokenizer ) : base( tokenizer ) { }

        private void ProcessDistinct()
        {
            _statement.Distinct = Tokenizer.TokenEquals( DISTINCT );
        }

        private void ProcessTop()
        {
            if ( Tokenizer.TokenEquals( TOP ) )
            {
                int top;
                if ( !Int32.TryParse( CurrentToken, out top ) )
                    throw new Exception( String.Format( "Expected integer but found: '{0}'", CurrentToken ) );

                _statement.Top = (int) top;
                ReadNextToken();
            }
        }

        private void ProcessFields()
        {
            do
            {
                ProcessField();

            } while ( Tokenizer.TokenEquals( COMMA ) );
        }

        private void ProcessField()
        {
            //string field = CurrentToken;
            //string? alias = null;
            //if (Tokenizer.TokenEquals(AS) || IsNextTokenWithinSet(new[] { COMMA }))
            //{
            //    alias = field;
            //    ReadNextToken();
            //    field = CurrentToken;
            //}

            //_statement.Fields.Add(new Field() { Name = field, Alias = alias });
            //ReadNextToken();

            _statement.Fields.Add( new Field() { Name = CurrentToken } );
            ReadNextToken();
        }

        private void ProcessFrom()
        {
            ExpectToken( FROM );
            do
            {
                Table table = new Table() { Name = ProcessTableName() };
                _statement.From.Add(table );

                if ( Tokenizer.TokenEquals( AS ) || IsNextTokenWithinSet( new[] { INNER, JOIN, LEFT, RIGHT, FULL, COMMA } ))
                {
                    table.Alias = CurrentToken;
                    ReadNextToken();
                }

            } while ( Tokenizer.TokenEquals( COMMA ) );

            // ProcessJoins
        }

        private string ProcessTableName()
        {
            //TODO: Allow processing of the full format: database.owner.table
            string table = CurrentToken;
            ReadNextToken();
            return table;
        }

        public override IStatement Execute()
        {
            _statement = new SelectStatement();

            ProcessDistinct();
            ProcessTop();
            ProcessFields();
            ProcessFrom();

            return _statement;
        }
    }
}
