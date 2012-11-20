using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Xml;

namespace BrickRed.WebParts.Facebook.Wall.FacebookEvents.FacebookListEventReciever
{
    /// <summary>
    /// List Events
    /// </summary>
    public class FacebookListEventReciever : SPListEventReceiver
    {
        /// <summary>
        /// A list is being deleted.
        /// </summary>
        public override void ListDeleting(SPListEventProperties properties)
        {
            base.ListDeleting(properties);
            XmlDocument doc;
            string _path;

            SPContentTypeCollection contentTypeColl = properties.List.ContentTypes;

            if (contentTypeColl.Count > 0)
            {
                foreach (SPContentType contentType in contentTypeColl)
                {
                    if (contentType.Name == "BrickRed.Facebook.ContentType")
                    {
                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            doc = GetXMLFIle(out _path);
                            if (doc != null)
                            {
                                try
                                {
                                    XmlNode node = doc.SelectSingleNode("Facebook/Contents/Content[ListName='" + properties.ListTitle + "' and SiteURL='" + properties.WebUrl + "']");

                                    if (node != null)
                                    {
                                        node.ParentNode.RemoveChild(node);
                                        doc.Save(_path);
                                    }
                                }
                                catch { };
                            }
                        });
                    }
                }
            }
        }


        /// <summary>
        /// This method returns the master XML
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        private XmlDocument GetXMLFIle(out string _path)
        {
            XmlDocument doc = null;
            _path = SPUtility.GetGenericSetupPath("\\TEMPLATE\\XML\\BrickRed.Facebook.Wall\\FacebookEvents.xml");

            try
            {
                doc = new XmlDocument();
                doc.Load(_path);
            }
            catch (Exception ex)
            {
                throw (new SPException("Could not locate the master xml: " + ex.Message));
            }
            return doc;
        }

        /// <summary>
        /// A list was added.
        /// </summary>
        public override void ListAdded(SPListEventProperties properties)
        {
            base.ListAdded(properties);

            XmlDocument doc;
            string _path;

            SPContentTypeCollection contentTypeColl = properties.List.ContentTypes;

            if (contentTypeColl.Count > 0)
            {
                foreach (SPContentType contentType in contentTypeColl)
                {
                    if (contentType.Name == "BrickRed.Facebook.ContentType")
                    {
                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            try
                            {
                                doc = GetXMLFIle(out _path);
                                if (doc != null)
                                {

                                    XmlNode Node = doc.CreateElement("Content");

                                    XmlNode listName = doc.CreateElement("ListName");
                                    XmlNode siteURL = doc.CreateElement("SiteURL");

                                    listName.InnerText = properties.ListTitle;
                                    siteURL.InnerText = properties.WebUrl;

                                    Node.AppendChild(listName);
                                    Node.AppendChild(siteURL);

                                    doc.SelectSingleNode("Facebook/Contents").AppendChild(Node);
                                    doc.Save(_path);

                                }
                            }
                            catch (Exception ex)
                            {
                                throw (new SPException("List Added Event Reciever: " + ex.Message));
                            }
                        });
                    }
                }
            }
        }
    }
}
