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

    public class PositionFundService
    {
        readonly string connString = HttpContext.Current.User.Identity.GetClientConnectionString();
        public IEnumerable<PositionsFundHistoryVm> getAllPositionFunds()
        {
            IEnumerable<PositionsFundHistoryVm> positionFunds;
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            using (clientDbContext)
            {
                positionFunds = clientDbContext.PositionsFundHistory
                  .Select(x => new PositionsFundHistoryVm
                  {
                      ID = x.ID,
                      FundHistoryID = x.FundHistoryID,
                      PositionBudgetID = x.PositionBudgetID,
                      PositionAmount = x.PositionAmount

                  }).ToList();
            }

            var fundHistoryList = getAllFundHistory();
            foreach (var positionFund in positionFunds)
            {
                positionFund.BudgetTitle = positionFund.PositionBudgetID.ToString();
                positionFund.FundCode = fundHistoryList.SingleOrDefault(a => a.ID == positionFund.FundHistoryID).FundCode;
                positionFund.FundDescription = fundHistoryList.SingleOrDefault(a => a.ID == positionFund.FundHistoryID).FundDescription;
            }
            return positionFunds;
        }


        public IEnumerable<PositionFundsVm> getAllBudgetFunds()
        {
            IEnumerable<PositionFundsVm> positionFunds;
            IEnumerable<PositionBudgets> positionBudgets = null;
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            using (clientDbContext)
            {
                 positionBudgets = clientDbContext.PositionsBudgets.ToList();
                positionFunds = clientDbContext.PositionFunds
                  .Select(x => new PositionFundsVm
                  {
                      Amount = x.Amount,
                      FundID = x.FundID,
                      PositionBudgetID = x.PositionBudgetID,

                  }).ToList();
            }

            var fundHistoryList = getAllFunds();
           
            foreach (var positionFund in positionFunds)
            {
                positionFund.BudgetTitle = positionFund.PositionBudgetID.ToString();
                positionFund.FundCode = fundHistoryList.SingleOrDefault(a => a.ID == positionFund.FundID).code;
                positionFund.FundDescription = fundHistoryList.SingleOrDefault(a => a.ID == positionFund.FundID).Description;
                positionFund.PositionID = positionBudgets.Where(b => b.ID == positionFund.PositionBudgetID).Select(a => a.PositionID).FirstOrDefault();
            }
            return positionFunds;
        }


        public IEnumerable<PositionsFundHistoryVm> getPositionFunds(int positionBudgetID)
        {
            IEnumerable<PositionsFundHistoryVm> positionFunds = getAllPositionFunds().Where(x => x.PositionBudgetID == positionBudgetID);
            return positionFunds;
        }

        public PositionsFundHistoryVm getPositionFund(int ID)
        {
            PositionsFundHistoryVm positionsFundHistory = getAllPositionFunds().SingleOrDefault(x => x.ID == ID);
            return positionsFundHistory;
        }

        public IEnumerable<FundsVm> getAllFunds()
        {
            IEnumerable<FundsVm> positionFunds;
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            using (clientDbContext)
            {
                positionFunds = clientDbContext.Funds
                  .Select(x => new FundsVm
                  {
                      ID = x.ID,
                      Description = x.Description,
                      code = x.Code,
                      Active = x.Active

                  }).ToList();
            }
            return positionFunds.Where(x=>x.Active);

        }

        public IEnumerable<FundHistoryVm> getAllFundHistory()
        {
            IEnumerable<FundHistoryVm> fundHistory;
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            using (clientDbContext)
            {
                fundHistory = clientDbContext.FundHistory
                  .Select(x => new FundHistoryVm
                  {
                      ID = x.ID,
                      FundID = x.FundID,
                      EffectiveDate = x.EffectiveDate,
                      Amount = x.Amount

                  }).ToList();
            }
            var funds = getAllFunds();
            foreach (var history in fundHistory)
            {
                history.FundDescription = funds.SingleOrDefault(a => a.ID == history.FundID).Description;
                history.FundCode = funds.SingleOrDefault(a => a.ID == history.FundID).code;
            }


            return fundHistory;

        }

        public IEnumerable<PositionBudgetFundAllocationVm> getPositionBudgetFundAllocation(int positionBudgetID)
        {
            IEnumerable<PositionBudgetFundAllocationVm> PositionBudgetFundAllocation;
            //Modification Improvement
            IEnumerable<PositionFundsVm> positionFunds;
            IEnumerable<PositionBudgets> positionBudgets = null;
            IEnumerable<FundsVm> fundHistoryList = null;
            IEnumerable<FundHistoryVm> fundHistory;
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            using (clientDbContext)
            {
                positionBudgets = clientDbContext.PositionsBudgets.ToList();

                positionFunds = clientDbContext.PositionFunds
                  .Select(x => new PositionFundsVm
                  {
                      Amount = x.Amount,
                      FundID = x.FundID,
                      PositionFundID = x.PositionFundID,
                      PositionBudgetID = x.PositionBudgetID,

                  }).Where(m => m.PositionBudgetID== positionBudgetID).ToList();

                fundHistoryList = clientDbContext.Funds
                 .Select(x => new FundsVm
                 {
                     ID = x.ID,
                     Description = x.Description,
                     code = x.Code,
                     Active = x.Active

                 }).ToList();

                fundHistory = clientDbContext.FundHistory
                      .Select(x => new FundHistoryVm
                      {
                          ID = x.ID,
                          FundID = x.FundID,
                          EffectiveDate = x.EffectiveDate,
                          Amount = x.Amount

                      }).ToList();


            }
            foreach (var history in fundHistory)
            {
                history.FundDescription = fundHistoryList.FirstOrDefault(a => a.ID == history.FundID).Description;
                history.FundCode = fundHistoryList.FirstOrDefault(a => a.ID == history.FundID).code;
            }

            foreach (var positionFund in positionFunds)
            {
                positionFund.BudgetTitle = positionFund.PositionBudgetID.ToString();
                positionFund.FundCode = fundHistoryList.FirstOrDefault(a => a.ID == positionFund.FundID).code;
                positionFund.FundDescription = fundHistoryList.FirstOrDefault(a => a.ID == positionFund.FundID).Description;
                positionFund.PositionID = positionBudgets.FirstOrDefault(b => b.ID == positionFund.PositionBudgetID).PositionID;
                positionFund.PositionFundID = positionFund.PositionFundID;
            }


            var allPositionFunds = positionFunds.ToList();
            int positionID = allPositionFunds.Where(x => x.PositionBudgetID == positionBudgetID).Select(x => x.PositionID).FirstOrDefault();
            var positionFundList = allPositionFunds.Where(x => x.PositionID == positionID).ToList();// 
            var budgetFundList = allPositionFunds.Where(x => x.PositionBudgetID == positionBudgetID).ToList();
            var allFundHistory = fundHistory;
            var fundCodes = positionFunds.Select(a => a.FundCode).Distinct().ToList();

            PositionBudgetFundAllocation = fundCodes.Select(x => new PositionBudgetFundAllocationVm
            {
                FundCode = x.ToString(),
                //  Funds Stored into Fund History
                TotalFund = allFundHistory.Where(a => a.FundCode == x).Select(a => a.Amount).Sum(),
                //  Funds Allocated  for this Position
                PositionAllocation = positionFundList.Where(a => a.FundCode == x).Select(a => a.Amount).Sum(),
                // Fund Allocated for All Positions
                TotalAllocation = allPositionFunds.Where(a => a.FundCode == x).Select(a => a.Amount).Sum(),
                // Fund Allocated For This Budget
                Budget = budgetFundList.Where(a => a.FundCode == x).Select(a => a.Amount).Sum(),


            }).ToList();

            return PositionBudgetFundAllocation;
        }
        internal FundHistoryVm getNewFundHistory()
        {
            var fundHistory = new FundHistoryVm();
            fundHistory.FundDefinitionList = getAllFundHistory().ToList();
            return fundHistory;

        }

        public IEnumerable<PositionBudgetFundAllocationVm> getAllPositionBudgetFundAllocation()
        {
            IEnumerable<PositionBudgetFundAllocationVm> PositionBudgetFundAllocation = null;
            var allPositionFunds = getAllPositionFunds();
            var budgets = allPositionFunds.Select(a => a.PositionBudgetID).Distinct().ToList();
            foreach (var positionBudgetID in budgets)
            {
                PositionBudgetFundAllocation = getPositionBudgetFundAllocation(positionBudgetID).ToList();

            }
            return PositionBudgetFundAllocation;
        }
       
        internal string savePositionBudgetFund(PositionFundsVm positionBudgetFundsVM)
        {
            string returnmsg = "";
            var newitem = new PositionFund
            {
                FundID = positionBudgetFundsVM.FundID,
                PositionBudgetID = positionBudgetFundsVM.PositionBudgetID,
                Amount = positionBudgetFundsVM.Amount
            };
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            var PositionBudgetRecord = clientDbContext.PositionsBudgets.Where(m => m.ID == positionBudgetFundsVM.PositionBudgetID).FirstOrDefault();
            var FundRecord = clientDbContext.Funds.Where(m => m.ID == positionBudgetFundsVM.FundID).FirstOrDefault();
            var AllocationRecord = clientDbContext.PositionFunds.Where(m => m.FundID == positionBudgetFundsVM.FundID && m.PositionBudgetID== positionBudgetFundsVM.PositionBudgetID).FirstOrDefault();
            //var fundHistory = GetFundHistory(positionBudgetFundsVM.FundID);
            var fundHistory = getPositionBudgetFundAllocation(newitem.PositionBudgetID).ToList();
            try
            {
                if (positionBudgetFundsVM.Amount> FundRecord.Amount)
                {
                    return "Fund Allocation should not exceed Total amount of Fund Code.";
                }
                else if (positionBudgetFundsVM.Amount> PositionBudgetRecord.BudgetAmount)
                {
                    return "Fund Allocation should not exceed Total amount of Budget line.";
                }
                else
                {
                    if (AllocationRecord==null)
                    {
                        clientDbContext.PositionFunds.Add(newitem);
                        clientDbContext.SaveChanges();
                        returnmsg = newitem.PositionBudgetID.ToString();
                    }
                    else
                    {
                        AllocationRecord.Amount = positionBudgetFundsVM.Amount;
                        clientDbContext.SaveChanges();
                        returnmsg = newitem.PositionBudgetID.ToString();
                    }
                   
                }
                //        var fundCode = clientDbContext.Funds.FirstOrDefault(x => x.ID == newitem.FundID).Code;
                //        var budgetAmount = clientDbContext.PositionsBudgets.FirstOrDefault(x => x.ID == newitem.PositionBudgetID).BudgetAmount;
                //        var allocations = fundHistory.FirstOrDefault(x => x.FundCode == fundCode);
                //        //Start Check
                //        var FundAllocation = allocations.TotalAllocation;
                //        var TotalFund = allocations.TotalFund;
                //        var BudgetAllocations = fundHistory.Select(x => x.Budget).Sum();

                //        if (TotalFund < (FundAllocation + newitem.Amount))
                //        {
                //            return "Fund Allocation should not exceed Total amount of Fund Code.";
                //        }
                //        else if (budgetAmount < (BudgetAllocations + newitem.Amount))
                //        {
                //            return "Fund Allocation should not exceed Total amount of Budget line.";
                //        }
                //        //End Check
                //        clientDbContext.PositionFunds.Add(newitem);
                //        clientDbContext.SaveChanges();
                //        returnmsg = newitem.PositionBudgetID.ToString();

            }
            catch (System.Exception ex) { returnmsg = "Amount is not defined for the fund."; }
            return returnmsg;
        }

        internal string SavePositionFundHistory(FundHistoryAddVm fundHistoryVm)
        {
            string returnmsg = "";
            var fundHistory = new FundHistory
            {
                FundID = 0,
                Amount = fundHistoryVm.Amount.Value,
                EffectiveDate = fundHistoryVm.EffectiveDate


            };

            var Fund = new Funds
            {
                Code = fundHistoryVm.FundCode,
                Description = fundHistoryVm.FundDescription,
                Active = true

            };
            try
            {
                using (ClientDbContext clientDbContext = new ClientDbContext(connString))
                {
                    var existingID = clientDbContext.Funds.Where(x => x.Code == Fund.Code.Trim() && x.Description == Fund.Description.Trim()).Select(x => x.ID);
                    if (existingID.Count() == 0)
                    {
                        clientDbContext.Funds.Add(Fund);
                        clientDbContext.SaveChanges();
                        fundHistory.FundID = Fund.ID;
                    }
                    else
                    {
                        fundHistory.FundID = existingID.FirstOrDefault();
                    }

                    clientDbContext.FundHistory.Add(fundHistory);
                    clientDbContext.SaveChanges();
                    returnmsg = fundHistory.FundID.ToString();
                }
            }
            catch (System.Exception ex) { returnmsg = ex.ToString(); }
            return returnmsg;
        }

        internal PositionFundsVm getNewPositionBudgetFundAllocation()
        {
            var fundAllocation = new PositionFundsVm();
            fundAllocation.FundCodes = getAllFunds().Select(
                x=> new DropDownModel { keyvalue = x.ID.ToString() ,keydescription= x.Description})
                .ToList();
            return fundAllocation;
        }


       
    }     
}