(function () {
    var $r = $("#wizardRoot");
    if (!$r.length) return;

    var employerId = $r.data("employer");
    var applicationId = $r.data("app");
    var step = parseInt($r.data("step"), 10) || 1;

    function go(s) {
        var base = $r.data("wizard-url");
        window.location = base + "?employerId=" + employerId + "&applicationId=" + applicationId + "&step=" + s;
    }

    function collectForm() {
        var data = {
            employerId: employerId,
            applicationId: applicationId,
            step: step,
            attestationName: $("#attestName").val() || null
        };
        $r.find("input, select, textarea").each(function () {
            var name = $(this).attr("name");
            if (name) data[name] = $(this).val();
        });
        return data;
    }

    function saveThen(next) {
        $.ajax({
            url: $r.data("save-url"),
            type: "POST",
            data: collectForm(),
            success: function (res) {
                if (!res.success) { alert(res.message || "Save failed"); return; }
                if (typeof next === "function") next(res);
                else go(res.nextStep || (step + 1));
            },
            error: function () { alert("Save failed"); }
        });
    }

    $r.on("click", ".btn-next", function () {
        if (step === 1) { go(2); return; }
        saveThen();
    });

    $r.on("click", ".btn-prev", function () {
        go(Math.max(1, step - 1));
    });

    function upload($input, category, docId, signer) {
        if (!$input[0].files || !$input[0].files.length) { alert("Choose a file first."); return; }
        var fd = new FormData();
        fd.append("file", $input[0].files[0]);
        fd.append("employerId", employerId);
        fd.append("applicationId", applicationId);
        fd.append("fileCategory", category);
        if (docId) fd.append("documentSetupId", docId);
        if (signer) fd.append("signerName", signer);
        $.ajax({
            url: $r.data("upload-url"),
            type: "POST",
            data: fd,
            processData: false,
            contentType: false,
            success: function (res) {
                if (!res.success) { alert(res.message || "Upload failed"); return; }
                alert("Uploaded: " + res.fileName);
                go(step);
            },
            error: function () { alert("Upload failed"); }
        });
    }

    $r.on("click", ".btn-upload-doc", function () {
        var docId = $(this).data("docid");
        var $block = $(this).closest(".form-group");
        var $file = $block.find(".doc-file");
        var signer = $block.find(".doc-signer").val();
        if ($file.data("sign") == "1" && !signer) { alert("Signature name is required for this document."); return; }
        upload($file, "Additional", docId, signer);
    });

    $("#btnUploadResume").on("click", function () { upload($("#resumeFile"), "Resume", null, null); });
    $("#btnUploadOther").on("click", function () { upload($("#otherFile"), "Other", null, null); });

    var refCount = $(".ref-block").length;
    $("#btnAddRef").on("click", function () {
        var i = refCount++;
        $("#refList").append(
            '<div class="ref-block" style="border:1px solid #eee;padding:10px;margin-bottom:10px;">' +
            '<div class="row"><div class="col-md-4"><label>Name</label><input class="form-control" name="ref_name_' + i + '" /></div>' +
            '<div class="col-md-4"><label>Relationship</label><input class="form-control" name="ref_rel_' + i + '" /></div>' +
            '<div class="col-md-4"><label>Company</label><input class="form-control" name="ref_co_' + i + '" /></div></div>' +
            '<div class="row" style="margin-top:8px;"><div class="col-md-4"><label>Phone</label><input class="form-control" name="ref_phone_' + i + '" /></div>' +
            '<div class="col-md-4"><label>Email</label><input class="form-control" name="ref_email_' + i + '" /></div>' +
            '<div class="col-md-4"><label>Years known</label><input class="form-control" name="ref_years_' + i + '" /></div></div></div>'
        );
    });

    var empCount = $(".emp-block").length;
    $("#btnAddEmp").on("click", function () {
        var i = empCount++;
        $("#empList").append(
            '<div class="emp-block" style="border:1px solid #eee;padding:10px;margin-bottom:10px;">' +
            '<div class="row"><div class="col-md-4"><label>Employer</label><input class="form-control" name="emp_name_' + i + '" /></div>' +
            '<div class="col-md-4"><label>Title</label><input class="form-control" name="emp_title_' + i + '" /></div>' +
            '<div class="col-md-2"><label>Start</label><input type="date" class="form-control" name="emp_start_' + i + '" /></div>' +
            '<div class="col-md-2"><label>End</label><input type="date" class="form-control" name="emp_end_' + i + '" /></div></div>' +
            '<div class="row" style="margin-top:8px;"><div class="col-md-8"><label>Duties</label><input class="form-control" name="emp_duties_' + i + '" /></div>' +
            '<div class="col-md-4"><label>Reason left</label><input class="form-control" name="emp_reason_' + i + '" /></div></div></div>'
        );
    });

    var eduCount = $(".edu-block").length;
    $("#btnAddEdu").on("click", function () {
        var i = eduCount++;
        $("#eduList").append(
            '<div class="edu-block" style="border:1px solid #eee;padding:10px;margin-bottom:10px;">' +
            '<div class="row"><div class="col-md-4"><label>School</label><input class="form-control" name="edu_school_' + i + '" /></div>' +
            '<div class="col-md-3"><label>Degree</label><input class="form-control" name="edu_degree_' + i + '" /></div>' +
            '<div class="col-md-3"><label>Field</label><input class="form-control" name="edu_field_' + i + '" /></div>' +
            '<div class="col-md-2"><label>Year</label><input class="form-control" name="edu_year_' + i + '" /></div></div></div>'
        );
    });

    $("#btnSubmitApp").on("click", function () {
        var name = $("#attestName").val();
        if (!name) { alert("Please type your name to sign."); return; }
        $.post($r.data("submit-url"), {
            employerId: employerId,
            applicationId: applicationId,
            attestationName: name
        }, function (res) {
            if (!res.success) { alert(res.message || "Submit failed"); return; }
            window.location = "/Apply/Complete?employerId=" + employerId + "&applicationId=" + applicationId;
        });
    });
})();
