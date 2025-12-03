using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using ExecViewHrk.WebUI.Models;
using Microsoft.AspNet.Identity;
using System;

namespace ExecViewHrk.WebUI.Infrastructure
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext()
            : base("AdminDbContext") 
        { 
            //this.Database.Connection.ConnectionString = ""; 
           
            //if (System.Web.HttpContext.Current.Session != null)
            //{ 
            //    var xxx = System.Web.HttpContext.Current.Session["AdminDbContext"];

                
            //}
                
        }

        static AppIdentityDbContext()
        {
            Database.SetInitializer<AppIdentityDbContext>(new IdentityDbInit());
        }

        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
    }

    public class IdentityDbInit
            : NullDatabaseInitializer<AppIdentityDbContext>
    { 
    }

    //public class IdentityDbInit
    //        : DropCreateDatabaseIfModelChanges<AppIdentityDbContext>
    //{
    //    protected override void Seed(AppIdentityDbContext context)
    //    {
    //        PerformInitialSetup(context);
    //        base.Seed(context);
    //    }

    //    public void PerformInitialSetup(AppIdentityDbContext context)
    //    {
    //        AppUserManager userMgr = new AppUserManager(new UserStore<AppUser>(context));
    //        AppRoleManager roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));

    //        string roleName = "HrkAdministrators";
    //        string userName = "webtimeadmin@resnav.com";
    //        string password = "11111111";
    //        string email = "webtimeadmin@resnav.com"; 

    //        if (!roleMgr.RoleExists(roleName))
    //        {
    //            roleMgr.Create(new AppRole(roleName));
    //        }

    //        AppUser user = userMgr.FindByName(userName);
    //        if (user == null)
    //        {
    //            userMgr.Create(new AppUser { UserName = userName, Email = email, LastPasswordChangeDate = DateTime.Today },
    //                password);
    //            user = userMgr.FindByName(userName);
    //        }

    //        if (!userMgr.IsInRole(user.Id, roleName))
    //        {
    //            userMgr.AddToRole(user.Id, roleName);
    //        }

    //        roleName = "HrkAdministrators";
    //        userName = "drizzo@resnav.com";
    //        password = "11111111";
    //        email = "drizzo@resnav.com";

    //        if (!roleMgr.RoleExists(roleName))
    //        {
    //            roleMgr.Create(new AppRole(roleName));
    //        }

    //        user = userMgr.FindByName(userName);
    //        if (user == null)
    //        {
    //            userMgr.Create(new AppUser { UserName = userName, Email = email, LastPasswordChangeDate = DateTime.Today },
    //                password);
    //            user = userMgr.FindByName(userName);
    //        }

    //        if (!userMgr.IsInRole(user.Id, roleName))
    //        {
    //            userMgr.AddToRole(user.Id, roleName);
    //        }

    //        roleName = "ClientAdministrators";
    //        userName = "drizzo@dprtechnology.com";
    //        password = "11111111";
    //        email = "drizzo@dprtechnology.com";

    //        if (!roleMgr.RoleExists(roleName))
    //        {
    //            roleMgr.Create(new AppRole(roleName));
    //        }

    //        user = userMgr.FindByName(userName);
    //        if (user == null)
    //        {
    //            userMgr.Create(new AppUser { UserName = userName, Email = email, LastPasswordChangeDate = DateTime.Today },
    //                password);
    //            user = userMgr.FindByName(userName);
    //        }

    //        if (!userMgr.IsInRole(user.Id, roleName))
    //        {
    //            userMgr.AddToRole(user.Id, roleName);
    //        }

    //        roleName = "Employees";
    //        userName = "sales@dprtechnology.com";
    //        password = "11111111";
    //        email = "sales@dprtechnology.com";

    //        if (!roleMgr.RoleExists(roleName))
    //        {
    //            roleMgr.Create(new AppRole(roleName));
    //        }

    //        user = userMgr.FindByName(userName);
    //        if (user == null)
    //        {
    //            userMgr.Create(new AppUser { UserName = userName, Email = email, LastPasswordChangeDate = DateTime.Today },
    //                password);
    //            user = userMgr.FindByName(userName);
    //        }

    //        if (!userMgr.IsInRole(user.Id, roleName))
    //        {
    //            userMgr.AddToRole(user.Id, roleName);
    //        }

    //        // add AccountManager and any other roles
    //    }
    //}
}