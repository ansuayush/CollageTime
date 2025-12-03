using ExecViewHrk.EfClient;
using ExecViewHrk.WebUI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System;
using System.Data.SqlClient;

namespace ExecViewHrk.WebUI.Services
{
    public class SalaryGradeService
    {
        readonly string connString = HttpContext.Current.User.Identity.GetClientConnectionString();

        public IEnumerable<DdlSalaryGrades> getALL()
        {
            IEnumerable<DdlSalaryGrades> ddlSalaryGradesList = null;
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            using (clientDbContext)
            {
                ddlSalaryGradesList = clientDbContext.DdlSalaryGrades.Where(x => x.active == true).ToList();
            }
            return ddlSalaryGradesList;
        }

        public IEnumerable<SalaryGradeHistoryListVm> GetSalaryGradHistorylst()
        {
            IEnumerable<SalaryGradeHistoryListVm> ddlSalaryGradeHistoryList = null;
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            ddlSalaryGradeHistoryList = (from psgsh in clientDbContext.PositionSalaryGradeSourceHistories
                             join sg in clientDbContext.DdlSalaryGrades on
                                 psgsh.SalaryGradeID equals sg.SalaryGradeID
                             where sg.active == true
                             select new SalaryGradeHistoryListVm
                             {
                                 description = sg.description,
                                 salaryMaximum = psgsh.salaryMaximum.ToString(),
                                 salaryMidpoint = psgsh.salaryMidpoint.ToString(),
                                 salaryMinimum = psgsh.salaryMinimum.ToString(),
                                 validFrom = psgsh.ValidDate,
                                 ChangeDate = psgsh.ChangeEffectiveDate
                             }).ToList();
            return ddlSalaryGradeHistoryList;
        }

        public List<SalaryGradeVm> getALLHistory()
        {
            List<SalaryGradeVm> ddlSalaryGradeHistoryList = new List<SalaryGradeVm>();
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            using (clientDbContext)
            {
                ddlSalaryGradeHistoryList = clientDbContext.DdlSalaryGradeHistory
                .Include(a => a.DdlSalaryGrades)
                .Select(x => new SalaryGradeVm
                {
                    ID = x.ID,
                    SalaryGradeID = x.SalaryGradeID,
                    code = x.DdlSalaryGrades.code,
                    description = x.DdlSalaryGrades.description,
                    salaryMaximum = x.salaryMaximum,
                    salaryMidpoint = x.salaryMidpoint,
                    salaryMinimum = x.salaryMinimum,
                    validFrom = x.validFrom,
                    active = x.DdlSalaryGrades.active
                }).Where(x => x.active == true).ToList();
            }
            return ddlSalaryGradeHistoryList;
        }

        public string getSalaryGrade(int salaryGradeID)
        {
            string grade = string.Empty;
            if (salaryGradeID > 0)
            {
                grade = getALL().SingleOrDefault(x => x.SalaryGradeID == salaryGradeID).description;
            }
            return grade;
        }


        public string getSalaryGradeForPosition(int positionID)
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            string grade = string.Empty;
            using (clientDbContext)
            {
                int salaryGradeID = 0;
                if (clientDbContext.PositionSalaryGrades.Where(x => x.PositionId == positionID).Count() > 0)
                {
                    salaryGradeID = clientDbContext.PositionSalaryGrades.Where(x => x.PositionId == positionID).OrderByDescending( m => m.enteredDate).FirstOrDefault().salaryGradeID;
                }
                grade = salaryGradeID.ToString();

            }
            return grade;
        }

