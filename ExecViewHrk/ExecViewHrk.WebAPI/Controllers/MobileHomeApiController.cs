using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Domain.Models;
using System;
using System.Web.Http;

namespace ExecViewHrk.WebAPI.Controllers
{
    [RoutePrefix("api/mobilehome")]
    public class MobileHomeApiController : ApiController
    {
        #region Private Members
        private IEPositionRepository _positionRepo;
        private ITimeCardsMobileRepository _timecardMobileRepo;
        private ILookupTablesRepository _ilookuprepo;
        private ITimeCardMatrixReposotory _timeCardsMatrixRepo;
        #endregion

        public MobileHomeApiController(
            IEPositionRepository positionRepo,
            ITimeCardsMobileRepository timecardMobileRepo,
            ILookupTablesRepository ilookuprepo,
            ITimeCardMatrixReposotory timeCardsMatrixRepo
            )
        {
            _positionRepo = positionRepo;
            _timecardMobileRepo = timecardMobileRepo;
            _ilookuprepo = ilookuprepo;
            _timeCardsMatrixRepo = timeCardsMatrixRepo;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok("Hello World");
        }

        #region Position Repo

        [HttpGet]
        [Route("getepositionlist/{personId}/{empId}")]
        public IHttpActionResult GetEPositionList(int personId, int empId)
        {
            try
            {
                return Ok(_positionRepo.GetEPositionList(personId, empId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        #endregion

        #region Timecard Mobile Repo

        [HttpGet]
        [Route("getemployeetimecardbydate/{employeeId}/{positionId}/{punchTime}/{companyCodeId/personId}")]
        public IHttpActionResult GetEmployeeTimeCardByDate(int employeeId, int positionId, DateTime punchTime, int companyCodeId, int personId)
        {
            try
            {
                if (positionId == -1)
                    return Ok(_timecardMobileRepo.GetEmployeeTimeCardByDate(employeeId, null, punchTime, companyCodeId, personId));
                else
                    return Ok(_timecardMobileRepo.GetEmployeeTimeCardByDate(employeeId, positionId, punchTime, companyCodeId, personId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("insertemployeepositionpunch")]
        public IHttpActionResult InsertEmployeePositionPunch(TimeCardPayload payload)
        {
            try
            {
                //var epositionlist = Query<TimeCardVm>("GetE_PositionIdbasedonpunchdate", new { @EmployeeId = payload.EmployeeId, @positionId = payload.PositionId, @Companycodeid = payload.CompanyCode }).ToList();
                //var epositionid = epositionlist.Select(x => x.E_PositionId).FirstOrDefault();





                return Ok(_timecardMobileRepo.InsertEmployeePositionPunch(payload.EmployeeId, payload.PositionId, payload.PunchType, payload.PunchTime, payload.CompanyCode, payload.UserName, payload.FileName, null,0,1));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("getemployeetimecardsbypayperiod/{employeeId}/{payPeriodId}/{isArchived}/")]
        public IHttpActionResult GetEmployeeTimeCardsByPayPeriod(int employeeId, int payPeriodId, bool isArchived)
        {
            try
            {
                return Ok(_timecardMobileRepo.GetEmployeeTimeCardsByPayPeriod(employeeId, payPeriodId, isArchived));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        #endregion

        #region Lookup Repo

        [HttpGet]
        [Route("getemployeecurrentpayperiod/{employeeId}/{isArchived}/")]
        public IHttpActionResult GetEmployeeCurrentPayPeriod(int employeeId, bool isArchived)
        {
            try
            {
                return Ok(_ilookuprepo.GetEmployeeCurrentPayPeriod(employeeId, isArchived));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        [HttpGet]
        [Route("getpayperiodlist/{employeeId}/{isArchived}/")]
        public IHttpActionResult GetPayPeriodList(int employeeId, bool isArchived)
        {
            try
            {
                return Ok(_ilookuprepo.GetPayPeriodsList(employeeId, isArchived));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        #endregion

        #region TimeCard Matrix Repo

        [HttpGet]
        [Route("gettimecardweektotallist/{empid}/{departmentId}/{payPeriodId}/{isArchive}/")]
        public IHttpActionResult GetTimeCardWeekTotalList(int empid, int departmentId, int payPeriodId, bool isArchive)
        {
            try
            {
                if (departmentId == -1)
                    return Ok(_timeCardsMatrixRepo.GetTimeCardWeekTotalList(empid, null, payPeriodId, isArchive));
                else
                    return Ok(_timeCardsMatrixRepo.GetTimeCardWeekTotalList(empid, departmentId, payPeriodId, isArchive));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        #endregion
    }
}