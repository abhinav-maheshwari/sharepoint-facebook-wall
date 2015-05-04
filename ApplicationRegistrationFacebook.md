## Application registration on facebook ##
  1. Login and [register your application](http://www.facebook.com/developers/createapp.php) on Facebook
> <br />
> ![http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/register-application-facebook.png](http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/register-application-facebook.png)



&lt;BR/&gt;


Now provide the name of the application

> ![http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/register-application-2.png](http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/register-application-2.png)
> 

&lt;BR/&gt;


> Enter the captcha value if asked


&lt;BR/&gt;



![http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/register-application-3.png](http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/register-application-3.png)

> <br />
Now your application is created. Your App id is the client id and App Secret is the Client Secret that we are going to use in future configurations.
> <br />
> ![http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/configure-facebook.png](http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/configure-facebook.png)


> 

&lt;BR/&gt;


> > Also configure the website URL for the site to use. The domain name should be same from where you are going to authorize the calls


> ![http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/facebook-site-url.png](http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/facebook-site-url.png)

# Local environment setup #
If you don't have a public domain name like research.brickred.com, you may still try this application with your local system configuration. For that:<br><br>
1.	Open C:\Windows\System32\Drivers\etc\hosts file (in notepad) and add following entry:<br>
127.0.0.1	research.brickred.com<br>
(There is a Tab in between)<br>
<br>
<img src='http://sharepoint-facebook-wall.googlecode.com/svn/wiki/images/4.0/hosts.png' />

<b>Use your local machine name for this configuration instead of brickred.com domain name like <a href='http://machinename.local'>http://machinename.local</a></b>

<h1>Next Steps : SharePoint WebParts Configuration</h1>

<ul><li><a href='ShowWallConfiguration.md'>Configure Show Facebook wall webpart </a>
</li><li><a href='WriteOnWallConfiguration.md'>Configure Write on Facebook Wall Webpart</a>
</li><li><a href='LikeBoxConfiguration.md'>Configure Facebook Like Box webpart </a>
</li><li><a href='LikeButtonConfiguration.md'>Configure Facebook Like Button webpart</a>