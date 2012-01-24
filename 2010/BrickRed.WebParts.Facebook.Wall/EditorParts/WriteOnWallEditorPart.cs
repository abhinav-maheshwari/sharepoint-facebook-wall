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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace BrickRed.WebParts.Facebook.Wall
{
    public class WriteOnWallEditorPart:EditorPart
    {
        private RadioButtonList rdoBtnListPostLocation;       
        private TextBox txtPageID;
        private RadioButtonList rdoBtnListPostAsWhat;       
        private TextBox txtAuthClientID;
        private TextBox txtAuthClientSecret;
        private TextBox txtAuthCode;
        private TextBox txtAuthRedirectUrl;
        private Panel pnlProperty;
        private Label lblProperty;
        private Panel pnlFacebookSettings;
        private Panel pnlCommonSettings;
        private Panel pnlPostToWallSettings;      
        private CheckBox chkShowUserName;
        private Panel pnlPropertyName;
        private Panel pnlPropertyControl;
        private HtmlGenericControl pnlPropertyPostAsWhat;
        private Panel seperatorDiv;
        private LiteralControl lineBreak;
        private HtmlGenericControl paragraph;

        private TextBox txtUserID;
        private CheckBox chkShowHeader;
        private CheckBox chkShowHeaderImage;

        public WriteOnWallEditorPart(string webPartID)
        {
            this.ID = "WriteOnWallEditorPart" + webPartID;
            this.Title = "Facebook Settings";

        }

        protected override void CreateChildControls() 
        { 
            base.CreateChildControls();
            pnlFacebookSettings = new Panel();
            pnlCommonSettings = new Panel();
            pnlPostToWallSettings = new Panel();
            lineBreak = new LiteralControl("<br/>");
            seperatorDiv = new Panel();
            seperatorDiv.Attributes.Add("class", "UserDottedLine");
            seperatorDiv.Attributes.Add("style", "width: 100%;");
            
            //Code
            pnlProperty = new Panel();           
            pnlPropertyName = new Panel();
            pnlPropertyControl = new Panel();

            lblProperty = new Label();
            lblProperty.Text = "Code:<br/><br/>";

            pnlPropertyName.Controls.Add(lblProperty);
            pnlProperty.Controls.Add(pnlPropertyName);
           
            txtAuthCode = new TextBox();
            txtAuthCode.Width = Unit.Percentage(100);
            pnlPropertyControl.Controls.Add(txtAuthCode);
            pnlProperty.Controls.Add(pnlPropertyControl);

            paragraph = new HtmlGenericControl("p");
            paragraph.Controls.Add(pnlProperty);
            pnlCommonSettings.Controls.Add(paragraph);
           
            //Client ID
            pnlProperty = new Panel();
            pnlPropertyName = new Panel();
            pnlPropertyControl = new Panel();
            
            lblProperty = new Label();
            lblProperty.Text = "Client ID:<br/><br/>";
            pnlPropertyName.Controls.Add(lblProperty);
            pnlProperty.Controls.Add(pnlPropertyName);
          
            txtAuthClientID = new TextBox();
            txtAuthClientID.Width = Unit.Percentage(100);
            pnlPropertyControl.Controls.Add(txtAuthClientID);
            pnlProperty.Controls.Add(pnlPropertyControl);

            paragraph = new HtmlGenericControl("p");
            paragraph.Controls.Add(pnlProperty);
            pnlCommonSettings.Controls.Add(paragraph);

            //Redirect Url
            pnlProperty = new Panel();
            pnlPropertyName = new Panel();
            pnlPropertyControl = new Panel();


            lblProperty = new Label();
            lblProperty.Text = "Redirect Url:<br/><br/>";
            pnlPropertyName.Controls.Add(lblProperty);
            pnlProperty.Controls.Add(pnlPropertyName);
           
            txtAuthRedirectUrl = new TextBox();
            txtAuthRedirectUrl.Width = Unit.Percentage(100);
            pnlPropertyControl.Controls.Add(txtAuthRedirectUrl);
            pnlProperty.Controls.Add(pnlPropertyControl);

            paragraph = new HtmlGenericControl("p");
            paragraph.Controls.Add(pnlProperty);
            pnlCommonSettings.Controls.Add(paragraph);
           

            //Client Secret
            pnlProperty = new Panel();
            pnlPropertyName = new Panel();
            pnlPropertyControl = new Panel();

            lblProperty = new Label();
            lblProperty.Text = "Client Secret:<br/><br/>";
            pnlPropertyName.Controls.Add(lblProperty);
            pnlProperty.Controls.Add(pnlPropertyName);
           
            txtAuthClientSecret = new TextBox();
            txtAuthClientSecret.Width = Unit.Percentage(100);
            pnlPropertyControl.Controls.Add(txtAuthClientSecret);
            pnlProperty.Controls.Add(pnlPropertyControl);

            paragraph = new HtmlGenericControl("p");
            paragraph.Controls.Add(pnlProperty);
            pnlCommonSettings.Controls.Add(paragraph);


            //User ID
            pnlProperty = new Panel();
            pnlPropertyName = new Panel();
            pnlPropertyControl = new Panel();

            lblProperty = new Label();
            lblProperty.Text = "User Id / User Name / Page Id:<br/><br/>";
            pnlPropertyName.Controls.Add(lblProperty);
            pnlProperty.Controls.Add(pnlPropertyName);

            txtUserID = new TextBox();
            txtUserID.Width = Unit.Percentage(100);
            pnlPropertyControl.Controls.Add(txtUserID);
            pnlProperty.Controls.Add(pnlPropertyControl);

            paragraph = new HtmlGenericControl("p");
            paragraph.Controls.Add(pnlProperty);
            pnlCommonSettings.Controls.Add(paragraph);

            //Show Header 
            pnlProperty = new Panel();
            pnlPropertyName = new Panel();
            pnlPropertyControl = new Panel();

            lblProperty = new Label();
            lblProperty.Text = "Show header:";
            pnlPropertyName.Controls.Add(lblProperty);
            pnlProperty.Controls.Add(pnlPropertyName);

            chkShowHeader = new CheckBox();
            chkShowHeader.Checked = false;
            chkShowHeader.AutoPostBack = false;
            pnlPropertyControl.Controls.Add(chkShowHeader);
            pnlProperty.Controls.Add(pnlPropertyControl);

            paragraph = new HtmlGenericControl("p");
            paragraph.Controls.Add(pnlProperty);
            pnlCommonSettings.Controls.Add(paragraph);

            //Show Header Image 
            pnlProperty = new Panel();
            pnlPropertyName = new Panel();
            pnlPropertyControl = new Panel();

            lblProperty = new Label();
            lblProperty.Text = "Show header image:";
            pnlPropertyName.Controls.Add(lblProperty);
            pnlProperty.Controls.Add(pnlPropertyName);

            chkShowHeaderImage = new CheckBox();
            chkShowHeaderImage.Checked = false;
            chkShowHeaderImage.AutoPostBack = false;
            pnlPropertyControl.Controls.Add(chkShowHeaderImage);
            pnlProperty.Controls.Add(pnlPropertyControl);

            paragraph = new HtmlGenericControl("p");
            paragraph.Controls.Add(pnlProperty);
            pnlCommonSettings.Controls.Add(paragraph);

            //Show user name 
           
            pnlProperty = new Panel();
            pnlPropertyName = new Panel();
            pnlPropertyControl = new Panel();

            lblProperty = new Label();
            lblProperty.Text = "Would you like to show current user id?";
            pnlPropertyName.Controls.Add(lblProperty);
            pnlProperty.Controls.Add(pnlPropertyName);
            
            chkShowUserName = new CheckBox();
            chkShowUserName.Checked = false;
            chkShowUserName.AutoPostBack = false;
            pnlPropertyControl.Controls.Add(chkShowUserName);
            pnlProperty.Controls.Add(pnlPropertyControl);
           

            //check for anonymous access, if it is enabled, don't show current user name as it will be null
            if (SPContext.Current.Web.CurrentUser == null)
            {
                lblProperty.Visible = false;
                chkShowUserName.Visible = false;
            }

            paragraph = new HtmlGenericControl("p");
            paragraph.Controls.Add(pnlProperty);
            pnlCommonSettings.Controls.Add(paragraph);           
            pnlFacebookSettings.Controls.Add(pnlCommonSettings);

            seperatorDiv = new Panel();
            seperatorDiv.Attributes.Add("class", "UserDottedLine");
            seperatorDiv.Attributes.Add("style", "width: 100%;");
            pnlFacebookSettings.Controls.Add(seperatorDiv);
         
            //Post on your wall/page wall radio buttons
                      
            pnlProperty = new Panel();
            pnlPropertyName = new Panel();
            pnlPropertyControl = new Panel();

            LiteralControl heading = new LiteralControl("Where would you like to post?");
            pnlProperty.Controls.Add(heading);
                      
            rdoBtnListPostLocation = new RadioButtonList();
            rdoBtnListPostLocation.Items.Add(new ListItem("On my wall", "postToYourWall"));
            rdoBtnListPostLocation.Items.Add(new ListItem("On my page", "postToPageWall"));
            rdoBtnListPostLocation.Items.Add(new ListItem("On my group", "postToGroupWall"));
            rdoBtnListPostLocation.Items.FindByValue("postToYourWall").Selected = true;
            rdoBtnListPostLocation.AutoPostBack = false;
            pnlPropertyControl.Controls.Add(rdoBtnListPostLocation);
            pnlProperty.Controls.Add(pnlPropertyControl);

            pnlPostToWallSettings.Controls.Add(pnlProperty);

           
            //Post as this user/page radio buttons and page id text box

            pnlPropertyPostAsWhat = new HtmlGenericControl("div");
            pnlPropertyPostAsWhat.Attributes.Add("id", "postAsWhatDiv");

            seperatorDiv = new Panel();
            seperatorDiv.Attributes.Add("class", "UserDottedLine");
            seperatorDiv.Attributes.Add("style", "width: 100%;");

            pnlPropertyPostAsWhat.Controls.Add(seperatorDiv);
       
            pnlPropertyName = new Panel();
            pnlPropertyControl = new Panel();

            Label lblPageID = new Label();
            lblPageID.Text = "Page ID / Group ID:<br/><br/>";
            pnlPropertyName.Controls.Add(lblPageID);
            pnlPropertyPostAsWhat.Controls.Add(pnlPropertyName);

            txtPageID = new TextBox();
            pnlPropertyControl.Controls.Add(txtPageID);
            pnlPropertyPostAsWhat.Controls.Add(pnlPropertyControl);
          
            pnlPropertyPostAsWhat.Controls.Add(lineBreak);

            heading = new LiteralControl("Who should post the messages?");
            pnlPropertyPostAsWhat.Controls.Add(heading);
                    
            rdoBtnListPostAsWhat = new RadioButtonList();
            rdoBtnListPostAsWhat.Items.Add(new ListItem("Current User", "postAsThisUser"));
            rdoBtnListPostAsWhat.Items.Add(new ListItem("Current Page", "postAsPage"));
            rdoBtnListPostAsWhat.Items[0].Selected = true;
            rdoBtnListPostAsWhat.Items.FindByValue("postAsPage").Attributes.Add("ID", "rdoPostAsPage");
            rdoBtnListPostAsWhat.AutoPostBack = false;
            pnlPropertyPostAsWhat.Controls.Add(rdoBtnListPostAsWhat);

            seperatorDiv = new Panel();
            seperatorDiv.Attributes.Add("class", "UserDottedLine");
            seperatorDiv.Attributes.Add("style", "width: 100%;");

            pnlPropertyPostAsWhat.Controls.Add(seperatorDiv);

            pnlPropertyPostAsWhat.Attributes.Add("style", "display:none;");
         
            pnlFacebookSettings.Controls.Add(pnlPostToWallSettings);
         
            pnlFacebookSettings.Controls.Add(pnlPropertyPostAsWhat);
         
            this.Controls.Add(pnlFacebookSettings);

        }

        public override bool ApplyChanges() 
        { 
            EnsureChildControls();

            WriteOnWall webPart = WebPartToEdit as WriteOnWall; 

            if (webPart != null) 
            { 
                webPart.OAuthCode = txtAuthCode.Text;
                webPart.OAuthClientID = txtAuthClientID.Text;
                webPart.OAuthClientSecret = txtAuthClientSecret.Text;
                webPart.OAuthRedirectUrl = txtAuthRedirectUrl.Text;
                webPart.UserID = txtUserID.Text.Trim();
                webPart.ShowHeader = chkShowHeader.Checked ? true : false;
                webPart.ShowHeaderImage = chkShowHeaderImage.Checked ? true : false;

                //checking for sites with anonymous access and setting show user name property
                if (chkShowUserName.Visible)
                {
                    webPart.EnableShowUserName = chkShowUserName.Checked;
                }
                else
                {
                    webPart.EnableShowUserName = false;
                }

                //check if posting to your wall or page's wall
                if (rdoBtnListPostLocation.SelectedItem.Value.Equals("postToYourWall"))
                {
                    webPart.PostOnProfile = true;
                    webPart.PostOnGroupWall = false;
                    pnlPropertyPostAsWhat.Attributes["style"] = "display:none";
                }
                else if (rdoBtnListPostLocation.SelectedItem.Value.Equals("postToGroupWall"))
                {
                    webPart.PostOnProfile = false;
                    webPart.PostOnGroupWall = true;
                    pnlPropertyPostAsWhat.Attributes["style"] = "display:block";
                }
                else
                {
                    webPart.PostOnProfile = false;
                    pnlPropertyPostAsWhat.Attributes["style"] = "display:block";
                }

                //setting facebook page id given by the user
                webPart.OAuthPageID = txtPageID.Text;

                //Post as Current User if the post is to be posted at the Groups wall
                if (!webPart.PostOnGroupWall)
                {
                    //check if posting as this user or as page
                    if (rdoBtnListPostAsWhat.SelectedItem.Value.Equals("postAsPage"))
                    {
                        webPart.PostAsPage = true;
                    }
                    else
                    {
                        webPart.PostAsPage = false;
                    }
                }
                else
                {
                    webPart.PostAsPage = false;
                }
            }
 
            return true; 
        }         
        
        public override void SyncChanges() 
        { 
            EnsureChildControls();
            WriteOnWall webPart = WebPartToEdit as WriteOnWall;
                if (webPart != null)
                {
                    txtAuthCode.Text = webPart.OAuthCode;
                    txtAuthClientID.Text = webPart.OAuthClientID;
                    txtAuthClientSecret.Text = webPart.OAuthClientSecret;
                    txtAuthRedirectUrl.Text = webPart.OAuthRedirectUrl;

                    if (chkShowUserName.Visible)
                    {
                        chkShowUserName.Checked = webPart.EnableShowUserName;

                    }

                    if (webPart.PostOnProfile)
                    {
                        rdoBtnListPostLocation.Items.FindByValue("postToYourWall").Selected = true;
                        pnlPropertyPostAsWhat.Attributes["style"] = "display:none";
                    }
                    else if (webPart.PostOnGroupWall)
                    {
                        rdoBtnListPostLocation.Items.FindByValue("postToGroupWall").Selected = true;
                        pnlPropertyPostAsWhat.Attributes["style"] = "display:block";
                    }
                    else
                    {
                        rdoBtnListPostLocation.Items.FindByValue("postToPageWall").Selected = true;
                        pnlPropertyPostAsWhat.Attributes["style"] = "display:block";
                    }

                    txtPageID.Text = webPart.OAuthPageID;

                    if (webPart.PostAsPage)
                    {
                        rdoBtnListPostAsWhat.Items.FindByValue("postAsPage").Selected = true;
                    }
                    else
                    {
                        rdoBtnListPostAsWhat.Items.FindByValue("postAsThisUser").Selected = true;
                    }
               }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            string scriptShowHidePostAsWhatDiv = @"<script language='javascript' type='text/javascript'>
                                                    // Show hide post as what div based on selected value
                                                     function ShowHidePostAsWhatDiv(radiobuttonlistitemvalue,divid)
                                                        {                                                           
                                                           if(radiobuttonlistitemvalue=='postToPageWall')
                                                              {
                                                                 document.getElementById(divid).style.display='block';
                                                                 document.getElementById('rdoPostAsPage').style.display='block';
                                                               }
                                                            else if(radiobuttonlistitemvalue=='postToYourWall')
                                                              {
                                                                 document.getElementById(divid).style.display='none';   
                                                              }
                                                            else
                                                              {
                                                                 document.getElementById(divid).style.display='block';                                                                 
                                                                 document.getElementById('rdoPostAsPage').style.display='none';
                                                              }
                                                             
                                                        }
                                                    </script>";

            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "scriptShowHidePostAsWhatDiv", scriptShowHidePostAsWhatDiv);
            foreach (ListItem item in rdoBtnListPostLocation.Items)
            {
                item.Attributes.Add("onclick", "ShowHidePostAsWhatDiv('" + item.Value + "','postAsWhatDiv');");
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter output) 
        {
            base.Render(output);
        } 
    }
}
