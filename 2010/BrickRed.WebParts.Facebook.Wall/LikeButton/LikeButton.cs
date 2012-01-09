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
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace BrickRed.WebParts.Facebook.Wall.LikeButton
{
    [ToolboxItemAttribute(false)]
    public class LikeButton : WebPart
    {
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
        }

        [WebBrowsable(true),
         Category("Facebook Settings"),
         Personalizable(PersonalizationScope.Shared),
         Microsoft.SharePoint.WebPartPages.WebPartStorage(Microsoft.SharePoint.WebPartPages.Storage.Shared),
         DefaultValue(""),
         WebDisplayName("URL to Like"),
         WebDescription("Please enter URL to Like")]
        public string PageURL { get; set; }


        [WebBrowsable(true),
        Category("Facebook Settings"),
        Personalizable(PersonalizationScope.Shared),
        Microsoft.SharePoint.WebPartPages.WebPartStorage(Microsoft.SharePoint.WebPartPages.Storage.Shared),
        DefaultValue(""),
        WebDisplayName("Send Button (XFBML Only)"),
        WebDescription("Please specify Send Button (XFBML Only)")]
        public bool SendButton { get; set; }

        [WebBrowsable(true),
         Category("Facebook Settings"),
         Personalizable(PersonalizationScope.Shared),
         Microsoft.SharePoint.WebPartPages.WebPartStorage(Microsoft.SharePoint.WebPartPages.Storage.Shared),
         DefaultValue("standard"),
         WebDisplayName("Layout Style"),
         WebDescription("Please choose Layout Style")]
        public BrickRed.WebParts.Facebook.Wall.Enumerators.LayoutStyleEnum LayoutStyle { get; set; }

        [WebBrowsable(true),
         Category("Facebook Settings"),
         Personalizable(PersonalizationScope.Shared),
         Microsoft.SharePoint.WebPartPages.WebPartStorage(Microsoft.SharePoint.WebPartPages.Storage.Shared),
         DefaultValue(""),
         WebDisplayName("Show Faces"),
         WebDescription("Please specify Show Faces or not?")]
        public bool ShowFaces { get; set; }

        [WebBrowsable(true),
         Category("Facebook Settings"),
         Personalizable(PersonalizationScope.Shared),
         Microsoft.SharePoint.WebPartPages.WebPartStorage(Microsoft.SharePoint.WebPartPages.Storage.Shared),
         DefaultValue("like"),
         WebDisplayName("Verb to display"),
         WebDescription("Please choose Verb To Display")]
        public BrickRed.WebParts.Facebook.Wall.Enumerators.VerbToDisplayEnum VerbToDisplay { get; set; }

        [WebBrowsable(true),
         Category("Facebook Settings"),
         Personalizable(PersonalizationScope.Shared),
         Microsoft.SharePoint.WebPartPages.WebPartStorage(Microsoft.SharePoint.WebPartPages.Storage.Shared),
         DefaultValue("light"),
         WebDisplayName("Color Scheme"),
         WebDescription("Please choose Color Scheme")]
        public BrickRed.WebParts.Facebook.Wall.Enumerators.ColorSchemeEnum ColorScheme { get; set; }

        [WebBrowsable(true),
         Category("Facebook Settings"),
         Personalizable(PersonalizationScope.Shared),
         Microsoft.SharePoint.WebPartPages.WebPartStorage(Microsoft.SharePoint.WebPartPages.Storage.Shared),
         DefaultValue(""),
         WebDisplayName("LikeButton Font"),
         WebDescription("Please choose Font")]
        public BrickRed.WebParts.Facebook.Wall.Enumerators.FontsEnum LikeButtonFont { get; set; }
        
        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            base.RenderContents(writer);
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(@"<div id='fb-root'></div>");
            strBuilder.Append(@"
                            <script>
                                    (function(d, s, id) {
                                        var js, fjs = d.getElementsByTagName(s)[0];
                                        if (d.getElementById(id)) return;
                                        js = d.createElement(s); js.id = id;
                                        js.src = '//connect.facebook.net/en_US/all.js#xfbml=1';
                                        fjs.parentNode.insertBefore(js, fjs);
                                    } (document, 'script', 'facebook-jssdk'));
                                </script>
                            ");
            string width = string.Empty;
            string cssClass = "fb-like";

            if (Width.Value > 0.0)
                width = "data-width='" + Width.Value + "'";

            if (ColorScheme == BrickRed.WebParts.Facebook.Wall.Enumerators.ColorSchemeEnum.dark)
                cssClass = "fb-like dark_background";

            string font = StringValueOf(LikeButtonFont).Replace("_", " ");

            strBuilder.Append(@"<div class='" + cssClass + "' data-href='" + PageURL + "' data-send='" + SendButton + "' data-layout='" + LayoutStyle + "' " + width + " data-show-faces='" + ShowFaces + "' data-action='" + VerbToDisplay + "' data-colorscheme='" + ColorScheme + "' data-font='" + font + "'></div>");
            writer.Write(strBuilder);
        }

        public string StringValueOf(Enum value)
        {
            string returnString = string.Empty;
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi != null)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    returnString = attributes[0].Description;
                }
                else
                {
                    returnString = value.ToString();
                }
            }
            return returnString;
        }
    }
}
