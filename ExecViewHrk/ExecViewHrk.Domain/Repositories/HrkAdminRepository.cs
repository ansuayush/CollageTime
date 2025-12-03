using ExecViewHrk.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Repositories
{
    public class HrkAdminRepository : RepositoryBase, IHrkAdminRepository
    {
        public string PartialReset()
        {
            Execute("USP_RESET_DATA");
            return "OK";
        }
    }
}
