using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using Alx.Models;
using Alx.App_Start;

namespace Alx
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            OAuthWebSecurity.RegisterFacebookClient(
                appId: "673880506068611",
               appSecret: "79d9070470b4d5f6c70709dc67ca2c3a");

            //OAuthWebSecurity.RegisterGoogleClient();
            OAuthWebSecurity.RegisterClient(new GooglePlusClient("895297505523-tdungd05fb5m9mh3u4e7p0d2n9rvlepl.apps.googleusercontent.com",
                "BkKwwdkJx_AsDfrrA5qQxW7C"), "google", null);
            //Some text hrere.

            
        }
    }
}
