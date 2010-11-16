using System;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System.ComponentModel;
using System.Net;
using System.Web.Script.Serialization;
using System.IO;

namespace BrickRed.WebParts.Facebook.Wall
{
    [Guid("a6cd8610-e819-41d7-a45c-4eb4db06147c")]
    public class ShowWall : System.Web.UI.WebControls.WebParts.WebPart
    {
        public ShowWall()
        {
        }

        Label LblMessage;

        #region Webpart Properties
        [WebBrowsable(true),
       Category("Facebook Settings"),
       Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
       DefaultValue(""),
       WebDisplayName("User Id / User Name"),
       WebDescription("Please enter user id")]

        public string UserID { get; set; }


        [WebBrowsable(true),
       Category("Facebook Settings"),
       Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
       DefaultValue("10"),
       WebDisplayName("Wall Count"),
       WebDescription("Please enter number of wall you want to display")]

        public int WallCount { get; set; }

        [WebBrowsable(true),
       Category("Facebook Settings"),
       Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
       DefaultValue("true"),
       WebDisplayName("Show Description"),
       WebDescription("Would you like to show description")]

        public bool EnableShowDesc { get; set; }

        #endregion

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            int i = 0;
            try
            {
                Table mainTable;
                TableRow tr, tr2;
                TableCell tc, tc3;
                Label Caption, Caption2;
                mainTable = new Table();
                mainTable.Width = Unit.Percentage(100);
                mainTable.CellSpacing = 0;
                mainTable.CellPadding = 5;
                mainTable.BorderWidth = 1;

                mainTable.CssClass = "ms-listviewtable";
                this.Controls.Add(mainTable);

                // Get an access token in some manner.
                // By default you can only get public info.

                if (!String.IsNullOrEmpty(this.UserID))
                {
                    JSONObject me = GetFeeds();
                    if (me.Dictionary["data"] != null)
                    {
                        JSONObject[] feeds = me.Dictionary["data"].Array;

                        foreach (JSONObject feed in feeds)
                        {
                            if (i < this.WallCount)
                            {
                                tr = new TableRow();
                                mainTable.Rows.Add(tr);
                                tc = new TableCell();
                                tc.Width = Unit.Percentage(30);
                                tr.Cells.Add(tc);

                                Caption = new Label();
                                Caption.Font.Bold = true;
                                Caption.Text = feed.Dictionary["message"].String;
                                tc.Controls.Add(Caption);

                                tr2 = new TableRow();
                                tc3 = new TableCell();

                                if (this.EnableShowDesc)
                                {
                                    tc3.VerticalAlign = VerticalAlign.Top;
                                    mainTable.Rows.Add(tr2);
                                    tr2.Cells.Add(tc3);

                                    Caption2 = new Label();
                                    Caption2.Text = relativeTime(feed.Dictionary["created_time"].String.ToString());
                                    tc3.Controls.Add(Caption2);
                                }

                                if (i % 2 == 0)
                                {
                                    tr.CssClass = "";
                                    tr2.CssClass = "";
                                    tc.CssClass = "ms-vb";
                                    tc3.CssClass = "ms-vb";
                                }
                                else
                                {
                                    tr.CssClass = "ms-alternating";
                                    tr2.CssClass = "ms-alternating";
                                    tc.CssClass = "ms-vb";
                                    tc3.CssClass = "ms-vb";
                                }
                            }
                            else
                            {
                                break;
                            }
                            i++;
                        }
                    }
                }
                else
                {
                    throw new Exception("User ID / User Name  missing in webpart properties.");
                }
            }
            catch (Exception Ex)
            {
                LblMessage = new Label();
                LblMessage.Text = Ex.Message;
                this.Controls.Add(LblMessage);
            }
        }

        private string relativeTime(string pastTime)
        {
            DateTime origStamp = DateTime.Parse(pastTime.ToString());
            DateTime curDate = DateTime.Now;

            TimeSpan ts = curDate.Subtract(origStamp);
            string strReturn = string.Empty;

            if (ts.Days >= 1)
            {
                strReturn = String.Format("{0:hh:mm tt MMM dd}" + "th", Convert.ToDateTime(pastTime).ToUniversalTime());
            }
            else
            {
                if (ts.Hours >= 1)
                    strReturn = "about " + ts.Hours + " hours ago";
                else
                {
                    if (ts.Minutes >= 1)
                    {
                        strReturn = "about " + ts.Minutes + " minutes ago";
                    }
                    else
                        strReturn = "about " + ts.Seconds + " seconds ago";
                }
            }
            return strReturn;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        private JSONObject GetFeeds()
        {
            JSONObject obj = null;
            try
            {
                string url = string.Format("https://graph.facebook.com/{0}/feed", this.UserID);

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string retVal = reader.ReadToEnd();

                    obj = JSONObject.CreateFromString(retVal);
                    if (obj.IsDictionary && obj.Dictionary.ContainsKey("error"))
                    {
                        throw new Exception(obj.Dictionary["error"].Dictionary["type"].String, new Exception(obj.Dictionary["error"].Dictionary["message"].String));
                    }
                }
            }
            catch (Exception Ex)
            {
                LblMessage = new Label();
                LblMessage.Text = Ex.Message;
                this.Controls.Add(LblMessage);
            }
            return obj;
        }
    }
}
