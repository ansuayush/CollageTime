using ExecViewHrk.EfClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ExecViewHrk.WebUI.Helpers
{
    /// <summary>
    /// Resolves field labels: company-specific stMetaData → stMetaTable.DisplayName → field key.
    /// </summary>
    public static class StLabelService
    {
        private static readonly ConcurrentDictionary<string, string> Cache = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static string GetLabel(ClientDbContext db, string fieldKey, int? companyId = null, string fallback = null)
        {
            if (string.IsNullOrWhiteSpace(fieldKey)) return fallback ?? "";
            string cacheKey = (companyId ?? 0) + "|" + fieldKey.Trim();
            string cached;
            if (Cache.TryGetValue(cacheKey, out cached)) return cached;

            var meta = db.StMetaTables.FirstOrDefault(m => m.Field == fieldKey);
            string label = null;
            if (meta != null && companyId.HasValue && companyId.Value > 0)
            {
                var companyLabel = db.StMetaDatas.FirstOrDefault(d => d.FieldId == meta.Id && d.CompanyId == companyId.Value);
                if (companyLabel != null && !string.IsNullOrWhiteSpace(companyLabel.DisplayName))
                    label = companyLabel.DisplayName;
            }
            if (string.IsNullOrWhiteSpace(label) && meta != null && !string.IsNullOrWhiteSpace(meta.DisplayName))
                label = meta.DisplayName;
            if (string.IsNullOrWhiteSpace(label))
                label = fallback ?? fieldKey;

            Cache[cacheKey] = label;
            return label;
        }

        public static string GetLabelByFieldId(ClientDbContext db, int fieldId, int? companyId = null)
        {
            var meta = db.StMetaTables.FirstOrDefault(m => m.Id == fieldId);
            if (meta == null) return "";
            if (companyId.HasValue && companyId.Value > 0)
            {
                var companyLabel = db.StMetaDatas.FirstOrDefault(d => d.FieldId == fieldId && d.CompanyId == companyId.Value);
                if (companyLabel != null && !string.IsNullOrWhiteSpace(companyLabel.DisplayName))
                    return companyLabel.DisplayName;
            }
            return meta.DisplayName ?? meta.Field ?? "";
        }

        public static void LoadEmploymentFieldMeta(ClientDbContext db, dynamic viewBag, int? companyId = null)
        {
            StFormMetadataSchemaHelper.EnsureSchema(db);
            var empForm = db.StForms.FirstOrDefault(f => f.ControlId == "iEmployment" || f.FormName == "Employment");
            if (empForm == null) return;
            var section = db.StFormSections.FirstOrDefault(s => s.FormId == empForm.Id && s.SectionName == "Employment");
            if (section == null) return;

            var fields = db.StMetaTables.Where(f => f.SectionId == section.Id).ToList();
            var fieldIds = fields.Select(f => f.Id).ToList();
            var companyLabels = companyId.HasValue
                ? db.StMetaDatas.Where(d => fieldIds.Contains(d.FieldId) && d.CompanyId == companyId.Value)
                    .ToDictionary(d => d.FieldId, d => d.DisplayName)
                : new Dictionary<int, string>();

            var labels = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
            var visible = new Dictionary<string, bool>(System.StringComparer.OrdinalIgnoreCase);
            var required = new Dictionary<string, bool>(System.StringComparer.OrdinalIgnoreCase);

            foreach (var f in fields)
            {
                if (string.IsNullOrWhiteSpace(f.Field)) continue;
                string companyLabel;
                companyLabels.TryGetValue(f.Id, out companyLabel);
                labels[f.Field] = !string.IsNullOrWhiteSpace(companyLabel) ? companyLabel : (f.DisplayName ?? f.Field);
                visible[f.Field] = (f.IsVisible ?? f.IsAllowed) != false;
                required[f.Field] = f.IsRequired == true;
            }

            viewBag.StFieldLabels = labels;
            viewBag.StFieldVisible = visible;
            viewBag.StFieldRequired = required;
        }

        public static void Invalidate()
        {
            Cache.Clear();
        }
    }
}
