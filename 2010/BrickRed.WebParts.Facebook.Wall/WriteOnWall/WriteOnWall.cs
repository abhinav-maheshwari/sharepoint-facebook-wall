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
using System.Net;
using System.IO;
using Microsoft.SharePoint.WebPartPages;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace BrickRed.WebParts.Facebook.Wall
{
    [ToolboxItemAttribute(false)]
    public class WriteOnWall : Microsoft.SharePoint.WebPartPages.WebPart
    {
        Label LblMessage;
        TextBox textWall;

        #region Webpart Properties
        [WebBrowsable(true),
       Category("Facebook Settings"),
       Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
       DefaultValue(""),
       WebDisplayName("Code"),
       WebDescription("Please enter code")]

        public string OAuthCode { get; set; }

        [WebBrowsable(true),
       Category("Facebook Settings"),
       Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
       DefaultValue(""),
       WebDisplayName("Client ID"),
       WebDescription("Please enter client id")]

        public string OAuthClientID { get; set; }

        [WebBrowsable(true),
       Category("Facebook Settings"),
       Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
       DefaultValue(""),
       WebDisplayName("Redirect Url"),
       WebDescription("Please enter redirect url")]

        public string OAuthRedirectUrl { get; set; }

        [WebBrowsable(true),
       Category("Facebook Settings"),
       Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
       DefaultValue(""),
       WebDisplayName("Client Secret"),
       WebDescription("Please enter client secret id")]

        public string OAuthClientSecret { get; set; }


        [WebBrowsable(true),
        Category("Facebook Settings"),
        Personalizable(PersonalizationScope.Shared),
        WebPartStorage(Storage.Shared),
        WebDisplayName("Show User Name"),
        WebDescription("Would you like to show user name")]

        public bool EnableShowUserName { get; set; }

        #endregion

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            try
            {
                Table mainTable;
                TableRow tr;
                TableCell tc;

                Button buttonWriteOnWall;

                mainTable = new Table();
                mainTable.Width = Unit.Percentage(100);
                mainTable.CellSpacing = 5;
                mainTable.CellPadding = 0;

                this.Controls.Add(mainTable);

                tr = new TableRow();
                mainTable.Rows.Add(tr);
                tc = new TableCell();
                tc.Width = Unit.Percentage(30);
                tr.Cells.Add(tc);
                textWall = new TextBox();
                textWall.TextMode = TextBoxMode.MultiLine;
                textWall.MaxLength = 140;
                textWall.Width = Unit.Percentage(100);
                textWall.Height = Unit.Pixel(100);
                tc.Controls.Add(textWall);

                tr = new TableRow();
                mainTable.Rows.Add(tr);
                tc = new TableCell();
                tc.HorizontalAlign = HorizontalAlign.Right;
                tc.Width = Unit.Percentage(30);
                tr.Cells.Add(tc);
                buttonWriteOnWall = new Button();
                buttonWriteOnWall.ForeColor = Color.White;
                buttonWriteOnWall.BackColor = Color.FromArgb(84, 116, 186);
                buttonWriteOnWall.Text = "Share";
                buttonWriteOnWall.Click += new EventHandler(buttonWriteOnWall_Click);
                tc.Controls.Add(buttonWriteOnWall);

            }
            catch (Exception Ex)
            {
                LblMessage = new Label();
                LblMessage.Text = Ex.Message;
                this.Controls.Add(LblMessage);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            textWall.Text = "";
            if (this.EnableShowUserName)
                textWall.Text = SPContext.Current.Web.CurrentUser.Name + " : ";
        }

        void buttonWriteOnWall_Click(object sender, EventArgs e)
        {
            try
            {

                string oAuthToken;

                string url = string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}&scope=publish_stream", OAuthClientID, OAuthRedirectUrl, OAuthClientSecret, OAuthCode);

                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidateFacebookCertificate);

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string retVal = reader.ReadToEnd();
                    oAuthToken = retVal.Substring(retVal.IndexOf("=") + 1, retVal.Length - retVal.IndexOf("=") - 1);
                }

                url = string.Format("https://graph.facebook.com/me/feed?access_token={0}&message={1}", oAuthToken, textWall.Text.Trim());

                HttpWebRequest request2 = WebRequest.Create(url) as HttpWebRequest;
                request2.Method = "post";
                using (HttpWebResponse response2 = request2.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response2.GetResponseStream());
                    string retVal = reader.ReadToEnd();
                }

            }
            catch (Exception Ex)
            {
                LblMessage = new Label();
                LblMessage.Text = Ex.Message;
                this.Controls.Add(LblMessage);
            }
        }

        private static bool ValidateFacebookCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

    }
}
