using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using returnflix.Common;
using System.Data.SqlClient;

namespace returnflix
{
    public partial class Unsubscribe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DatabaseDataContext db = new DatabaseDataContext();

            returnflix.Common.User u = db.Users.Single(r => r.NetflixUserID == Request.QueryString["id"]);
            db.Users.DeleteOnSubmit(u);

            db.SubmitChanges();
        }
    }
}