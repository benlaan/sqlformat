using System;

namespace Laan.Sql.Parser.Parsers
{
    public class AlterStatementParser : IParser
    {
        private ITokenizer _tokenizer;

        /// <summary>
        /// Initializes a new instance of the CreateStatementParser class.
        /// </summary>
        public AlterStatementParser(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        public IStatement Execute()
        {
            IParser parser = null;

            if (_tokenizer.TokenEquals(Constants.Table))
                parser = new AlterTableStatementParser(_tokenizer);

            if (_tokenizer.TokenEquals(Constants.View))
                parser = new CreateViewStatementParser(_tokenizer) { IsAlter = true };

            if (_tokenizer.TokenEquals(Constants.Procedure) || _tokenizer.TokenEquals(Constants.Proc))
                parser = new CreateProcedureStatementParser(_tokenizer) { IsAlter = true };

            //if ( _tokenizer.TokenEquals( Constants.Trigger ) )
            //    parser = new AlterTriggerStatementParser( _tokenizer );

            return parser != null ? parser.Execute() : null;
        }
    }
}
