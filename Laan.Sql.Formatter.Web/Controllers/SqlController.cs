using System;
using System.Collections.Generic;
using System.Web.Http;

using Laan.Sql.Formatter.Web.Models;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Formatter.Web.Controllers
{
    public class SqlController : ApiController
    {
        [HttpPost]
        [Route("api/sql/format")]
        public SqlFormatResult PostFormatted([FromBody]string query)
        {
            var output = String.Empty;
            var engine = new FormattingEngine();
            var timer = new System.Diagnostics.Stopwatch();

            timer.Start();
            try
            {
                output = engine.Execute(query);

                return new SqlFormatResult
                {
                    Sql = output.Split(new[] { "\r\n" }, StringSplitOptions.None),
                    Duration = timer.Elapsed
                };
            }
            catch (ParserException ex)
            {
                output = ex.Message;
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
