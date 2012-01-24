﻿/*
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
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Generic;


namespace BrickRed.WebParts.Facebook.Wall
{
    [ToolboxItemAttribute(false)]
    public class ShowWall : Microsoft.SharePoint.WebPartPages.WebPart
    {
        #region Public Controls
        Label LblMessage;
        LinkButton lbtnNext = new LinkButton();
        TableCell tcContent = new TableCell();
        TableCell tcpaging = new TableCell();
        #endregion

        #region Check for the Posts User want to see
        protected FeedType _feedtype;
        public enum FeedType
        {
            Users = 0,
            Group = 1
        }
        #endregion

        #region Webpart Properties

        [WebBrowsable(true),
        Category("Facebook Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(""),
        WebDisplayName("Code:"),
        WebDescription("Please enter authorization code")]
        public string OAuthCode { get; set; }

        [WebBrowsable(true),
        Category("Facebook Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(""),
        WebDisplayName("Client ID:"),
        WebDescription("Please enter number of client id for application")]
        public string OAuthClientID { get; set; }

        [WebBrowsable(true),
        Category("Facebook Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(""),
        WebDisplayName("Redirect Url:"),
        WebDescription("Please enter redirect url")]
        public string OAuthRedirectUrl { get; set; }

        [WebBrowsable(true),
        Category("Facebook Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(""),
        WebDisplayName("Client Secret:"),
        WebDescription("Please enter client secret for application")]
        public string OAuthClientSecret { get; set; }

        //Check for the User Posts (User/Group)
        [WebBrowsable(true),
        Category("Facebook Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(true),
        WebDisplayName("Posts"),
        WebDescription("Please select the posts you want to see from")]
        public FeedType UserFeedType
        {
            get { return _feedtype; }
            set { _feedtype = value; }
        }

        [WebBrowsable(true),
        Category("Facebook Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(""),
        WebDisplayName("User Id / User Name / Page Id / Group Id"),
        WebDescription("Please enter user id whose posts you want to display")]
        public string UserID { get; set; }

        [WebBrowsable(true),
        Category("Facebook Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(10),
        WebDisplayName("Initial Wall Count"),
        WebDescription("Please enter no. of posts you want to display")]
        public int WallCount { get; set; }

        [WebBrowsable(true),
        Category("Facebook Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(true),
        WebDisplayName("Show my posts only"),
        WebDescription("Please Check  if want to show only owners posts")]
        public bool IsPosts { get; set; }

        [WebBrowsable(true),
        Category("Facebook Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(true),
        WebDisplayName("Show User Image"),
        WebDescription("Please Check  if want to display the image of users")]
        public bool ShowUserImage { get; set; }

        [WebBrowsable(true),
        Category("Facebook Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(true),
        WebDisplayName("Show header"),
        WebDescription("Would you like to show header")]
        public bool ShowHeader { get; set; }

        [WebBrowsable(true),
        Category("Facebook Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(false),
        WebDisplayName("Show header image"),
        WebDescription("Would you like to Show Header Image")]
        public bool ShowHeaderImage { get; set; }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();
            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            try
            {
                if (!String.IsNullOrEmpty(this.OAuthCode) ||
                    !String.IsNullOrEmpty(this.OAuthClientID) ||
                    !String.IsNullOrEmpty(this.OAuthRedirectUrl) ||
                    !String.IsNullOrEmpty(this.OAuthClientSecret) ||
                    !String.IsNullOrEmpty(this.UserID)
                    )
                {
                    this.Page.Header.Controls.Add(CommonHelper.InlineStyle());
                    ShowPagedFeeds();
                }
                else
                {
                    LblMessage = new Label();
                    LblMessage.Text = "Please set the values of facebook settings in webpart properties section in edit mode.";
                    this.Controls.Add(LblMessage);
                }
            }
            catch (Exception Ex)
            {
                LblMessage = new Label();
                LblMessage.Text = Ex.Message;
                this.Controls.Add(LblMessage);
            }
        }

        private void ShowPagedFeeds()
        {
            Table pagingTable;
            TableRow trpaging = new TableRow();
            TableCell tcpaging = new TableCell();
            pagingTable = new Table();
            pagingTable.Width = Unit.Percentage(100);
            pagingTable.CellSpacing = 0;
            pagingTable.CellPadding = 0;
            pagingTable.CssClass = "ms-viewlsts";
            lbtnNext.Text = "Older Posts";
            lbtnNext.ID = "lbtnNext";
            lbtnNext.Click += new EventHandler(lbtnNext_Click);

            Table Maintable = new Table();
            TableRow trContent;
            //Maintable.Width = Unit.Percentage(100);
            Maintable.CssClass = "fbMainTable";
            Maintable.CellPadding = 0;
            Maintable.CellSpacing = 0;

            //Create the header
            if (this.ShowHeader)
            {
                trContent = new TableRow();
                tcContent = new TableCell();
                tcContent.Controls.Add(CommonHelper.CreateHeader(this.UserID, this.ShowHeaderImage));
                tcContent.CssClass = "fbHeaderTitleBranded";
                trContent.Cells.Add(tcContent);
                Maintable.Rows.Add(trContent);
            }

            trContent = new TableRow();
            tcContent = new TableCell();
            tcContent.Controls.Add(showFeeds(string.Empty));

            //Table Maintable = new Table();
            //TableRow trContent = new TableRow();
            //tcContent.Controls.Add(showFeeds(string.Empty));
            trContent.Controls.Add(tcContent);
            Maintable.Controls.Add(trContent);
            tcpaging.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
            tcpaging.Attributes.Add("style", "background-color: #EDEFF4;border: 1px solid #D8DFEA;");
            tcpaging.Height = 20;
            tcpaging.Controls.Add(lbtnNext);
            trpaging.Controls.Add(tcpaging);
            pagingTable.Controls.Add(trpaging);
            this.Controls.Add(Maintable);
            this.Controls.Add(pagingTable);
        }

        void lbtnNext_Click(object sender, EventArgs e)
        {
            tcContent.Controls.Clear();
            tcContent.Controls.Add(showFeeds(Convert.ToString(ViewState["next"])));
            if (string.IsNullOrEmpty(Convert.ToString(ViewState["next"])))
            {
                Literal ltrTxtMessage = new Literal();
                ltrTxtMessage.Text = "There are no more posts to show.";
                tcpaging.Controls.Clear();
                tcpaging.Controls.Add(ltrTxtMessage);
            }
        }

        private Table showFeeds(string FeedURL)
        {
            Table mainTable = null;
            int i = 0;

            TableRow tr;
            TableCell tc;
            TableCell tcImage;
            mainTable = new Table();
            mainTable.Width = Unit.Percentage(100);
            mainTable.CellSpacing = 0;
            mainTable.CellPadding = 0;
            mainTable.CssClass = "ms-viewlsts";
            int feedsCount = 0;

            JSONObject me = GetFeeds(FeedURL);

            if (me.Dictionary["data"] != null)
            {
                JSONObject[] feedsprev = new JSONObject[1];
                JSONObject[] feeds = me.Dictionary["data"].Array;
                feedsCount = feeds.Length;
                if (ViewState["html"] != null)
                {
                    feedsprev = (JSONObject[])ViewState["html"] as JSONObject[];
                }
                int mergedarraylength = 0;
                if (feeds != null)
                {
                    mergedarraylength = feeds.Length;
                }
                if (feedsprev[0] != null)
                {
                    mergedarraylength += feedsprev.Length;
                }
                JSONObject[] mergedFeeds = new JSONObject[mergedarraylength];

                if (feedsprev[0] != null)
                {
                    feedsprev.CopyTo(mergedFeeds, 0);
                }
                if (feeds != null)
                {
                    if (feedsprev[0] != null)
                    {
                        feeds.CopyTo(mergedFeeds, feedsprev.Length);
                    }
                    else
                    {
                        feeds.CopyTo(mergedFeeds, 0);
                    }
                }

                ViewState["html"] = mergedFeeds;
                foreach (JSONObject feed in mergedFeeds)
                {

                    tr = new TableRow();
                    tr.CssClass = "ms-WPBorderBorderOnly fbMainRow";

                    //if (i % 2 != 0)
                    //{
                    //    tr.CssClass = "ms-alternatingstrong";
                    //}
                    if (ShowUserImage)
                    {
                        tcImage = new TableCell();
                        //tcImage.CssClass = "ms-vb2";
                        Image image = new Image();
                        image.ImageUrl = string.Format("https://graph.facebook.com/{0}/picture", feed.Dictionary["from"].Dictionary["id"].String);
                        image.CssClass = "fbHeaderImage";
                        tcImage.Width = Unit.Percentage(5);
                        tcImage.Controls.Add(image);
                        tcImage.VerticalAlign = VerticalAlign.Middle;
                        tr.Cells.Add(tcImage);
                    }
                    tc = new TableCell();
                    tc.CssClass = "ms-vb2";
                    tc.Controls.Add(ParseFeed(feed, i));
                    tr.Cells.Add(tc);
                    mainTable.Rows.Add(tr);

                    i++;
                }
            }

            if (feedsCount < WallCount)
            {
                ViewState["next"] = "";
            }
            else
            {
                ViewState["next"] = me.Dictionary["paging"].Dictionary["next"].String;
            }

            return mainTable;
        }

        //private string RelativeTime(string pastTime)
        //{
        //    DateTime origStamp = DateTime.Parse(pastTime.ToString());
        //    DateTime curDate = DateTime.Now;

        //    TimeSpan ts = curDate.Subtract(origStamp);
        //    string strReturn = string.Empty;

        //    if (ts.Days >= 1)
        //    {
        //        strReturn = String.Format("{0:hh:mm tt MMM dd}" + "th", Convert.ToDateTime(pastTime).ToUniversalTime());
        //    }
        //    else
        //    {
        //        if (ts.Hours >= 1)
        //            strReturn = "about " + ts.Hours + " hours ago";
        //        else
        //        {
        //            if (ts.Minutes >= 1)
        //            {
        //                strReturn = "about " + ts.Minutes + " minutes ago";
        //            }
        //            else
        //                strReturn = "about " + ts.Seconds + " seconds ago";
        //        }
        //    }
        //    return strReturn;
        //}

        private string RelativeTime(string pastTime)
        {
            DateTime origStamp = DateTime.Parse(pastTime.ToString());
            DateTime curDate = DateTime.Now;

            TimeSpan ts = curDate.Subtract(origStamp);
            string strReturn = string.Empty;

            if (ts.Days > 365)               //years
            {
                if (ts.Days == 365)
                    strReturn = "about " + 1 + " year ago";
                else
                    strReturn = "about " + ts.Days / 365 + " years ago";
            }
            else if (ts.Days >= 30)         //months
            {
                if (ts.Days == 30)
                    strReturn = "about " + 1 + " month ago";
                else
                    strReturn = "about " + ts.Days / 30 + " months ago";
            }
            else if (ts.Days >= 7)           //weeks
            {
                if (ts.Days == 7)
                    strReturn = "about " + 1 + " week ago";
                else
                    strReturn = "about " + ts.Days / 7 + " weeks ago";
            }
            else if (ts.Days > 0)          //days
            {
                strReturn = "about " + ts.Days + " days ago";
            }
            else if (ts.Hours >= 1)          //hours
            {
                strReturn = "about " + ts.Hours + " hours ago";
            }
            else
            {
                if (ts.Minutes >= 1)
                {
                    strReturn = "about " + ts.Minutes + " minutes ago";
                }
                else
                    strReturn = "about " + ts.Seconds + " seconds ago";
            }
            return strReturn;

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        private JSONObject GetFeeds(string FeedURL)
        {
            JSONObject obj = null;
            string url;
            HttpWebRequest request;
            string oAuthToken = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(FeedURL))
                {

                    url = string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}&scope=read_stream", OAuthClientID, OAuthRedirectUrl, OAuthClientSecret, OAuthCode);

                    //get the server certificate for calling https 
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidateFacebookCertificate);
                    request = WebRequest.Create(url) as HttpWebRequest;

                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        string retVal = reader.ReadToEnd();
                        oAuthToken = retVal.Substring(retVal.IndexOf("=") + 1, retVal.Length - retVal.IndexOf("=") - 1);
                    }

                    if (!String.IsNullOrEmpty(oAuthToken))
                    {
                        //If User wants to see the Posts from the users wall
                        if (UserFeedType.Equals(FeedType.Users))
                        {
                            if (IsPosts)
                            {
                                //if we need to show the user feeds only then call posts rest api
                                url = string.Format("https://graph.facebook.com/{0}/posts?access_token={1}&limit={2}", this.UserID, oAuthToken, WallCount);
                            }
                            else
                            {
                                //else we need to call the feed rest api
                                url = string.Format("https://graph.facebook.com/{0}/feed?access_token={1}&limit={2}", this.UserID, oAuthToken, WallCount);
                            }
                        }
                        //If the user wants to the posts from the Group wall
                        else
                        {
                            url = string.Format("https://graph.facebook.com/{0}/feed?access_token={1}&limit={2}", this.UserID, oAuthToken, WallCount);
                        }
                    }
                    else
                    {
                        throw (new Exception("The access token returned was not valid."));
                    }
                }
                else
                {
                    //this is the url that we got for next feed url...no need to generate the url from scratch
                    url = FeedURL;
                }

                //now send the request to facebook
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

        private Table ParseFeed(JSONObject feed, int counter)
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
                    childTable.CellPadding = 2;
                    feedTableCell.Controls.Add(childTable);

                    if (feed.Dictionary.ContainsKey("picture"))
                    {
                        childRow = new TableRow();
                        childTable.Rows.Add(childRow);
                        Image image = new Image();
                        image.ImageUrl = feed.Dictionary["picture"].String;
                        image.Width = Unit.Pixel(90);
                        childCell = new TableCell();
                        childCell.RowSpan = 3;
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

                    if (feed.Dictionary.ContainsKey("description"))
                    {
                        //first cell and add table of status data
                        //next row
                        childRow = new TableRow();
                        childTable.Rows.Add(childRow);
                        childCell = new TableCell();
                        childCell.Text = feed.Dictionary["description"].String;
                        childCell.CssClass = "fbWallDescription";
                        childRow.Cells.Add(childCell);
                    }
                    break;
            }

            //second row in main feed table to display the additional information
            feedTableRow = new TableRow();
            feedTable.Rows.Add(feedTableRow);
            
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
                childCell.Text = RelativeTime(feed.Dictionary["created_time"].String.ToString());
                childCell.Style.Add("color", "Gray");
            }

            return feedTable;
        }

        #region Method to validate facebook certificate

        private static bool ValidateFacebookCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        #endregion
    }
}
