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
       WebDisplayName("User Id / User Name / Page Id"),
       WebDescription("Please enter User Id /User Name/page Id")]

        public string UserID { get; set; }


        [WebBrowsable(true),
       Category("Facebook Settings"),
       Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
       DefaultValue("10"),
       WebDisplayName("Wall Count"),
       WebDescription("Please enter number of wall you want to display")]

        public int WallCount { get; set; }

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

                                if (i % 2 != 0)
                                {
                                    tr.CssClass = "ms-alternatingstrong";
                                }

                                tc = new TableCell();
                                tc.CssClass = "ms-vb2";
                                tc.Controls.Add(parseFeed(feed, i));
                                tr.Cells.Add(tc);
                                mainTable.Rows.Add(tr);
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

        private Table parseFeed(JSONObject feed, int counter)
        {
            Table feedTable;
            TableRow feedTableRow;
            TableCell feedTableCell;
            HyperLink objHyperLink;
            Table childTable = new Table();
            TableRow childRow = new TableRow();
            TableCell childCell;

            //first table row in main feed table
            feedTable = new Table();
            feedTableRow = new TableRow();
            feedTable.Rows.Add(feedTableRow);


            //first of all see what is the type of this feed

            switch (feed.Dictionary["type"].String)
            {
                case "status":
                    if (feed.Dictionary.ContainsKey("message"))
                    {
                        //first cell and add table of status data
                        feedTableCell = new TableCell();
                        feedTableRow.Cells.Add(feedTableCell);

                        childTable = new Table();
                        childTable.CellPadding = 5;
                        childRow = new TableRow();
                        childTable.Rows.Add(childRow);
                        childCell = new TableCell();
                        childCell.Text = feed.Dictionary["message"].String;
                        childRow.Cells.Add(childCell);
                        feedTableCell.Controls.Add(childTable);

                    }
                    break;
                case "photo":
                case "link":
                case "video":

                    //create a feed table cell and add child table
                    feedTableCell = new TableCell();
                    feedTableRow.Cells.Add(feedTableCell);
                    childTable = new Table();
                    childTable.CellPadding = 5;
                    feedTableCell.Controls.Add(childTable);

                    if (feed.Dictionary.ContainsKey("picture"))
                    {
                        childRow = new TableRow();
                        childTable.Rows.Add(childRow);
                        Image image = new Image();
                        image.ImageUrl = feed.Dictionary["picture"].String;

                        childCell = new TableCell();
                        childCell.RowSpan = 4;
                        childRow.Cells.Add(childCell);
                        childCell.Controls.Add(image);

                    }


                    if (feed.Dictionary.ContainsKey("name"))
                    {
                        //next row
                        childRow = new TableRow();
                        childTable.Rows.Add(childRow);
                        childCell = new TableCell();
                        childRow.Cells.Add(childCell);

                        objHyperLink = new HyperLink();
                        childCell.Controls.Add(objHyperLink);
                        objHyperLink.Text = feed.Dictionary["name"].String;
                        objHyperLink.Target = "_New";
                        if (feed.Dictionary.ContainsKey("link"))
                        {
                            objHyperLink.NavigateUrl = feed.Dictionary["link"].String;
                        }
                    }


                    if (feed.Dictionary.ContainsKey("message"))
                    {
                        //next row
                        childRow = new TableRow();
                        childTable.Rows.Add(childRow);
                        childCell = new TableCell();
                        childCell.Text = feed.Dictionary["message"].String;
                        childRow.Cells.Add(childCell);
                    }




                    if (feed.Dictionary.ContainsKey("description"))
                    {
                        //first cell and add table of status data
                        //next row
                        childRow = new TableRow();
                        childTable.Rows.Add(childRow);
                        childCell = new TableCell();
                        childCell.Text = feed.Dictionary["description"].String;
                        childRow.Cells.Add(childCell);
                    }

                    break;
            }



            //second row in main feed table to display the additional information

            feedTableRow = new TableRow();
            feedTable.Rows.Add(feedTableRow);
            if (counter % 2 != 0)
            {
                feedTableRow.CssClass = "ms-alternatingstrong";
            }
            //first cell for feed icon
            feedTableCell = new TableCell();
            feedTableRow.Cells.Add(feedTableCell);

            ///now the child table for data
            childTable = new Table();
            childTable.CellPadding = 5;
            childRow = new TableRow();
            childTable.Rows.Add(childRow);
            feedTableCell.Controls.Add(childTable);


            if (feed.Dictionary.ContainsKey("icon"))
            {
                Image image = new Image();
                image.ImageUrl = feed.Dictionary["icon"].String;

                childCell = new TableCell();
                childRow.Cells.Add(childCell);
                childCell.Controls.Add(image);

            }

            if (feed.Dictionary.ContainsKey("created_time"))
            {
                childCell = new TableCell();
                childRow.Cells.Add(childCell);
                childCell.Text = relativeTime(feed.Dictionary["created_time"].String.ToString());
                childCell.Style.Add("color", "Gray");
            }

            return feedTable;
        }
    }
}
