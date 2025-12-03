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

    public class PositionFundingSourceService
    {
        readonly string connString = HttpContext.Current.User.Identity.GetClientConnectionString();

        public IEnumerable<DropDownModel> getAllFundCodes()
        {
            IEnumerable<DropDownModel> positionFunds;
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            using (clientDbContext)
            {
                positionFunds = clientDbContext.Funds
                  .Select(x => new DropDownModel
                  {
                      keyvalue = x.ID.ToString(),
                      keydescription = x.Description

                  }).ToList();
            }
            return positionFunds;

        }
        public List<PosionFundingSourceHistoryListVM> getposionFundingSourceHistoryList()
        {
            List<PosionFundingSourceHistoryListVM> getFundingSourceHistoryList = null;
            ClientDbContext clientDbContext = new ClientDbContext(connString);
            getFundingSourceHistoryList = (from PFSH in clientDbContext.PositionFundingSourceHistories
                                           join FD in clientDbContext.Funds
                                    on PFSH.FundCodeID equals FD.ID
                                           where FD.Active == true

                                           where FD.Active == true
                                           select new PosionFundingSourceHistoryListVM
                                           {
                                               EffectiveDate = PFSH.EffectiveDate,
                                               FundCodeID = FD.Description,
                                               Percentage = PFSH.Percentage.ToString(),
                                               ChangeEffectiveDate = PFSH.ChangeEffectiveDate
                                           }).ToList();
            return getFundingSourceHistoryList;
        }
        public IEnumerable<PositionFundingSourceGroupVM> getposionFundingSourceList(int? positionID)
        {
            IEnumerable<PositionFundingSourceGroupVM> fundingGroup = null;
            IEnumerable<PosionFundingSourceListVM> getFundingSourceList = null;
            IEnumerable<PositionFundingSource> allSources = null;
            IEnumerable<Funds> funds = null;
            using (ClientDbContext clientDbContext = new ClientDbContext(connString))
            {
                allSources = clientDbContext.PositionFundingSource.Where(x => x.PositionId == positionID).ToList();
                funds = clientDbContext.Funds.ToList();

            }

            getFundingSourceList = allSources.Join(funds, s => s.FundCodeID, f => f.ID,
              (Source, fund) => new PosionFundingSourceListVM {
                  EffectiveDate = Source.EffectiveDate,
                  ID = Source.PositionFundingSourceID.ToString(),
                  FundCode = fund.Description,
                  FundPercentage = Source.Percentage,
                  PositionId = Source.PositionId
             }).ToList();

            List<PositionFundingSourceGroupVM> EffectiveGroup = new List<PositionFundingSourceGroupVM>();
             getFundingSourceList.Select(x => x.EffectiveDate).Distinct().Each(item =>
            {
                var pfs = new PositionFundingSourceGroupVM();
                pfs.EffectiveDate = item;
                pfs.FundingGroup = item.ToShortDateString().FormatWith("MMDDYYYY");
                pfs.Percentage = 100;
                pfs.PositionId = Convert.ToInt16(positionID);
                pfs.EditposionFundingSourceList = getFundingSourceList.Where(x => x.EffectiveDate == item).ToList();
                EffectiveGroup.Add(pfs);

            });
            fundingGroup = EffectiveGroup.ToList();
            return fundingGroup;
        }

        public PositionFundingSourceGroupVM  getNewPositionFundingSource()
        {
            var newFundingSource = new PositionFundingSourceGroupVM();
            List<PosionFundingSourceListVM> lstFundingSounce = new List<PosionFundingSourceListVM>();
            newFundingSource.FundCodes = getAllFundCodes().ToList();
            newFundingSource.EditposionFundingSourceList = lstFundingSounce.ToList();
            return newFundingSource;
        }

        public  IEnumerable<PosionFundingSourceListVM> addNewFundingSource(IEnumerable<PosionFundingSourceListVM> sources , PosionFundingSourceListVM pFS)
        {
            var list = sources.ToList();
                list.Add(pFS);
            return list;
        }

        public string  SaveFundingSourceList(IEnumerable<PosionFundingSourceListVM> sources ,DateTime effectiveDate)
        {
            var list = sources.ToList();
            var existingEnities = list.Where(x => Convert.ToInt32(x.ID) > 0).ToList();
            string returnmsg = "";
            var percentage = list.Select(x => x.FundPercentage).Sum();
            if(percentage < 100)
            {
               return "Percentage Exceeds.";
            }
            else
            {
                try
                {
                    ClientDbContext clientDbContext = new ClientDbContext(connString);
                    var fsList = clientDbContext.PositionFundingSource.ToList();
                     fsList = fsList.Where(x => x.EffectiveDate == effectiveDate).ToList();
                    var isExists = fsList.Count > 0;
                    if (isExists)
                    {
                        for(int i=0; i< list.Count; i++)
                        {
                            var entity = new PositionFundingSource();
                            if (Convert.ToInt32(list[i].ID )< 0){
                                entity.FundCodeID = Convert.ToByte(list[i].FundCodeID);
                                entity.EffectiveDate = effectiveDate;
                                entity.PositionId = list[i].PositionId;
                                entity.Percentage = Convert.ToByte(list[i].FundPercentage);
                                clientDbContext.PositionFundingSource.Add(entity);
                            }
                            else
                            {
                                entity = fsList.FirstOrDefault(x => x.PositionFundingSourceID == Convert.ToInt32(list[i].ID));
                                entity.FundCodeID = Convert.ToByte(list[i].FundCodeID);
                                entity.EffectiveDate = effectiveDate;
                                entity.PositionId = list[i].PositionId;
                                entity.Percentage = Convert.ToByte(list[i].FundPercentage);

                            }
                            returnmsg = entity.PositionId.ToString();
                        }

                        for (int i = 0; i < fsList.Count; i++)
                        {
                            var entityFound = existingEnities.Where(x => Convert.ToInt32(x.ID) == fsList[i].PositionFundingSourceID);
                            if(entityFound == null)
                            {
                                clientDbContext.PositionFundingSource.Remove(fsList[i]);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            var entity = new PositionFundingSource();
                            if (Convert.ToInt32(list[i].ID) < 0)
                            {
                                entity.FundCodeID = Convert.ToByte(list[i].FundCodeID);
                                entity.EffectiveDate = effectiveDate;
                                entity.PositionId = list[i].PositionId;
                                entity.Percentage = Convert.ToByte(list[i].FundPercentage);
                                clientDbContext.PositionFundingSource.Add(entity);
                            }
                            returnmsg = entity.PositionId.ToString();
                        }
                    }
                  clientDbContext.SaveChanges();
                }

                catch (System.Exception ex) { returnmsg = ex.ToString(); }
                return returnmsg;
            }
        }

    }
}