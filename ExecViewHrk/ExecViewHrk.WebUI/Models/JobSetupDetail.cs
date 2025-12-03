using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class JobSetupDetail
    {
        public int id { get; set; }

        public int Jobid { get; set; }
        [Required]
        [Display(Name = "Job Title")]
        public string title { get; set; }

        [Required]
        [Display(Name = "Job Code")]
        public string JobCode { get; set; }

        public string hdJobCode { get; set; }

        [StringLength(100)]
        public string JobDescription { get; set; }


        [StringLength(50)]
        public string jobClassID { get; set; }
        public string jobClassIDDescription { get; set; }

        public string workersCompensationID { get; set; }
        public string workersCompensationIDDescription { get; set; }
        public string eeoJobCodeID { get; set; }
        public string eeoJobCodeIDDescription { get; set; }
        public string eeoJobTrainingStatusID { get; set; }
        public string eeoJobTrainingStatusIDDescription { get; set; }
        public string FLSAID { get; set; }
        public string FLSAIDDescription { get; set; }
        public string unionID { get; set; }
        public string unionIDDescription { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? endDate { get; set; }
        public DateTime? lastEvaluationDate { get; set; }
        public DateTime? enteredDate { get; set; }
        public string salaryRange { get; set; }
        public string enteredBy { get; set; }
        public string Notes { get; set; }
        public string requirements { get; set; }
        public short? JobFamilyId { get; set; }
        public string JobFamilyIdDescription { get; set; }
        [Required]
        [Display(Name = "Company Code")]
        public string CompanyCodeID { get; set; }
        public string CompanyCodeIdDescription { get; set; }
        public List<ExecViewHrk.EfClient.ddlUnions> UnionsList { get; set; }
        public List<ExecViewHrk.EfClient.ddlWorkersCompensations> WorkersCompensationsList { get; set; }
        public List<ExecViewHrk.EfClient.ddlEEOJobCodes> EEOJobCodesList { get; set; }
        public List<ExecViewHrk.EfClient.ddlJobClasses> JobClassList { get; set; }
        public List<ExecViewHrk.EfClient.ddlEEOJobTrainingStatuses> EEOJobTrainingStatusesList { get; set; }
        public List<ExecViewHrk.EfClient.ddlFLSAs> FLSAList { get; set; }
        public List<ExecViewHrk.EfClient.ddlJobFamilys> JobFamilyList { get; set; }

        public List<ExecViewHrk.EfClient.CompanyCode> CompanyCodeList { get; set; }
    }
}