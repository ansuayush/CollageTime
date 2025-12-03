using System.Web;
using ExecViewHrk.EfClient;
using Kendo.Mvc.Extensions;
using ExecViewHrk.WebUI.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using ExecViewHrk.Models;

namespace ExecViewHrk.WebUI.Services
{
    public class PositionBudgetService 
    {
        readonly string connString = HttpContext.Current.User.Identity.GetClientConnectionString();
        readonly PositionFundService positionFundService = new PositionFundService();
        readonly PositionFundingSourceService positionFundingSourceService = new PositionFundingSourceService();
        public IEnumerable<PositionBudgetsVM> getAllPositionBudgets(int? ID , int?PositionID)
        {
            IEnumerable<PositionBudgetsVM> positionsBudgets = null;
            IEnumerable<PositionBudgets> positionsBudgeList = null;
            IEnumerable<PositionBudgetMonths> BudgetMonthList = null;
            IEnumerable<Positions> AllPositions = null;
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            using (clientDbContext)
            {
                if(PositionID.HasValue && !ID.HasValue)
                {
                    positionsBudgeList = PositionID.HasValue ? clientDbContext.PositionsBudgets.Where(x => x.PositionID == PositionID).ToList() : clientDbContext.PositionsBudgets.ToList();
                    BudgetMonthList = clientDbContext.PositionsBudgetsMonths.ToList();
                }
                else
                {
                    positionsBudgeList = ID.HasValue ? clientDbContext.PositionsBudgets.Where(x => x.ID == ID).ToList() : clientDbContext.PositionsBudgets.ToList();
                    BudgetMonthList = clientDbContext.PositionsBudgetsMonths.Where(x => x.PositionBudgetsID == ID.Value).ToList();
                }
                AllPositions = clientDbContext.Positions.ToList();
                
            }
            positionsBudgets = positionsBudgeList.Select(x => new PositionBudgetsVM
                  {
                      ID = x.ID,
                      PositionID = x.PositionID,
                      BudgetYear = x.BudgetYear,
                      BudgetMonth = x.BudgetMonth,
                      BudgetAmount = x.BudgetAmount,
                      FTE = x.FTE

                  }).ToList();
            
          
            foreach (var budget in positionsBudgets)
            {
                budget.BudgetMonthText = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(budget.BudgetMonth);
                budget.PositionTitle = AllPositions.SingleOrDefault(x => x.PositionId == budget.PositionID).Title;
                budget.BudgetFundAllocations =  positionFundService.getPositionBudgetFundAllocation(budget.ID).ToList();
                budget.BudgetMonthList = BudgetMonthList.Where(x => x.PositionBudgetsID == budget.ID)
                    .Select(x => new PositionBudgetMonthsVM
                    {   ID = x.ID,
                        BudgetAmount = x.BudgetAmount,
                        Month =System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.BudgetMonth).Substring(0,3)
                    }).ToList();
                budget.MonthList = getMonthList();
                budget.YearList = getYearList();
            }
            return positionsBudgets;
        }



        public IEnumerable<PositionBudgetsVM> getPositionBudgets(int? budgetId, int? positionID)
        {
            IEnumerable<PositionBudgetsVM> positionsBudgets = getAllPositionBudgets(null, positionID).Where(x => x.PositionID == positionID);
            return positionsBudgets.ToList();
        }


        public PositionBudgetsVM getPositionBudget(int ID)
        {
            PositionBudgetsVM positionsBudget = getAllPositionBudgets(ID,null).SingleOrDefault(x => x.ID == ID);
            if (positionsBudget.BudgetFundAllocations.Count>0)
            {
                foreach (var item in positionsBudget.BudgetFundAllocations)
                {
                    item.PositionFundID = ID;
                }
            }
            return positionsBudget;
        }

        public PositionBudgetsVM getNewPositionBudget(short positionId)
        {
            PositionBudgetsVM positionsBudget = new PositionBudgetsVM();
            positionsBudget.MonthList = getMonthList();
            positionsBudget.YearList = getYearList();
            positionsBudget.PositionID = positionId;
            return positionsBudget;
        }
       
