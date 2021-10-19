using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WSprospectos.Global;

namespace WSprospectos
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ConnectionString = Configurations.ConnectionString;
            this.Label1.Text = ConnectionString.Split(';')[1].Split('=')[1];
        }
    }
}