using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using returnflix.Common;
using WrapNetflix;
using System.Configuration;

namespace returnflix
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void uxAuthorize_Click(object sender, EventArgs e)
        {
            if (this.ddEmailAddress.Text.Length > 0)
            {
                NetflixConnection conn = new NetflixConnection(ConfigurationManager.AppSettings["NetflixConsumerKey"], ConfigurationManager.AppSettings["NetflixConsumerSecret"]);
                RequestToken reqTok = conn.GenerateRequestToken();

                Session["REQUEST_TOKEN"] = reqTok;
                Session["EMAIL_ADDRESS"] = this.ddEmailAddress.Text;

                Response.Redirect(reqTok.PermissionUrl + "&oauth_callback=http%3A%2F%2F" + Request.ServerVariables["SERVER_NAME"] + "%2Fauthorize.aspx");
            }
            else
            {
                Response.Redirect("~/default.aspx?error=Please enter your email address and try again.");
            }
        }
    }
}