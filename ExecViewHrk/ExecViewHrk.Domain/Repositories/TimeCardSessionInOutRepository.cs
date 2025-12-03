using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.EfClient;
using ExecViewHrk.Models;
using ExecViewHrk.Domain.Interface;

namespace ExecViewHrk.Domain.Repositories
{
  public class TimeCardSessionInOutRepository:RepositoryBase,ITimeCardSessionInOutRepository
    {

        public List<TimeCardSessionInOutConfigsVm> GetTimeCardSessionList()
        {
            try
            {
                var list = _context.TimeCardSessionInOutConfigs
                    .Select(tc => new TimeCardSessionInOutConfigsVm()
                    {
                        TimeCardSessionId = tc.TimeCardSessionId,
                        Toggle = tc.Toggle,
                     Session=tc.Session,
                     MaxHours=tc.MaxHours
                      
                    }).ToList();
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public TimeCardSessionInOutConfigsVm GetTimecradssessiondeatils(int timecardssessionId)
        {
            TimeCardSessionInOutConfigsVm tcsvm = new TimeCardSessionInOutConfigsVm();
            tcsvm = (from tcvm in _context.TimeCardSessionInOutConfigs
                     where(tcvm.TimeCardSessionId==timecardssessionId)
                     select new TimeCardSessionInOutConfigsVm
                     {
                         TimeCardSessionId=tcvm.TimeCardSessionId,
                         Toggle = tcvm.Toggle,
                       Session=tcvm.Session,
                       MaxHours=tcvm.MaxHours
                     }).FirstOrDefault();
            return tcsvm;
        }
        public bool updatetimecardsseiion(TimeCardSessionInOutConfigsVm tcsvm)
        {
            var status = false;
            var tcsvmdetails = _context.TimeCardSessionInOutConfigs
                        .Where(x => x.TimeCardSessionId == tcsvm.TimeCardSessionId)
                        .SingleOrDefault();
            try
            {
                if (tcsvmdetails != null)
                {
                    tcsvmdetails.Toggle = tcsvm.Toggle;
                    tcsvmdetails.MaxHours = tcsvm.MaxHours;
                    tcsvmdetails.Session = tcsvm.Session;
                    _context.SaveChanges();
                    status = true;
                }                 
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return status;
        }
    }
}
