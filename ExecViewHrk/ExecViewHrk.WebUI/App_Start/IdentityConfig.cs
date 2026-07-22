using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using ExecViewHrk.WebUI.Infrastructure;
using System;

namespace ExecViewHrk.WebUI.App_Start
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {

            app.CreatePerOwinContext<AppIdentityDbContext>(AppIdentityDbContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                // Sliding expiration keeps employees authenticated while they keep punching
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromDays(14),
                CookieName = ".AspNet.ApplicationCookie",
                CookieHttpOnly = true
            });
        }
    }
}