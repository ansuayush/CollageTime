using System.Collections.Generic;

namespace ExecViewHrk.WebUI.Models
{
    public class StModuleVm
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public string ControlId { get; set; }
    }

    public class StFormVm
    {
        public int Id { get; set; }
        public string FormName { get; set; }
        public int? ModuleId { get; set; }
        public string ControlId { get; set; }
        public bool? IsUsed { get; set; }
        public string TableName { get; set; }
    }

    public class StSectionVm
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public string SectionName { get; set; }
        public int? Sort { get; set; }
        public string TableName { get; set; }
    }

    public class StMetaFieldVm
    {
        public int Id { get; set; }
        public int? SectionId { get; set; }
        public string Field { get; set; }
        public string FieldType { get; set; }
        public string DisplayName { get; set; }
        public string CompanyDisplayName { get; set; }
        public string EffectiveLabel { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsVisible { get; set; }
        public bool? IsAllowed { get; set; }
        public bool? IsReadOnly { get; set; }
        public string PanelName { get; set; }
        public string SampleData { get; set; }
    }

    public class StFieldSaveVm
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public string DisplayName { get; set; }
        public bool IsRequired { get; set; }
        public bool IsVisible { get; set; }
        public bool AllowAdd { get; set; }
        public bool AllowEdit { get; set; }
        public bool IsReadOnly { get; set; }
    }

    public class StSecurityRowVm
    {
        public int Id { get; set; }
        public int TargetId { get; set; }
        public string TargetName { get; set; }
        public string RoleName { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanExport { get; set; }
        public bool IsActive { get; set; }
    }
}
