(function () {
    var $root = $("#onboardingProfilesRoot");
    if (!$root.length) return;

    function fillDocTypes(selected) {
        $.getJSON($root.data("lookups-url"), { lookupType: "DocumentType" }, function (res) {
            var $sel = $("#docType").empty().append("<option value=''>-- Select --</option>");
            (res.data || []).forEach(function (l) {
                if (!l.IsActive) return;
                $sel.append("<option value='" + l.LookupId + "'" + (selected == l.LookupId ? " selected" : "") + ">" + l.Description + "</option>");
            });
        });
    }

    function reloadPage() {
        if (typeof LoadMenu === "function") LoadMenu("ProfilesPartial", "OnboardingSetup");
    }

    $("#btnAddProfile").on("click", function () {
        $("#profileFormTitle").text("Add Profile");
        $("#profileId").val(0);
        $("#profileName").val("");
        $("#profileDescription").val("");
        $("#profileActive").prop("checked", true);
        $("#profileFormPanel").show();
        $("#docsPanel").hide();
    });

    $(document).on("click", ".btn-edit-profile", function () {
        var $b = $(this);
        $("#profileFormTitle").text("Edit Profile");
        $("#profileId").val($b.data("id"));
        $("#profileName").val($b.data("name"));
        $("#profileDescription").val($b.data("desc"));
        $("#profileActive").prop("checked", $b.data("active") == "1" || $b.data("active") === 1);
        $("#profileFormPanel").show();
        $("#docsPanel").hide();
    });

    $("#btnCancelProfile").on("click", function () { $("#profileFormPanel").hide(); });

    $("#btnSaveProfile").on("click", function () {
        $.post($root.data("save-url"), {
            profileId: $("#profileId").val(),
            profileName: $("#profileName").val(),
            description: $("#profileDescription").val(),
            isActive: $("#profileActive").is(":checked")
        }, function (res) {
            if (!res.success) { alert(res.message || "Failed"); return; }
            reloadPage();
        });
    });

    $(document).on("click", ".btn-del-profile", function () {
        if (!confirm("Delete this profile and its documents?")) return;
        $.post($root.data("delete-url"), { profileId: $(this).data("id") }, function (res) {
            if (!res.success) { alert(res.message || "Failed"); return; }
            reloadPage();
        });
    });

    function loadDocs(profileId, name) {
        $("#docsProfileId").val(profileId);
        $("#docsProfileName").text(name || "");
        $("#docsPanel").show();
        $("#profileFormPanel").hide();
        $("#docForm").hide();
        $.getJSON($root.data("docs-url"), { profileId: profileId }, function (res) {
            var $tb = $("#docsTable tbody").empty();
            (res.data || []).forEach(function (d) {
                $tb.append("<tr>" +
                    "<td>" + (d.DocumentName || "") + "</td>" +
                    "<td>" + (d.DocumentTypeName || "") + "</td>" +
                    "<td>" + (d.RequiresSignature ? "Yes" : "No") + "</td>" +
                    "<td>" + (d.EnableUpload ? "Yes" : "No") + "</td>" +
                    "<td>" +
                    (d.FilePath ? "<a class='btn btn-xs btn-default' target='_blank' href='" + $root.data("download-doc-url") + "?profileDocumentId=" + d.ProfileDocumentId + "'>Download</a> " : "") +
                    "<button type='button' class='btn btn-xs btn-danger btn-del-doc' data-id='" + d.ProfileDocumentId + "'>Delete</button>" +
                    "</td></tr>");
            });
            if (!(res.data || []).length) $tb.append("<tr><td colspan='5'>No documents.</td></tr>");
        });
    }

    $(document).on("click", ".btn-docs-profile", function () {
        loadDocs($(this).data("id"), $(this).data("name"));
    });

    $("#btnAddDoc").on("click", function () {
        $("#docId").val(0);
        $("#docName").val("");
        $("#docFile").val("");
        $("#docReqSig").prop("checked", true);
        $("#docEnableUpload").prop("checked", false);
        $("#docSort").val(0);
        fillDocTypes(null);
        $("#docForm").show();
    });

    $("#btnCancelDoc").on("click", function () { $("#docForm").hide(); });

    $("#btnSaveDoc").on("click", function () {
        var fd = new FormData();
        fd.append("profileDocumentId", $("#docId").val());
        fd.append("profileId", $("#docsProfileId").val());
        fd.append("documentName", $("#docName").val());
        fd.append("documentTypeId", $("#docType").val() || "");
        fd.append("requiresSignature", $("#docReqSig").is(":checked"));
        fd.append("enableUpload", $("#docEnableUpload").is(":checked"));
        fd.append("sortOrder", $("#docSort").val() || 0);
        fd.append("isActive", true);
        var file = $("#docFile")[0].files[0];
        if (file) fd.append("file", file);

        $.ajax({
            url: $root.data("save-doc-url"),
            type: "POST",
            data: fd,
            processData: false,
            contentType: false,
            success: function (res) {
                if (!res.success) { alert(res.message || "Failed"); return; }
                loadDocs($("#docsProfileId").val(), $("#docsProfileName").text());
            }
        });
    });

    $(document).on("click", ".btn-del-doc", function () {
        if (!confirm("Delete document?")) return;
        $.post($root.data("delete-doc-url"), { profileDocumentId: $(this).data("id") }, function (res) {
            if (!res.success) { alert(res.message || "Failed"); return; }
            loadDocs($("#docsProfileId").val(), $("#docsProfileName").text());
        });
    });

    $("#btnManageDocTypes").on("click", function () {
        $("#lookupPanel").show();
        refreshLookups();
    });

    $("#btnCloseLookup").on("click", function () { $("#lookupPanel").hide(); fillDocTypes($("#docType").val()); });

    function refreshLookups() {
        $.getJSON($root.data("lookups-url"), { lookupType: "DocumentType" }, function (res) {
            var $ul = $("#lookupList").empty();
            (res.data || []).forEach(function (l) {
                $ul.append("<li class='m-b-5'>" + l.Description +
                    " <button type='button' class='btn btn-xs btn-danger btn-del-lookup' data-id='" + l.LookupId + "'>x</button></li>");
            });
        });
    }

    $("#btnAddLookup").on("click", function () {
        var desc = $("#newLookupDesc").val();
        if (!desc) return;
        $.post($root.data("save-lookup-url"), {
            lookupId: 0, lookupType: "DocumentType", code: "", description: desc, sortOrder: 0, isActive: true
        }, function (res) {
            if (!res.success) { alert(res.message || "Failed"); return; }
            $("#newLookupDesc").val("");
            refreshLookups();
        });
    });

    $(document).on("click", ".btn-del-lookup", function () {
        $.post($root.data("delete-lookup-url"), { lookupId: $(this).data("id") }, function () { refreshLookups(); });
    });
})();
