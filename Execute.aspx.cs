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
using System.Text;
using System.Configuration;

namespace returnflix
{
    public partial class Execute : System.Web.UI.Page
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ddLogMessage.Text = SendEmailNotifications();            
        }

        public static string SendEmailNotifications()
        {
            StringBuilder output = new StringBuilder();

            NetflixConnection conn = new NetflixConnection(ConfigurationManager.AppSettings["NetflixConsumerKey"], ConfigurationManager.AppSettings["NetflixConsumerSecret"]);
            
            DatabaseDataContext db = new DatabaseDataContext();
            foreach (returnflix.Common.User u in db.Users)
            {
                try
                {
                    output.AppendFormat("Executing Netflix account for {0} {1}...<br />", u.FirstName, u.LastName);

                    AccessToken accTok = new AccessToken(u.NetflixUserID, u.NetflixAccessToken, u.NetflixAccessTokenSecret);

                    System.Xml.Linq.XElement atHomeFeed = conn.RequestXmlResource(NetflixUrls.GetUsersAtHome(accTok, 0, 10, DateTime.Now.AddDays(-7)), accTok);
                    string currentMovie = atHomeFeed.Elements("at_home_item").First().Element("title").Attribute("regular").Value;
                    int currentMovieDays = DateTime.Now.Subtract(UnixTimeToDateTime(atHomeFeed.Elements("at_home_item").First().Element("estimated_arrival_date").Value)).Days;

                    System.Xml.Linq.XElement queueFeed = conn.RequestXmlResource(NetflixUrls.GetUsersQueuesDiscAvailable(accTok, QueueSortOrder.QueueSequence, 0, 10, DateTime.Now.AddYears(-1)), accTok);
                    string nextMovie = queueFeed.Elements("queue_item").First().Element("title").Attribute("regular").Value;

                    output.AppendFormat("\"{0}\" has been at home for {1} days.<br />", currentMovie, currentMovieDays);

                    if (DateTime.Now.Subtract(u.LastNotificationDate).Days >= 3 &&
                        currentMovieDays >= 7)
                    {
                        System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                        msg.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["SmtpEmailAddress"], "Returnflix");
                        msg.To.Add(u.EmailAddress);
                        msg.Subject = string.Format("Don't Forget to Return \"{0}\" to Netflix", currentMovie);
                        msg.Body = string.Format("Hi {0},\n\nHave you had a chance to watch \"{1}\"? This movie has been at home for a week now. You should return this DVD to Netflix soon so that you can receive your next movie, \"{2}\".\n\nThanks!\n\n--\nReturnflix\nhttp://returnflix.com\n\nTired of the reminders? Unsubscribe at http://returnflix.com/unsubscribe.aspx?id={3}", u.FirstName, currentMovie, nextMovie, u.NetflixUserID);
                        msg.IsBodyHtml = false;

                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                        smtp.Host = ConfigurationManager.AppSettings["SmtpHost"];
                        smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
                        smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SmtpUsername"], ConfigurationManager.AppSettings["SmtpPassword"]);

                        output.AppendFormat("Sending message to {0} {1} ({2}).<br />", u.FirstName, u.LastName, u.EmailAddress);

                        smtp.Send(msg);

                        u.LastNotificationDate = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    output.AppendFormat("Failed due to {0}.<br />", ex.Message);
                }

                output.AppendFormat("Done.<br /><br />");
            }

            db.SubmitChanges();

            return output.ToString();
        }

        public static DateTime UnixTimeToDateTime(string text)
        {
            double seconds = double.Parse(text, System.Globalization.CultureInfo.InvariantCulture);
            return Epoch.AddSeconds(seconds);
        }
    }
}