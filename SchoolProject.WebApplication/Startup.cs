using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace SchoolProject.WebApplication {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app) {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });


            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }
    }
}