        public int getSalaryGradeIDbySalaryDescription(string description)
        {
            short returnid = 0;
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var salaryGradInDb = clientDbContext.DdlSalaryGrades.Where(x => (x.description == description) && (x.code==description)).SingleOrDefault();
            if (salaryGradInDb == null)
            {
                var newhistoryitem = new DdlSalaryGrades();
                newhistoryitem.description = description;
                newhistoryitem.code= description;
                newhistoryitem.active = true;
                clientDbContext.DdlSalaryGrades.Add(newhistoryitem);
                clientDbContext.SaveChanges();
                return newhistoryitem.SalaryGradeID;
            }
            else { return salaryGradInDb.SalaryGradeID; }
            //return returnid;
        }
        public string saveSalaryItem(AddSalaryGradeitem addSalaryitem)
        {
            string returnmsg = "";
            var newitem = new DdlSalaryGradeHistory();
            if (addSalaryitem.GridID == 0)
            {
                int salaryGradID = getSalaryGradeIDbySalaryDescription(addSalaryitem.description);
                newitem.SalaryGradeID = salaryGradID;
                newitem.validFrom = addSalaryitem.validDate;
                newitem.salaryMinimum = addSalaryitem.salaryMin;
                newitem.salaryMidpoint = addSalaryitem.salaryMid;
                newitem.salaryMaximum = addSalaryitem.salaryMax;

                try
                {
                    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                    {
                        clientDbContext.DdlSalaryGradeHistory.Add(newitem);
                        clientDbContext.SaveChanges();
                        returnmsg = newitem.ID.ToString();
                    }
                }
                catch (System.Exception ex) { returnmsg = ex.ToString(); }
            }else
            {
                ClientDbContext clientDbContext = new ClientDbContext(connString);
                var SalaryGradeInDb = clientDbContext.DdlSalaryGradeHistory.Where(x => x.ID == addSalaryitem.GridID).SingleOrDefault();
                ////*************Add item in history****************
                    var newhistoryitem = new PositionSalaryGradeSourceHistories();
                    newhistoryitem.SalaryGradeID = SalaryGradeInDb.SalaryGradeID;
                    newhistoryitem.salaryMaximum = addSalaryitem.salaryMax;
                    newhistoryitem.salaryMidpoint = addSalaryitem.salaryMid;
                    newhistoryitem.salaryMinimum = addSalaryitem.salaryMin;
                    newhistoryitem.ValidDate = addSalaryitem.validDate;
                    newhistoryitem.ChangeEffectiveDate = System.DateTime.Now;
                    newhistoryitem.DdlSalaryGradeHistoriesID = addSalaryitem.GridID;
                    clientDbContext.PositionSalaryGradeSourceHistories.Add(newhistoryitem);
                    clientDbContext.SaveChanges();

                ////**************Update item ***********************
                int salaryGradID = getSalaryGradeIDbySalaryDescription(addSalaryitem.description);
                SalaryGradeInDb.validFrom = addSalaryitem.validDate;
                SalaryGradeInDb.salaryMinimum = addSalaryitem.salaryMin;
                SalaryGradeInDb.salaryMidpoint = addSalaryitem.salaryMid;
                SalaryGradeInDb.salaryMaximum = addSalaryitem.salaryMax;
                SalaryGradeInDb.SalaryGradeID = salaryGradID;
                clientDbContext.SaveChanges();
                returnmsg = SalaryGradeInDb.ID.ToString();
            }
            return returnmsg;
        }


