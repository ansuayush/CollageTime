using ExecViewHrk.WebUI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ExecViewHrk.WebUI.Models;
using ExecViewHrk.EfAdmin;
using System.Threading.Tasks;

namespace ExecViewHrk.WebUI.Controllers
{
    [AllowAnonymous]
    public class CommonController : Controller
    {
        // GET: Common
        [AllowAnonymous]
        public ActionResult Error()
        {
            //int x=0, y=4, z=y/x;


            return View();
        }
    

        public ActionResult ChangePassword()
        { 
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(PasswordViewModel passwordViewModel)
        {
            if (ModelState.IsValid)
            {
                string password = passwordViewModel.Password;

                if (passwordViewModel.Password != passwordViewModel.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Password and Confirm Password do not match.");

                    return View(passwordViewModel);
                }
                AdminDbContext adminDbContext = new AdminDbContext();
                

                //AppUser user = UserManager.Find(User.Identity.Name, passwordViewModel.Password);
                
                var aspNetUser = adminDbContext.AspNetUsers.Where(x => x.UserName == User.Identity.Name).SingleOrDefault();
                if (aspNetUser == null)
                {
                    ModelState.AddModelError("", "Cannot find User.");
                    return View(passwordViewModel);
                }
                else
                {
                    AppUser user = await UserManager.FindByIdAsync(aspNetUser.Id);
                    
                    IdentityResult validPass = null;
                    
                    validPass = await UserManager.PasswordValidator.ValidateAsync(password);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
                        IdentityResult result = await UserManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            aspNetUser.LastPasswordChangeDate = DateTime.Now;
                            adminDbContext.SaveChanges();
                            return Redirect("PasswordChanged");
                        }
                        else
                        {
                            AddErrorsFromResult(result);
                            return View(passwordViewModel);
                        }
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                        return View(passwordViewModel);
                    }

                    //UserManager.RemovePassword(aspNetUser.Id);
                    //UserManager.AddPassword(aspNetUser.Id, passwordViewModel.Password);
                    
                    //aspNetUser.LastPasswordChangeDate = DateTime.Now;
                    //adminDbContext.SaveChanges();

                    //string returnUrl = "";
                    //System.Collections.Generic.IList<string> userRoles = UserManager.GetRoles(user.Id);
                    
                    //return Redirect("PasswordChanged");
                }
            }
            else
            {
                ModelState.AddModelError("", "The update cannot be made at this time.");
                return View(passwordViewModel);
            }
        }

        public ActionResult PasswordChanged()
        {
            return View();
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }

}