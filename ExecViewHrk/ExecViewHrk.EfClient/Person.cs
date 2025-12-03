namespace ExecViewHrk.EfClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Persons")]
    public partial class Person
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Person()
        {
            Employees = new HashSet<Employee>();
            Managers = new HashSet<Manager>();
            PersonADAs = new HashSet<PersonADA>();
            PersonAdditionals = new HashSet<PersonAdditional>();
            PersonAddresses = new HashSet<PersonAddress>();
            PersonDisabilities = new HashSet<PersonDisability>();
            PersonEducations = new HashSet<PersonEducation>();
            PersonExaminations = new HashSet<PersonExamination>();
            PersonImages = new HashSet<PersonImage>();
            PersonInnoculations = new HashSet<PersonInnoculation>();
            PersonLicenses = new HashSet<PersonLicens>();
            PersonMemberships = new HashSet<PersonMembership>();
            PersonPassports = new HashSet<PersonPassport>();
            PersonPhoneNumbers = new HashSet<PersonPhoneNumber>();
            PersonProperties = new HashSet<PersonProperty>();
            PersonRelationships = new HashSet<PersonRelationship>();
            PersonRelationships1 = new HashSet<PersonRelationship>();
            PersonSkills = new HashSet<PersonSkill>();
            PersonTests = new HashSet<PersonTest>();
            PersonTrainings = new HashSet<PersonTraining>();
            PersonVehicles = new HashSet<PersonVehicle>();
            UserNamesPersons = new HashSet<UserNamesPerson>();
        }

        public int PersonId { get; set; }

        //[Required]
       // [StringLength(9)]
        public string SSN { get; set; }

        public bool? ShowSSN { get; set; }

        [Required]
        [StringLength(50)]
        public string Lastname { get; set; }

        [Required]
        [StringLength(50)]
        public string Firstname { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        public int? PrefixId { get; set; }

        public int? SuffixId { get; set; }

        [StringLength(50)]
        public string PreferredName { get; set; }

        [StringLength(100)]
        public string eMail { get; set; }

        [StringLength(100)]
        public string AlternateEMail { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DOB { get; set; }

        public int? GenderId { get; set; }

        public int? ActualMaritalStatusId { get; set; }

        [StringLength(50)]
        public string MaidenName { get; set; }

        public bool? IsDependent { get; set; }
        public bool? IsStudent { get; set; }
        public bool? IsTrainer { get; set; }
        public bool? IsApplicant { get; set; }


        [StringLength(50)]
        public string EnteredBy { get; set; }

        public DateTime? EnteredDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> Employees { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Manager> Managers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonADA> PersonADAs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonAdditional> PersonAdditionals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonAddress> PersonAddresses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonDisability> PersonDisabilities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonEducation> PersonEducations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonExamination> PersonExaminations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonImage> PersonImages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonInnoculation> PersonInnoculations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonLicens> PersonLicenses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonMembership> PersonMemberships { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonPassport> PersonPassports { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonPhoneNumber> PersonPhoneNumbers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonProperty> PersonProperties { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonRelationship> PersonRelationships { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonRelationship> PersonRelationships1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonSkill> PersonSkills { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonTest> PersonTests { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonTraining> PersonTrainings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonVehicle> PersonVehicles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserNamesPerson> UserNamesPersons { get; set; }
    }
}
