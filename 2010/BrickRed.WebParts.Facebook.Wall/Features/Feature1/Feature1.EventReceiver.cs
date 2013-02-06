using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Administration;

namespace BrickRed.WebParts.Facebook.Wall.Features.Feature1
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("0c941888-9318-40b6-8d52-a7087fd551d0")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite Site = properties.Feature.Parent as SPSite;

            //Check if the Facebook event job is already created, then delete the job
            foreach (SPJobDefinition jobDef in Site.WebApplication.JobDefinitions)
            {
                if (jobDef.Name.Equals("BrickRed.Fcebook.Events"))
                {
                    jobDef.Delete();
                }
            }

            //Configure job
            BrickRed.WebParts.Facebook.Wall.FacebookEvents.FacebookEvents objJob = new BrickRed.WebParts.Facebook.Wall.FacebookEvents.FacebookEvents("BrickRed.Facebook.Events", Site.WebApplication);
            SPMinuteSchedule minSched = new SPMinuteSchedule();
            minSched.BeginSecond = 0;
            minSched.EndSecond = 59;
            minSched.Interval = 2;

            objJob.Schedule = minSched;
            objJob.IsDisabled = true;
            objJob.Update();
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPSite Site = properties.Feature.Parent as SPSite;

            foreach (SPJobDefinition jobDef in Site.WebApplication.JobDefinitions)
            {
                if (jobDef.Name.Equals("BrickRed.Facebook.Events"))
                {
                    jobDef.Delete();
                }
            }
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
