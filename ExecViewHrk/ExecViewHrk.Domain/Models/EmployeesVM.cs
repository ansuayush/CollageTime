using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.Models
{
    public class EmployeesVM
    {
        public int EmployeeId { get; set; }

        public int PersonId { get; set; }

        [StringLength(50)]
        public string CompanyCode { get; set; }

        public decimal? Rate { get; set; }

        public int? RateTypeId { get; set; }

        [StringLength(50)]
        public string FileNumber { get; set; }

        public byte EmploymentNumber { get; set; }

        public byte? EmploymentStatusId { get; set; }
        public string StatusCode { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime HireDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? TerminationDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? PlannedServiceStartDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ActualServiceStartDate { get; set; }

        public short? EmployeeTypeID { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ProbationEndDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? TrainingEndDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? SeniorityDate { get; set; }

        public int? PayFrequencyId { get; set; }

        [StringLength(2)]
        public string FedExemptions { get; set; }

        public byte? WorkedStateTaxCodeId { get; set; }

        public int? MaritalStatusID { get; set; }

        public decimal? Hours { get; set; }

        [StringLength(50)]
        public string EnteredBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? EnteredDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? ModifiedDate { get; set; }

        public short? DepartmentId { get; set; }

        public short? CompanyCodeId { get; set; }

        public short? TimeCardTypeId { get; set; }
        public string EmployeeFullName { get; set; }
        public string EmployeeRole { get; set; }
        public string PersonName { get; set; }

        public int deptid { get; set; }
        public bool IsPostionEnded { get; set; }
    }
}
