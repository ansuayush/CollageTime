using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace ExecViewHrk.Domain.Repositories
{
    public class TestRepository : RepositoryBase, ITestRepository
    {
        public void DeletePersonTest(int _personTestId)
        {
            var dbRecord = _context.PersonTests.Where(x => x.PersonTestId == _personTestId).FirstOrDefault();
            if (dbRecord != null)
            {
                _context.PersonTests.Remove(dbRecord);

                _context.SaveChanges();

            }
        }

        public List<DropDownModel> GetEvaluationTestList()
        {
            var list = _context.DdlEvaluationTests.Where(x => x.Active == true)
                .Select(m => new DropDownModel
                {
                    keyvalue = m.EvaluationTestId.ToString(),
                    keydescription = m.Description
                })
                .OrderBy(x => x.keydescription)
                .ToList();
            return list;
        }
        public List<PersonTestVm> GetPersonTestList(int _personId)
        {
            var list = _context.PersonTests
                .Include(x => x.DdlEvaluationTest)
                .Include(x => x.Person)
                .Where(x => x.PersonId == _personId)
                                .Select(x => new PersonTestVm
                                {
                                    PersonTestId = x.PersonTestId,
                                    PersonId = x.PersonId,
                                    PersonName = x.Person.Lastname + ", " + x.Person.Firstname,
                                    EvaluationTestId = x.EvaluationTestId,
                                    EvaluationTestDescription = x.DdlEvaluationTest.Description,
                                    TestDate = x.TestDate,
                                    Score = x.Score,
                                    Grade = x.Grade,
                                    Administrator = x.Administrator,
                                    EnteredBy = x.EnteredBy,
                                    EnteredDate = x.EnteredDate,
                                    //ModifiedBy = x.ModifiedBy,
                                    //ModifiedDate = x.ModifiedDate,
                                    Notes = x.Notes,
                                }).ToList();
            return list;
        }
        public PersonTestVm GetPersonTestDetail(int _personTestId)
        {
            var row = _context.PersonTests
               .Include(x => x.DdlEvaluationTest.Description)
               .Where(x => x.PersonTestId == _personTestId)
               .Select(x => new PersonTestVm
               {
                   PersonTestId = x.PersonTestId,
                   EvaluationTestId = x.EvaluationTestId,
                   EvaluationTestDescription = x.DdlEvaluationTest.Description,
                   TestDate = x.TestDate,
                   Score = x.Score,
                   Grade = x.Grade,
                   Administrator = x.Administrator,
                   EnteredBy = x.EnteredBy,
                   EnteredDate = x.EnteredDate,
                   Notes = x.Notes,
               })
               .FirstOrDefault();
            return row;
        }
    }
}