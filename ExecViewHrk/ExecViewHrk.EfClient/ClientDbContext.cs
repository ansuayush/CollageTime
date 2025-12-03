namespace ExecViewHrk.EfClient
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity.Infrastructure;

    public partial class ClientDbContext : DbContext
    {
        public ClientDbContext(string connString)
            : base(connString)
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
        public ClientDbContext(string connString, int timeoutValue)
            : base(connString)
        {
            this.Configuration.LazyLoadingEnabled = false;

            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            objectContext.CommandTimeout = timeoutValue;

        }

        public ClientDbContext()
        {

        }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<ExternalUserClient> ExternalUserClients { get; set; }
        public virtual DbSet<UserCompany> UserCompanys { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<ADPAccNumber> ADPAccNumbers { get; set; }
        public virtual DbSet<ADPFieldMapping> ADPFieldMappings { get; set; }
        public virtual DbSet<BenefitDeductionCode> BenefitDeductionCodes { get; set; }
        public virtual DbSet<BenefitExpansionField> BenefitExpansionFields { get; set; }
        public virtual DbSet<BenefitGroup> BenefitGroups { get; set; }
        public virtual DbSet<BenefitOESchedule> BenefitOESchedules { get; set; }
        public virtual DbSet<BenefitOption> BenefitOptions { get; set; }
        public virtual DbSet<BenefitProvider> BenefitProviders { get; set; }
        public virtual DbSet<BusinessUnit> BusinessUnits { get; set; }
        public virtual DbSet<CompanyCode> CompanyCodes { get; set; }
        public virtual DbSet<DdlAccommodationType> DdlAccommodationTypes { get; set; }
        public virtual DbSet<DdlAddressType> DdlAddressTypes { get; set; }
        public virtual DbSet<DdlApplicantSource> DdlApplicantSources { get; set; }
        public virtual DbSet<DdlCitizenship> DdlCitizenships { get; set; }
        public virtual DbSet<DdlCountry> DdlCountries { get; set; }
        public virtual DbSet<DdlDegreeType> DdlDegreeTypes { get; set; }
        public virtual DbSet<DdlDisabilityType> DdlDisabilityTypes { get; set; }
        public virtual DbSet<DdlEducationEstablishment> DdlEducationEstablishments { get; set; }
        public virtual DbSet<DdlEducationLevel> DdlEducationLevels { get; set; }
        public virtual DbSet<DdlEducationType> DdlEducationTypes { get; set; }
        public virtual DbSet<DdlEeoType> DdlEeoTypes { get; set; }
        public virtual DbSet<DdlEmployeeType> DdlEmployeeTypes { get; set; }
        public virtual DbSet<DdlEmploymentStatus> DdlEmploymentStatuses { get; set; }
        public virtual DbSet<DdlEvaluationTest> DdlEvaluationTests { get; set; }
        public virtual DbSet<DdlGender> DdlGenders { get; set; }
        public virtual DbSet<DdlHospital> DdlHospitals { get; set; }
        public virtual DbSet<DdlI9DocumentTypes> DdlI9DocumentTypes { get; set; }
        public virtual DbSet<DdlInnoculationType> DdlInnoculationTypes { get; set; }
        public virtual DbSet<DdlLicenseType> DdlLicenseTypes { get; set; }
        public virtual DbSet<DdlMajor> DdlMajors { get; set; }
        public virtual DbSet<DdlMaritalStatus> DdlMaritalStatuses { get; set; }
        public virtual DbSet<DdlMedicalExaminationType> DdlMedicalExaminationTypes { get; set; }
        public virtual DbSet<DdlPayFrequency> DdlPayFrequencies { get; set; }
        public virtual DbSet<DdlPhoneType> DdlPhoneTypes { get; set; }
        public virtual DbSet<DdlPrefix> DdlPrefixes { get; set; }
        public virtual DbSet<DdlProfessionalBody> DdlProfessionalBodies { get; set; }
        public virtual DbSet<DdlPropertyType> DdlPropertyTypes { get; set; }
        public virtual DbSet<DdlQualificationType> DdlQualificationTypes { get; set; }
        public virtual DbSet<DdlRateType> DdlRateTypes { get; set; }
        public virtual DbSet<DdlRegionalChapter> DdlRegionalChapters { get; set; }
        public virtual DbSet<DdlRelationshipType> DdlRelationshipTypes { get; set; }
        public virtual DbSet<DdlSkillLevel> DdlSkillLevels { get; set; }
        public virtual DbSet<DdlBusinessLevelTypes> DdlBusinessLevelTypes { get; set; }
        public virtual DbSet<DdlEEOFileStatuses> DdlEEOFileStatuses { get; set; }
        public virtual DbSet<DdlEINs> DdlEINs { get; set; }
        public virtual DbSet<DdlSkill> DdlSkills { get; set; }
        public virtual DbSet<DdlSkillType> DdlSkillTypes { get; set; }
        public virtual DbSet<DdlState> DdlStates { get; set; }
        public virtual DbSet<DdlSuffix> DdlSuffixes { get; set; }
        public virtual DbSet<DdlSupplier> DdlSuppliers { get; set; }
        public virtual DbSet<DdlTimeCardType> DdlTimeCardTypes { get; set; }
        public virtual DbSet<DdlTrainingCours> DdlTrainingCourses { get; set; }
        public virtual DbSet<DdlTrainingCoursesGroup> DdlTrainingCoursesGroups { get; set; }
        public virtual DbSet<DdlUnit> DdlUnits { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<E_Positions> E_Positions { get; set; }
        public virtual DbSet<E_PositionSalaryHistories> E_PositionSalaryHistories { get; set; }
        public virtual DbSet<EarningsCode> EarningsCodes { get; set; }
        public virtual DbSet<EmployeeAllocation> EmployeeAllocations { get; set; }
        public virtual DbSet<EmployeeI9Documents> EmployeeI9Documents { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeActuals> EmployeeActuals { get; set; }
        //public virtual DbSet<hrBUSINESSLEVELS> hrBUSINESSLEVELS { get; set; }
        public virtual DbSet<FormTemplateField> FormTemplateFields { get; set; }
        public virtual DbSet<FormTemplateFieldType> FormTemplateFieldTypes { get; set; }
        public virtual DbSet<FormTemplate> FormTemplates { get; set; }
        public virtual DbSet<FormTemplateSelectionGroup> FormTemplateSelectionGroups { get; set; }
        public virtual DbSet<FormTemplateSelectionItem> FormTemplateSelectionItems { get; set; }
        public virtual DbSet<FormTemplateWorkflow> FormTemplateWorkflows { get; set; }
        public virtual DbSet<FormWorkflowFieldPermission> FormWorkflowFieldPermissions { get; set; }
        public virtual DbSet<HoursCode> HoursCodes { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<ManagerDepartment> ManagerDepartments { get; set; }
        public virtual DbSet<Manager> Managers { get; set; }
        public virtual DbSet<ManagersPositions> ManagersPositions { get; set; }
        public virtual DbSet<PayPeriod> PayPeriods { get; set; }
        public virtual DbSet<PersonADA> PersonADAs { get; set; }
        public virtual DbSet<PersonAdditional> PersonAdditionals { get; set; }
        public virtual DbSet<PersonAddress> PersonAddresses { get; set; }
        public virtual DbSet<PersonDisability> PersonDisabilities { get; set; }
        public virtual DbSet<PersonEducation> PersonEducations { get; set; }
        public virtual DbSet<PersonExamination> PersonExaminations { get; set; }
        public virtual DbSet<PersonImage> PersonImages { get; set; }
        public virtual DbSet<PersonInnoculation> PersonInnoculations { get; set; }
        public virtual DbSet<PersonLicens> PersonLicenses { get; set; }
        public virtual DbSet<PersonMembership> PersonMemberships { get; set; }
        public virtual DbSet<PersonPassport> PersonPassports { get; set; }
        public virtual DbSet<PersonPhoneNumber> PersonPhoneNumbers { get; set; }
        public virtual DbSet<PersonProperty> PersonProperties { get; set; }
        public virtual DbSet<PersonRelationship> PersonRelationships { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<PersonSkill> PersonSkills { get; set; }
        public virtual DbSet<PersonTest> PersonTests { get; set; }
        public virtual DbSet<PersonTraining> PersonTrainings { get; set; }
        public virtual DbSet<PersonVehicle> PersonVehicles { get; set; }
        public virtual DbSet<PersonWorkPermit> PersonWorkPermits { get; set; }
        public virtual DbSet<PositionClassification> PositionClassifications { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<TimeCardApproval> TimeCardApprovals { get; set; }
        public virtual DbSet<TimeCardDisplayColumn> TimeCardDisplayColumns { get; set; }
        public virtual DbSet<TimeCard> TimeCards { get; set; }
        public virtual DbSet<TimeOffRequest> TimeOffRequests { get; set; }
        public virtual DbSet<UserDefinedSegment1s> UserDefinedSegment1s { get; set; }
        public virtual DbSet<UserDefinedSegment2s> UserDefinedSegment2s { get; set; }
        public virtual DbSet<WorkflowMember> WorkflowMembers { get; set; }
        public virtual DbSet<PositionBusinessLevels> PositionBusinessLevels { get; set; }
        public virtual DbSet<Workflow> Workflows { get; set; }
        public virtual DbSet<PositionBudgets> PositionsBudgets { get; set; }
        public virtual DbSet<DdlPositionCategory> DdlPositionCategory { get; set; }
        public virtual DbSet<PerformanceProfiles> PerformanceProfiles { get; set; }
        public virtual DbSet<PerformanceProfileSections> PerformanceProfilesSections { get; set; }
        public virtual DbSet<DdlPerformancePotentials> DdlPerformancePotentials { get; set; }

        public virtual DbSet<DdlPositionTypes> DdlPositionTypes { get; set; }
        public virtual DbSet<DdlPositionGrade> DdlPositionGrade { get; set; }
        public virtual DbSet<PositionBudgetMonths> PositionsBudgetsMonths { get; set; }

        public virtual DbSet<ddlUnions> DdlUnions { get; set; }
        public virtual DbSet<ddlWorkersCompensations> DdlWorkersCompensations { get; set; }
        public virtual DbSet<ddlSemisters> DdlSemisters { get; set; }
        public virtual DbSet<ddlEEOJobCodes> DdlEEOJobCodes { get; set; }
        public virtual DbSet<ddlJobClasses> DdlJobClasses { get; set; }
        public virtual DbSet<ddlEEOJobTrainingStatuses> DdlEEOJobTrainingStatuses { get; set; }
        public virtual DbSet<ddlFLSAs> DddlFLSAs { get; set; }
        public virtual DbSet<PositionSalaryGrades> PositionSalaryGrades { get; set; }
        public virtual DbSet<DdlSalaryGrades> DdlSalaryGrades { get; set; }
        public virtual DbSet<Contracts> Contracts { get; set; }
        public virtual DbSet<ddlJobFamilys> DdlJobFamilys { get; set; }
        public virtual DbSet<Funds> Funds { get; set; }

        public virtual DbSet<FundHistory> FundHistory { get; set; }

        public virtual DbSet<PositionsFundHistory> PositionsFundHistory { get; set; }
        public virtual DbSet<DdlSalaryGradeHistory> DdlSalaryGradeHistory { get; set; }
        public virtual DbSet<PositionFundingSource> PositionFundingSource { get; set; }

        public virtual DbSet<PositionFundingSourceHistories> PositionFundingSourceHistories { get; set; }
        public virtual DbSet<DdlPayGroup> DdlPayGroups { get; set; }

        public virtual DbSet<PositionFund> PositionFunds { get; set; }
        public virtual DbSet<PositionSalaryGradeSourceHistories> PositionSalaryGradeSourceHistories { get; set; }

        public virtual DbSet<employeeSalaryComponents> EmployeeSalaryComponents { get; set; }

        public virtual DbSet<ddlSalaryComponents> DdlSalaryComponents { get; set; }

        public virtual DbSet<ddlPayTypes> DdlPayTypes { get; set; }
        public virtual DbSet<PositionBudgetSchedules> PositionBudgetSchedules { get; set; }
        public virtual DbSet<UserNamesPerson> UserNamesPersons { get; set; }

        public virtual DbSet<ManagerLockouts> ManagerLockouts { get; set; }
        public virtual DbSet<TimeCardsArchive> TimeCardsArchive { get; set; }
        public virtual DbSet<TimeCardNotes> TimeCardsNotes { get; set; }
        public virtual DbSet<TimeCardNotesArchive> TimeCardNotesArchives { get; set; }
        public virtual DbSet<EmployeesAllowedTakens> EmployeesAllowedTakens { get; set; }
        public virtual DbSet<PositionHistory> PositionHistory { get; set; }
        public virtual DbSet<TimeCardSessionInOutConfigs> TimeCardSessionInOutConfigs { get; set; }
        public virtual DbSet<Providers> Providers { get; set; }
        public virtual DbSet<TimeCardsAudits> TimeCardsAudits { get; set; }

        public virtual DbSet<FTPBannerFileStatus> FTPBannerFileStatus { get; set; }

        // Added for Designated Supervisors Override #1484
        public virtual DbSet<DesignatedSupervisors> DesignatedSupervisors { get; set; }

        public virtual DbSet<DesignatedPositions> DesignatedPositions { get; set; }

        public virtual DbSet<DesignatedManagerDepartment> DesignatedManagerDepartment { get; set; }
        public virtual DbSet<DdlGLCodes> DdlGLCodes { get; set; }
        public virtual DbSet<TreatyNonTreatyTrackingStatus> TreatyNonTreatyTrackingStatus { get; set; }
        public virtual DbSet<PayPeriodsExportedLogs> PayPeriodsExportedLog { get; set; }
        public virtual DbSet<EmployeeClass> EmployeeClass { get; set; }
        public virtual DbSet<Treatylimithistories> Treatylimithistories { get; set; }

        public virtual DbSet<EmployeeRetroHours> EmployeeRetroHours { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
            modelBuilder.Entity<ADPFieldMapping>()
                .Property(e => e.ADPFieldMappingCode)
                .IsUnicode(false);

            modelBuilder.Entity<ADPFieldMapping>()
                .HasMany(e => e.EarningsCodes)
                .WithRequired(e => e.ADPFieldMapping)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ADPFieldMapping>()
                .HasMany(e => e.HoursCodes)
                .WithRequired(e => e.ADPFieldMapping)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BenefitDeductionCode>()
                .Property(e => e.BenefitDeductionCodeCode)
                .IsUnicode(false);

            modelBuilder.Entity<BenefitDeductionCode>()
                .Property(e => e.BenefitDeductionCodeDescription)
                .IsUnicode(false);

            modelBuilder.Entity<BenefitExpansionField>()
                .Property(e => e.BenefitExpansionFieldCode)
                .IsUnicode(false);

            modelBuilder.Entity<BenefitExpansionField>()
                .Property(e => e.BenefitExpansionFieldDescription)
                .IsUnicode(false);

            modelBuilder.Entity<BenefitGroup>()
                .Property(e => e.BenefitGroupCode)
                .IsUnicode(false);

            modelBuilder.Entity<BenefitGroup>()
                .Property(e => e.BenefitGroupDescription)
                .IsUnicode(false);

            modelBuilder.Entity<BenefitGroup>()
                .Property(e => e.DeductionCode)
                .IsUnicode(false);

            modelBuilder.Entity<BenefitGroup>()
                .Property(e => e.ScheduleGroup)
                .IsUnicode(false);

            modelBuilder.Entity<BenefitOESchedule>()
                .Property(e => e.ScheduleGroup)
                .IsUnicode(false);

            modelBuilder.Entity<BenefitOption>()
                .Property(e => e.BenefitOptionCode)
                .IsUnicode(false);

            modelBuilder.Entity<BenefitOption>()
                .Property(e => e.BenefitOptionDescription)
                .IsUnicode(false);

            modelBuilder.Entity<BenefitProvider>()
                .Property(e => e.BenefitProviderCode)
                .IsUnicode(false);

            modelBuilder.Entity<BenefitProvider>()
                .Property(e => e.BenefitProviderDescription)
                .IsUnicode(false);

            modelBuilder.Entity<BusinessUnit>()
                .Property(e => e.BusinessUnitCode)
                .IsUnicode(false);

            modelBuilder.Entity<BusinessUnit>()
                .Property(e => e.BusinessUnitDescription)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyCode>()
                .Property(e => e.CompanyCodeCode)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyCode>()
                .Property(e => e.CompanyCodeDescription)
                .IsUnicode(false);

            modelBuilder.Entity<CompanyCode>()
                .HasMany(e => e.BenefitDeductionCodes)
                .WithRequired(e => e.CompanyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CompanyCode>()
                .HasMany(e => e.BenefitExpansionFields)
                .WithRequired(e => e.CompanyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CompanyCode>()
                .HasMany(e => e.BenefitGroups)
                .WithRequired(e => e.CompanyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CompanyCode>()
                .HasMany(e => e.BenefitOESchedules)
                .WithRequired(e => e.CompanyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CompanyCode>()
                .HasMany(e => e.BenefitOptions)
                .WithRequired(e => e.CompanyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CompanyCode>()
                .HasMany(e => e.BenefitProviders)
                .WithRequired(e => e.CompanyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CompanyCode>()
                .HasMany(e => e.Departments)
                .WithRequired(e => e.CompanyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CompanyCode>()
                .HasMany(e => e.EarningsCodes)
                .WithRequired(e => e.CompanyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CompanyCode>()
                .HasMany(e => e.HoursCodes)
                .WithRequired(e => e.CompanyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CompanyCode>()
                .HasMany(e => e.Jobs)
                .WithRequired(e => e.CompanyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CompanyCode>()
                .HasMany(e => e.TimeCards)
                .WithRequired(e => e.CompanyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CompanyCode>()
                .HasMany(e => e.TimeOffRequests)
                .WithRequired(e => e.CompanyCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlAccommodationType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlAccommodationType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlAccommodationType>()
                .HasMany(e => e.PersonADAs)
                .WithRequired(e => e.DdlAccommodationType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlAddressType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlAddressType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlAddressType>()
                .HasMany(e => e.PersonAddresses)
                .WithRequired(e => e.DdlAddressType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlPositionCategory>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlPositionCategory>()
               .Property(e => e.code)
               .IsUnicode(false);

            // modelBuilder.Entity<DdlPositionCategory>()
            //   .HasMany(e => e.E_Positions)
            //  .WithRequired(e => e.DdlPositionCategory)
            //   .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlPositionGrade>()
               .Property(e => e.Description)
               .IsUnicode(false);

            modelBuilder.Entity<DdlPositionGrade>()
               .Property(e => e.Code)
               .IsUnicode(false);

            //modelBuilder.Entity<DdlPositionGrade>()
            //    .HasMany(e => e.E_Positions)
            //    .WithRequired(e => e.DdlPositionGrade)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlPositionTypes>()
               .Property(e => e.description)
               .IsUnicode(false);

            modelBuilder.Entity<DdlPositionTypes>()
               .Property(e => e.code)
               .IsUnicode(false);

            //modelBuilder.Entity<DdlPositionType>()
            //    .HasMany(e => e.E_Positions)
            //    .WithRequired(e => e.DdlPositionType)
            //    .WillCascadeOnDelete(false);
            modelBuilder.Entity<DdlPerformancePotentials>()
               .Property(e => e.Description)
               .IsUnicode(false);

            modelBuilder.Entity<DdlPerformancePotentials>()
               .Property(e => e.Code)
               .IsUnicode(false);


            modelBuilder.Entity<DdlApplicantSource>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlApplicantSource>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlApplicantSource>()
                .Property(e => e.AddressLineOne)
                .IsUnicode(false);

            modelBuilder.Entity<DdlApplicantSource>()
                .Property(e => e.AddressLineTwo)
                .IsUnicode(false);

            modelBuilder.Entity<DdlApplicantSource>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<DdlApplicantSource>()
                .Property(e => e.ZipCode)
                .IsUnicode(false);

            modelBuilder.Entity<DdlApplicantSource>()
                .Property(e => e.PhoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<DdlApplicantSource>()
                .Property(e => e.FaxNumber)
                .IsUnicode(false);

            modelBuilder.Entity<DdlApplicantSource>()
                .Property(e => e.Contact)
                .IsUnicode(false);

            modelBuilder.Entity<DdlApplicantSource>()
                .Property(e => e.WebAddress)
                .IsUnicode(false);

            modelBuilder.Entity<DdlApplicantSource>()
                .Property(e => e.AccountNumber)
                .IsUnicode(false);

            modelBuilder.Entity<DdlApplicantSource>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<DdlCitizenship>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlCitizenship>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlBusinessLevelTypes>()
              .Property(e => e.Description)
              .IsUnicode(false);

            modelBuilder.Entity<DdlBusinessLevelTypes>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEEOFileStatuses>()
             .Property(e => e.Description)
             .IsUnicode(false);

            modelBuilder.Entity<DdlEEOFileStatuses>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEINs>()
          .Property(e => e.EIN)
          .IsUnicode(false);

            modelBuilder.Entity<DdlEINs>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlCountry>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlCountry>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlCountry>()
                .HasMany(e => e.PersonLicenses)
                .WithRequired(e => e.DdlCountry)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlCountry>()
                .HasMany(e => e.PersonPassports)
                .WithRequired(e => e.DdlCountry)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlCountry>()
                .HasMany(e => e.PersonWorkPermits)
                .WithRequired(e => e.DdlCountry)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlDegreeType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlDegreeType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlDegreeType>()
                .HasMany(e => e.PersonEducations)
                .WithOptional(e => e.DdlDegreeType)
                .HasForeignKey(e => e.MajorId);

            modelBuilder.Entity<DdlDegreeType>()
                .HasMany(e => e.PersonEducations1)
                .WithOptional(e => e.DdlDegreeType1)
                .HasForeignKey(e => e.MinorId);

            modelBuilder.Entity<DdlDisabilityType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlDisabilityType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlDisabilityType>()
                .HasMany(e => e.PersonADAs)
                .WithRequired(e => e.DdlDisabilityType)
                .HasForeignKey(e => e.AssociatedDisabilityId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlDisabilityType>()
                .HasMany(e => e.PersonDisabilities)
                .WithRequired(e => e.DdlDisabilityType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlEducationEstablishment>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationEstablishment>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationEstablishment>()
                .Property(e => e.AddressLineOne)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationEstablishment>()
                .Property(e => e.AddressLineTwo)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationEstablishment>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationEstablishment>()
                .Property(e => e.ZipCode)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationEstablishment>()
                .Property(e => e.PhoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationEstablishment>()
                .Property(e => e.FaxNumber)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationEstablishment>()
                .Property(e => e.AccountNumber)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationEstablishment>()
                .Property(e => e.Contact)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationEstablishment>()
                .Property(e => e.WebAddress)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationEstablishment>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationLevel>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationLevel>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationLevel>()
                .HasMany(e => e.PersonEducations)
                .WithOptional(e => e.DdlEducationLevel)
                .HasForeignKey(e => e.LevelAchievedId);

            modelBuilder.Entity<DdlEducationType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEducationType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEeoType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEeoType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEmployeeType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEmployeeType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEmploymentStatus>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEmploymentStatus>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEvaluationTest>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlEvaluationTest>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlGender>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlGender>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlHospital>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlHospital>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlHospital>()
                .Property(e => e.AddressLineOne)
                .IsUnicode(false);

            modelBuilder.Entity<DdlHospital>()
                .Property(e => e.AddressLineTwo)
                .IsUnicode(false);

            modelBuilder.Entity<DdlHospital>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<DdlHospital>()
                .Property(e => e.ZipCode)
                .IsUnicode(false);

            modelBuilder.Entity<DdlHospital>()
                .Property(e => e.PhoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<DdlHospital>()
                .Property(e => e.FaxNumber)
                .IsUnicode(false);

            modelBuilder.Entity<DdlHospital>()
                .Property(e => e.Contact)
                .IsUnicode(false);

            modelBuilder.Entity<DdlHospital>()
                .Property(e => e.WebAddress)
                .IsUnicode(false);

            modelBuilder.Entity<DdlHospital>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<DdlI9DocumentTypes>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlI9DocumentTypes>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlInnoculationType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlInnoculationType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlInnoculationType>()
                .HasMany(e => e.PersonInnoculations)
                .WithRequired(e => e.DdlInnoculationType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlLicenseType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlLicenseType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlLicenseType>()
                .HasMany(e => e.PersonLicenses)
                .WithRequired(e => e.DdlLicenseType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlMajor>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlMajor>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlMaritalStatus>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlMaritalStatus>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlMedicalExaminationType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlMedicalExaminationType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlMedicalExaminationType>()
                .HasMany(e => e.PersonExaminations)
                .WithRequired(e => e.DdlMedicalExaminationType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlPayFrequency>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlPayFrequency>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlPayFrequency>()
                .HasMany(e => e.PayPeriods)
                .WithRequired(e => e.DdlPayFrequency)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlPhoneType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlPhoneType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlPhoneType>()
                .HasMany(e => e.PersonPhoneNumbers)
                .WithRequired(e => e.DdlPhoneType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlPrefix>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlPrefix>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlProfessionalBody>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlProfessionalBody>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlProfessionalBody>()
                .Property(e => e.WebAddress)
                .IsUnicode(false);

            modelBuilder.Entity<DdlProfessionalBody>()
                .HasMany(e => e.PersonMemberships)
                .WithRequired(e => e.DdlProfessionalBody)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlPropertyType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlPropertyType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlPropertyType>()
                .HasMany(e => e.PersonProperties)
                .WithRequired(e => e.DdlPropertyType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlQualificationType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlQualificationType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlRateType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlRateType>()
                .Property(e => e.Code)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<DdlRegionalChapter>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlRegionalChapter>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlRelationshipType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlRelationshipType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlRelationshipType>()
                .HasMany(e => e.PersonRelationships)
                .WithRequired(e => e.DdlRelationshipType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlSkillLevel>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSkillLevel>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSkill>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSkill>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSkill>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSkill>()
                .HasMany(e => e.PersonSkills)
                .WithRequired(e => e.DdlSkill)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlSkillType>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSkillType>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlState>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<DdlState>()
                .Property(e => e.Code)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<DdlState>()
                .HasMany(e => e.Employees)
                .WithOptional(e => e.DdlState)
                .HasForeignKey(e => e.WorkedStateTaxCodeId);

            modelBuilder.Entity<DdlState>()
                .HasMany(e => e.PersonLicenses)
                .WithRequired(e => e.DdlState)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlSuffix>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSuffix>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSupplier>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSupplier>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSupplier>()
                .Property(e => e.AddressLine1)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSupplier>()
                .Property(e => e.AddressLine2)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSupplier>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSupplier>()
                .Property(e => e.ZipCode)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSupplier>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSupplier>()
                .Property(e => e.Fax)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSupplier>()
                .Property(e => e.Contact)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSupplier>()
                .Property(e => e.WebPage)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSupplier>()
                .Property(e => e.AccountNumber)
                .IsUnicode(false);

            modelBuilder.Entity<DdlSupplier>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<DdlTimeCardType>()
                .Property(e => e.TimeCardTypeCode)
                .IsUnicode(false);

            modelBuilder.Entity<DdlTimeCardType>()
                .Property(e => e.TimeCardTypeDescription)
                .IsUnicode(false);

            modelBuilder.Entity<DdlTimeCardType>()
                .HasOptional(e => e.TimeCardDisplayColumn)
                .WithRequired(e => e.DdlTimeCardType);

            modelBuilder.Entity<DdlTrainingCours>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlTrainingCours>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlTrainingCours>()
                .HasMany(e => e.PersonTrainings)
                .WithRequired(e => e.DdlTrainingCours)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DdlTrainingCoursesGroup>()
                .Property(e => e.GroupName)
                .IsUnicode(false);

            modelBuilder.Entity<DdlUnit>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DdlUnit>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Department>()
                .Property(e => e.DepartmentDescription)
                .IsUnicode(false);

            modelBuilder.Entity<Department>()
                .Property(e => e.DepartmentCode)
                .IsUnicode(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.EmployeeAllocations)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.ManagerDepartments)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.TimeCards)
                .WithOptional(e => e.Department)
                .HasForeignKey(e => e.TempDeptId);

            modelBuilder.Entity<E_Positions>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<E_Positions>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<E_Positions>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<E_Positions>()
                .HasMany(e => e.E_PositionSalaryHistories)
                .WithRequired(e => e.E_Positions)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<E_PositionSalaryHistories>()
                .Property(e => e.PayRate)
                .HasPrecision(8, 2);

            modelBuilder.Entity<E_PositionSalaryHistories>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<E_PositionSalaryHistories>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<E_PositionSalaryHistories>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<EarningsCode>()
                .Property(e => e.EarningsCodeCode)
                .IsUnicode(false);

            modelBuilder.Entity<EarningsCode>()
                .Property(e => e.EarningsCodeDescription)
                .IsUnicode(false);

            modelBuilder.Entity<EmployeeAllocation>()
                .Property(e => e.AllocationPercent)
                .HasPrecision(5, 2);

            modelBuilder.Entity<EmployeeI9Documents>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<EmployeeI9Documents>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.CompanyCode)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Rate)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Employee>()
                .Property(e => e.FileNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.FedExemptions)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmployeeAllocations)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmployeeI9Documents)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.TimeCardApprovals)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.TimeCards)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.TimeOffRequests)
                .WithRequired(e => e.Employee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FormTemplateField>()
                .Property(e => e.HtmlId)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateField>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateField>()
                .Property(e => e.Value)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateField>()
                .Property(e => e.Label)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateField>()
                .Property(e => e.Position)
                .HasPrecision(6, 2);

            modelBuilder.Entity<FormTemplateField>()
                .Property(e => e.SelectionGroup)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateField>()
                .Property(e => e.CheckBoxRadioGroupName)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateField>()
                .HasMany(e => e.FormWorkflowFieldPermissions)
                .WithRequired(e => e.FormTemplateField)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FormTemplateFieldType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplate>()
                .HasMany(e => e.FormTemplateFields)
                .WithRequired(e => e.FormTemplate)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FormTemplate>()
                .HasMany(e => e.FormTemplateWorkflows)
                .WithRequired(e => e.FormTemplate)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FormTemplateSelectionGroup>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateSelectionGroup>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateSelectionGroup>()
                .Property(e => e.ExecViewTable)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateSelectionGroup>()
                .Property(e => e.ExecViewTextColumn)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateSelectionGroup>()
                .Property(e => e.ExecViewValueColumn)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateSelectionGroup>()
                .Property(e => e.ExecViewSql)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateSelectionGroup>()
                .HasMany(e => e.FormTemplateSelectionItems)
                .WithRequired(e => e.FormTemplateSelectionGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FormTemplateSelectionItem>()
                .Property(e => e.Text)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateSelectionItem>()
                .Property(e => e.Value)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateSelectionItem>()
                .Property(e => e.Position)
                .HasPrecision(6, 2);

            modelBuilder.Entity<FormTemplateWorkflow>()
                .Property(e => e.FormTemplateWorkflowName)
                .IsUnicode(false);

            modelBuilder.Entity<FormTemplateWorkflow>()
                .HasMany(e => e.FormWorkflowFieldPermissions)
                .WithRequired(e => e.FormTemplateWorkflow)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HoursCode>()
                .Property(e => e.HoursCodeCode)
                .IsUnicode(false);

            modelBuilder.Entity<HoursCode>()
                .Property(e => e.RateOverride)
                .HasPrecision(18, 0);

            modelBuilder.Entity<HoursCode>()
                .Property(e => e.RateMultiplier)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Job>()
                .Property(e => e.JobCode)
                .IsUnicode(false);

            modelBuilder.Entity<Job>()
                .Property(e => e.JobDescription)
                .IsUnicode(false);

            modelBuilder.Entity<Job>()
                .HasMany(e => e.TimeCards)
                .WithOptional(e => e.Job)
                .HasForeignKey(e => e.TempJobId);

            modelBuilder.Entity<Location>()
                .Property(e => e.LocationCode)
                .IsUnicode(false);

            modelBuilder.Entity<Location>()
                .Property(e => e.LocationDescription)
                .IsUnicode(false);

            modelBuilder.Entity<Manager>()
                .HasMany(e => e.ManagerDepartments)
                .WithRequired(e => e.Manager)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PayPeriod>()
                .HasMany(e => e.TimeCardApprovals)
                .WithRequired(e => e.PayPeriod)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PersonADA>()
                .Property(e => e.EstimatedCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PersonADA>()
                .Property(e => e.ActualCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PersonADA>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonADA>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonADA>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonAdditional>()
                .Property(e => e.BirthPlace)
                .IsUnicode(false);

            modelBuilder.Entity<PersonAdditional>()
                .Property(e => e.Veteran)
                .IsUnicode(false);

            modelBuilder.Entity<PersonAdditional>()
                .Property(e => e.DisabledComments)
                .IsUnicode(false);

            modelBuilder.Entity<PersonAdditional>()
                .Property(e => e.Doctor)
                .IsUnicode(false);

            modelBuilder.Entity<PersonAdditional>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonAdditional>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonAdditional>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonAddress>()
                .Property(e => e.AddressLineOne)
                .IsUnicode(false);

            modelBuilder.Entity<PersonAddress>()
                .Property(e => e.AddressLineTwo)
                .IsUnicode(false);

            modelBuilder.Entity<PersonAddress>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<PersonAddress>()
                .Property(e => e.ZipCode)
                .IsUnicode(false);

            modelBuilder.Entity<PersonAddress>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonAddress>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonAddress>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonDisability>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonDisability>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonDisability>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonEducation>()
                .Property(e => e.Grade)
                .IsUnicode(false);

            modelBuilder.Entity<PersonEducation>()
                .Property(e => e.Gpa)
                .IsUnicode(false);

            modelBuilder.Entity<PersonEducation>()
                .Property(e => e.CreditsEarned)
                .IsUnicode(false);

            modelBuilder.Entity<PersonEducation>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonEducation>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonEducation>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonExamination>()
                .Property(e => e.Examiner)
                .IsUnicode(false);

            modelBuilder.Entity<PersonExamination>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonExamination>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonExamination>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonImage>()
                .Property(e => e.PersonImageMimeType)
                .IsUnicode(false);

            modelBuilder.Entity<PersonInnoculation>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonInnoculation>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonInnoculation>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonLicens>()
                .Property(e => e.LicenseNumber)
                .IsUnicode(false);

            modelBuilder.Entity<PersonLicens>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonLicens>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonLicens>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonMembership>()
                .Property(e => e.Number)
                .IsUnicode(false);

            modelBuilder.Entity<PersonMembership>()
                .Property(e => e.Fee)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PersonMembership>()
                .Property(e => e.ProfessionalTitle)
                .IsUnicode(false);

            modelBuilder.Entity<PersonMembership>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonMembership>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonMembership>()
                .Property(e => e.RegionalChapter)
                .IsUnicode(false);

            modelBuilder.Entity<PersonMembership>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonPassport>()
                .Property(e => e.PassportNumber)
                .IsUnicode(false);

            modelBuilder.Entity<PersonPassport>()
                .Property(e => e.PassportStorage)
                .IsUnicode(false);

            modelBuilder.Entity<PersonPassport>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonPassport>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonPhoneNumber>()
                .Property(e => e.PhoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<PersonPhoneNumber>()
                .Property(e => e.Extension)
                .IsUnicode(false);

            modelBuilder.Entity<PersonPhoneNumber>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonPhoneNumber>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonProperty>()
                .Property(e => e.EstimatedValue)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PersonProperty>()
                .Property(e => e.AssetNumber)
                .IsUnicode(false);

            modelBuilder.Entity<PersonProperty>()
                .Property(e => e.PropertyDescription)
                .IsUnicode(false);

            modelBuilder.Entity<PersonProperty>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonProperty>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonProperty>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonRelationship>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonRelationship>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.SSN)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.Lastname)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.Firstname)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.MiddleName)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.PreferredName)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.eMail)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.AlternateEMail)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.MaidenName)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.Managers)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonADAs)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonAdditionals)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonAddresses)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonDisabilities)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonEducations)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonExaminations)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonImages)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonInnoculations)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonLicenses)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonMemberships)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonPassports)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonPhoneNumbers)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonProperties)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonRelationships)
                .WithRequired(e => e.Person)
                .HasForeignKey(e => e.PersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonRelationships1)
                .WithRequired(e => e.Person1)
                .HasForeignKey(e => e.RelationPersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonSkills)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonTests)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonTrainings)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonVehicles)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Person>()
            //    .HasOptional(e => e.PersonWorkPermit)
            //    .WithRequired(e => e.Person);

            modelBuilder.Entity<PersonSkill>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonSkill>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonTest>()
                .Property(e => e.Score)
                .IsUnicode(false);

            modelBuilder.Entity<PersonTest>()
                .Property(e => e.Grade)
                .IsUnicode(false);

            modelBuilder.Entity<PersonTest>()
                .Property(e => e.Administrator)
                .IsUnicode(false);

            modelBuilder.Entity<PersonTest>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonTest>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonTraining>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonTraining>()
                .Property(e => e.TravelCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PersonTraining>()
                .Property(e => e.HotelMealsExpense)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PersonTraining>()
                .Property(e => e.ActualCourseCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PersonTraining>()
                .Property(e => e.Venue)
                .IsUnicode(false);

            modelBuilder.Entity<PersonTraining>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonTraining>()
                .Property(e => e.CompleteStatus)
                .IsUnicode(false);

            modelBuilder.Entity<PersonTraining>()
                .Property(e => e.Grade)
                .IsUnicode(false);

            modelBuilder.Entity<PersonTraining>()
                .Property(e => e.EnrollStatus)
                .IsUnicode(false);

            modelBuilder.Entity<PersonVehicle>()
                .Property(e => e.LicenseNumber)
                .IsUnicode(false);

            modelBuilder.Entity<PersonVehicle>()
                .Property(e => e.Make)
                .IsUnicode(false);

            modelBuilder.Entity<PersonVehicle>()
                .Property(e => e.Model)
                .IsUnicode(false);

            modelBuilder.Entity<PersonVehicle>()
                .Property(e => e.Color)
                .IsUnicode(false);

            modelBuilder.Entity<PersonVehicle>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonVehicle>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonVehicle>()
                .Property(e => e.ModifiedBy)
                .IsUnicode(false);

            modelBuilder.Entity<PersonWorkPermit>()
                .Property(e => e.WorkPermitNumber)
                .IsUnicode(false);

            modelBuilder.Entity<PersonWorkPermit>()
                .Property(e => e.WorkPermitType)
                .IsUnicode(false);

            modelBuilder.Entity<PersonWorkPermit>()
                .Property(e => e.IssuingAuthority)
                .IsUnicode(false);

            modelBuilder.Entity<PersonWorkPermit>()
                .Property(e => e.Notes)
                .IsUnicode(false);

            modelBuilder.Entity<PersonWorkPermit>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            modelBuilder.Entity<PositionClassification>()
                .Property(e => e.ClassificationCriteria)
                .IsUnicode(false);

            modelBuilder.Entity<Position>()
                .Property(e => e.PositionDescription)
                .IsUnicode(false);

            modelBuilder.Entity<Position>()
                .Property(e => e.PositionCode)
                .IsUnicode(false);

            modelBuilder.Entity<Position>()
                .HasMany(e => e.E_Positions)
                .WithRequired(e => e.Position)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TimeCard>()
                .Property(e => e.ApprovedBy)
                .IsUnicode(false);

            modelBuilder.Entity<UserDefinedSegment1s>()
                .Property(e => e.UserDefinedSegment1Code)
                .IsUnicode(false);

            modelBuilder.Entity<UserDefinedSegment1s>()
                .Property(e => e.UserDefinedSegment1Description)
                .IsUnicode(false);

            modelBuilder.Entity<UserDefinedSegment2s>()
                .Property(e => e.UserDefinedSegment2Code)
                .IsUnicode(false);

            modelBuilder.Entity<UserDefinedSegment2s>()
                .Property(e => e.UserDefinedSegment2Description)
                .IsUnicode(false);

            modelBuilder.Entity<WorkflowMember>()
                .Property(e => e.UserOrGroupName)
                .IsUnicode(false);

            modelBuilder.Entity<WorkflowMember>()
                .Property(e => e.Position)
                .HasPrecision(6, 2);

            modelBuilder.Entity<WorkflowMember>()
                .HasMany(e => e.FormWorkflowFieldPermissions)
                .WithRequired(e => e.WorkflowMember)
                .HasForeignKey(e => e.WorlflowMemberId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Workflow>()
                .Property(e => e.WorkflowName)
                .IsUnicode(false);

            modelBuilder.Entity<Workflow>()
                .Property(e => e.WorkflowDescription)
                .IsUnicode(false);

            modelBuilder.Entity<Workflow>()
                .HasMany(e => e.FormTemplateWorkflows)
                .WithRequired(e => e.Workflow)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Workflow>()
                .HasMany(e => e.WorkflowMembers)
                .WithRequired(e => e.Workflow)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BusinessUnit>()
                .Property(e => e.BusinessUnitCode)
                .IsUnicode(false);
            modelBuilder.Entity<BusinessUnit>()
                .Property(e => e.BusinessUnitDescription)
                .IsUnicode(false);
            modelBuilder.Entity<Department>()
                .Property(e => e.DepartmentDescription)
                .IsUnicode(false);

            modelBuilder.Entity<Location>()
                .Property(e => e.LocationDescription)
                .IsUnicode(false);

            modelBuilder.Entity<Job>()
                .Property(e => e.JobCode)
                .IsUnicode(false);

            modelBuilder.Entity<Job>()
                .Property(e => e.JobDescription)
                .IsUnicode(false);


            modelBuilder.Entity<DdlPayGroup>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<DdlPayGroup>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.UserNamesPersons)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserNamesPerson>()
                .Property(e => e.EnteredBy)
                .IsUnicode(false);

            // Added for Designated Supervisor Override #1484
            modelBuilder.Entity<DesignatedSupervisors>()
                .Property(e => e.ManagerPersonId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // #1634 - Alternate Bahavior
            modelBuilder.Entity<DesignatedPositions>().HasKey(s => new { s.ManagerPersonId, s.E_PositionId });

            // #1634 - Alternate Bahavior - Additional dependency with ManagerDepartment
            modelBuilder.Entity<DesignatedManagerDepartment>().HasKey(s => new { s.ManagerPersonId, s.ManagerDepartmentId });
        }
    }
}