        public string saveBudgetEntity(PositionBudgetsVM  positionBudgetsVM)
        {
                string returnmsg = "";
                int nUI_Year = positionBudgetsVM.BudgetYear;
                int nUI_Month = positionBudgetsVM.BudgetMonth;
                decimal nYearMonthToInsert = (nUI_Year * 100) + nUI_Month;
               
            try
            {
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    var allBudgets = clientDbContext.PositionsBudgets.Where(a => a.PositionID == positionBudgetsVM.PositionID).ToList();
                    PositionBudgets entity = allBudgets.FirstOrDefault(a => a.ID == positionBudgetsVM.ID);
                    if (entity == null)
                    {
                        entity = new PositionBudgets();
                        foreach (var budget in allBudgets)
                        {
                            int nYear = budget.BudgetYear;
                            int nMonth = budget.BudgetMonth;
                            int nYearMonth = (nYear * 100) + nMonth;
                            if (Math.Abs(nYearMonth - nYearMonthToInsert) < 100)
                            {
                                returnmsg = "Cannot insert new budget. Month/Year must be at least 12 months apart from budgets already defined.";
                                return returnmsg;
                            }
                        }
                    }
                    // Push Amount  into another table (PositionBudgetMonths)
                    var lst = getMonthList();
                    entity.PositionID = positionBudgetsVM.PositionID;
                    entity.BudgetMonth = positionBudgetsVM.BudgetMonth;
                    entity.BudgetYear = positionBudgetsVM.BudgetYear;
                    entity.BudgetAmount = positionBudgetsVM.BudgetAmount.Value;
                    entity.FTE = positionBudgetsVM.FTE.Value;
                    if(positionBudgetsVM.ID == 0)
                    {
                        clientDbContext.PositionsBudgets.Add(entity);
                    }
                    clientDbContext.SaveChanges();

                    // positionBudgetsVM is New
                    if(positionBudgetsVM.ID == 0 || positionBudgetsVM.BudgetMonthList == null)
                    {
                        var start = new DateTime(positionBudgetsVM.BudgetYear, positionBudgetsVM.BudgetMonth, 1);
                        var endMonth = (positionBudgetsVM.BudgetMonth - 1) == 0 ? 12 : (positionBudgetsVM.BudgetMonth - 1);
                        var end = new DateTime(positionBudgetsVM.BudgetYear + 1, endMonth, 2);
                        end = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));
                        var BudgetMonthList = Enumerable.Range(0, 12).Select(a => start.AddMonths(a)).TakeWhile(a => a <= end).Select(e => e.ToString("MMMM"));
                        decimal budgetAmt = (positionBudgetsVM.BudgetAmount.Value) / 12;
                        foreach (var item in BudgetMonthList)
                        {
                            PositionBudgetMonths positionBudgetMonth = new PositionBudgetMonths();
                            positionBudgetMonth.BudgetAmount = budgetAmt;
                            positionBudgetMonth.PositionBudgetsID = entity.ID;
                            positionBudgetMonth.BudgetMonth = Convert.ToByte(lst.FindIndex(x => x.keydescription == item)+1);
                            positionBudgetMonth.DisplayPosition = Convert.ToByte(positionBudgetsVM.BudgetMonth);
                            clientDbContext.PositionsBudgetsMonths.Add(positionBudgetMonth);
                        }
                    }
                    else
                    {
                        var budgetMonthList = clientDbContext.PositionsBudgetsMonths.Where(x => x.PositionBudgetsID == entity.ID).ToList();
                        if (budgetMonthList.Any())
                        {
                            foreach(var item in positionBudgetsVM.BudgetMonthList)
                            {
                                var monthEntity = budgetMonthList.FirstOrDefault(x => x.ID == item.ID);
                                 monthEntity.BudgetAmount = item.BudgetAmount;
                            }
                        }
                    }
                    clientDbContext.SaveChanges();
                    returnmsg = entity.PositionID.ToString();
                }
            }
            catch (System.Exception ex) { returnmsg = "Can not save ,Please check year month amount or FTE!"; }
            return returnmsg;
        }
        public IEnumerable<Position> getPositions()
        {
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var Positions = clientDbContext.Positions.ToList();
            return Positions;
        }
        public IEnumerable<PositionFundingSourceGroupVM> getFundCodeWithEffectiveDate(int? positionID)
        {
            List<PositionFundingSourceGroupVM> posionFundingSourceList;
            posionFundingSourceList = positionFundingSourceService.getposionFundingSourceList(positionID).ToList();
            return posionFundingSourceList.ToList();
        }



        public List<DropDownModel> getMonthList()
        {
            
           return  Enumerable.Range(1, 12).Select(x => new DropDownModel
             { keyvalue = x.ToString(),
              keydescription = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(x)
             }).ToList();
        }

        public List<DropDownModel> getYearList()
        {
           
            return  Enumerable.Range(2011,9).Select(x => new DropDownModel
            {
                keyvalue = x.ToString(),
                keydescription = x.ToString(),
            }).ToList();
        }
        public List<PosionFundingSourceHistoryListVM> getFundingSourceHistoryList()
        {
            List<PosionFundingSourceHistoryListVM> posionFundingSourceHistoryList = new List<PosionFundingSourceHistoryListVM>();
           // posionFundingSourceHistoryList = positionFundService.getposionFundingSourceHistoryList();
            return posionFundingSourceHistoryList;
        }

        internal IEnumerable<PositionBudgetFundAllocationVm> getPositionBudgetFundAllocation(int? positionBudgetID)
        {
            IEnumerable<PositionBudgetFundAllocationVm> PositionBudgetFundAllocations = null;
            if (positionBudgetID.HasValue)
            {
                PositionBudgetFundAllocations = positionFundService.getPositionBudgetFundAllocation(positionBudgetID.Value).ToList();
            }
            return PositionBudgetFundAllocations;

        }

        internal PositionFundsVm GetNewPositionBudgetFundAllocation()
        {
           return positionFundService.getNewPositionBudgetFundAllocation();
        }

        internal string savePositionBudgetFund(PositionFundsVm positionBudgetFundsVM)
        {
            return positionFundService.savePositionBudgetFund(positionBudgetFundsVM);
        }


        internal FundHistoryVm getNewFundHistory()
        {
           return positionFundService.getNewFundHistory();
        }

        internal string SavePositionFundHistory(FundHistoryAddVm fundHistoryVm)
        {
            return positionFundService.SavePositionFundHistory(fundHistoryVm);
        }

        internal string SaveBudgeMonthAmount(PositionBudgetMonths positionBudgetMonths)
        {
            string returnmsg = string.Empty;
            try
            {
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    var budgetEntityList = clientDbContext.PositionsBudgetsMonths.Where(a => a.PositionBudgetsID == positionBudgetMonths.PositionBudgetsID).ToList();
                    var entity = budgetEntityList.FirstOrDefault(x => x.ID == positionBudgetMonths.ID);
                    entity.BudgetAmount = positionBudgetMonths.BudgetAmount;
                    clientDbContext.SaveChanges();
                    var budget = clientDbContext.PositionsBudgets.FirstOrDefault(x => x.ID == positionBudgetMonths.PositionBudgetsID);
                    budget.BudgetAmount = budgetEntityList.Select(x => x.BudgetAmount).Sum();
                    clientDbContext.SaveChanges();
                    returnmsg = positionBudgetMonths.PositionBudgetsID.ToString();
                }
            }
            catch (System.Exception ex) { returnmsg = ex.ToString(); }
            return returnmsg;
        }

        internal PositionFundingSourceGroupVM getNewPositionFundingSource()
        {
            return positionFundingSourceService.getNewPositionFundingSource();
        }

        internal string SaveFundingSourceList(List<PosionFundingSourceListVM> sources, DateTime effectiveDate)
        {
            return positionFundingSourceService.SaveFundingSourceList(sources, effectiveDate);
        }



        #region Budget Save


        #endregion
    }
}