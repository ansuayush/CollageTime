using ExecViewHrk.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Models;

namespace ExecViewHrk.Domain.Repositories
{
    public class EarningsCodeRepository : RepositoryBase, IEarningsCodeRepository
    {
        public EarningCodeVm GetEarningsCodeDetails(int earningsCodeId)
        {
            EarningCodeVm earningCodeVm = new EarningCodeVm();
            try
            {
                if (earningsCodeId != 0)
                {
                    earningCodeVm = _context.EarningsCodes.Where(e => e.EarningsCodeId == earningsCodeId).Select(e => new EarningCodeVm
                    {
                        EarningsCodeId = e.EarningsCodeId,
                        CompanyCodeId = e.CompanyCodeId,
                        EarningsCode = e.EarningsCodeCode,
                        EarningsCodeDescription = e.EarningsCodeDescription,
                        ADPFieldMappingId = e.ADPFieldMappingId,
                        EarningsCodeOffset = e.EarningsCodeOffset,
                        DeductionCodeOffset = e.DeductionCodeOffset,
                        Active = e.IsEarningsCodeActive,
                        TreatyCode=e.TreatyCode,
                        IsDefault=e.IsDefault,
                    }).FirstOrDefault();
                }

                return earningCodeVm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EarningCodeVm> GetEarningsCodeList()
        {
            List<EarningCodeVm> getEarningCodeList = new List<EarningCodeVm>();
            try
            {
                getEarningCodeList = _context.EarningsCodes.Select(e => new EarningCodeVm
                {
                    EarningsCodeId = e.EarningsCodeId,
                    CompanyCodeId = e.CompanyCodeId,
                    EarningsCode = e.EarningsCodeCode,
                    EarningsCodeDescription = e.EarningsCodeDescription,
                    ADPFieldMappingId = e.ADPFieldMappingId,
                    Active = e.IsEarningsCodeActive,
                    EarningsCodeOffset = e.EarningsCodeOffset,
                    DeductionCodeOffset = e.DeductionCodeOffset,
                    IsDefault=e.IsDefault
                }).ToList();
                return getEarningCodeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
