(function () {
    var $root = $("#stMetaRoot");
    if (!$root.length) return;

    function url(n) { return $root.attr("data-" + n) || ""; }
    function esc(s) {
        return String(s == null ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;");
    }

    var selectedFieldId = 0;

    function fillSelect($el, rows, valueKey, textKey, placeholder) {
        var html = placeholder ? "<option value=''>" + esc(placeholder) + "</option>" : "";
        (rows || []).forEach(function (r) {
            html += "<option value='" + r[valueKey] + "'>" + esc(r[textKey]) + "</option>";
        });
        $el.html(html);
    }

    function loadModules() {
        $.getJSON(url("modules-url"), function (r) {
            var rows = (r && r.data) || [];
            fillSelect($("#stModule"), rows, "Id", "ModuleName", "— Select module —");
            fillSelect($("#stNewFormModule"), rows, "Id", "ModuleName", "— Module —");
        });
    }

    function loadCompanies() {
        $.getJSON(url("companies-url"), function (r) {
            var html = "<option value=''>— Default labels —</option>";
            ((r && r.data) || []).forEach(function (c) {
                html += "<option value='" + c.Id + "'>" + esc(c.Name) + "</option>";
            });
            $("#stCompany").html(html);
        });
    }

    function loadForms(moduleId, $target, placeholder) {
        $.getJSON(url("forms-url"), { moduleId: moduleId || null }, function (r) {
            fillSelect($target, (r && r.data) || [], "Id", "FormName", placeholder || "— Select form —");
        });
    }

    function loadSections(formId, $target, placeholder) {
        $.getJSON(url("sections-url"), { formId: formId || null }, function (r) {
            fillSelect($target, (r && r.data) || [], "Id", "SectionName", placeholder || "— Select section —");
        });
    }

    function loadFields() {
        var sectionId = $("#stSection").val();
        var companyId = $("#stCompany").val() || null;
        var $tb = $("#stFieldTable tbody").empty();
        if (!sectionId) {
            $tb.append("<tr><td colspan='4' class='text-muted'>Select a section to load fields.</td></tr>");
            return;
        }
        $.getJSON(url("fields-url"), { sectionId: sectionId, companyId: companyId }, function (r) {
            var rows = (r && r.data) || [];
            if (!rows.length) {
                $tb.append("<tr><td colspan='4' class='text-muted'>No fields in this section.</td></tr>");
                return;
            }
            rows.forEach(function (f) {
                $tb.append("<tr class='st-field-row' data-id='" + f.Id + "'>" +
                    "<td>" + esc(f.Field) + "</td>" +
                    "<td>" + esc(f.FieldType || "") + "</td>" +
                    "<td>" + esc(f.DisplayName || "") + "</td>" +
                    "<td>" + esc(f.EffectiveLabel || "") + "</td></tr>");
            });
        });
    }

    function selectField(id) {
        selectedFieldId = id;
        $("#stEditFieldId").val(id);
        $.getJSON(url("field-url"), { id: id, companyId: $("#stCompany").val() || null }, function (r) {
            if (!r || !r.success) { alert((r && r.message) || "Unable to load field"); return; }
            var d = r.data;
            $("#stSelectedFieldName").text(d.Field || ("#" + id));
            $("#stEditLabel").val(d.DisplayName || "");
            $("#stEditRequired").prop("checked", !!d.IsRequired);
            $("#stEditVisible").prop("checked", !!d.IsVisible);
            $("#stEditReadOnly").prop("checked", !!d.IsReadOnly);
            $("#stEditAllowAdd").prop("checked", d.AllowAdd !== false);
            $("#stEditAllowEdit").prop("checked", d.AllowEdit !== false);
        });
    }

    function saveLabel() {
        if (!selectedFieldId) { alert("Select a field first."); return; }
        $.post(url("save-label-url"), {
            Id: selectedFieldId,
            CompanyId: $("#stCompany").val() || null,
            DisplayName: $("#stEditLabel").val(),
            IsRequired: $("#stEditRequired").is(":checked"),
            IsVisible: $("#stEditVisible").is(":checked"),
            IsReadOnly: $("#stEditReadOnly").is(":checked"),
            AllowAdd: $("#stEditAllowAdd").is(":checked"),
            AllowEdit: $("#stEditAllowEdit").is(":checked")
        }, function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Save failed"); return; }
            loadFields();
        });
    }

    function loadRoles() {
        $.getJSON(url("roles-url"), function (r) {
            fillSelect($("#stSecRole"), (r && r.data) || [], "Name", "Name", "— Role —");
        });
    }

    function loadFormSecurity() {
        var formId = $("#stSecForm").val() || null;
        $.getJSON(url("form-sec-url"), { formId: formId }, function (r) {
            var $tb = $("#stFormSecTable tbody").empty();
            var rows = (r && r.data) || [];
            if (!rows.length) {
                $tb.append("<tr><td colspan='8' class='text-muted'>No form security rows yet.</td></tr>");
                return;
            }
            rows.forEach(function (s) {
                $tb.append("<tr>" +
                    "<td>" + esc(s.TargetName) + "</td>" +
                    "<td>" + esc(s.RoleName) + "</td>" +
                    "<td>" + (s.CanView ? "✓" : "") + "</td>" +
                    "<td>" + (s.CanAdd ? "✓" : "") + "</td>" +
                    "<td>" + (s.CanEdit ? "✓" : "") + "</td>" +
                    "<td>" + (s.CanDelete ? "✓" : "") + "</td>" +
                    "<td>" + (s.CanExport ? "✓" : "") + "</td>" +
                    "<td>" + (s.IsActive ? "Yes" : "No") + "</td></tr>");
            });
        });
    }

    $(document).off(".stMeta");
    $(document).on("change.stMeta", "#stModule", function () {
        loadForms($(this).val(), $("#stForm"), "— Select form —");
        $("#stSection").html("<option value=''>— Select section —</option>");
        $("#stFieldTable tbody").empty();
    });
    $(document).on("change.stMeta", "#stForm", function () {
        loadSections($(this).val(), $("#stSection"), "— Select section —");
        loadSections($(this).val(), $("#stNewFieldSection"), "— Section —");
        $("#stFieldTable tbody").empty();
    });
    $(document).on("change.stMeta", "#stSection, #stCompany", loadFields);
    $(document).on("click.stMeta", "#stFieldTable tbody tr.st-field-row", function () {
        $("#stFieldTable tbody tr").removeClass("active");
        $(this).addClass("active");
        selectField($(this).attr("data-id"));
    });
    $(document).on("click.stMeta", "#stSaveLabel", saveLabel);
    $(document).on("click.stMeta", "#stResetLabel", function () {
        if (selectedFieldId) selectField(selectedFieldId);
    });

    $(document).on("click.stMeta", "#stAddFormSec", function () {
        $.post(url("save-form-sec-url"), {
            id: 0,
            formId: $("#stSecForm").val(),
            roleName: $("#stSecRole").val(),
            canView: $("#stSecView").is(":checked"),
            canAdd: $("#stSecAdd").is(":checked"),
            canEdit: $("#stSecEdit").is(":checked"),
            canDelete: $("#stSecDelete").is(":checked"),
            canExport: $("#stSecExport").is(":checked"),
            isActive: $("#stSecActive").is(":checked")
        }, function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Save failed"); return; }
            loadFormSecurity();
        });
    });
    $(document).on("change.stMeta", "#stSecForm", loadFormSecurity);

    $(document).on("click.stMeta", "#stSaveModule", function () {
        $.post(url("save-module-url"), {
            id: 0,
            moduleName: $("#stNewModuleName").val(),
            controlId: $("#stNewModuleControl").val()
        }, function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Failed"); return; }
            $("#stNewModuleName, #stNewModuleControl").val("");
            loadModules();
        });
    });
    $(document).on("click.stMeta", "#stSaveForm", function () {
        $.post(url("save-form-url"), {
            id: 0,
            formName: $("#stNewFormName").val(),
            moduleId: $("#stNewFormModule").val(),
            controlId: $("#stNewFormControl").val(),
            isUsed: true,
            tableName: $("#stNewFormTable").val()
        }, function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Failed"); return; }
            $("#stNewFormName, #stNewFormControl, #stNewFormTable").val("");
            loadForms($("#stModule").val(), $("#stForm"));
            loadForms(null, $("#stSecForm"), "— All forms —");
            loadForms(null, $("#stNewSectionForm"), "— Form —");
        });
    });
    $(document).on("click.stMeta", "#stSaveSection", function () {
        $.post(url("save-section-url"), {
            id: 0,
            formId: $("#stNewSectionForm").val(),
            sectionName: $("#stNewSectionName").val(),
            sort: $("#stNewSectionSort").val() || 1,
            tableName: ""
        }, function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Failed"); return; }
            $("#stNewSectionName").val("");
            loadSections($("#stForm").val(), $("#stSection"));
            loadSections($("#stNewSectionForm").val(), $("#stNewFieldSection"), "— Section —");
        });
    });
    $(document).on("click.stMeta", "#stSaveMetaField", function () {
        $.post(url("save-meta-url"), {
            id: 0,
            sectionId: $("#stNewFieldSection").val(),
            field: $("#stNewFieldKey").val(),
            fieldType: $("#stNewFieldType").val() || "varchar",
            displayName: $("#stNewFieldLabel").val(),
            panelName: "",
            isRequired: false,
            isVisible: true
        }, function (res) {
            if (!res || !res.success) { alert((res && res.message) || "Failed"); return; }
            $("#stNewFieldKey, #stNewFieldLabel").val("");
            loadFields();
        });
    });

    loadModules();
    loadCompanies();
    loadRoles();
    loadForms(null, $("#stSecForm"), "— All forms —");
    loadForms(null, $("#stNewSectionForm"), "— Form —");
    loadFormSecurity();
})();
