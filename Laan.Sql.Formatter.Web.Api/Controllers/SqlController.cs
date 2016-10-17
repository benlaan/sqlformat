using System;
using System.Collections.Generic;
using System.Web.Http;

using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Formatter.Web.Api.Controllers
{
    public class SqlFormatResult
    {
        public IList<string> Sql { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class SqlController : ApiController
    {
        [HttpGet]
        [Route("api/sql/format")]
        public SqlFormatResult GetFormatted(string query)
        {
            string output = String.Empty;

            var engine = new FormattingEngine();
            var timer = new System.Diagnostics.Stopwatch();
            try
            {
                timer.Start();
                output = engine.Execute(query);

                return new SqlFormatResult
                {
                    Sql = output.Split(new[] { "\r\n" }, StringSplitOptions.None),
                    Duration = timer.Elapsed
                };
            }
            catch (ParserException ex)
            {
                output = ex.ToString();
            }
            catch (Exception ex)
            {
                output = "ERROR" + Environment.NewLine + ex.ToString();
            }
            finally
            {
                timer.Stop();
            }

            return new SqlFormatResult
            {
                Sql = new[] { output },
                Duration = timer.Elapsed
            };
        }

    }
}
