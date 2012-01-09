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
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Reflection;
using System.ComponentModel;
using System.Text;

namespace BrickRed.WebParts.Facebook.Wall
{
    [ToolboxItemAttribute(false)]
    public class LikeBox : System.Web.UI.WebControls.WebParts.WebPart
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
         WebDisplayName("Facebook Page URL"),
         WebDescription("Please enter Facebook page url")]
        public string PageURL { get; set; }

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
         WebDisplayName("Show Faces"),
         WebDescription("Please specify Show Faces or not?")]
        public bool ShowFaces { get; set; }

        [WebBrowsable(true),
         Category("Facebook Settings"),
         Personalizable(PersonalizationScope.Shared),
         Microsoft.SharePoint.WebPartPages.WebPartStorage(Microsoft.SharePoint.WebPartPages.Storage.Shared),
         DefaultValue(""),
         WebDisplayName("LikeBox Border Color"),
         WebDescription("Please specify Border Color")]
        public string LikeBoxBorderColor { get; set; }

        [WebBrowsable(true),
         Category("Facebook Settings"),
         Personalizable(PersonalizationScope.Shared),
         Microsoft.SharePoint.WebPartPages.WebPartStorage(Microsoft.SharePoint.WebPartPages.Storage.Shared),
         DefaultValue(""),
         WebDisplayName("Show stream"),
         WebDescription("Please specify Show stream or not?")]
        public bool ShowStream { get; set; }

        [WebBrowsable(true),
         Category("Facebook Settings"),
         Personalizable(PersonalizationScope.Shared),
         Microsoft.SharePoint.WebPartPages.WebPartStorage(Microsoft.SharePoint.WebPartPages.Storage.Shared),
         DefaultValue(""),
         WebDisplayName("Show header"),
         WebDescription("Please specify Show header or not?")]
        public bool ShowHeader { get; set; }

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
            string height = string.Empty;
            string cssClass = "fb-like-box";
            if (Width.Value == 0.0)
                Width = new Unit(292, UnitType.Point);
            if (Height.Value > 0.0)
                height = "data-height='" + Height.Value + "'";
            if (ColorScheme == BrickRed.WebParts.Facebook.Wall.Enumerators.ColorSchemeEnum.dark)
                cssClass = "fb-like-box dark_background";

            strBuilder.Append(@"<div class='" + cssClass + "' data-href='" + PageURL + "' data-width='" + Width.Value + "' " + height + "  data-colorscheme='" + ColorScheme + "' data-show-faces='" + ShowFaces + "' data-border-color='" + LikeBoxBorderColor + "' data-stream='" + ShowStream + "' data-header='" + ShowHeader + "'></div>");
            writer.Write(strBuilder);
        }
    }
}