        public string savefundingSourceItem(AddPositionFundSourceitem addFundSourceitem)
        {
            string returnmsg = "";
            var newitem = new PositionFundingSource();
            try
            {
                if (addFundSourceitem.PositionFundingSourceID == 0)
                {
                    newitem.EffectiveDate = addFundSourceitem.EffectiveDate;
                    newitem.FundCodeID = addFundSourceitem.FundCodeID;
                    newitem.Percentage = addFundSourceitem.Percentage;
                    using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                    {
                        clientDbContext.PositionFundingSource.Add(newitem);
                        clientDbContext.SaveChanges();
                        returnmsg = newitem.PositionFundingSourceID.ToString();
                    }
                }
                else
                {
                    ClientDbContext clientDbContext = new ClientDbContext(connString);
                    //************************************************************************
                    int countpercentage = 0;
                    var perData = (from PFS in clientDbContext.PositionFundingSource
                                   join FD in clientDbContext.Funds
                                   on PFS.FundCodeID equals FD.ID
                                   where FD.Active == true && PFS.EffectiveDate == addFundSourceitem.EffectiveDate
                                   select new 
                                   {
                                       PositionFundingSourceID = PFS.PositionFundingSourceID,
                                       Percentage = PFS.Percentage
                                   }).ToList();// clientDbContext.PositionFundingSource.Where(x => x.EffectiveDate == addFundSourceitem.EffectiveDate).ToList();
                    if (perData != null)
                    {
                        for (int i = 0; i < perData.Count; i++)
                        {
                            if (perData[i].PositionFundingSourceID == addFundSourceitem.PositionFundingSourceID)
                            {
                                countpercentage = countpercentage + addFundSourceitem.Percentage;
                            }
                            else
                            {
                                if (i == 0)
                                    countpercentage = Convert.ToInt32(perData[i].Percentage.ToString());
                                else
                                    countpercentage = countpercentage + Convert.ToInt32(perData[i].Percentage.ToString());
                            }
                        }
                    }
                    if (countpercentage != 100)
                    {
                        return "";
                    }
                    else
                    {
                        //************************************************************************
                        var FundSourceInDb = clientDbContext.PositionFundingSource.Where(x => x.PositionFundingSourceID == addFundSourceitem.PositionFundingSourceID).SingleOrDefault();
                        //*************Add item in history****************
                        var newhistoryitem = new PositionFundingSourceHistories();
                        newhistoryitem.EffectiveDate = FundSourceInDb.EffectiveDate;
                        newhistoryitem.FundCodeID = FundSourceInDb.FundCodeID;
                        newhistoryitem.Percentage = FundSourceInDb.Percentage;
                        newhistoryitem.ChangeEffectiveDate = System.DateTime.Now;
                        newhistoryitem.PositionFundingSourceID = FundSourceInDb.PositionFundingSourceID;
                        clientDbContext.PositionFundingSourceHistories.Add(newhistoryitem);
                        clientDbContext.SaveChanges();
                        //**************Update item ***********************
                        FundSourceInDb.EffectiveDate = addFundSourceitem.EffectiveDate;
                        FundSourceInDb.FundCodeID = addFundSourceitem.FundCodeID;
                        FundSourceInDb.Percentage = addFundSourceitem.Percentage;
                        clientDbContext.SaveChanges();
                        returnmsg = newitem.PositionFundingSourceID.ToString();
                    }
                }
            }
            catch (System.Exception ex) { returnmsg = ex.ToString(); }
            return returnmsg;
        }
        public string saveSalaryHistoryItem(int E_PositionId, int RateType, int PayRate, int HoursPerPayPeriod, DateTime? EffectiveDate)
        {
            string returnmsg = "";
            var newitem = new E_PositionSalaryHistories
            {
                E_PositionId = E_PositionId,
                PayRate = PayRate,
                HoursPerPayPeriod = HoursPerPayPeriod,
                EffectiveDate = EffectiveDate,
            };
            try
            {
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    clientDbContext.E_PositionSalaryHistories.Add(newitem);
                    clientDbContext.SaveChanges();
                    clientDbContext.Database.ExecuteSqlCommand("Update [E_Positions] SET RateTypeId = @RateTypeId WHERE E_PositionId = @E_PositionId", new SqlParameter("@RateTypeId", RateType), new SqlParameter("@E_PositionId", E_PositionId));
                    returnmsg = newitem.E_PositionSalaryHistoryId.ToString();
                }
            }
            catch (System.Exception ex) { returnmsg = ex.ToString(); }
            return returnmsg;
        }

       public string PercentageonEffDate(DateTime EffDate)
        {
            string returnmsg = "";
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            try
            {
                int countpercentage = 0;
                var perData = clientDbContext.PositionFundingSource.Where(x => x.EffectiveDate == EffDate).ToList();
                if (perData != null)
                {
                    for (int i = 0; i < perData.Count; i++)
                    {
                        if (i == 0)
                            countpercentage = Convert.ToInt32(perData[i].Percentage.ToString());
                        else
                            countpercentage = countpercentage+Convert.ToInt32(perData[i].Percentage.ToString());
                    }
                }
                returnmsg = countpercentage.ToString();
            }
            catch(Exception ex) { }
            return returnmsg;
        }
    }
}