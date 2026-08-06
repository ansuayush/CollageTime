(function () {
    var $root = $("#selfOnboardRulesRoot");
    if (!$root.length) return;

    function fillSelect($el, items, placeholder) {
        $el.empty().append("<option value=''>" + (placeholder || "-- Select --") + "</option>");
        (items || []).forEach(function (i) {
            $el.append("<option value='" + i.id + "'>" + i.text + "</option>");
        });
    }

    function previewUserName() {
        var f = ($("#soFirstName").val() || "").replace(/\s+/g, "");
        var l = ($("#soLastName").val() || "").replace(/\s+/g, "");
        var badge = ($("#soFileNumber").val() || "");
        var digits = badge.length > 4 ? badge.slice(-4) : badge;
        $("#soUserNamePreview").val((f + l + digits).toLowerCase());
    }

    $.getJSON($root.data("lookups-url"), function (res) {
        if (!res.success) { alert(res.message || "Failed to load"); return; }
        fillSelect($("#soPosition"), res.positions, "-- Position --");
        fillSelect($("#soProfile"), res.profiles, "-- Profile --");
        fillSelect($("#soOfferLetter"), res.offerLetters, "-- Offer letter --");
        $("#soFileNumber").val(res.nextBadge || "");
        previewUserName();
    });

    $("#soPosition").on("change", function () {
        var id = $(this).val();
        $("#soCandidate").empty().append("<option value=''>Loading...</option>");
        if (!id) {
            $("#soCandidate").html("<option value=''>-- Select position first --</option>");
            return;
        }
        $.getJSON($root.data("candidates-url"), { positionId: id }, function (res) {
            fillSelect($("#soCandidate"), (res.data || []).map(function (c) {
                return {
                    id: c.applicationId,
                    text: c.text,
                    firstName: c.firstName,
                    lastName: c.lastName,
                    homeEmail: c.homeEmail,
                    applicantId: c.applicantId
                };
            }), "-- Candidate --");
            $("#soCandidate option").each(function () {
                var opt = (res.data || []).filter(function (c) { return String(c.applicationId) === $(this).val(); }.bind(this))[0];
                if (opt) {
                    $(this).data("first", opt.firstName).data("last", opt.lastName).data("email", opt.homeEmail).data("applicant", opt.applicantId);
                }
            });
        });
    });

    $("#soCandidate").on("change", function () {
        var $opt = $(this).find(":selected");
        if (!$opt.val()) return;
        $("#soFirstName").val($opt.data("first") || "");
        $("#soLastName").val($opt.data("last") || "");
        $("#soHomeEmail").val($opt.data("email") || "");
        previewUserName();
    });

    $("#soFirstName,#soLastName,#soFileNumber").on("input", previewUserName);

    $("#btnSendHireNotice").on("click", function () {
        var $opt = $("#soCandidate").find(":selected");
        $("#soNoticeResult,#soNoticeError").hide();
        $.post($root.data("send-url"), {
            positionId: $("#soPosition").val() || null,
            profileId: $("#soProfile").val() || null,
            applicationId: $("#soCandidate").val() || null,
            applicantId: $opt.data("applicant") || null,
            firstName: $("#soFirstName").val(),
            lastName: $("#soLastName").val(),
            homeEmail: $("#soHomeEmail").val(),
            fileNumber: $("#soFileNumber").val(),
            offerLetterId: $("#soOfferLetter").val() || null
        }, function (res) {
            if (!res.success) {
                $("#soNoticeError").text(res.message || "Failed").show();
                return;
            }
            var msg = res.message || "Sent.";
            if (res.userName) msg += " Username: " + res.userName;
            if (res.tempPassword) msg += " Temp password: " + res.tempPassword;
            if (res.message && res.message.indexOf("email send failed") >= 0)
                $("#soNoticeError").text(msg).show();
            else
                $("#soNoticeResult").text(msg).show();
            loadRecentHires();
        }).fail(function (xhr) {
            $("#soNoticeError").text(xhr.responseText || "Request failed").show();
        });
    });

    function loadRecentHires() {
        $.getJSON($root.data("hires-url"), function (res) {
            var $tb = $("#soRecentHires tbody").empty();
            (res.data || []).slice(0, 15).forEach(function (h) {
                $tb.append("<tr>" +
                    "<td>" + (h.FirstName || "") + " " + (h.LastName || "") + "</td>" +
                    "<td>" + (h.HomeEmail || "") + "</td>" +
                    "<td>" + (h.GeneratedUserName || "") + "</td>" +
                    "<td>" + (h.Status || "") + "</td>" +
                    "<td><button type='button' class='btn btn-xs btn-default btn-resend-hire' data-id='" + h.HireId + "'>Resend email</button></td>" +
                    "</tr>");
            });
            if (!(res.data || []).length) $tb.append("<tr><td colspan='5'>No invites yet.</td></tr>");
        });
    }

    $(document).on("click", ".btn-resend-hire", function () {
        var id = $(this).data("id");
        $("#soNoticeResult,#soNoticeError").hide();
        $.post($root.data("resend-url"), { hireId: id }, function (res) {
            if (!res.success) {
                $("#soNoticeError").text(res.message || "Failed").show();
                return;
            }
            $("#soNoticeResult").text(res.message || "Resent.").show();
        }).fail(function (xhr) {
            $("#soNoticeError").text(xhr.responseText || "Request failed").show();
        });
    });

    loadRecentHires();
})();
