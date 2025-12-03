using EmployMeMatch.WebUI.Models;
using ExecViewHrk.Domain.Helper;
using ExecViewHrk.EfAdmin;
using ExecViewHrk.WebUI.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        public ActionResult About()
        {
            //int x = 4, y = 0, z = x / y;
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactUsModel contactUsModel)
        {
            EmailProcessor emailProcessor = new EmailProcessor();
            //emailProcessor.Send("webtimeAdmin@resnav.com", "webtimeAdmin@resnav.com", "ExecView"
            //    , "Hello ExecView Admin, A message from the Home Contact Page has been sent from " + contactUsModel.Name
            //    + Environment.NewLine + Environment.NewLine
            //    + "The Message sent is as follows"
            //    + Environment.NewLine + Environment.NewLine
            //    + contactUsModel.Message);

            emailProcessor.Send("webtimeAdmin@resnav.com", "technicalSupport@hrknowledge.com", "ExecView"
                , "Hello ExecView Admin, A message from the Home Contact Page has been sent from " + contactUsModel.Name
                + Environment.NewLine + Environment.NewLine
                + "The Message sent is as follows"
                + Environment.NewLine + Environment.NewLine
                + contactUsModel.Message);

            AdminDbContext adminDbContext = new AdminDbContext();
            adminDbContext.MessageLogs.Add(new MessageLog
            {
                Category = "Contact Non User",
                DateTime = DateTime.Now,
                Message = contactUsModel.Message,
                Source = contactUsModel.EmailAddress
            });
            adminDbContext.SaveChanges();

            TempData["message"] = "Your message has been sent.";

            return RedirectToAction("GenericMessage");
        }

        public ActionResult GenericMessage()
        {
            return View();
        }

        // GET: Home
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                IList<string> userRoles = UserManager.GetRoles(HttpContext.User.Identity.GetUserId());
                if (userRoles.Contains("HrkAdministrators"))
                {
                    return Redirect("~/HrkAdmin");
                }
                return View();
            }
            return View();
        }
    }
}