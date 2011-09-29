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
using System.Web.UI;
using Microsoft.SharePoint;
using System.Web.UI.WebControls;

namespace BrickRed.WebParts.Facebook.Wall
{
    class ProgressTemplate : ITemplate
    {

        public string ImagePath { get; set; }      

        public ProgressTemplate()
        {
            ImagePath = "http://static.ak.fbcdn.net/rsrc.php/v1/yb/r/GsNJNwuI-UM.gif";           
        }

        public void InstantiateIn(Control container)
        {
            Table tblProgress = new Table();
            tblProgress.Width = Unit.Percentage(100);
            tblProgress.CellSpacing = 0;
            tblProgress.CellPadding = 0;
            tblProgress.CssClass = "ms-viewlsts";
            TableRow trProgress = new TableRow();
            TableCell tcProgress = new TableCell();
            tcProgress.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
            tcProgress.Height = 20;
            tcProgress.Attributes.Add("style", "background-color: #EDEFF4;border: 1px solid #D8DFEA;");
            Image img = new Image();
            img.ImageUrl = ImagePath;
            tcProgress.Controls.Add(img);  
            trProgress.Controls.Add(tcProgress);
            tblProgress.Controls.Add(trProgress);
            container.Controls.Add(tblProgress);
            
        }
    }
}
