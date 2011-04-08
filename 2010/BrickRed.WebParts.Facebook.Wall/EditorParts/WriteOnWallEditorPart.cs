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

            LiteralControl lineBreak = new LiteralControl("<br/>");

            HtmlGenericControl paragraph; 

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
           

            //Show user name 
           
            pnlProperty = new Panel();
            pnlPropertyName = new Panel();
            pnlPropertyControl = new Panel();

            lblProperty = new Label();
            lblProperty.Text = "Show User Name:<br/><br/>";
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


           

            LiteralControl heading = new LiteralControl("<h5>Post Location Settings:</h3>");

            pnlFacebookSettings.Controls.Add(heading);

            LiteralControl hr = new LiteralControl("<hr style='color:black;height:1px'/>");

            pnlFacebookSettings.Controls.Add(hr);

           

            pnlFacebookSettings.Controls.Add(lineBreak);

            //Post on your wall/page wall radio buttons
            pnlProperty = new Panel();
          

            rdoBtnListPostLocation = new RadioButtonList();
            rdoBtnListPostLocation.Items.Add(new ListItem("Post on your wall", "postToYourWall"));
            rdoBtnListPostLocation.Items.Add(new ListItem("Post on your page's wall", "postToPageWall"));
            rdoBtnListPostLocation.Items.FindByValue("postToYourWall").Selected = true;
            rdoBtnListPostLocation.AutoPostBack = false;         
                     
            pnlProperty.Controls.Add(rdoBtnListPostLocation);

            pnlPostToWallSettings.Controls.Add(pnlProperty);


            
           
            //Post as this user/page radio buttons and page id text box
         

            pnlPropertyPostAsWhat = new HtmlGenericControl("div");
            pnlPropertyPostAsWhat.Attributes.Add("id", "postAsWhatDiv");

            pnlPropertyName = new Panel();
            pnlPropertyControl = new Panel();


            heading = new LiteralControl("<h5>Post on page's wall settings:</h3>");

            pnlPropertyPostAsWhat.Controls.Add(heading);

            hr = new LiteralControl("<hr style='color:black;height:1px'/>");

            pnlPropertyPostAsWhat.Controls.Add(hr);

            pnlPropertyPostAsWhat.Controls.Add(lineBreak);


            Label lblPageID = new Label();
            lblPageID.Text = "Page ID:<br/><br/>";
            pnlPropertyName.Controls.Add(lblPageID);
            pnlPropertyPostAsWhat.Controls.Add(pnlPropertyName);

            txtPageID = new TextBox();
            pnlPropertyControl.Controls.Add(txtPageID);
            pnlPropertyPostAsWhat.Controls.Add(pnlPropertyControl);

            rdoBtnListPostAsWhat = new RadioButtonList();
            rdoBtnListPostAsWhat.Items.Add(new ListItem("Post as this user", "postAsThisUser"));
            rdoBtnListPostAsWhat.Items.Add(new ListItem("Post as the page", "postAsPage"));
            rdoBtnListPostAsWhat.Items[0].Selected = true;
            rdoBtnListPostAsWhat.AutoPostBack = false;
            pnlPropertyPostAsWhat.Controls.Add(rdoBtnListPostAsWhat);

           
            pnlPropertyPostAsWhat.Attributes.Add("style", "display:none");

            pnlPostToWallSettings.Controls.Add(pnlPropertyPostAsWhat);
         
            pnlFacebookSettings.Controls.Add(pnlPostToWallSettings);

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
                    pnlPropertyPostAsWhat.Attributes["style"] = "display:none";
                }
                else
                {
                    webPart.PostOnProfile = false;                 
                    pnlPropertyPostAsWhat.Attributes["style"] = "display:block";
                }

                //setting facebook page id given by the user
                webPart.OAuthPageID = txtPageID.Text;

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
                                                               }
                                                            else
                                                              {
                                                                 document.getElementById(divid).style.display='none';   
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
