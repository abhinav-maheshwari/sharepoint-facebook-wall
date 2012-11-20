using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;
using System.Xml;
using Microsoft.SharePoint.Utilities;
using System.IO;
using System.Net;
using Microsoft.SharePoint;

namespace BrickRed.WebParts.Facebook.Wall.FacebookEvents
{
    class FacebookEvents : SPJobDefinition
    {
        public FacebookEvents()
            : base()
        {
        }


        public FacebookEvents(string JobName, SPWebApplication SPWebApp)
            : base(JobName, SPWebApp, SPServer.Local, SPJobLockType.None)
        {
            this.Title = "BrickRed.Facebook.Events";
        }

        public override void Execute(Guid targetInstanceId)
        {
            #region Declarations
            XmlDocument doc;
            string _path = string.Empty; ;
            string ListName = string.Empty;
            string SiteURL = string.Empty;
            string OAuthClientID = string.Empty;
            string OAuthRedirectUrl = string.Empty;
            string OAuthClientSecret = string.Empty;
            string OAuthCode = string.Empty;
            string UserId = string.Empty;
            string OAuthToken = string.Empty;

            #endregion

            #region Get SPJob Properties 
            //Values from SPJob properties
            if (Properties["OAuthClientID"] != null) { OAuthClientID = (string)Properties["OAuthClientID"]; }
            if (Properties["OAuthRedirectUrl"] != null) { OAuthRedirectUrl = (string)Properties["OAuthRedirectUrl"]; }
            if (Properties["OAuthClientSecret"] != null) { OAuthClientSecret = (string)Properties["OAuthClientSecret"]; }
            if (Properties["OAuthCode"] != null) { OAuthCode = (string)Properties["OAuthCode"]; }
            if (Properties["UserId"] != null) { UserId = (string)Properties["UserId"]; }
            #endregion

            #region Facebook Event Code

            if (!string.IsNullOrEmpty(OAuthClientID) &&
                !string.IsNullOrEmpty(OAuthRedirectUrl) &&
                !string.IsNullOrEmpty(OAuthClientSecret) &&
                !string.IsNullOrEmpty(OAuthCode) &&
                !string.IsNullOrEmpty(UserId))
            {

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    //Get XML
                    doc = GetXMLFIle(out _path);

                    if (doc != null)
                    {
                        //Read XML to Access List
                        XmlNode ParentNode = doc.SelectSingleNode("Facebook/Contents");

                        //Checking if theres any entry in the master XMl
                        if (ParentNode.HasChildNodes)
                        {
                            foreach (XmlNode contentNodes in ParentNode.ChildNodes)
                            {
                                foreach (XmlNode childNode in contentNodes.ChildNodes)
                                {
                                    if (childNode.Name.Equals("ListName")) { ListName = Convert.ToString(childNode.InnerText); }
                                    if (childNode.Name.Equals("SiteURL")) { SiteURL = Convert.ToString(childNode.InnerText); }
                                }

                                using (SPSite objSite = new SPSite(SiteURL))
                                {
                                    using (SPWeb objWeb = objSite.OpenWeb())
                                    {
                                        SPList objList = objWeb.Lists.TryGetList(ListName);

                                        if (objList != null)
                                        {
                                            foreach (SPListItem item in objList.Items)
                                            {
                                                //Synchronized the items that are not synchronized
                                                if (!Convert.ToBoolean(item["IsSynchronized"]))
                                                {
                                                    #region Filling Properties Values

                                                    string postUrl = string.Empty;

                                                    // Values from List
                                                    string Eventname = Convert.ToString(item["FBEventType"]);
                                                    string start_time = GetUTCFormatTime(item["FBEventStartDate"]);
                                                    string end_time = GetUTCFormatTime(item["FBEvenEndDate"]);
                                                    string location = Convert.ToString(item["FBEventPlace"]);
                                                    string description = Convert.ToString(item["FBEventDescription"]);
                                                    string privacy_type = Convert.ToString(item["FBPrivacyType"]);
                                                    #endregion

                                                    //Get the Authorization code from Facebook
                                                    if (string.IsNullOrEmpty(OAuthToken))
                                                        OAuthToken = CommonHelper.GetOAuthToken("create_event", OAuthClientID, OAuthRedirectUrl, OAuthClientSecret, OAuthCode);

                                                    #region Create Post URL
                                                    //If End Time, Description and Location is not mentioned for the Event
                                                    if (string.IsNullOrEmpty(end_time) && string.IsNullOrEmpty(description) && string.IsNullOrEmpty(location))
                                                        postUrl = string.Format("https://graph.facebook.com/me/events?access_token={0}&name={1}&start_time={2}&privacy_type={3}"
                                                    , OAuthToken, Eventname, start_time, privacy_type);

                                                    //If End Time and Description is not mentioned for the Event
                                                    else if (string.IsNullOrEmpty(end_time) && string.IsNullOrEmpty(description))
                                                        postUrl = string.Format("https://graph.facebook.com/me/events?access_token={0}&name={1}&start_time={2}&location={3}&privacy_type={4}"
                                                        , OAuthToken, Eventname, start_time, location, privacy_type);

                                                    //If Description and Location is not mentioned for the Event
                                                    else if (string.IsNullOrEmpty(description) && string.IsNullOrEmpty(location))
                                                        postUrl = string.Format("https://graph.facebook.com/me/events?access_token={0}&name={1}&start_time={2}&end_time={3}&privacy_type={4}"
                                                   , OAuthToken, Eventname, start_time, end_time, privacy_type);

                                                    //If End Time and Location is not mentioned for the Event
                                                    else if (string.IsNullOrEmpty(end_time) && string.IsNullOrEmpty(location))
                                                        postUrl = string.Format("https://graph.facebook.com/me/events?access_token={0}&name={1}&start_time={2}&description={3}&privacy_type={4}"
                                                    , OAuthToken, Eventname, start_time, description, privacy_type);

                                                    else
                                                        postUrl = string.Format("https://graph.facebook.com/me/events?access_token={0}&name={1}&start_time={2}&end_time={3}&description={4}&location={5}&privacy_type={6}"
                                                        , OAuthToken, Eventname, start_time, end_time, description, location, privacy_type);

                                                    #endregion

                                                    //Post Event
                                                    bool isCreated = PostEvent(postUrl);

                                                    //item["IsSynchronized"] = true;
                                                    item["IsSynchronized"] = isCreated;
                                                    item.Update();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }
            else
            {
                throw (new SPException("Please provide the Facebook application properties to the JOB"));
            }
            #endregion
        }

        /// <summary>
        /// Provides the UTC time format
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        private string GetUTCFormatTime(object Time)
        {
            if (Time != null)
            {
                DateTime sDate = (DateTime)(Time);
                string UTCformat = sDate.Year + "-" + sDate.Month + "-" + sDate.Day + "T" + sDate.TimeOfDay.Hours + ":" + sDate.TimeOfDay.Minutes + ":" + sDate.TimeOfDay.Seconds;
                return UTCformat;
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Posts the event to the Facebook with the URL provided
        /// </summary>
        /// <param name="postUrl"></param>
        /// <returns></returns>
        private bool PostEvent(string postUrl)
        {
            if (!String.IsNullOrEmpty(postUrl))
            {
                HttpWebRequest request2 = WebRequest.Create(postUrl) as HttpWebRequest;
                request2.Method = "post";
                using (HttpWebResponse response2 = request2.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response2.GetResponseStream());
                    string retVal = reader.ReadToEnd();

                    if (!String.IsNullOrEmpty(retVal))
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
            else
                return false;
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
                throw (new SPException("Job could not locate the master xml: " + ex.Message));
            }
            return doc;
        }
    }
}
