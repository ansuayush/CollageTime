using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ExecViewHrk.WebUI.Models
{
    public class PersonAdaVm
    {

        public int PersonAdaId { get; set; }

        [Required]
        public int PersonId { get; set; }
        public string PersonName { get; set; }

        [Required(ErrorMessage = "The Accommodation Description field is required.")]
        public int AccommodationTypeId { get; set; }

        //[Required(ErrorMessage = "The Accommodation Description field is required.")]
        public string AccommodationDescription { get; set; }
        [Required(ErrorMessage = "The Associated Disability Description field is required.")]

        public int AssociatedDisabilityTypeId { get; set; }

        //[Required(ErrorMessage = "The Associated Disability Description field is required.")]
        public string AssociatedDisabilityDescription { get; set; }

        public DateTime? RequestedDate { get; set; }
        public Decimal? EstimatedCost { get; set; }
        public bool? AccommodationProvided { get; set; }
        public DateTime? ProvidedDate { get; set; }
        public Decimal? ActualCost { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        public string EnteredBy { get; set; }
        public DateTime? EnteredDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<ExecViewHrk.EfClient.DdlDisabilityType> DisabilityList { get; set; }

        public List<ExecViewHrk.EfClient.DdlAccommodationType> AccomodationList { get; set; }

        public int DisabilityTypeId { get; set; }

    }
}