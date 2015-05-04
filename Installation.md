## Prerequisites ##
  1. WSS 3.0 / MOSS 2007 Environment
> > OR
  1. SharePoint Foundation 2010 / SharePoint 2010 Environment
## Step 1. WSP Installation ##
  1. [Download](http://code.google.com/p/sharepoint-facebook-wall/downloads/list) WSP for 2007 or WSP for 2010 and unzip the Facebook archive. This contains the WSP file, deploy solution batch file and retract solution batch file
### 2007 Installation ###
  1. Change URL property from "<<Server URL>>" to your server URL in both the deploy and retract solution files
  1. Now run deploy solution file <br /> ![http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/install-solution.png](http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/install-solution.png)
  1. This will add the solution in solutions gallery <br /> http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/solution-properties.PNG
  1. This will also deploy the feature in site collection features gallery on specified web portal url<br /> ![http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/write-features-gallery.png](http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/write-features-gallery.png)

### 2010 Installation ###
  1. Right click on  PackageDeploy.ps1 file and click "Run with PowerShell"  <br /> ![http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/powershellinstall.png](http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/powershellinstall.png)
  1. Enter your web application URL when prompted <br /> ![http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/InputWebURL.png](http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/InputWebURL.png)
  1. IF you have already installed this solution before then you will be given 3 options : <br /> Retract and Install solution<br />Upgrade Solution<br />Exit <br /> ![http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/SolutionAlreadyExists.png](http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/SolutionAlreadyExists.png)
  1. In this demo, I opted for 1st option <br /> ![http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/Deployment-Success.png](http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/Deployment-Success.png)
  1. This will add the solution in solutions gallery <br /> http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/solution-properties.PNG
  1. This will also deploy the feature in site collection features gallery on specified web portal url<br /> ![http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/write-features-gallery.png](http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/write-features-gallery.png)

# Next Step : Facebook Configuration #

  * You can [register](ApplicationRegistrationFacebook.md) application on Facebook with our step by step registration guide.
