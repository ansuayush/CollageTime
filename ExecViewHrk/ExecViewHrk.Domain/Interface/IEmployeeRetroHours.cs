using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Domain.Interface
{
   public interface IEmployeeRetroHours : IDisposable, IBaseRepository
    {

        void RetroDelete(int RetroHourId);
    }
}
