using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Infrastructure;
using ExecViewHrk.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace ExecViewHrk.WebUI.Helpers
{
    public class PopulateField
    {
        public PopulateField() { }
       
        ClientDbContext clientDbContext;
        public PopulateField(ClientDbContext _clientDbContext)
        {
            clientDbContext = _clientDbContext;
        }

        internal static IList<SkillTypeVm> PopulateSkillTypes()
        {
            //string connString = User.Identity.GetClientConnectionString();
            //using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                PopulateField pf = new PopulateField();
                IList<SkillTypeVm> skillTypesList = pf.clientDbContext.DdlSkillTypes
                        .Select(s => new SkillTypeVm
                        {
                            SkillTypeId = s.SkillTypeId,
                            SkillTypeDescription = s.Description //+ "-" + e.SkillId.ToString()
                        })
                        .OrderBy(s => s.SkillTypeDescription).ToList();

                skillTypesList.Insert(0, new SkillTypeVm { SkillTypeId = 0, SkillTypeDescription = "--select one--" });

                //ViewData["skillTypesList"] = skillTypesList;
                // ViewData["defaultSkillType"] = skillTypesList.First();
                return skillTypesList;
            }
        }

    }
}