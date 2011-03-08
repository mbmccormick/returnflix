using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using returnflix.Common;
using WrapNetflix;
using System.Xml;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Configuration;

namespace returnflix
{
    public partial class Authorize : System.Web.UI.Page
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                NetflixConnection conn = new NetflixConnection(ConfigurationManager.AppSettings["NetflixConsumerKey"], ConfigurationManager.AppSettings["NetflixConsumerSecret"]);
                RequestToken reqTok = (RequestToken)Session["REQUEST_TOKEN"];

                AccessToken accTok;
                if (Session["ACCESS_TOKEN"] != null)
                {
                    accTok = (AccessToken)Session["ACCESS_TOKEN"];
                }
                else
                {
                    accTok = reqTok.ConvertToAccessToken();
                    Session["ACCESS_TOKEN"] = accTok;
                }
                
                System.Xml.Linq.XElement userFeed = conn.RequestXmlResource(NetflixUrls.GetUsers(accTok), accTok);
                string firstName = userFeed.Element("first_name").Value;
                string lastName = userFeed.Element("last_name").Value;
                
                Boolean isNewUser = false;
                string currentMovie = "";
                int currentMovieDays = 0;
                try
                {
                    System.Xml.Linq.XElement atHomeFeed = conn.RequestXmlResource(NetflixUrls.GetUsersAtHome(accTok, 0, 10, DateTime.Now.AddDays(-7)), accTok);
                    currentMovie = atHomeFeed.Elements("at_home_item").First().Element("title").Attribute("regular").Value;
                    currentMovieDays = DateTime.Now.Subtract(UnixTimeToDateTime(atHomeFeed.Elements("at_home_item").First().Element("estimated_arrival_date").Value)).Days;
                }
                catch
                {
                    isNewUser = true;
                }

                DatabaseDataContext db = new DatabaseDataContext();

                var count = (from r in db.Users where r.NetflixUserID == accTok.UserId select r).Count();
                if (count == 0)
                {
                    returnflix.Common.User u = new returnflix.Common.User();

                    u.NetflixUserID = accTok.UserId;
                    u.NetflixAccessToken = accTok.Token;
                    u.NetflixAccessTokenSecret = accTok.TokenSecret;
                    u.FirstName = firstName;
                    u.LastName = lastName;
                    u.EmailAddress = (string)Session["EMAIL_ADDRESS"];
                    u.LastNotificationDate = Convert.ToDateTime("1/1/1900 12:00:00 AM");
                    u.CreatedDate = DateTime.Now;

                    db.Users.InsertOnSubmit(u);

                    if (isNewUser == false)
                        this.uxAuthorizeMessage.Text = string.Format("<b>Welcome to Returnflix, {0}!</b><br /><br />Your account is now activated and we're monitoring your rental activity. We'll let you know if you forget to send \"{1}\" and future movies back to Netflix!", firstName, currentMovie);
                    else
                        this.uxAuthorizeMessage.Text = string.Format("<b>Welcome to Returnflix, {0}!</b><br /><br />Your account is now activated and we're monitoring your rental activity. It looks like you're new to Netflix or you don't have any movies at home right now. Nonetheless, when you do get your first movie from Netflix, we'll let you know if you forget to send it back!", firstName, currentMovie);
                    this.ddUnsubscribe.NavigateUrl = string.Format("/unsubscribe.aspx?id={0}", u.NetflixUserID);
                }
                else
                {
                    returnflix.Common.User u = db.Users.Single(r => r.NetflixUserID == accTok.UserId);

                    u.NetflixAccessToken = accTok.Token;
                    u.NetflixAccessTokenSecret = accTok.TokenSecret;

                    this.uxAuthorizeMessage.Text = string.Format("<b>Hello, {0}. Welcome back!</b><br /><br />Your account is active and we're monitoring your rental activity. You've had \"{1}\" for {2} days. We'll let you know if you forget to send this and future movies back to Netflix!", firstName, currentMovie, currentMovieDays);
                    this.ddUnsubscribe.NavigateUrl = string.Format("/unsubscribe.aspx?id={0}", u.NetflixUserID);
                }

                db.SubmitChanges();
            }
        }

        public static DateTime UnixTimeToDateTime(string text)
        {
            double seconds = double.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
            return Epoch.AddSeconds(seconds);
        }
    }
}