using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecViewHrk.Models;

namespace ExecViewHrk.Domain.Interface
{
    public interface ITestRepository : IDisposable, IBaseRepository
    {
        List<DropDownModel> GetEvaluationTestList();
        List<PersonTestVm> GetPersonTestList(int _personId);
        void DeletePersonTest(int _personTestId);
        PersonTestVm GetPersonTestDetail(int _personId);

    }
}
