using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExecViewHrk.EfClient
{
    /// <summary>
    /// Metadata / form-security tables from Stformtables architecture
    /// (Module → Form → Section → Field + company labels).
    /// </summary>

    [Table("stModules")]
    public partial class StModule
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string ModuleName { get; set; }
        [StringLength(50)]
        public string ControlId { get; set; }
    }

    [Table("stForms")]
    public partial class StForm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [StringLength(50)]
        public string FormName { get; set; }
        public int? ModuleId { get; set; }
        [StringLength(50)]
        public string ControlId { get; set; }
        public bool? IsUsed { get; set; }
        [StringLength(25)]
        public string TableName { get; set; }
    }

    [Table("stFormSections")]
    public partial class StFormSection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int FormId { get; set; }
        [StringLength(50)]
        public string SectionName { get; set; }
        public int? Sort { get; set; }
        public bool? IsAllowedInWorkFlow { get; set; }
        [StringLength(50)]
        public string TableName { get; set; }
        public bool? IsAllowedInTemplate { get; set; }
        public bool? IsAllowedInReports { get; set; }
    }

    [Table("stMetaTable")]
    public partial class StMetaTable
    {
        [Key]
        public int Id { get; set; }
        public int? SectionId { get; set; }
        [StringLength(50)]
        public string Field { get; set; }
        [StringLength(50)]
        public string FieldType { get; set; }
        public int? FieldMaxLength { get; set; }
        public bool? IsValid { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsAllowed { get; set; }
        public bool? IsAllowedInWorkFlow { get; set; }
        [StringLength(10)]
        public string HTablesCode { get; set; }
        [StringLength(50)]
        public string DataTextField { get; set; }
        [StringLength(50)]
        public string DataValueField { get; set; }
        public string DisplayName { get; set; }
        [StringLength(50)]
        public string PanelName { get; set; }
        public bool? IsImportableField { get; set; }
        [StringLength(50)]
        public string ImportDisplayName { get; set; }
        [StringLength(1)]
        public string ImportType { get; set; }
        public bool? IsMassChangeable { get; set; }
        [StringLength(10)]
        public string MassType { get; set; }
        [StringLength(50)]
        public string SampleData { get; set; }
        public bool? IsAllowedinReports { get; set; }
        public bool? IsAllowedInTemplate { get; set; }
        public bool? IsVisible { get; set; }
        public bool? IsReadOnly { get; set; }
        public bool? IsSearchable { get; set; }
        public bool? IsSortable { get; set; }
        [StringLength(500)]
        public string DefaultValue { get; set; }
        [StringLength(1000)]
        public string ValidationExpression { get; set; }
        [StringLength(255)]
        public string Placeholder { get; set; }
        [StringLength(1000)]
        public string ToolTip { get; set; }
    }

    [Table("stMetaData")]
    public partial class StMetaData
    {
        [Key]
        public int Id { get; set; }
        public int FieldId { get; set; }
        public int CompanyId { get; set; }
        public string DisplayName { get; set; }
    }

    [Table("stModuleSecurity")]
    public partial class StModuleSecurity
    {
        [Key]
        public int Id { get; set; }
        public int ModuleId { get; set; }
        [Required, StringLength(256)]
        public string RoleName { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanExport { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("stFormSecurity")]
    public partial class StFormSecurity
    {
        [Key]
        public int Id { get; set; }
        public int FormId { get; set; }
        [Required, StringLength(256)]
        public string RoleName { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanExport { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("stSectionSecurity")]
    public partial class StSectionSecurity
    {
        [Key]
        public int Id { get; set; }
        public int SectionId { get; set; }
        [Required, StringLength(256)]
        public string RoleName { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("stFieldSecurity")]
    public partial class StFieldSecurity
    {
        [Key]
        public int Id { get; set; }
        public int FieldId { get; set; }
        [Required, StringLength(256)]
        public string RoleName { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool? IsRequired { get; set; }
        public bool IsActive { get; set; }
    }
}
