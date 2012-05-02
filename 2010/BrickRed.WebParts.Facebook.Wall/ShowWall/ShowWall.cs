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
using System.Net.Security;
using System.Collections.Generic;
using System.Drawing;
using System.Text;


namespace BrickRed.WebParts.Facebook.Wall
{
    [ToolboxItemAttribute(false)]
    public class ShowWall : Microsoft.SharePoint.WebPartPages.WebPart
    {
        #region Public Controls
        Label LblMessage;
        string oAuthToken;
        LinkButton lbtnNext = new LinkButton();
        TableCell tcContent = new TableCell();
        TableCell tcpaging = new TableCell();
        string ImagePath = SPContext.Current.Web.Url + "/_layouts/BrickRed.WebParts.Facebook.Wall/Images/";
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

        [WebBrowsable(true),
        Category("Facebook Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        DefaultValue(""),
        WebDisplayName("User Id / User Name / Page Id"),
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
        DefaultValue(false),
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
                    //first get the authentication token 
                    oAuthToken = CommonHelper.GetOAuthToken("read_stream", OAuthClientID, OAuthRedirectUrl, OAuthClientSecret, OAuthCode);

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
            UpdatePanel refreshName = new UpdatePanel();
            ScriptManager scriptHandler = new ScriptManager();
            UpdateProgress updateProgress = new UpdateProgress();
            Table pagingTable;
            TableRow trpaging = new TableRow();

            pagingTable = new Table();
            pagingTable.ID = "pagingTable";
            pagingTable.Width = Unit.Percentage(100);
            pagingTable.CellSpacing = 0;
            pagingTable.CellPadding = 0;
            pagingTable.CssClass = "fbPagingTable";
            lbtnNext.Text = "Older Posts";
            lbtnNext.ID = "lbtnNext";
            lbtnNext.Click += new EventHandler(lbtnNext_Click);
            Table Maintable = new Table();
            TableRow trContent;
            Maintable.CssClass = "fbMainTable";
            Maintable.CellPadding = 0;
            Maintable.CellSpacing = 0;

            //Create the header
            if (this.ShowHeader)
            {
                trContent = new TableRow();
                tcContent = new TableCell();
                tcContent.Controls.Add(CommonHelper.CreateHeader(this.UserID, this.oAuthToken, this.ShowHeaderImage));
                tcContent.CssClass = "fbHeaderTitleBranded";
                trContent.Cells.Add(tcContent);
                Maintable.Rows.Add(trContent);
            }

            trContent = new TableRow();
            tcContent = new TableCell();

            //get the feeds here 
            tcContent.Controls.Add(ShowFeeds(string.Empty));

            trContent.Controls.Add(tcContent);
            Maintable.Controls.Add(trContent);
            tcpaging.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
            tcpaging.ID = "tcpaging";

            tcpaging.Controls.Add(lbtnNext);
            trpaging.Controls.Add(tcpaging);

            pagingTable.Controls.Add(trpaging);
            scriptHandler.ID = "scriptHandler";
            refreshName.ID = "refreshName";
            refreshName.UpdateMode = UpdatePanelUpdateMode.Conditional;
            refreshName.ChildrenAsTriggers = true;
            updateProgress.AssociatedUpdatePanelID = "refreshName";

            updateProgress.ProgressTemplate = new ProgressTemplate();
            refreshName.ContentTemplateContainer.Controls.Add(Maintable);
            refreshName.ContentTemplateContainer.Controls.Add(pagingTable);

            if (ScriptManager.GetCurrent(this.Page) == null)
            {
                this.Controls.Add(scriptHandler);
            }
            this.Controls.Add(refreshName);
            this.Controls.Add(updateProgress);
            lbtnNext.OnClientClick = pagingTable.ClientID + ".style.visibility='hidden';";
        }

        void lbtnNext_Click(object sender, EventArgs e)
        {
            tcContent.Controls.Clear();
            tcContent.Controls.Add(ShowFeeds(Convert.ToString(ViewState["next"])));
            if (string.IsNullOrEmpty(Convert.ToString(ViewState["next"])))
            {
                Literal ltrTxtMessage = new Literal();
                ltrTxtMessage.Text = "There are no more posts to show.";
                tcpaging.Controls.Clear();
                tcpaging.Controls.Add(ltrTxtMessage);
            }
        }

        private Table ShowFeeds(string FeedURL)
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
                    tr.CssClass = "fbMainRow";

                    if (ShowUserImage)
                    {
                        tcImage = new TableCell();
                        System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
                        image.ImageUrl = string.Format("https://graph.facebook.com/{0}/picture", feed.Dictionary["from"].Dictionary["id"].String);
                        image.CssClass = "fbHeaderImage";
                        tcImage.Width = Unit.Percentage(5);
                        tcImage.Controls.Add(image);
                        tcImage.VerticalAlign = VerticalAlign.Middle;
                        tr.Cells.Add(tcImage);
                    }
                    tc = new TableCell();
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
            try
            {
                if (string.IsNullOrEmpty(FeedURL))
                {
                    if (!String.IsNullOrEmpty(oAuthToken))
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
                        childTable.CellPadding = 2;
                        childRow = new TableRow();
                        childTable.Rows.Add(childRow);
                        childCell = new TableCell();
                        childCell.Text = feed.Dictionary["message"].String;
                        childRow.Cells.Add(childCell);
                        feedTableCell.Controls.Add(childTable);
                    }
                    if (feed.Dictionary.ContainsKey("story"))
                    {
                        //first cell and add table of status data
                        feedTableCell = new TableCell();
                        feedTableRow.Cells.Add(feedTableCell);

                        childTable = new Table();
                        childTable.CellPadding = 2;
                        childRow = new TableRow();
                        childTable.Rows.Add(childRow);
                        childCell = new TableCell();
                        childCell.Text = feed.Dictionary["story"].String;
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
                        System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
                        image.ImageUrl = feed.Dictionary["picture"].String;
                        image.Width = Unit.Pixel(90);
                        childCell = new TableCell();
                        childCell.RowSpan = 4;
                        childRow.Cells.Add(childCell);
                        childCell.Controls.Add(image);
                    }

                    if (feed.Dictionary.ContainsKey("message"))
                    {
                        //next row
                        childRow = new TableRow();
                        childTable.Rows.Add(childRow);
                        childCell = new TableCell();
                        childRow.Cells.Add(childCell);

                        //Remove the link from the message
                        string message = feed.Dictionary["message"].String;
                        if(message.ToLower().Contains("http"))
                        {
                            message = message.Remove( message.IndexOf("http"), feed.Dictionary["link"].String.Length);
                        }
                        childCell.Text = message;
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
            childTable.CellPadding = 2;
            childRow = new TableRow();
            childTable.Rows.Add(childRow);
            feedTableCell.Controls.Add(childTable);

            if (feed.Dictionary.ContainsKey("icon"))
            {
                System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
                image.ImageUrl = feed.Dictionary["icon"].String;

                childCell = new TableCell();
                childRow.Cells.Add(childCell);
                childCell.Controls.Add(image);

            }

            //Show Likes
            if (feed.Dictionary.ContainsKey("likes"))
            {
                childCell = new TableCell();
                childRow.Cells.Add(childCell);
                System.Web.UI.WebControls.Image img_Like = new System.Web.UI.WebControls.Image();
                img_Like.ImageUrl = ImagePath + "Likes.png";
                img_Like.CssClass = "fbLikes";

                childCell.Controls.Add(img_Like);

                //Showing the number of likes
                childCell = new TableCell();
                childRow.Cells.Add(childCell);
                Label lbl_Likes = new Label();
                lbl_Likes.Text = feed.Dictionary["likes"].Dictionary["count"].String + " people";
                lbl_Likes.CssClass = "fbLikes";

                // get the story id
                string[] fbinfo = feed.Dictionary["id"].String.Split('_');
                lbl_Likes.Attributes.Add("onClick", "javascript:window.open('https://www.facebook.com/" + this.UserID + "/posts/" + fbinfo[1] + "','_newtab');");
                img_Like.Attributes.Add("onClick", "javascript:window.open('https://www.facebook.com/" + this.UserID + "/posts/" + fbinfo[1] + "','_newtab');");

                Label lbl_likeThis = new Label();
                lbl_likeThis.Text = " like this.";

                childCell.Controls.Add(lbl_Likes);
                childCell.Controls.Add(lbl_likeThis);
            }
            else        // Show only like image
            {
                childCell = new TableCell();
                childRow.Cells.Add(childCell);
                System.Web.UI.WebControls.Image img_Like = new System.Web.UI.WebControls.Image();
                img_Like.ImageUrl = ImagePath + "Likes.png";
                img_Like.CssClass = "fbLikes";
                string[] fbinfo = feed.Dictionary["id"].String.Split('_');
                img_Like.Attributes.Add("onClick", "javascript:window.open('https://www.facebook.com/" + this.UserID + "/posts/" + fbinfo[1] + "','_newtab');");

                childCell.Controls.Add(img_Like);
            }

            //show Comments
            if (feed.Dictionary.ContainsKey("comments"))
            {
                childCell = new TableCell();
                childRow.Cells.Add(childCell);
                Label lbl_Comment = new Label();
                lbl_Comment.Text = "Comment";
                lbl_Comment.CssClass = "fbLikes";

                // get the story id
                string[] fbinfo = feed.Dictionary["id"].String.Split('_');
                lbl_Comment.Attributes.Add("onClick", "javascript:window.open('https://www.facebook.com/" + this.UserID + "/posts/" + fbinfo[1] + "','_newtab');");

                childCell.Controls.Add(lbl_Comment);
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

    }
}
