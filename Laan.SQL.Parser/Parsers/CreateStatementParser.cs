using System;

namespace Laan.Sql.Parser.Parsers
{
    public class CreateStatementParser : IParser
    {
        private ITokenizer _tokenizer;

        /// <summary>
        /// Initializes a new instance of the CreateStatementParser class.
        /// </summary>
        public CreateStatementParser(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        #region IParser

        public IStatement Execute()
        {
            IParser parser = null;

            if (_tokenizer.TokenEquals(Constants.Table))
                parser = new CreateTableStatementParser(_tokenizer);

            if (_tokenizer.TokenEquals(Constants.View))
                parser = new CreateViewStatementParser(_tokenizer);

            if (_tokenizer.TokenEquals(Constants.Procedure) || _tokenizer.TokenEquals(Constants.Proc))
                parser = new CreateProcedureStatementParser(_tokenizer) { IsShortForm = _tokenizer.Current == Constants.Proc };

            //if ( _tokenizer.TokenEquals( Constants.Trigger ) )
            //    parser = new CreateTriggerStatementParser( _tokenizer );

            if (_tokenizer.IsNextToken(
                    Constants.Unique,
                    Constants.Clustered,
                    Constants.NonClustered,
                    Constants.Index
                )
            )
                parser = new CreateIndexParser(_tokenizer);

            return parser != null ? parser.Execute() : null;
        }
        #endregion

    }
}