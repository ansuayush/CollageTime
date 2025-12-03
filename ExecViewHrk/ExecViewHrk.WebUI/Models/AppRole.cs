using Microsoft.AspNet.Identity.EntityFramework;

namespace ExecViewHrk.WebUI.Models
{
    public class AppRole : IdentityRole
    {

        public AppRole() : base() { }

        public AppRole(string name) : base(name) { }
    }
}