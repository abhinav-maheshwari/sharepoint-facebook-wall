/*
 ===========================================================================
 Copyright (c) 2010 BrickRed Technologies Limited

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sub-license, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
 ===========================================================================
 */
using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System.Net;
using System.IO;

namespace BrickRed.WebParts.Facebook.Wall
{
    [ToolboxItemAttribute(false)]
    public class ShowWall : Microsoft.SharePoint.WebPartPages.WebPart
    {
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
                TableRow tr;
                TableCell tc;
                mainTable = new Table();
                mainTable.Width = Unit.Percentage(100);
                mainTable.CellSpacing = 0;
                mainTable.CellPadding = 0;

                mainTable.CssClass = "ms-viewlsts";
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
                                //mainTable.Rows.Add(tr);
                                tc = new TableCell();
                                tc.CssClass = "ms-vb2";
                                tc.Width = Unit.Percentage(30);

                                bool dataFound = false;

                                if (feed.Dictionary.ContainsKey("message"))
                                {
                                    dataFound = true;
                                    tc.Text = feed.Dictionary["message"].String;
                                }
                                else if (feed.Dictionary.ContainsKey("description"))
                                {
                                    dataFound = true;
                                    tc.Text = feed.Dictionary["description"].String;
                                }
                                else if (feed.Dictionary.ContainsKey("name"))
                                {
                                    dataFound = true;
                                    tc.Text = feed.Dictionary["name"].String;

                                }

                                if (dataFound)
                                {
                                    tr.Cells.Add(tc);
                                    mainTable.Rows.Add(tr);

                                    if (i % 2 != 0)
                                    {
                                        tr.CssClass = "ms-alternatingstrong";
                                    }

                                    tr = new TableRow();
                                    tc = new TableCell();
                                    tc.CssClass = "ms-vb2";

                                    if (this.EnableShowDesc)
                                    {
                                        // tc3.VerticalAlign = VerticalAlign.Top;
                                        mainTable.Rows.Add(tr);
                                        tc.Text = relativeTime(feed.Dictionary["created_time"].String.ToString());
                                        tc.Style.Add("color", "Gray");
                                        tr.Cells.Add(tc);
                                        if (i % 2 != 0)
                                        {
                                            tr.CssClass = "ms-alternatingstrong";
                                        }
                                    }
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
            string url;
            HttpWebRequest request;
            string oAuthToken = string.Empty;

            try
            {

                url = string.Format("http://graph.facebook.com/{0}/feed", this.UserID);
                request = WebRequest.Create(url) as HttpWebRequest;
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
