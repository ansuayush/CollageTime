(function () {
    var $root = $("#mySelfOnboardRoot");
    if (!$root.length) return;

    function soUrl(name) {
        // Prefer attr() - jQuery .data() can mishandle multi-hyphen data-* keys
        return $root.attr("data-" + name) || $root.data(name) || "";
    }

    var readOnly = $root.data("readonly") == "1" || $root.data("readonly") === 1;
    var isHrReview = $root.attr("data-hr-review") == "1" || $root.data("hr-review") == "1";
    var reviewHireId = parseInt($root.attr("data-hire-id") || $root.data("hire-id") || "0", 10) || 0;
    var currentStep = parseInt($root.data("step"), 10) || 1;
    var lookups = null;
    var hireName = ($("#FirstName").val() || "") + " " + ($("#LastName").val() || "");
    var bankList = [];

    function withHireId(data) {
        data = data || {};
        if (isHrReview && reviewHireId > 0) data.hireId = reviewHireId;
        return data;
    }

    function fillSelect($el, items, selected) {
        $el.empty().append("<option value=''>--Select--</option>");
        (items || []).forEach(function (i) {
            $el.append("<option value='" + i.id + "'" + (String(selected) === String(i.id) ? " selected" : "") + ">" + i.text + "</option>");
        });
    }

    function showStep(step) {
        if (isHrReview && step <= 1) step = 7;
        currentStep = step;
        $(".so-step").hide();
        $(".so-step[data-step='" + step + "']").show();
        $("#soStepNav li").removeClass("active");
        $("#soStepNav li[data-step='" + step + "']").addClass("active");
        if (isHrReview) {
            $("#soStepNav").show();
        } else if (step <= 1) {
            $("#soStepNav").hide();
        } else {
            $("#soStepNav").show();
        }
        if (step === 7 && typeof buildReview === "function") {
            try { buildReview(); } catch (e) { /* keep step visible even if summary refresh fails */ }
        }
    }

    // HR: Get Started is hidden, so Review must be shown immediately (otherwise blank page)
    if (isHrReview) {
        try { showStep(7); } catch (e) {
            $(".so-step").hide();
            $(".so-step[data-step='7']").show();
            $("#soStepNav").show();
        }
    }

    function appendBankRow(b) {
        var id = b.bankAccountId || b.BankAccountId;
        if (!id) return;
        var typeName = b.accountTypeName || b.AccountTypeName || "";
        var bank = b.bankName || b.BankName || "";
        var routing = b.routingNumber || b.RoutingNumber || "";
        var account = b.accountNumber || b.AccountNumber || "";
        var primary = (b.isPrimary === true || b.IsPrimary === true) ? "Yes" : "";
        $("#soBankTable tbody tr[data-id='" + id + "']").remove();
        if (primary === "Yes") {
            $("#soBankTable tbody tr td:nth-child(5)").text("");
        }
        $("#soBankTable tbody").append(
            "<tr data-id='" + id + "'>" +
            "<td>" + $("<div/>").text(typeName).html() + "</td>" +
            "<td>" + $("<div/>").text(bank).html() + "</td>" +
            "<td>" + $("<div/>").text(routing).html() + "</td>" +
            "<td>" + $("<div/>").text(account).html() + "</td>" +
            "<td>" + primary + "</td>" +
            "<td><button type='button' class='btn btn-xs btn-danger btn-del-bank' data-id='" + id + "'>Delete</button></td>" +
            "</tr>"
        );
    }

    function renderBankTable(list) {
        bankList = list || [];
        $("#soBankTable tbody").empty();
        bankList.forEach(function (b) { appendBankRow(b); });
    }

    function refreshBanks(done) {
        $.ajax({
            url: soUrl("bank-list-url"),
            type: "GET",
            dataType: "json",
            data: withHireId({}),
            cache: false,
            success: function (res) {
                if (res && res.success) renderBankTable(res.data || []);
                if (done) done(res);
            },
            error: function () {
                if (done) done(null);
            }
        });
    }

    function clearBankForm() {
        $("#AccountTypeId").val("");
        $("#BankName").val("");
        $("#RoutingNumber").val("");
        $("#AccountNumber").val("");
        $("#IsPrimary").prop("checked", false);
    }

    function hasPendingBankForm() {
        return !!(($("#BankName").val() || "").trim()
            || ($("#RoutingNumber").val() || "").trim()
            || ($("#AccountNumber").val() || "").trim()
            || ($("#AccountTypeId").val() || ""));
    }

    function saveBankAccountAjax(options) {
        options = options || {};
        var quiet = !!options.quiet;
        var done = options.done;
        if (!($("#BankName").val() || "").trim()) {
            if (!quiet) alert("Enter bank name.");
            if (done) done(false);
            return;
        }
        if (!($("#RoutingNumber").val() || "").trim()) {
            if (!quiet) alert("Enter routing number.");
            if (done) done(false);
            return;
        }
        if (!($("#AccountNumber").val() || "").trim()) {
            if (!quiet) alert("Enter account number.");
            if (done) done(false);
            return;
        }

        var payload = {
            bankAccountId: 0,
            bankName: $("#BankName").val(),
            routingNumber: $("#RoutingNumber").val(),
            accountNumber: $("#AccountNumber").val(),
            isPrimary: $("#IsPrimary").is(":checked")
        };
        var typeId = $("#AccountTypeId").val();
        if (typeId) payload.accountTypeId = typeId;

        $.ajax({
            url: soUrl("bank-save-url"),
            type: "POST",
            data: payload,
            dataType: "json",
            success: function (res) {
                if (!res || !res.success) {
                    if (!quiet) alert((res && res.message) || "Failed to save bank account");
                    if (done) done(false);
                    return;
                }
                appendBankRow(res);
                bankList.push({
                    BankAccountId: res.bankAccountId || res.BankAccountId,
                    AccountTypeName: res.accountTypeName || res.AccountTypeName,
                    BankName: res.bankName || res.BankName,
                    RoutingNumber: res.routingNumber || res.RoutingNumber,
                    AccountNumber: res.accountNumber || res.AccountNumber,
                    IsPrimary: res.isPrimary === true || res.IsPrimary === true
                });
                clearBankForm();
                if (!quiet) alert("Bank account saved.");
                if (done) done(true);
            },
            error: function (xhr) {
                if (!quiet) alert("Failed to save bank account. " + (xhr.responseText || xhr.statusText || ""));
                if (done) done(false);
            }
        });
    }

    function savePendingBankThen(next) {
        if (readOnly || !hasPendingBankForm()) {
            if (next) next(true);
            return;
        }
        saveBankAccountAjax({ quiet: true, done: next });
    }

    function buildReview() {
        $("#soReviewSummary").html("<p>Loading review...</p>");
        refreshBanks(function () {
            var html = "<p><strong>Name:</strong> " + ($("#FirstName").val() || "") + " " + ($("#LastName").val() || "") + "</p>";
            html += "<p><strong>Emails:</strong> " + ($("#WorkEmail").val() || "") + " / " + ($("#HomeEmail").val() || "") + "</p>";
            html += "<p><strong>I-9:</strong> " + (($("#btnOpenI9").text() || "").toLowerCase().indexOf("unsign") >= 0 ? "Signed" : "Pending") + "</p>";
            html += "<p><strong>Tax / W-4:</strong> " + (($("#btnOpenW4").text() || "").toLowerCase().indexOf("unsign") >= 0 ? "Signed" : "Pending") + "</p>";
            html += "<p><strong>Bank accounts:</strong> " + bankList.length + "</p>";
            if (bankList.length > 0) {
                html += "<ul>";
                bankList.forEach(function (b) {
                    var typeName = b.AccountTypeName || b.accountTypeName || "Account";
                    var bank = b.BankName || b.bankName || "";
                    var acct = b.AccountNumber || b.accountNumber || "";
                    html += "<li>" + $("<div/>").text(typeName + " - " + bank + " (" + acct + ")").html() + "</li>";
                });
                html += "</ul>";
            }
            $("#soReviewSummary").html(html);
        });
    }

    function personalPayload(extra) {
        var data = {
            step: currentStep,
            PrefixId: $("#PrefixId").val(),
            SuffixId: $("#SuffixId").val(),
            FirstName: $("#FirstName").val(),
            MiddleName: $("#MiddleName").val(),
            LastName: $("#LastName").val(),
            WorkEmail: $("#WorkEmail").val(),
            HomeEmail: $("#HomeEmail").val(),
            Phone: $("#Phone").val(),
            MaritalStatusId: $("#MaritalStatusId").val(),
            EthnicityId: $("#EthnicityId").val(),
            GenderId: $("#GenderId").val(),
            LicenseCountryId: $("#LicenseCountryId").val(),
            Address1: $("#Address1").val(),
            City: $("#City").val(),
            StateId: $("#StateId").val(),
            Zip: $("#Zip").val(),
            CountryId: $("#CountryId").val(),
            EmergencyName: $("#EmergencyName").val(),
            EmergencyPhone: $("#EmergencyPhone").val(),
            RelationshipTypeId: $("#RelationshipTypeId").val(),
            FilingStatusId: $("#FilingStatusId").val(),
            WorkingCountryId: $("#WorkingCountryId").val(),
            WorkingStateId: $("#WorkingStateId").val(),
            StateTaxStatusId: $("#StateTaxStatusId").val()
        };
        if (extra) $.extend(data, extra);
        return data;
    }

    function saveThen(nextStep) {
        if (readOnly) { showStep(nextStep); return; }
        var stepToSave = currentStep;
        $.post(soUrl("save-url"), personalPayload({ step: stepToSave }), function (res) {
            if (!res.success) { alert(res.message || "Save failed"); return; }
            showStep(nextStep);
        });
    }

    var i9FormSaved = false;
    var pendingI9Sign = false;
    var taxFormSaved = false;

    function toDateInput(val) {
        if (!val) return "";
        var d = new Date(val);
        if (isNaN(d.getTime())) {
            var m = String(val).match(/(\d{4})-(\d{2})-(\d{2})/);
            if (m) return m[1] + "-" + m[2] + "-" + m[3];
            return "";
        }
        var mm = ("0" + (d.getMonth() + 1)).slice(-2);
        var dd = ("0" + d.getDate()).slice(-2);
        return d.getFullYear() + "-" + mm + "-" + dd;
    }

    function syncI9CitizenPanels() {
        var v = $("input[name='i9CitizenStatus']:checked").val();
        $("#trLaw").toggle(v === "1");
        $("#trAlien").toggle(v === "2");
    }

    function validateI9Form() {
        var status = parseInt($("input[name='i9CitizenStatus']:checked").val(), 10) || 0;
        var today = new Date();
        today.setHours(0, 0, 0, 0);

        if (status === 1) {
            var lawDt = $("#rdpLawExpiration").val();
            if (!lawDt) { alert("Please select an expiration date."); return false; }
            if (new Date(lawDt) <= today) { alert("Please enter an expiration date after the current date."); return false; }
            if (!$("#rcLawCitizenof").val()) { alert("Please select Citizen of."); return false; }
        }
        if (status === 2) {
            var alienDt = $("#rdpStartDate").val();
            if (!alienDt) { alert("Please select an expiration date."); return false; }
            if (new Date(alienDt) <= today) { alert("Please enter an expiration date after the current date."); return false; }
            if (!$("#rcAlienCitizenof").val()) { alert("Please select Citizen of."); return false; }
        }
        if (!$("#btnChkfederal").is(":checked")) {
            alert("Please check that you agree to federal law.");
            return false;
        }
        var alienReg = ($("#txtAlienRegistration").val() || "").trim();
        var admission = ($("#txtAdmissionNumber").val() || "").trim();
        var passport = ($("#txtPassPortNumber").val() || "").trim();
        var country = $("#rcCountryofIssuance").val();
        if (alienReg && (admission || passport || country)) {
            alert("Please enter either an Alien Registration Number/USCIS Number OR Form I-94 Admission Number.");
            return false;
        }
        return true;
    }

    function i9Payload(extra) {
        var status = parseInt($("input[name='i9CitizenStatus']:checked").val(), 10) || 0;
        var data = {
            citizenStatus: status,
            alienNumber: $("#txtAlienNumber").val(),
            permanentResidentExpire: $("#rdpLawExpiration").val(),
            lawCitizenOfId: $("#rcLawCitizenof").val() || null,
            lawCitizenOfText: ($("#rcLawCitizenof").val() ? $("#rcLawCitizenof option:selected").text() : ""),
            alienAuthorizedUntil: $("#rdpStartDate").val(),
            alienCitizenOfId: $("#rcAlienCitizenof").val() || null,
            alienCitizenOfText: ($("#rcAlienCitizenof").val() ? $("#rcAlienCitizenof option:selected").text() : ""),
            alienRegistrationNumber: $("#txtAlienRegistration").val(),
            admissionNumber: $("#txtAdmissionNumber").val(),
            passportNumber: $("#txtPassPortNumber").val(),
            countryOfIssuanceId: $("#rcCountryofIssuance").val() || null,
            countryOfIssuanceText: ($("#rcCountryofIssuance").val() ? $("#rcCountryofIssuance option:selected").text() : ""),
            translatorNotUsed: $("#chkTranslatorNotUsed").is(":checked"),
            translatorUsed: $("#chkTranslatorUsed").is(":checked"),
            federalLawAcknowledged: $("#btnChkfederal").is(":checked"),
            hideSsnOnForm: $("#btnchkSSN").is(":checked")
        };
        if (extra) $.extend(data, extra);
        return data;
    }

    function loadI9Form(done) {
        $.getJSON(soUrl("i9-url"), withHireId({}), function (res) {
            if (!res.success) { if (done) done(false); return; }
            var countries = res.countries || (lookups && lookups.countries) || [];
            var d = res.data || {};
            fillSelect($("#rcLawCitizenof"), countries, d.LawCitizenOfId);
            fillSelect($("#rcAlienCitizenof"), countries, d.AlienCitizenOfId);
            fillSelect($("#rcCountryofIssuance"), countries, d.CountryOfIssuanceId);

            $("input[name='i9CitizenStatus'][value='" + (d.CitizenStatus || 0) + "']").prop("checked", true);
            $("#txtAlienNumber").val(d.AlienNumber || "");
            $("#rdpLawExpiration").val(toDateInput(d.PermanentResidentExpire));
            $("#rdpStartDate").val(toDateInput(d.AlienAuthorizedUntil));
            $("#txtAlienRegistration").val(d.AlienRegistrationNumber || "");
            $("#txtAdmissionNumber").val(d.AdmissionNumber || "");
            $("#txtPassPortNumber").val(d.PassportNumber || "");
            $("#chkTranslatorNotUsed").prop("checked", d.TranslatorNotUsed !== false);
            $("#chkTranslatorUsed").prop("checked", !!d.TranslatorUsed);
            $("#btnChkfederal").prop("checked", !!d.FederalLawAcknowledged);
            $("#btnchkSSN").prop("checked", !!d.HideSsnOnForm);
            syncI9CitizenPanels();

            i9FormSaved = !!(d.FederalLawAcknowledged);
            $("#i9SavedMsg").toggle(i9FormSaved && !d.IsSigned);
            if (d.IsSigned) {
                $("#btnOpenI9").text("View or Unsign I9");
                $("#linkViewI9").show();
            }
            if (done) done(true);
        }).fail(function () { if (done) done(false); });
    }

    function saveI9Form(onSuccess) {
        if (!validateI9Form()) return;
        $.post(soUrl("i9-save-url"), i9Payload(), function (res) {
            if (!res.success) { alert(res.message || "Save failed"); return; }
            i9FormSaved = true;
            $("#i9SavedMsg").show();
            if (onSuccess) onSuccess(res);
            else alert(res.message || "Form I-9 saved.");
        });
    }

    function validateTaxForm() {
        if (!$("#FederalExempt").is(":checked") && !$("#FilingStatusId").val()) {
            alert("Please select Filing Status.");
            return false;
        }
        if (!$("#WorkingCountryId").val()) { alert("Please select Work in Country."); return false; }
        if (!$("#WorkingStateId").val()) { alert("Please select Work in State."); return false; }
        if (!$("#StateExempt").is(":checked")) {
            if (!$("#StateTaxStatusId").val()) { alert("Please select State Taxes Withholding Status."); return false; }
            if (!($("#StateExemptions").val() || "").trim()) { alert("Please enter State Taxes Exemptions."); return false; }
        }
        return true;
    }

    function taxPayload(extra) {
        var data = {
            filingStatusId: $("#FilingStatusId").val() || null,
            otherIncomeAmount: $("#OtherIncomeAmount").val(),
            deductionsAmount: $("#DeductionsAmount").val(),
            extraWithholdingAmount: $("#ExtraWithholdingAmount").val(),
            extraWithholdingPercent: $("#ExtraWithholdingPercent").val() || "0",
            federalExempt: $("#FederalExempt").is(":checked"),
            copyFromFederal: $("#CopyFromFederal").is(":checked"),
            workingCountryId: $("#WorkingCountryId").val() || null,
            workingStateId: $("#WorkingStateId").val() || null,
            stateTaxStatusId: $("#StateTaxStatusId").val() || null,
            stateExemptions: $("#StateExemptions").val(),
            stateAdditionalWithholdingAmount: $("#StateAdditionalWithholdingAmount").val(),
            stateAdditionalWithholdingPercent: $("#StateAdditionalWithholdingPercent").val() || "0",
            stateExempt: $("#StateExempt").is(":checked")
        };
        if (extra) $.extend(data, extra);
        return data;
    }

    function loadTaxForm(done) {
        $.getJSON(soUrl("tax-url"), withHireId({}), function (res) {
            if (!res.success) { if (done) done(false); return; }
            var d = res.data || {};
            fillSelect($("#FilingStatusId"), lookups.filingStatus, d.FilingStatusId);
            fillSelect($("#WorkingCountryId"), lookups.countries, d.WorkingCountryId);
            fillSelect($("#WorkingStateId"), lookups.states, d.WorkingStateId);
            fillSelect($("#StateTaxStatusId"), lookups.stateTaxStatus, d.StateTaxStatusId);
            $("#OtherIncomeAmount").val(d.OtherIncomeAmount != null ? d.OtherIncomeAmount : "");
            $("#DeductionsAmount").val(d.DeductionsAmount != null ? d.DeductionsAmount : "");
            $("#ExtraWithholdingAmount").val(d.ExtraWithholdingAmount != null ? d.ExtraWithholdingAmount : "");
            $("#ExtraWithholdingPercent").val(d.ExtraWithholdingPercent != null ? d.ExtraWithholdingPercent : "0");
            $("#FederalExempt").prop("checked", !!d.FederalExempt);
            $("#CopyFromFederal").prop("checked", !!d.CopyFromFederal);
            $("#StateExemptions").val(d.StateExemptions || "");
            $("#StateAdditionalWithholdingAmount").val(d.StateAdditionalWithholdingAmount != null ? d.StateAdditionalWithholdingAmount : "");
            $("#StateAdditionalWithholdingPercent").val(d.StateAdditionalWithholdingPercent != null ? d.StateAdditionalWithholdingPercent : "0");
            $("#StateExempt").prop("checked", !!d.StateExempt);
            taxFormSaved = !!(d.FilingStatusId || d.FederalExempt);
            $("#taxSavedMsg").toggle(taxFormSaved && !d.IsSigned);
            if (d.IsSigned) {
                $("#btnOpenW4").text("View or Unsign W-4");
                $("#linkViewW4").show();
            }
            if (done) done(true);
        }).fail(function () { if (done) done(false); });
    }

    function saveTaxForm(onSuccess) {
        if (!validateTaxForm()) return;
        $.post(soUrl("tax-save-url"), taxPayload(), function (res) {
            if (!res.success) { alert(res.message || "Save failed"); return; }
            taxFormSaved = true;
            $("#taxSavedMsg").show();
            if (onSuccess) onSuccess(res);
            else alert(res.message || "Tax election saved.");
        });
    }

    function fillW4Preview() {
        $("#w4EmpName").text(hireName || (($("#FirstName").val() || "") + " " + ($("#LastName").val() || "")));
        $("#w4FileNumber").text($("#soStatusBar").text().match(/Badge:\s*(\S+)/) ? RegExp.$1 : "");
        $("#w4Email").text($("#HomeEmail").val() || "");
        $("#w4Address").text(($("#Address1").val() || "") + " " + ($("#City").val() || "") + " " + ($("#Zip").val() || ""));
        $("#w4FilingStatus").text($("#FilingStatusId option:selected").text() || "");
        $("#w4OtherIncome").text($("#OtherIncomeAmount").val() || "0");
        $("#w4Deductions").text($("#DeductionsAmount").val() || "0");
        $("#w4ExtraAmt").text($("#ExtraWithholdingAmount").val() || "0");
        $("#w4ExtraPct").text($("#ExtraWithholdingPercent").val() || "0");
        $("#w4FedExempt").text($("#FederalExempt").is(":checked") ? "Yes" : "No");
        $("#w4WorkState").text(($("#WorkingCountryId option:selected").text() || "") + " / " + ($("#WorkingStateId option:selected").text() || ""));
        $("#w4StateStatus").text($("#StateTaxStatusId option:selected").text() || "");
        $("#w4StateExempts").text($("#StateExemptions").val() || "");
        $("#w4StateExtraAmt").text($("#StateAdditionalWithholdingAmount").val() || "0");
        $("#w4StateExtraPct").text($("#StateAdditionalWithholdingPercent").val() || "0");
        $("#w4SigName").text(hireName || (($("#FirstName").val() || "") + " " + ($("#LastName").val() || "")));
        $("#w4SigWhen").text(new Date().toLocaleString());
    }

    $.ajax({
        url: soUrl("wizard-url"),
        type: "GET",
        dataType: "json",
        data: withHireId({}),
        cache: false,
        success: function (res) {
            if (!res || !res.success) {
                refreshBanks(function () { showStep(isHrReview ? 7 : currentStep); });
                return;
            }
            lookups = res.lookups || {};
            var p = (res.data && res.data.Personal) || {};
            fillSelect($("#PrefixId"), lookups.prefixes, p.PrefixId);
            fillSelect($("#SuffixId"), lookups.suffixes, p.SuffixId);
            fillSelect($("#MaritalStatusId"), lookups.marital, p.MaritalStatusId);
            fillSelect($("#EthnicityId"), lookups.ethnicity, p.EthnicityId);
            fillSelect($("#GenderId"), lookups.genders, p.GenderId);
            fillSelect($("#LicenseCountryId"), lookups.countries, p.LicenseCountryId);
            fillSelect($("#CountryId"), lookups.countries, p.CountryId);
            fillSelect($("#StateId"), lookups.states, p.StateId);
            fillSelect($("#RelationshipTypeId"), lookups.relationships, p.RelationshipTypeId);
            fillSelect($("#AccountTypeId"), lookups.accountTypes, null);

            hireName = ((p.FirstName || $("#FirstName").val()) + " " + (p.LastName || $("#LastName").val())).trim();
            if (isHrReview)
                currentStep = 7;
            else if (res.data && res.data.Hire && res.data.Hire.CurrentStep)
                currentStep = res.data.Hire.CurrentStep;

            var banks = (res.data && (res.data.BankAccounts || res.data.bankAccounts)) || [];
            renderBankTable(banks);

            loadI9Form(function () {
                loadTaxForm(function () {
                    refreshBanks(function () { showStep(currentStep); });
                });
            });
        },
        error: function () {
            refreshBanks(function () { showStep(isHrReview ? 7 : currentStep); });
        }
    });

    $("#soStepNav li").on("click", function () {
        var step = parseInt($(this).data("step"), 10);
        if (!step) return;
        if (currentStep === 6 && step !== 6 && !readOnly && hasPendingBankForm()) {
            savePendingBankThen(function (ok) {
                if (ok) showStep(step);
            });
            return;
        }
        showStep(step);
    });

    $(document).on("click", ".btn-so-next", function () {
        if (currentStep === 1) { showStep(2); return; }
        if (currentStep === 3) {
            if (readOnly) { showStep(4); return; }
            if (!validateI9Form()) return;
            saveI9Form(function () { showStep(4); });
            return;
        }
        if (currentStep === 5) {
            if (readOnly) { showStep(6); return; }
            if (!validateTaxForm()) return;
            saveTaxForm(function () { showStep(6); });
            return;
        }
        if (currentStep === 6) {
            if (readOnly) { showStep(7); return; }
            savePendingBankThen(function (ok) {
                if (!ok) return;
                saveThen(7);
            });
            return;
        }
        if (currentStep >= 7) return;
        saveThen(currentStep + 1);
    });

    $(document).on("click", ".btn-so-prev", function () {
        if (currentStep > 1) showStep(currentStep - 1);
    });

    $(document).on("change", "input[name='i9CitizenStatus']", function () {
        syncI9CitizenPanels();
        i9FormSaved = false;
        $("#i9SavedMsg").hide();
    });

    $("#chkTranslatorNotUsed").on("change", function () {
        if (this.checked) $("#chkTranslatorUsed").prop("checked", false);
        i9FormSaved = false;
        $("#i9SavedMsg").hide();
    });
    $("#chkTranslatorUsed").on("change", function () {
        if (this.checked) $("#chkTranslatorNotUsed").prop("checked", false);
        i9FormSaved = false;
        $("#i9SavedMsg").hide();
    });

    $("#txtAlienNumber, #rdpLawExpiration, #rcLawCitizenof, #rdpStartDate, #rcAlienCitizenof, #txtAlienRegistration, #txtAdmissionNumber, #txtPassPortNumber, #rcCountryofIssuance, #btnChkfederal, #btnchkSSN").on("change input", function () {
        i9FormSaved = false;
        $("#i9SavedMsg").hide();
    });

    $("#btnSaveI9").on("click", function () {
        if (readOnly) return;
        saveI9Form();
    });

    $("#btnSaveTax").on("click", function () {
        if (readOnly) return;
        saveTaxForm();
    });

    $("#CopyFromFederal").on("change", function () {
        if (!this.checked) return;
        var fedText = ($("#FilingStatusId option:selected").text() || "").toLowerCase();
        $("#StateTaxStatusId option").each(function () {
            var t = ($(this).text() || "").toLowerCase();
            if (!t || t.indexOf("select") >= 0) return;
            if (fedText.indexOf("single") >= 0 && t.indexOf("single") >= 0) $("#StateTaxStatusId").val($(this).val());
            if (fedText.indexOf("married") >= 0 && t.indexOf("married") >= 0) $("#StateTaxStatusId").val($(this).val());
        });
        taxFormSaved = false;
        $("#taxSavedMsg").hide();
    });

    $("#FilingStatusId, #OtherIncomeAmount, #DeductionsAmount, #ExtraWithholdingAmount, #ExtraWithholdingPercent, #FederalExempt, #WorkingCountryId, #WorkingStateId, #StateTaxStatusId, #StateExemptions, #StateAdditionalWithholdingAmount, #StateAdditionalWithholdingPercent, #StateExempt").on("change input", function () {
        taxFormSaved = false;
        $("#taxSavedMsg").hide();
    });

    $("#btnOpenI9").on("click", function () {
        var isUnsign = (($(this).text() || "").toLowerCase().indexOf("unsign") >= 0);
        if (isUnsign) {
            if (readOnly) return;
            $.post(soUrl("unsign-url"), { documentKey: "I9", profileDocumentId: null }, function (res) {
                if (!res.success) { alert(res.message || "Failed"); return; }
                $("#btnOpenI9").text(res.buttonText || "View & Sign I9");
                $("#linkViewI9").hide();
            });
            return;
        }

        if (!validateI9Form()) return;
        saveI9Form(function () {
            pendingI9Sign = true;
            $("#sigDocKey").val("I9");
            $("#sigDocId").val("");
            $("#sigName").text(hireName || (($("#FirstName").val() || "") + " " + ($("#LastName").val() || "")));
            $("#sigWhen").text(new Date().toLocaleString());
            $("#sigIp").text("Captured on save");
            $("#sigTxn").text("Generated on save");
            $("#soSignModal").modal("show");
        });
    });

    $("#btnOpenW4").on("click", function () {
        var isUnsign = (($(this).text() || "").toLowerCase().indexOf("unsign") >= 0);
        if (isUnsign) {
            if (readOnly) return;
            $.post(soUrl("unsign-url"), { documentKey: "W4", profileDocumentId: null }, function (res) {
                if (!res.success) { alert(res.message || "Failed"); return; }
                $("#btnOpenW4").text(res.buttonText || "View and Sign W-4");
                $("#linkViewW4").hide();
            });
            return;
        }

        if (!validateTaxForm()) return;
        saveTaxForm(function () {
            fillW4Preview();
            $("#btnConfirmSignW4").prop("disabled", !!readOnly);
            $("#soW4Modal").modal("show");
        });
    });

    $("#btnConfirmSignW4").on("click", function () {
        if (readOnly) return;
        $.post(soUrl("w4-sign-url"), taxPayload({
            signedName: $("#w4SigName").text()
        }), function (res) {
            if (!res.success) { alert(res.message || "Failed"); return; }
            $("#soW4Modal").modal("hide");
            $("#btnOpenW4").text(res.buttonText || "View or Unsign W-4");
            $("#linkViewW4").show();
            $("#taxSavedMsg").hide();
            alert("W-4 signed and saved to employee documents.\nName: " + res.signedName + "\nDate: " + res.signedDate + "\nIP: " + res.signedIp + "\nTxn: " + res.transactionId);
        });
    });

    $(document).on("click", ".btn-sign-doc", function () {
        if (readOnly) return;
        var $btn = $(this);
        var key = $btn.data("key");
        var docId = $btn.data("doc") || "";
        var isUnsign = ($btn.text() || "").toLowerCase().indexOf("unsign") >= 0 || ($btn.text() || "").toLowerCase().indexOf("un sign") >= 0;

        if (isUnsign) {
            $.post(soUrl("unsign-url"), { documentKey: key, profileDocumentId: docId || null }, function (res) {
                if (!res.success) { alert(res.message || "Failed"); return; }
                $btn.text(res.buttonText || (key === "DOC" ? "Sign" : ("Sign " + key)));
                $btn.closest("tr").find(".doc-status").text("Pending");
                $btn.closest("tr").find(".doc-check").hide();
                if (key === "W4") $("#linkViewW4").hide();
            });
            return;
        }

        pendingI9Sign = false;
        $("#sigDocKey").val(key);
        $("#sigDocId").val(docId);
        $("#sigName").text(hireName || (($("#FirstName").val() || "") + " " + ($("#LastName").val() || "")));
        $("#sigWhen").text(new Date().toLocaleString());
        $("#sigIp").text("Captured on save");
        $("#sigTxn").text("Generated on save");
        $("#soSignModal").modal("show");
    });

    $("#btnConfirmSign").on("click", function () {
        var key = $("#sigDocKey").val();
        var docId = $("#sigDocId").val();

        if (key === "I9" || pendingI9Sign) {
            pendingI9Sign = false;
            $.post(soUrl("i9-sign-url"), i9Payload({ signedName: $("#sigName").text() }), function (res) {
                if (!res.success) { alert(res.message || "Failed"); return; }
                $("#soSignModal").modal("hide");
                $("#btnOpenI9").text(res.buttonText || "View or Unsign I9");
                $("#linkViewI9").show();
                $("#i9SavedMsg").hide();
                alert("I-9 signed and saved to employee documents.\nName: " + res.signedName + "\nDate: " + res.signedDate + "\nIP: " + res.signedIp + "\nTxn: " + res.transactionId);
            });
            return;
        }

        $.post(soUrl("sign-url"), {
            documentKey: key,
            profileDocumentId: docId || null,
            signedName: $("#sigName").text()
        }, function (res) {
            if (!res.success) { alert(res.message || "Failed"); return; }
            $("#soSignModal").modal("hide");
            var $btn = $(".btn-sign-doc").filter(function () {
                return $(this).data("key") == key && String($(this).data("doc") || "") === String(docId || "");
            }).first();
            if (!$btn.length && key === "W4") $btn = $("#btnSignW4");
            $btn.text(res.buttonText || (key === "DOC" ? "Unsign" : ("Unsign " + key)));
            $btn.closest("tr").find(".doc-status").text("Signed");
            $btn.closest("tr").find(".doc-check").css("color", "green").show();
            if (key === "W4") $("#linkViewW4").show();
            alert("Signed.\nName: " + res.signedName + "\nDate: " + res.signedDate + "\nIP: " + res.signedIp + "\nTxn: " + res.transactionId);
        });
    });

    $(document).on("change", ".so-upload", function () {
        if (readOnly) return;
        var file = this.files[0];
        if (!file) return;
        var $input = $(this);
        var docId = $input.data("doc");
        var fd = new FormData();
        fd.append("profileDocumentId", docId);
        fd.append("file", file);
        $.ajax({
            url: soUrl("upload-url"),
            type: "POST",
            data: fd,
            processData: false,
            contentType: false,
            success: function (res) {
                if (!res.success) { alert(res.message || "Upload failed"); return; }
                var $cell = $input.closest("td");
                if (!$cell.find(".btn-view-pdf").length) {
                    var viewBase = soUrl("view-doc-url") || "";
                    var href = viewBase + (viewBase.indexOf("?") >= 0 ? "&" : "?") + "profileDocumentId=" + encodeURIComponent(docId) + "&preferSigned=true";
                    $cell.prepend('<a class="btn btn-xs btn-default btn-view-pdf" href="' + href + '" target="_blank" rel="noopener">View PDF</a> ');
                }
                alert("Uploaded: " + (res.fileName || "file"));
            }
        });
    });

    $("#btnAddBank").on("click", function () {
        if (readOnly) return;
        saveBankAccountAjax({ quiet: false });
    });

    $(document).on("click", ".btn-del-bank", function () {
        if (readOnly) return;
        var id = $(this).data("id");
        $.ajax({
            url: soUrl("bank-del-url"),
            type: "POST",
            data: { bankAccountId: id },
            dataType: "json",
            success: function (res) {
                if (!res || !res.success) { alert((res && res.message) || "Failed"); return; }
                $("#soBankTable tbody tr[data-id='" + id + "']").remove();
                bankList = bankList.filter(function (b) {
                    return String(b.BankAccountId || b.bankAccountId) !== String(id);
                });
            },
            error: function () { alert("Failed to delete bank account."); }
        });
    });

    $("#btnSubmitOnboarding").on("click", function () {
        if (readOnly || isHrReview) return;
        if (!confirm("Submit your new hire documents?")) return;
        $.post(soUrl("submit-url"), function (res) {
            if (!res.success) { alert(res.message || "Failed"); return; }
            $("#soDoneMessage").text(res.message || "Congratulations! You have completed your new hire documents.");
            $("#soDoneTxn").text(res.transactionId || "");
            $("#soDoneDate").text(res.confirmationDate || "");
            $("#soDoneModal").modal("show");
            $root.data("readonly", "1");
            readOnly = true;
            $("#btnSubmitOnboarding").prop("disabled", true);
            $("#soRejectionBanner").hide();
        });
    });

    $("#btnHrApproveHire").on("click", function () {
        if (!isHrReview || !reviewHireId) return;
        if (!confirm("Approve this registration as Hired?")) return;
        $.post(soUrl("approve-url"), { hireId: reviewHireId }, function (res) {
            if (!res.success) { alert(res.message || "Failed"); return; }
            alert(res.message || "Approved.");
            if (typeof LoadMenu === "function") LoadMenu("HireReviewPartial", "SelfOnboarding");
        });
    });

    $("#btnHrRejectHire").on("click", function () {
        if (!isHrReview || !reviewHireId) return;
        $(".so-reject-form").prop("checked", false);
        $("#soRejectReason").val("");
        $("#soRejectModal").modal("show");
    });

    $("#btnConfirmRejectHire").on("click", function () {
        if (!isHrReview || !reviewHireId) return;
        var forms = [];
        $(".so-reject-form:checked").each(function () { forms.push($(this).val()); });
        var reason = ($("#soRejectReason").val() || "").trim();
        if (!forms.length) { alert("Select at least one form to correct."); return; }
        if (!reason) { alert("Enter a rejection reason."); return; }
        $.post(soUrl("reject-url"), {
            hireId: reviewHireId,
            reason: reason,
            formNames: forms.join(", ")
        }, function (res) {
            if (!res.success) { alert(res.message || "Failed"); return; }
            $("#soRejectModal").modal("hide");
            alert(res.message || "Rejection sent.");
            if (typeof LoadMenu === "function") LoadMenu("HireReviewPartial", "SelfOnboarding");
        });
    });
})();
