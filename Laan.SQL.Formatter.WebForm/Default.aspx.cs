using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Laan.SQL.Formatter.Web
{
    public partial class Formatter : System.Web.UI.Page
    {
        protected void Page_Load( object sender, EventArgs e )
        {

        }

        protected void btnConvert_Click( object sender, EventArgs e )
        {
            string output = "";

            var engine = new FormattingEngine();
            try
            {
                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                try
                {
                    output = engine.Execute( sqlInput.Text );
                }
                finally
                {
                    timer.Stop();
                }
                timeTaken.Text = timer.ElapsedMilliseconds.ToString( "0:00:0000" );
            }
            catch ( Exception ex )
            {
                output = "ERROR" + Environment.NewLine + ex.ToString();
            }
            sqlOutput.DataSource = output.Split( new[] { "\r\n" }, StringSplitOptions.None );
            sqlOutput.DataBind();
        }
    }
}
