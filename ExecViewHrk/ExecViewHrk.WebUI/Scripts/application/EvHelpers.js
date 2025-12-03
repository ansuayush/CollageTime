var _personContext;
var FormFocusField = "";
var specificFieldValidatiommsg = "";
var requiredFieldValidatiommsg = false;

(function ($) {
    $.fn.serializeObject = function () {

        var self = this,
            json = {},
            push_counters = {},
            patterns = {
                "validate": /^[a-zA-Z][a-zA-Z0-9_]*(?:\[(?:\d*|[a-zA-Z0-9_]+)\])*$/,
                "key": /[a-zA-Z0-9_]+|(?=\[\])/g,
                "push": /^$/,
                "fixed": /^\d+$/,
                "named": /^[a-zA-Z0-9_]+$/
            };


        this.build = function (base, key, value) {
            base[key] = value;
            return base;
        };

        this.push_counter = function (key) {
            if (push_counters[key] === undefined) {
                push_counters[key] = 0;
            }
            return push_counters[key]++;
        };

        $.each($(this).serializeArray(), function () {

            // skip invalid keys
            if (!patterns.validate.test(this.name)) {
                return;
            }

            var k,
                keys = this.name.match(patterns.key),
                merge = this.value,
                reverse_key = this.name;

            while ((k = keys.pop()) !== undefined) {

                // adjust reverse_key
                reverse_key = reverse_key.replace(new RegExp("\\[" + k + "\\]$"), '');

                // push
                if (k.match(patterns.push)) {
                    merge = self.build([], self.push_counter(reverse_key), merge);
                }

                // fixed
                else if (k.match(patterns.fixed)) {
                    merge = self.build([], k, merge);
                }

                // named
                else if (k.match(patterns.named)) {
                    merge = self.build({}, k, merge);
                }
            }

            json = $.extend(true, json, merge);
        });

        return json;
    };
})(jQuery);

function error_handler(e) {
    if (e.errors) {
        toastr.error(e.errors);
    }
}
function addPerson() {
    openPersonPopup(0);
}
function openPersonPopup(_personId) {
    loading(true);
    var formURL = $("#ApplicationUrl").val() + '/Person/PersonDetail?personId=' + _personId;

    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);

    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');

}
function refreshScreen(selectedPersonId) {
    if (typeof (selectedPersonId) != "undefined") {
        GetEmployeeProfile(selectedPersonId);
    }
}

function GetEmployeeProfile(ddlSelectedPersonId) {


    if (ddlSelectedPersonId == "") return;
    $("#SelectedPersonID").val(ddlSelectedPersonId);
    $.ajax({
        url: $("#ApplicationUrl").val() + "Person/GetPersonProfileAjax",
        type: "POST",
        cache: false,
        beforeSend: function () {
            loading(true);
        },
        data: {
            personId: ddlSelectedPersonId,
            personType: $("#SelectedPersonType").val()
        },
        success: function (response) {
            $("#SelectedPersonID").val(response.PersonId);
            $("#PersonNameLabel").text(response.FirstName + " " + response.LastName);
            $('#personPictureDiv').html('<img width="100" height="100" src="data:image/jpg;base64,' + response.PersonImageBase64 + '"/>');
            $("#PersonCompanyCodeLabel").text(response.CompanyCode == null ? "" : response.CompanyCode);
            $("#PersonFileNumberLabel").text(response.FileNumber == null ? "" : response.FileNumber);
            $("#PersonHireDateLabel").text(response.HireDate == null ? "" : response.HireDate);
            $("#PersonPhoneLabel").text(response.PhoneNumber == null ? "" : response.PhoneNumber);
            $("#PersonPositionTitleLabel").text(response.PositionTitle == null ? "" : response.PositionTitle);
            LoadContents($("#HD_VIEW").val(), $("#HD_CONTROLLER").val());
            //return;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //toastr.error(errorThrown, "error");
            loading(false);
        },
    });
}


///Nitesh////////////


function ValidateForm(fromID) {
    loading(true);
    var FormValid = true;
    var IsValid = true;
    FormFocusField = "";
    specificFieldValidatiommsg = "";
    $("#" + fromID + " *").filter(':input').not("input[type=hidden]").each(function () {
        try {
            if (FormValid)
                FormValid = ValidateElements($(this).attr("id"), true);
            else
                ValidateElements($(this).attr("id"), true);
        }
        catch (err) {
            //alert(err);
        }
        // This try catch block is added temporarily to by pass issue with additional fields in claim detail. it will be removed later
    });

    //VLCT-131 restrict save if FormValid is false and cutom validation is true.
    IsValid = FormValid;
    
    if (!FormValid) {
        //if any validation occur need to apply the format.
        ApplyFormCurrecnyOrNumberFromat(fromID);
    }

    if (!FormValid && FormFocusField != "") {
        $("#" + FormFocusField + "").focus();
        var labelValue = GetlabelText(FormFocusField);
        if (labelValue.indexOf("*") > -1) {
            if (typeof (IsIframe) == "string")
                IsIframe = replaceAll(labelValue, "*", "");
            else
                toastr.error(replaceAll(labelValue, "*", ""));
        }
        else {
            if (typeof (IsIframe) == "string")
                IsIframe = replaceAll(labelValue, "*", "");
            else
                toastr.error(labelValue);
        }
    }
    if (!FormValid) { loading(false); }
    return FormValid;
}

function GetlabelText(elementID) {
    var elementText = "Please enter valid data.";
    var newelementText = "";
    try {
        var eleID = "#" + elementID;
        if (requiredFieldValidatiommsg) {
            //Commented out but may required.
            //if (eleID == "#" || eleID == "#undefined") { elementText = "Please enter valid data."; }
            //else
            //{
            //    if ($("#" + FormFocusField + "").prev().length > 0) {
            //        if ($("#" + FormFocusField + "").prev().hasClass("control-label")) { elementText = "Please enter " + $("#" + FormFocusField + "").prev()[0].textContent; }
            //    }
            //    else {
            //        elementText = "Please enter " + $("#" + FormFocusField + "").parent().prev()[0].textContent;
            //    }
            //}
            //requiredFieldValidatiommsg = false;
        }
        else {
            elementText = "<ul>" + specificFieldValidatiommsg + "</ul>";
            if (eleID == "#TerminationDate")
                var newelementText = elementText.replace("TerminationDate", "Termination Date")
        }
    } catch (err) {
    }
    if (eleID == "#TerminationDate") {
        return newelementText;
    }
    else {
        return elementText;
    }

}

function GetFieldCaption(elementID) {
    var elementText = "";
    try {
        if (elementID != "#" && elementID != "undefined") {
            if ($("#" + elementID + "").parent().hasClass("time_pick")) {
                elementText = $("#" + elementID + "").parent().prev()[0].textContent;
            }
            else {
                if (($("#" + elementID + "").prev().length > 0) && (!$("#" + elementID + "").prev().hasClass("input-group-addon"))) {

                    elementText = $("#" + elementID + "").prev()[0].textContent.replace("*","");
                   

                }
                else {
                    if (($("#" + elementID + "").parent().hasClass("form-group") || $("#" + elementID + "").parent().hasClass("input-group")) && ($("#" + elementID + "").parent().prev().length > 0))
                        elementText = $("#" + elementID + "").parent().prev()[0].textContent;
                    else if (($("#" + elementID + "").parent().parent().hasClass("form-group")) && ($("#" + elementID + "").parent().parent().prev().length > 0))
                        elementText = $("#" + elementID + "").parent().parent().prev()[0].textContent;
                }
            }
        }
    } catch (err) {
    }
    return replaceAll(elementText, "*", "");
}

function SetRequiredField(fieldID, isTrue) {
    var fieldCaption = GetFieldCaption(fieldID)
    if (isTrue) {
        $("#" + fieldID + "").data('required', 'Y');
        $("#" + fieldID + "").prev().html(fieldCaption + "<span style='color: red'><strong>*</strong></span>");
    }
    else {
        $("#" + fieldID + "").data('required', 'N');
        $("#" + fieldID + "").prev().html(fieldCaption);
    }
}

function SetReadOnlyField(fieldID, isTrue) {
    var fieldCaption = GetFieldCaption(fieldID)
    if (isTrue) {

        if ($("#" + fieldID + "").is('select')) {
            $("#" + fieldID + "").attr("disabled", true);
        } else {
            $("#" + fieldID + "").prop("readonly", true);
        }
        $("#" + fieldID + "").removeAttr("tabindex");
        $("#" + fieldID + "").attr("tabindex", -1);
        $("#" + fieldID + "").attr("onfocus", "this.blur();");

    }
    else {
        if ($("#" + fieldID + "").is('select')) {
            $("#" + fieldID + "").removeAttr("disabled");
        } else {
            $("#" + fieldID + "").prop("readonly", false);
        }
        $("#" + fieldID + "").removeAttr("tabindex");
        $("#" + fieldID + "").removeAttr("onfocus");
    }
}

function AttachCurrencyAndNumberEvents(jqElementObject) {
    var datatype = $(jqElementObject).data("type");
    if (datatype == "currency") {
        $(jqElementObject).keydown(function (e) { EnterOnlyDecimal(e, jqElementObject); })
        $(jqElementObject).focus(function () { RemoveCurrencyFormat(jqElementObject); })
        $(jqElementObject).focusout(function () { ApplyCurrencyFormat(jqElementObject); })
        $(jqElementObject).trigger('focusout');
    }

    if (datatype == "number") {
        if ($(jqElementObject).data("decimalpt") == 0)
        { $(jqElementObject).keydown(function (e) { EntryOnlyInteger(e); }); }
        else {
            $(jqElementObject).keydown(function (e) { EnterOnlyDecimal(e, jqElementObject); })
        }
        $(jqElementObject).focus(function () { RemoveCurrencyFormat(jqElementObject); })
        $(jqElementObject).focusout(function () { ApplyNumberFormat(jqElementObject); })
        $(jqElementObject).trigger('focusout');
    }

    if (datatype == "year") {
        $(jqElementObject).keydown(function (e) { EntryOnlyInteger(e); });
    }

    if (datatype == "SSN") {
        $(jqElementObject).keyup(function (e) { $(jqElementObject).val(GetFormatedTaxID($(jqElementObject).val())); })
    }

    if (datatype == "percentage") {
        $(jqElementObject).keydown(function (e) { EnterOnlyDecimal(e, jqElementObject); })
        $(jqElementObject).focus(function () { RemovePercentFormat(jqElementObject); })
    }

    if (datatype == "smalldate" || datatype == "date") {
        removeDateMasking(jqElementObject);
        $(jqElementObject).on('blur', function (e) { checkValidation(e, this); });
        $(jqElementObject).keyup(function (e) {
            if (!$(this).is(':disabled, [readonly]')) {
                resetDatepickerPosition(e, $(this));
            }
            dateFormatingOnValidation(e, this);
        });
    }

    $(jqElementObject).focus(function () { scrollBehindfooterControl(this); });
}

/**
 * Gets whether elementId is an empty additional field.
 * @param {} elementId 
 * @param {} actualValue
 * @returns {} 
 */

function IsAdditionalFieldEmpty(elementId, actualValue) {
    if (!elementId || elementId.toUpperCase().substring(0, 4) !== 'ADF_') return false;
    if (!actualValue) return true;
    return actualValue.toString().trim().length === 0;
}

function IsRequired(elementId) {
    var value = $("#" + elementId).data("required");
    return value && value.toString().toUpperCase() === "Y";
}

function ValidateElements(elementID, PostValidation) {
    //data-type : string, number, date, smalldate, SSN
    //data-decimalpt : any integer (Validate Up to Maximum Decimal Places)
    //data-required : "Y", any other value no
    //For number/integer data-min = any number value and data-max=any number value
    var eleID = "#" + elementID;
    if (eleID == "#" || eleID == "#undefined") return true;

    var datatype = $(eleID).data("type");
    var dataVal = $(eleID).data("val");

    var decimalpoint = $(eleID).data("decimalpt");
    var isRequired = IsRequired(elementID);
    var eleCaption = GetFieldCaption(elementID);

    var _valueType = $(eleID).data("value-type");
    var elementvalue = "";
    if (datatype == "multiselect") {
        elementvalue = $(eleID).text();
    }
    else {
        if ($(eleID).val() != null) {
            elementvalue = $(eleID).val();
        }
    }

    // Required validation
    if (isRequired) {
        // VMLIP-238: Must allow whitespace to count as valid input for non-additional fields.
        if (elementvalue.length === 0 || IsAdditionalFieldEmpty(elementID, elementvalue) || $(eleID + " option:selected").text() == "Select") {
            $(eleID).addClass("input-error");
            if (FormFocusField === "") {
                FormFocusField = elementID;
            }
            specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>The " + eleCaption + " field is Required.</li>";
            return false;
        }
        else {
            $(eleID).removeClass("input-error");
        }
    } // kishan 28/sep 2017 for NRVISION2-655 Fields not display highlighted even after save.

    switch (datatype) {
        case "number":
            var minval = $(eleID).data("min");
            minval = parseFloat(minval);
            if (isNaN(minval)) minval = -999999999999; //Default negative value

            var maxval = $(eleID).data("max");
            maxval = parseFloat(maxval);
            if (isNaN(maxval)) maxval = 999999999999; //Default positive value

            //Make number type is valid number
            decimalpoint = parseInt(decimalpoint);
            if (isNaN(decimalpoint)) decimalpoint = 0;

            elementvalue = GetUnFormatedCurrencyOrNumberString(elementvalue);

            //  Below code only works in case of Poliy Backoffice. This change done for NRP-704. 
            var tempUrl = window.location.href;
            if (tempUrl.indexOf("NavRiskPolicy") > -1) {
                if (elementvalue.toString().indexOf(".") !== -1 && decimalpoint == 0) {
                    $(eleID).val(parseInt(elementvalue));
                    return;
                }
                if (isNaN(elementvalue) && dataVal == "empty")
                    return true;
            }
            ////////////////////////////////////////////
            elementvalue = parseFloat(elementvalue);

            if (isNaN(elementvalue)) elementvalue = 0;
            if (_valueType !== undefined && _valueType !== '' && _valueType.toLowerCase() === 'positivenumber') {
                if (elementvalue.toString().indexOf('-') !== -1) {
                    $(eleID).addClass("input-error");
                    toastr.error('The Value entered must be Positive Number.');
                    return false;
                }
            }

            if (elementvalue < minval || elementvalue > maxval) {
                $(eleID).addClass("input-error");
                if (FormFocusField == "") {
                    FormFocusField = elementID;
                }
                specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>" + eleCaption + ERR0014 + minval + " and " + maxval + ".</li>";
                if (!PostValidation) toastr.error(eleCaption + ERR0014 + minval + " and " + maxval + ".", "error");
                return false;
            }

            if (decimalpoint > 0) {
                var elevalue = elementvalue.toString();
                if (elevalue.indexOf('.') != -1) {
                    var decimalScale = elevalue.split(".")[1].length;
                    decimalScale = parseInt(decimalScale);
                    if (decimalScale > decimalpoint) {
                        $(eleID).addClass("input-error");
                        toastr.error("The value specified for " + eleCaption + " is out of range. You may not have more than " + decimalpoint + " digits after the decimal.", "error");
                        return false;
                    }
                }
            }

            if (!PostValidation) {
                if ($(eleID).data("format") == "noFormat") {
                    $(eleID).val($(eleID).val());
                }
                else {
                    if (elementvalue !== 0 || isRequired) {
                        $(eleID).val(GetFormatedNumberString(elementvalue, decimalpoint));
                    }
                }

            }
            else {
                //Set unformated to post correctly
                if ($(eleID).data("format") == "noFormat") {
                    $(eleID).val($(eleID).val());
                }
                else {
                    if (elementvalue !== 0 || isRequired) {
                        $(eleID).val(elementvalue.toFixed(decimalpoint));
                    }
                }
            }

            break;

        case "currency":
            var minval = $(eleID).data("min");
            minval = parseFloat(minval);
            if (isNaN(minval)) minval = -999999999999; //Default negative value

            var maxval = $(eleID).data("max");
            maxval = parseFloat(maxval);
            if (isNaN(maxval)) maxval = 999999999999; //Default positive value

            //Make number type is valid number
            decimalpoint = parseInt(decimalpoint);
            if (isNaN(decimalpoint)) decimalpoint = 2;

            //Remove if any currecny format added
            elementvalue = GetUnFormatedCurrencyOrNumberString(elementvalue);

            elementvalue = parseFloat(elementvalue);

            //  Below code only works in case of Poliy Backoffice. This change done for NRP-855. 
            var tempCurrUrl = window.location.href;
            if (tempCurrUrl.indexOf("NavRiskPolicy") > -1) {
                if (isNaN(elementvalue) && dataVal == "empty")
                    return true;
            }
            ////////////////////////////////////////////

            if (isNaN(elementvalue)) elementvalue = 0;

            if (_valueType !== undefined && _valueType !== '' && _valueType.toLowerCase() === 'positivenumber') {
                if (elementvalue.toString().indexOf('-') !== -1) {
                    $(eleID).addClass("input-error");
                    toastr.error('The Value entered must be Positive Number.');
                    return false;
                }
            }

            if (elementvalue < minval || elementvalue > maxval) {
                $(eleID).addClass("input-error");
                if (FormFocusField == "") {
                    FormFocusField = elementID;
                }
                if (tempCurrUrl.indexOf("NavRiskPolicy") > -1) {
                    if (eleCaption.indexOf("Value") > -1) {
                        specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>" + eleCaption + INFO0079 + minval + " and " + maxval + ".</li>";
                        if (!PostValidation) toastr.error(eleCaption + INFO0079 + minval + " and " + maxval + ".", "error");
                        return false;
                    }
                    else {
                        specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>" + eleCaption + INFO0078 + minval + " and " + maxval + ".</li>";
                        if (!PostValidation) toastr.error(eleCaption + INFO0078 + minval + " and " + maxval + ".", "error");
                        return false;
                    }
                }
                else {
                    specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>" + eleCaption + ERR0015 + minval + " and " + maxval + ".</li>";
                    if (!PostValidation) toastr.error(eleCaption + ERR0015 + minval + " and " + maxval + ".", "error");
                    return false;
                }
            }


            if (!PostValidation) {
                var absvalue = Math.abs(elementvalue);
                ApplyCurrencyFormat($(eleID));


            } else {
                //Set unformated to post correctly
                $(eleID).val(elementvalue);
            }

            break;
        case "date":
            if (elementvalue.length > 0) {
                if (isDate(elementvalue)) {
                    var minDate = new Date(1753, 0, 1, 0, 0, 0, 0); minDate.setFullYear(1753, 0, 1);
                    var maxDate = new Date(9999, 11, 31, 0, 0, 0, 0); maxDate.setFullYear(9999, 11, 31);

                    //Less than 1000 year part set 2000+ so implement this logic.
                    var yearpart = elementvalue.split("/")[2];
                    var iyearpart = parseFloat(yearpart);
                    if (iyearpart < 1753) {
                        elementvalue = "01/01/1752";
                    }

                    var objDate = new Date(elementvalue);

                    //January 1, 1753 to December 31, 9999 to an accuracy of 3.33 milliseconds
                    if (objDate < minDate || objDate > maxDate) {
                        $(eleID).addClass("input-error");
                        if (FormFocusField == "") {
                            FormFocusField = elementID;
                        }
                        specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>" + eleCaption + ERR0016 + "</li>";
                        if (!PostValidation) toastr.error(eleCaption + ERR0016, "error");
                        return false;
                    }
                } else {
                    $(eleID).addClass("input-error");
                    if (FormFocusField == "") {
                        FormFocusField = elementID;
                    }
                    specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>" + eleCaption + ERR0017 + "<br/>";
                    if (!PostValidation) toastr.error(eleCaption + ERR0017, "error");
                    $(eleID).val("");
                    return false;
                }
            }
            break;
        case "smalldate":
            if (elementvalue.length > 0) {
                YearFormatingOnFocusOut(eleID);
                elementvalue = $("" + eleID).val();
                if (isDate(elementvalue)) {
                    var minDate = new Date(1900, 0, 1, 0, 0, 0, 0); minDate.setFullYear(1900, 0, 1);
                    var maxDate = new Date(2079, 5, 6, 0, 0, 0, 0); maxDate.setFullYear(2079, 5, 6);

                    //Less than 1000 year part set 2000+ so implement this logic.
                    var yearpart = elementvalue.split("/")[2];
                    var iyearpart = parseFloat(yearpart);
                    if (iyearpart < 1900) {
                        elementvalue = "01/01/1899";
                    }




                    var objDate = new Date(elementvalue);
                    if (objDate < minDate || objDate > maxDate) {
                        $(eleID).addClass("input-error");
                        if (FormFocusField == "") {
                            FormFocusField = elementID;
                        }
                        specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>" + eleCaption + ERR0018 + "</li>";
                        if (!PostValidation) toastr.error(eleCaption + ERR0018, "error");
                        $(eleID).val("");
                        return false;
                    }
                    //NRVISION2-438 Kamalakanta Patel Date=22/08/2017 o Date Field should no display with red border when date is valid.
                    else {
                        $(eleID).removeClass("input-error");
                    }
                    //NRVISION2-438 Kamalakanta Patel Date=22/08/2017 o Date Field should no display with red border when date is valid.
                } else {
                    $(eleID).addClass("input-error");
                    if (FormFocusField == "") {
                        FormFocusField = elementID;
                    }
                    specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>" + ERR0331.replace("'#'", elementvalue).replace("End Date", eleCaption) + "</li>";
                    if (!PostValidation) toastr.error(ERR0331.replace("'#'", elementvalue).replace("End Date", eleCaption), "error");
                    $(eleID).val("");
                    return false;
                }
            }
            break;
        case "SSN":
            if (!ValidateTaxID(elementvalue)) {
                $(eleID).addClass("input-error");
                if (FormFocusField == "") {
                    FormFocusField = elementID;
                }
                specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>" + eleCaption + ERR0019 + "</li>";
                if (!PostValidation) {

                    $(eleID).val('');
                    toastr.error(eleCaption + ERR0019, "error");
                }
                return false;
            }
            else {
                $(eleID).val(GetFormatedTaxID(elementvalue));
            }
            break;
        case "email":
            var email = trim(elementvalue);
            if (email != "") {

                if (!ValidateEmail(email)) {
                    $(eleID).addClass("input-error");
                    if (FormFocusField == "") {
                        FormFocusField = elementID;
                    }
                    if (eleCaption != '') {
                        specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>" + ERR0020 + "</li>";
                        if (!PostValidation) toastr.error(ERR0020, "error");
                        return false;
                    }
                    else {
                        specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>" + ERR0020 + "</li>";
                        if (!PostValidation) toastr.error(ERR0020, "error");
                        return false;
                    }
                }
            }
            break;
        case "percentage":
            if ($(eleID).val() != "") {
                var strNumber = '';
                var num = parseFloat($(eleID).val(), 10).toFixed(2);
                if (num > 100)
                    strNumber = "100" + " %";
                else
                    strNumber = num + " %";
                $(eleID).val(strNumber.replace('.00', ''));
            }

            break;
        case "time":
            var time = trim(elementvalue);
            if (time != "") {
                if (!validateTime(time)) {
                    $(eleID).addClass("input-error");
                    if (FormFocusField == "") {
                        FormFocusField = elementID;
                    }
                    specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>" + eleCaption + ERR0021 + "</li>";
                    //if (!PostValidation) toastr.error(eleCaption + " : Invalid Time.", "error");
                    if (!PostValidation) {
                        if (eleCaption == "") {
                            toastr.error(eleCaption + ERR0021, "error");
                        }
                        else {
                            toastr.error(eleCaption + ERR0021, "error");
                        }

                    }
                    return false;
                }
            }
            break;
        case "alphanumeric":
            var zip = trim(elementvalue);
            if (zip != "") {
                if (!validateAlphaNumeric(zip)) {
                    $(eleID).addClass("input-error");
                    if (FormFocusField == "") {
                        FormFocusField = elementID;
                    }
                    specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>" + eleCaption + ERR0022 + "</li>";
                    if (!PostValidation) toastr.error(eleCaption + ERR0022, "error");
                    return false;
                }
            }
            break;
        case "zip":
            var zip = trim(elementvalue).replace("-", ""); // NRV620-126: Remove dash before validating because it is legal for zip.
            if (zip != "") {
                if (!validateAlphaNumeric(zip)) {
                    $(eleID).addClass("input-error");
                    if (FormFocusField == "") {
                        FormFocusField = elementID;
                    }
                    specificFieldValidatiommsg = specificFieldValidatiommsg + "<li>" + eleCaption + ERR0022 + "</li>";
                    if (!PostValidation) toastr.error(eleCaption + ERR0022, "error");
                    return false;
                }
            }
            break;
        case "year":
            var minYear = 1600;
            var maxYear = 2099;
            var ratVal = true;
            var currYear = parseInt(elementvalue);
            if (elementvalue != '') {
                if (!isNaN(elementvalue)) {
                    if ((currYear < minYear) || (currYear > maxYear)) {
                        $(eleID).addClass('input-error');
                        toastr.error('The value entered is not a valid 4 digit year, between the ranges of 1600 and 2099.');
                        ratVal = false;
                    }
                    else {
                        $(eleID).removeClass('input-error');
                    }
                }
                else {
                    $(eleID).removeClass('input-error');
                }

            }
            return ratVal;
            break;
        default:
            break;
    }

    return true;
}

function validateAlphaNumeric(value) {
    var regex = new RegExp("^[a-zA-Z0-9\b]+$");
    //  var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
    return regex.test(value);
}

function GetFormatedTaxID(strSSN) {
    var val = strSSN.replace(/\D/g, '');
    var newVal = '';
    if (val.length > 4) {
        this.value = val;
    }
    if ((val.length > 3) && (val.length < 6)) {
        newVal += val.substr(0, 3) + '-';
        val = val.substr(3);
    }
    if (val.length > 5) {
        newVal += val.substr(0, 3) + '-';
        newVal += val.substr(3, 2) + '-';
        val = val.substr(5, 4);
    }
    newVal += val;
    return newVal;
}

function ValidateEmail(email) {
    //NRVISION2-399 Entire application - No validation display while user add email address as email@domain...com. Kamalakanta Patel 29/08/2017 for validation for invalid email address should display.
    //var expr = /^[\w\-\.\+]+\@[a-zA-Z0-9\.\-]+\.[a-zA-z0-9]{2,}$/;
    var expr = /^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i;
    //NRVISION2-399 Entire application - No validation display while user add email address as email@domain...com. Kamalakanta Patel 29/08/2017 for validation for invalid email address should display.
    return expr.test(email);
}

function Validate_Time(time) {
    var dateReg = /^(1[012]|0[1-9]|[1-9]):[0-5][0-9](\\s)? (AM|PM)+$/;
    return dateReg.test(time.trim());
}

function isDate(value) {
    var isDate = false;
    try {
        var dateFormat = "mm/dd/yy";
        //GetDateFormatByCultureName(j_getvalue("g_hd_cultureName"));
        //        if (value.length < 11) {
        //            return isDate;
        //        }
        //        else
        if (value.length > 11 || value == '01/01/0000') {
            return isDate;
        }

        var arrFormat = new Array();

        if (dateFormat.indexOf('/') > -1) {
            arrFormat = dateFormat.split('/');
            for (var i = 0; i < arrFormat.length; i++) {
                if (arrFormat[i] == "yy") {
                    if (value.split('/')[i].length < 4) {
                        return isDate
                    }
                }
            }
        }
        else if (dateFormat.indexOf('.') > -1) {
            arrFormat = dateFormat.split('.');
            for (var i = 0; i < arrFormat.length; i++) {
                if (arrFormat[i] == "yy") {
                    if (value.split('.')[i].length < 4) {
                        return isDate
                    }
                }
            }
        }
        else if (dateFormat.indexOf('-') > -1) {
            arrFormat = dateFormat.split('-');
            for (var i = 0; i < arrFormat.length; i++) {
                if (arrFormat[i] == "yy") {
                    if (value.split('-')[i].length < 4) {
                        return isDate
                    }
                }
            }
        }

        $.datepicker.parseDate(dateFormat, value);
        isDate = true;
    }
    catch (e) {
    }

    return isDate;
}

function GetDayName(tday) {
    var dayname = "";
    switch (tday) {
        case 0:
            dayname = "Sunday";
            break;
        case 1:
            dayname = "Monday";
            break;
        case 2:
            dayname = "Tuesday";
            break;
        case 3:
            dayname = "Wednesday";
            break;
        case 4:
            dayname = "Thursday";
            break;
        case 5:
            dayname = "Friday";
            break;
        case 6:
            dayname = "Saturday";
            break;
        default:
            dayname = "N/A";
    }
    return dayname;
}

function GetDateDiff(d1, d2, interval) {
    var diff = 0;
    switch (interval) {
        case "ms":
            var t2 = d2.getTime();
            var t1 = d1.getTime();
            diff = parseInt(t2 - t1);
            break;
        case "s":
            var t2 = d2.getTime();
            var t1 = d1.getTime();
            diff = parseInt((t2 - t1) / (1000));
            break;
        case "m":
            var t2 = d2.getTime();
            var t1 = d1.getTime();
            diff = parseInt((t2 - t1) / (1000 * 60));
            break;
        case "h":
            var t2 = d2.getTime();
            var t1 = d1.getTime();
            diff = parseInt((t2 - t1) / (1000 * 60 * 60));
            break;
        case "d":
            var t2 = d2.getTime();
            var t1 = d1.getTime();
            diff = parseInt((t2 - t1) / (1000 * 60 * 60 * 24));
            break;
        case "w":
            var t2 = d2.getTime();
            var t1 = d1.getTime();
            diff = parseInt((t2 - t1) / (1000 * 60 * 60 * 24 * 7));
            break;
        case "mm":
            var d1Y = d1.getFullYear();
            var d2Y = d2.getFullYear();
            var d1M = d1.getMonth();
            var d2M = d2.getMonth();
            diff = (d2M + 12 * d2Y) - (d1M + 12 * d1Y);
            break;
        case "y":
            diff = d2.getFullYear() - d1.getFullYear();
            break;

        default:
            diff = null;
    }

    return diff;
}

function isToDateGreaterOrEqual(fromDateString, toDateString) {
    //Asume date format in mm/dd/yyyy
    var fromDatePart = fromDateString.split("/");
    var fromDate = new Date(fromDatePart[2], fromDatePart[0], fromDatePart[1]).getTime();

    var toDatePart = toDateString.split("/");
    var toDate = new Date(toDatePart[2], toDatePart[0], toDatePart[1]).getTime();

    return (fromDate <= toDate) ? true : false;
}

function GetFormatedCurrencyString(curvalue, decimalpoint) {

    var _decimalpoint = 2;

    if (typeof (decimalpoint) != "undefined") {
        _decimalpoint = parseInt(decimalpoint);
    }

    if (isNaN(_decimalpoint)) _decimalpoint = 2;

    curvalue = GetUnFormatedCurrencyOrNumberString(curvalue);

    curvalue = parseFloat(curvalue);
    if (isNaN(curvalue)) curvalue = 0;

    var absvalue = Math.abs(curvalue);
    //var fvalue = ((curvalue < 0) ? "-$" : '$') + absvalue.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
    var fvalue = ((curvalue < 0) ? "-$" : '$') + absvalue.toFixed(_decimalpoint).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();

    return fvalue;
}

function GetUnFormatedCurrencyOrNumberString(curvalue) {
    var newval = "," + curvalue;
    newval = newval.replace('$', '').replace(/,/g, '');

    //To match with other validation in the form// this will create problem value like 1.0003
    //newval = newval.replace('.00', '');


    return newval;
}

function GetFormatedNumberString(numberval, decimalpoint) {
    var number = GetUnFormatedCurrencyOrNumberString(numberval);
    number = parseFloat(number);
    if (isNaN(number)) number = 0;

    var decimalpoint = parseInt(decimalpoint);
    if (isNaN(decimalpoint)) decimalpoint = 0;

    var NoDecimal = (decimalpoint == 0);
    if ((decimalpoint == 0)) decimalpoint = 2;

    number = number.toFixed(decimalpoint) + '';
    var x = number.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }

    if (NoDecimal)
        return x1;
    else
        return x1 + x2;
}

function EntryOnlyInteger(e) {
    if (e.shiftKey || e.ctrlKey || e.altKey) { // if shift, ctrl or alt keys held down
        e.preventDefault();         // Prevent character input
    }
    else {
        var n = e.keyCode;
        if (!((n == 8)              // backspace
            || (n == 9)
            || (n == 46)                // delete
            || (n >= 35 && n <= 40)     // arrow keys/home/end
            || (n >= 48 && n <= 57)     // numbers on keyboard
            || (n >= 96 && n <= 105))   // number on keypad
        ) {
            e.preventDefault();
            // alert("in if");
            // Prevent character input
        };
    };
}

function NotEntrySpecialCharacter(e) {

    var keyCode = e.which;
    var keychar = e.key;

    ////To prevent any key with shift key for special char
    if (e.shiftKey && (keyCode >= 48 && keyCode <= 57)) {
        e.preventDefault(); return;
    }


}

function EnterOnlyDecimal(e, obj) {
    var keyCode = e.which;
    var keychar = e.key;
    var eval = $(obj).val();
    var minval = $(obj).data("min");
    if (isNaN(minval)) minval = 0;

    //To prevent any key with shift key for special char
    if (e.shiftKey && (keyCode >= 48 && keyCode <= 57)) {
        e.preventDefault(); return;
    }

    if (keyCode != 8 && keyCode != 9 && keyCode != 13 && keyCode != 37 && keyCode != 38 && keyCode != 39 && keyCode != 40 && keyCode != 46 && keyCode != 109 && keyCode != 110 && keyCode != 173 && keyCode != 189 && keyCode != 190) {
        if (keyCode < 48) {
            e.preventDefault();
        }
        else if (keyCode > 57 && keyCode < 96) {
            e.preventDefault();
        }
        else if (keyCode > 105) {
            e.preventDefault();
        }
    }
    else {
        if (keyCode == 190) {
            //Decimal point
            if (eval.indexOf('.') >= 0) {
                e.preventDefault();
            }
        }

        if (minval.toString().indexOf("-") == -1) {
            if (keyCode == 189 || (keychar == "-" && keyCode == 173)) {
                //Minus sign 189 for Chrome and IE, 173 for Firefox
                if (eval.indexOf('-') >= 0 || eval.length > 0) {
                    e.preventDefault();
                }
            }
        }

    }
}

function RemoveCurrencyFormat(obj) {
    var value = $(obj).val();

    $(obj).val(GetUnFormatedCurrencyOrNumberString(value));

    document.getElementById($(obj).attr("id")).select();
}

function ApplyCurrencyFormat(obj) {

    var neg = false;
    var value = $(obj).val();

    if ($(obj).data("val") == "empty" && value == "") {
        $(obj).val("");
    }
    else {

        var tempCurrUrl = window.location.href;
        if (tempCurrUrl.indexOf("NavRiskPolicy") > -1) {
            var dynamicDecimalpoint = $(obj).data("decimalpt");
            $(obj).val(GetFormatedCurrencyString(value, dynamicDecimalpoint));
        }
        else {
            $(obj).val(GetFormatedCurrencyString(value, 2)); // 2 fixed for claims.
        }
        //$(obj).val(GetFormatedCurrencyString(value));
        //SET NEGATIVE VALUE IN RED
        var numValue = parseFloat(GetUnFormatedCurrencyOrNumberString(value));
        if (isNaN(numValue)) numValue = 0;
        if (numValue < 0) $(obj).css("color", "#ff0000"); else $(obj).css("color", "#333");
    }

}

function ApplyCurrencyFormatforLbl(objid) {
    var amtpaid = $("#" + objid).text();
    $("#" + objid).text(GetFormatedCurrencyString(amtpaid, 2));
    //SET NEGATIVE VALUE IN RED
    var numValue = parseFloat(GetUnFormatedCurrencyOrNumberString(amtpaid));
    if (isNaN(numValue)) numValue = 0;
    if (numValue < 0) $("#" + objid).css("color", "#ff0000"); else $("#" + objid).css("color", "#333");
}

function ApplyNumberFormat(obj) {
    var neg = false;
    var value = $(obj).val();
    var decimalpoint = $(obj).data("decimalpt");

    var decimalpoint = parseInt(decimalpoint);
    if (isNaN(decimalpoint)) decimalpoint = 0;
    if ($(obj).data("val") == "empty" && value == "") {
        $(obj).val("");
    }
    else {
        if ($(obj).data("format") == "noFormat")
            $(obj).val(value);
        else
            $(obj).val(GetFormatedNumberString(value, decimalpoint));
    }

}

function ApplyFormCurrecnyOrNumberFromat(formID) {
    $("#" + formID + " *").filter(':input').not("input[type=hidden]").each(function () {
        var datatype = $(this).data("type");
        if (datatype == "currency") {
            ApplyCurrencyFormat(this);
        }

        if (datatype == "number") {
            ApplyNumberFormat(this);
        }
    });
}

function IsNumeric(evt) {
    var key = (evt.keyCode) ? evt.keyCode : evt.which;

    // Verify if the key entered was a numeric character (0-9) or a decimal (.)
    if ((key > 47 && key < 58) || (key >= 96 && key <= 105))   // number on keypad)
        // If it was, then allow the entry to continue

        return true;
    else
        return false;
}

//************** STRING UTILITY FUNCTION *****************
function escapeRegExp(string) {
    return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}

function replaceAll(str, find, replaceStr) {
    if (str != null && str != "")
        return str.replace(new RegExp(escapeRegExp(find), 'g'), replaceStr);
    else
        return "";
}

function trim(stringToTrim) {
    return (stringToTrim) ? stringToTrim.replace(/^\s+|\s+$/g, "") : '';
}

function ltrim(stringToTrim) {
    return (stringToTrim) ? stringToTrim.replace(/^\s+/, "") : '';
}

function rtrim(stringToTrim) {
    return (stringToTrim) ? stringToTrim.replace(/\s+$/, "") : '';
}

//************** Check Max Control Length *****************
function CheckControlLength(ctl, min, max) {
    var strText = ctl.value;
    if (strText.length > max || strText.length < min) {
        alert("Maximum capacity of " + max + " characters has been reached.");
        ctl.focus();
        return false;
    }
    return true;
}

//************** Get Current Date & Time *****************
function getCurrentDateTime() {
    var currentTime = new Date()
    var month = currentTime.getMonth() + 1
    var day = currentTime.getDate()
    var year = currentTime.getFullYear()
    var hours = currentTime.getHours()
    var minutes = currentTime.getMinutes()
    var seconds = currentTime.getSeconds()
    var val = "";
    if (month < 10) {
        month = "0" + month
    }
    if (day < 10) {
        day = "0" + day
    }
    if (minutes < 10) {
        minutes = "0" + minutes
    }
    if (seconds < 10) {
        seconds = "0" + seconds
    }
    if (hours > 11) {
        val = " PM";
    } else {
        val = " AM";
    }
    return (month + "/" + day + "/" + year + " " + hours + ":" + minutes + ":" + seconds + val);
}

function getCurrentDate() {
    var currentTime = new Date()
    var month = currentTime.getMonth() + 1
    var day = currentTime.getDate()
    var year = currentTime.getFullYear()

    var val = "";
    if (month < 10) {
        month = "0" + month
    }
    if (day < 10) {
        day = "0" + day
    }

    return (month + "/" + day + "/" + year);
}

/* Validate TaxID Number*/
function ValidateTaxID(taxid) {
    var ssn = taxid.replace(/\D/g, '');
    if (ssn != "") {
        if (ssn.length < 9 || ssn.length > 9) {
            return false;
        }
    }
    return true;
}

/* Validate Time HH:MM AM/PM */
function validateTime(strTime) {
    var time = strTime.split(':');
    if (time.length > 1) {
        var hour = time[0].replace(/\D/g, '');
        var minut = time[1].replace(/\D/g, '');
        if (hour.trim() > 24)
            return false;
        else if (minut.trim() > 59)
            return false;

    }
    return true;
}

function parseJsonData(data) {
    var retStr = "";

    try {
        var obj = JSON.parse(data);
        if (obj.Message != null)
            retStr = replaceAll(obj.Message, "\r\n", "<br /><br />")
        else
            retStr = replaceAll(obj, "\r\n", "<br /><br />")
    } catch (e) {
        retStr = data;
    }
    return retStr;
}

function CompareDates(fromDateString, toDateString) {
    if (fromDateString != "" && toDateString != "") {
        var fromDatePart = fromDateString.split("/");
        var fromDate = new Date(fromDatePart[2], fromDatePart[0], fromDatePart[1]).getTime();
        var toDatePart = toDateString.split("/");
        var toDate = new Date(toDatePart[2], toDatePart[0], toDatePart[1]).getTime();
        return (fromDate <= toDate) ? true : false;
    }
    else {

        return true;
    }
}

function YearFormatingOnFocusOut(eleID) {
    var obj = $(eleID);
    var dateFormat = $(obj).data('dateformat');
    var match = new RegExp(dateFormat
        .replace(/(\w+)\W(\w+)\W(\w+)/, "^\\s*($1)\\W*($2)?\\W*($3)?([0-9]*).*")
        .replace(/mm|dd/g, "\\d{2}")
        .replace(/yy/g, "\\d{4}"));
    var replace = "$1/$2/$3$4"
        .replace(/\//g, dateFormat.match(/\W/));


    var vFVal = $("" + eleID).val()
        .replace(/(^|\W)(?=\d\W)/g, "$10")   // padding
        .replace(match, replace)             // fields
        .replace(/(\W)+/g, "$1");            // remove repeats
    $("" + eleID).val(vFVal);
    var dtDate = $("" + eleID).val();
    var delimiter = /[^mdy]/.exec(dateFormat)[0], theFormat = dateFormat.split(delimiter), theDate = $("" + eleID).val().split(delimiter);

    var m, d, y;
    m = theDate[0];
    d = theDate[1];
    y = theDate[2];
    if (y != undefined && y.length == 2) {
        y = getCorrectYear(y);
        d = getCorrectDate(d, m, y);
        var dtDate = (
          m > 0 && m < 13 &&
          y && y.length === 4 &&
          d > 0 && d <= (new Date(y, m, 0)).getDate()
        )
        if (dtDate) {
            var Fdate = m + "/" + d + "/" + y;
            if (isDate(Fdate)) {
                $("" + eleID).val(Fdate);
                $("" + eleID).datepicker('setDate', Fdate);
                return;
            }
        }
    }
}



////ALL ERRROR MESSAGE
var ERR0001 = "There aren't any pending changes to save.";
var ERR0002 = "Category Name : Required.";
var ERR0003 = "Template Name : Required.";
var ERR0004 = "Subject : Required.";
var ERR0005 = "Received Date must be greater than or equal to Knowledge Date";
var ERR0006 = "Hire Date should be less than or equal to Retirement Date";
var ERR0007 = "Retirement Date should be greater than or equal to Hire Date";
var ERR0008 = "Start Date should be less than or equal to Termination Date";
var ERR0009 = "Termination Date should be greater than or equal to Start Date";
var ERR0010 = "You do not have rights to add ";
var ERR0011 = "You do not have rights to update ";
var ERR0012 = "You do not have rights to delete ";
var ERR0013 = "You do not have rights to access forms";
var ERR0014 = " Number value must be between ";
var ERR0015 = " Currency value must be between ";
var ERR0016 = " : Enter date between January 1, 1753 through December 31, 9999.";
var ERR0017 = " : Invalid date.";
var ERR0018 = " : Enter date between January 1, 1900 through June 6, 2079.";
var ERR0019 = " : Please provide complete Tax ID.";
var ERR0020 = "   Invalid Email.";
var ERR0021 = " : Invalid Time.";
var ERR0022 = " : Invalid Zip.";
var ERR0023 = " must be greater or equal to ";
var ERR0024 = " must be less or equal to ";
var ERR0025 = "Diary Search work in progress. Not completed.";
var ERR0026 = "There was an error in the attempt to open the program.";
var ERR0027 = "You do not have rights to access confidential.";
var ERR0028 = "You do not have right to acces Deposits/Withdrawal data.";
var ERR0029 = "Please enter a valid search criteria.";
var ERR0030 = " must be a number.";
var ERR0031 = "Payments through date must be greater than or equal to payments from date";
var ERR0032 = "Payments from date must be less than or equal to payments through date";
var ERR0033 = "Please enter a valid clear date.";
var ERR0034 = "You haven't selected a transaction.";
var ERR0035 = "Bank Account Organization access rights were not updated.";
var ERR0036 = "You do not have right to Add BankTransactions data.";
var ERR0037 = "This account can not be deleted because it is in use.";
var ERR0038 = "Could not get organization list. Please try again.";
var ERR0039 = "Bank Account Organization access rights were not updated.";
var ERR0040 = "Bank Account Organization access rights were updated.";
var ERR0041 = "You do not have right to Update BankTransactions data.";
var ERR0042 = "You do not have right to Delete BankTransactions data.";
var ERR0043 = "Please select a file to upload.";
var ERR0044 = "From Date : Required. <br/> Through Date : Required.";
var ERR0045 = "There is no matching data to display.";
var ERR0046 = "Through Date must be greater than or equal to From Date.";
var ERR0047 = "From Date must be less than or equal to Through Date.";
var ERR0048 = "No records were found to create a batch with these parameters.";
var ERR0049 = "To Date must be greater than or equal to From Date";
var ERR0050 = "From Date must be less than or equal to To Date";
var ERR0051 = "Please select EDI Batch";
var ERR0052 = "Inception Date should be less than Termination date.";
var ERR0053 = "Termination date should be greater than Inception Date.";
var ERR0054 = "There was a problem to update record.";
var ERR0055 = "This Access Group can be deleted only when user revoked all the permissions";
var ERR0056 = "Organization access permissions were updated.";
var ERR0057 = "You must select a node in the tree";
var ERR0058 = "Organization access permissions can not be deleted. Fail";
var ERR0059 = "Only one Claim Administrator is allowed on the system";
var ERR0060 = "You have to select First an batch"
var ERR0061 = "There was an error in the attempt to reset the batch.";
var ERR0062 = "Could not get location list. Please try again.";
var ERR0063 = "Examiner should be different to transfer the claim and diaries.";
var ERR0064 = "Claim not found for selected fields to transfer the claim and diaries.";
var ERR0065 = "Claim not found or something went wrong to transfer the claim and diaries."
var ERR0066 = "Claim not found for selected batch to transfer back the claim and diaries.";
var ERR0067 = "Transfer For All Locations should be checked or select any location from location list.";
var ERR0068 = "To copy a questionnaire, a unique program, form type and member type must be entered. Member Type is not required.";
var ERR0069 = "Cannot set the Reported Date prior to the Occurrence Date.";
var ERR0070 = "Selected record not deleted.";
var ERR0071 = "Some error during Prerequisite check";
var ERR0072 = "Some error in Menu generation";
var ERR0073 = "Some error in Policy Menu generation";
var ERR0074 = "Please select a record first";
var ERR0075 = "Please select an organization or organization unit (not the grouping letter: ";
var ERR0076 = "You can not delete this dictionary. This Dictionary has some system entry.";
var ERR0077 = "You must select an organization.";
var ERR0078 = " must be greater than or equal to Initial review ";
var ERR0079 = " must be greater than or equal to Subsequent Review ";
var ERR0080 = "The selected record has been deleted. Would you like to create a new record?";
var ERR0081 = "You Should have Assign MMR RRE field";
var ERR0082 = "Date Not found";
var ERR0083 = "You must select an organization type.";
var ERR0084 = "This Organization has been Insured from organization Tree. \n You can not delete from Here!";
var ERR0085 = "You cannot delete this record. As it is used as Insurer";
var ERR0086 = "Please Select Valid Insured Type.";
var ERR0087 = "You can not change code set value for this organization";
var ERR0088 = "There are still Active claims attached to this organization, or one of its children. So you can not change in active";
var ERR0089 = "An error occured in save record.";
var ERR0090 = "You can not insert duplicate record.";
var ERR0091 = "Exposure From must be less or equal to Exposure Through.";
var ERR0092 = "Exposure Through Date can not be Greater than today";
var ERR0093 = "There are additional levels and claims associated with the selected Organization. The requested operation cannot be performed";
var ERR0094 = "Loss Through Date must be greater than or equal to From Date";
var ERR0095 = "Loss From Date must be less than or equal to Through Date";
var ERR0096 = "Please change value Field entry.";
var ERR0097 = "Selected record can not be deleted.";
var ERR0098 = "The location tree is currently locked because it is associated with an existing occurrence.";
var ERR0099 = "An insured must be selected before a location can be searched.";
var ERR0100 = "Occurrence lookup in progress....";
var ERR0101 = "A partial occurrence number must be entered before an occurrence lookup can be performed.";
var ERR0102 = "A lookup can not be performed because this claim is already attached to an existing occurrence.";
var ERR0103 = "This Record can not be edited as status is approved.";
var ERR0104 = "There was an error in the attempt to load Loss Cause.";
var ERR0105 = "Could not update details. please try again.";
var ERR0106 = "You do not have rights to View %1%.";
var ERR0107 = "Selected %1% Claim transfered back successfully.";
var ERR0108 = "You do not have rights to access Events Detail data.";
var ERR0109 = "There was an error in the attempt to delete the claim event.";
var ERR0110 = "The recovery reversal attempt failed.";
var ERR0111 = "Could not delete Compensation Rate details. please try again.";
var ERR0112 = "You do not have rights to delete Claim Data.";
var ERR0113 = "Claim Type : Required.";
var ERR0114 = "You do not have rights to close a claim.";
var ERR0115 = "Loss Date cannot be after the current date.";
var ERR0116 = "Knowledge Date and received date cannot be after the current date.";
var ERR0117 = "Knowledge Date cannot be after the current date.";
var ERR0118 = "Received Date cannot be later than today's date.Please check the received date on this claim and ensure that it is correct.";
var ERR0119 = "Knowledge Date and Received Date cannot be before the Loss Date.";
var ERR0120 = "Received Date cannot be before the Knowledge Date.";
var ERR0121 = "Knowledge Date cannot be before Loss Date.";
var ERR0122 = "Received Date cannot be before the Loss Date.";
var ERR0123 = "Birth Date and Death Date cannot be greater than Current Date.";
var ERR0124 = "Birth Date cannot be greater than Current Date.";
var ERR0125 = "Death Date cannot be before Birth Date.";
var ERR0126 = "Death Date cannot be after the Current Date.";
var ERR0127 = "Birth Date should be less than or equal to Loss Date.";
var ERR0128 = "Death Date must be greater than or equal to  Loss Date.";
var ERR0129 = "Received Date cannot be after the current date.";
var ERR0130 = "Could not delete Lien Event Detail. please try again.";
var ERR0131 = "Could not deleteSettlement Negotiation History detail. please try again.";
var ERR0132 = "Additional Body Part Details deleted successfully.";
var ERR0133 = "Could not delete Additional Body Part Details. please try again.";
var ERR0134 = "You do not have right to update Case Management data.";
var ERR0135 = "Notepad Link Details saved successfully.";
var ERR0136 = "Please update the Correspondence indicator on an Active address.";
var ERR0137 = "Please mark this address as correspondence";
var ERR0138 = "End Date must be greater than or equal to Start Date";
var ERR0139 = "End Date/Time must be greater than start Date/Time!";
var ERR0140 = "Loss Date cannot be greater than current date.";
var ERR0141 = "Received date must be greater than Loss Date.";
var ERR0142 = "Please select one or more coverage for this claim.";
var ERR0143 = "The update attempt for Recoveries was not success full for the following reason: No payments found for this claim.";
var ERR0144 = "Payer Name : Required.";
var ERR0145 = "Loss Date : To date should be greater than from date.";
var ERR0146 = "Knowledge Date : To date should be greater than from date.";
var ERR0147 = "Received Date : To date should be greater than from date.";
var ERR0148 = "Policy From : To date should be greater than from date."
var ERR0149 = "Policy Through : To date should be greater than from date.";
var ERR0150 = "This is medicare recipient and may need to be reported to CMS.";
var ERR0151 = "There was an error in the attempt to transfer the claim : <br>";
var ERR0152 = "The Through Date should be greater than or equal to the From Date";
var ERR0153 = "There was was an error in the duplicate occurrence check. Please try again.";
var ERR0154 = "Occurrence Date cannot be after the current date.";
var ERR0155 = "Reported Date cannot be after the current date.";
var ERR0156 = "Reported Date cannot be before Occurrence Date.";
var ERR0157 = "Reported Date must be greater or equal to Occurrence Date.";
var ERR0158 = "Reported Date (Police) must be greater or equal to Occurrence Date.";
var ERR0159 = "Could not add Alternate Claimant. please try again.";
var ERR0160 = "Could not add organization. please try again.";
var ERR0161 = "Could not add person. please try again.";
var ERR0162 = "No Claim selected.";
var ERR0163 = "Please upload a document.";
var ERR0164 = "Notepads must be created or edited at the claim level.";
var ERR0165 = "New notepads must be entered at the claim level.";
var ERR0166 = "Add Date : To date should be greater than from date.";
var ERR0167 = "Please Select a Claim before uploading the Document.";
var ERR0168 = "The update attempt for Payment was not successful for the following reasons:-This Payment Type is not allowed on closed claims.";
var ERR0169 = "Claim Status is Pending! You will not be able to enter and save a payment.";
var ERR0170 = "Insufficient Reserves. This payment cannot be saved.";
var ERR0171 = "A Payee must be selected before saving a Payment";
var ERR0172 = "Please enter valid amount not more than 999.";
var ERR0173 = "Please enter valid data not more than 999999.";
var ERR0174 = "Please enter valid data not more than 9999999999.";
var ERR0175 = "You need to supply at least one character to perform a search!";
var ERR0176 = "You cannot save the reserve changes without comments.";
var ERR0177 = "You haven't selected a payment.";
var ERR0178 = "All the payments were removed from the batch,\ntherefore the batch has been deleted as well.";
var ERR0179 = "Not processed";
var ERR0180 = "There are no payments selected to print.";
var ERR0181 = "Print Date can not be less than today";
var ERR0182 = "There was an error in the attempt to open Print check";
var ERR0183 = "There was an error processing the batch: ";
var ERR0184 = "There are no payments selected to update.";
var ERR0185 = "A Diagnostic Code has not been selected.";
var ERR0186 = "You do not have right to update payment adjustment data.";
var ERR0187 = "You do not have right to update data.";
var ERR0188 = "You do not have right to Add Paymemt Adjustment data.";
var ERR0189 = "You do not have right to Add Paymemt data.";
var ERR0190 = "You do not have right to delete Paymemt Adjustment data.";
var ERR0191 = "You do not have right to delete Paymemt data.";
var ERR0192 = "Payment Adjustment Row deleted successfully.";
var ERR0193 = "You do not have rights to add refund data.";
var ERR0194 = "You do not have rights to update refund data.";
var ERR0195 = "You do not have rights to delete refund data.";
var ERR0196 = "Payment Refund Row deleted successfully.";
var ERR0197 = "You must enter a claim number.";
var ERR0198 = "No Matching PayType for this claim";
var ERR0199 = "No matching Coverages for this insured";
var ERR0200 = "No matching Bank Accounts for this claim";
var ERR0201 = "No Claims exist for this Insured/Claim Number.";
var ERR0202 = "Verify Coverage and Bank Account then click Continue to finish";
var ERR0203 = "There was an error transferring the payment: ";
var ERR0204 = "No printers are installed";
var ERR0205 = "A payee name must be selected.";
var ERR0206 = "Select a Coverage Code.";
var ERR0207 = "Only a committed, printed or posted payment can be refunded.";
var ERR0208 = "A voided payment cannot be refunded.";
var ERR0209 = "A stopped payment cannot be refunded.";
var ERR0210 = "A backed-out payment cannot be refunded.";
var ERR0211 = "There was an error in the attempt to open Bank Account list";
var ERR0212 = "You do not have rights to change bank accounts.";
var ERR0213 = "There was an error in the attempt to open Alternate Claimant Payee Addressess";
var ERR0214 = "The claimant must have a valid address to allow claimant payments!  \n  Please add a claimant address before adding a transaction.";
var ERR0215 = "The beneficiary must have a valid address to allow beneficiary payments!  \n  Please add a beneficiary address before adding a transaction.";
var ERR0216 = "There are no Beneficiaries for this claim or \n a beneficiary exists but has no address.";
var ERR0217 = "You do not have rights to update.";
var ERR0218 = "You do not have rights to add the record.";
var ERR0219 = "Please Enter Check Number.";
var ERR0220 = "You cannot make changes to a committed, printed or posted payment.";
var ERR0221 = "There was an error in the attempt to open duplicate Payment(s)";
var ERR0222 = "The vendor must have a valid address to allow vendor payments!.  Please add a vendor address before adding a transaction.";
var ERR0223 = "There was an error in the attempt to open Payment Transfer";
var ERR0224 = "This payment cannot be deleted because it has been Printed";
var ERR0225 = "This payment cannot be deleted because it has been Posted";
var ERR0226 = "Select a Payee Type.";
var ERR0227 = "A search is not necessary for the Claimant payee type.";
var ERR0228 = "This payment cannot be reversed. It has been refunded";
var ERR0229 = "You cannot reverse a non-posted payment.";
var ERR0230 = "Reserves cannot be entered for a claim in 'Pending' status \n" + "Change claim status on the Claim Detail page to 'Open'.";
var ERR0231 = "Financial transactions are not allowed on Information Only claims.";
var ERR0232 = "Claim Status is Pending! You will not be able to create and save a new reserve worksheet.";
var ERR0233 = "Loss Type is Info Only! You will not be able to create and save a new reserve worksheet.";
var ERR0234 = "There can only be one active worksheet at a time.";
var ERR0235 = "Claim Status is Pending! You will not be able to enter and save a scheduled payment.";
var ERR0236 = "Loss Type is Info Only! You will not be able to create and save a scheduled payment.";
var ERR0237 = "There are no Beneficiaries for this claim or a beneficiary exists but has no address";
var ERR0238 = "There are no Beneficiaries for this claim or a beneficiary exists but has no address.";
var ERR0239 = "The Release Date cannot be before the current date. Please re-enter a valid Release Date.";
var ERR0240 = "First Release Date should be Greater Than Todays Date";
var ERR0241 = "From Date must be less than or equal to the Through Date";
var ERR0242 = "The Final Thru Date should be greater than or equal to the Final From Date";
var ERR0243 = "Please select a batch before you proceed!";
var ERR0244 = "This batch has no committed payments, no further processing possible";
var ERR0245 = "A Payee must be selected before saving";
var ERR0246 = "Bank Account : Required.";
var ERR0247 = "No claim selected for this payment.";
var ERR0248 = "There was an error in the attempt to open Claim search.";
var ERR0249 = "You must select an Extract interface!";
var ERR0250 = "Both the from and through dates are required!";
var ERR0251 = "Service Date : To date should be greater than from date.";
var ERR0252 = "There was an error in the attempt to load RRE List.";
var ERR0253 = "You must pick an RRE before you can create an export.";
var ERR0254 = "You must pick an Insured and a RRE before proceeding.";
var ERR0255 = "You must create a batch before you can run an extract.";
var ERR0256 = "Through Date must be greater than or equal to From Date";
var ERR0257 = "From Date must be less than or equal to Through Date";
var ERR0258 = "Please select atleast one node from Location Tree";
var ERR0259 = "Please select an Error file to upload.";
var ERR0260 = "You need to supply the date that the file was submitted to ISO to match up with the error file.";
var ERR0261 = "Active letter can not be deleted.";
var ERR0262 = "Approved date should be less than or equal to the completed date. ";
var ERR0263 = "Loss Throuhgh date can not be lower than Loss From date.";
var ERR0264 = "Loss Through Date can not be Greater than today";
var ERR0265 = "Loss From Date can not be Greater than today. ";
var ERR0266 = "Date of Termination must be greater or equal to Date of Hire.";
var ERR0267 = "Claim Status is Pending!\n You will not be able to enter and save a salvage.";
var ERR0268 = "Please select file to create batch.";
var ERR0269 = "Please select file and source to create batch";
var ERR0270 = "There was an error in the attempt to open New Med Bill Import Batch";
var ERR0271 = "Please select a batch";
var ERR0272 = "You must select a batch for this operation.";
var ERR0273 = "No additional coverages exist for this policy.";
var ERR0274 = "Through Date must be greater or equal to From Date.";
var ERR0275 = "From Date must be less or equal to Through Date.";
var ERR0276 = "Could not delete Coverage. please try again.";
var ERR0277 = "Please select a Coverage.";
var ERR0278 = "No additional Program exist for this policy.";
var ERR0279 = "There was an error in the attempt to load program period pop-up.";
var ERR0280 = "The changes were unsuccessful.";
var ERR0281 = "And Error occured. The changes were unsuccessful.";
var ERR0282 = "Computing Vendor payments failure.";
var ERR0283 = "Marking corrected Vendors Failure.";
var ERR0284 = "You can not insert duplicate Diagnostic Code.";
var ERR0285 = "You do not have rights to add Personal Diary data.";
var ERR0286 = "From Date should be less than or equal to the Through Date.";
var ERR0287 = "Through Date should be greater than or equal to the From Date.";
var ERR0289 = "There are no errors for this EDI Transaction.";
var ERR0290 = "MTC Code : Required.";
var ERR0291 = "MTC Codes should not change";
var ERR0292 = "From Date must be greater than or equal to Injury Date";
var ERR0293 = "The update attempt for Lost Time was not successful for the following reasons:-Only one Lost Time allowed with blank thru date";
var ERR0294 = "Start Date should be less than or equal to End Date";
var ERR0295 = "End Date should be greater than or equal to Start Date";
var ERR0296 = "No ICD9/ICD10 codes found in medbills for this claim.";
var ERR0297 = "You do not have right to Access Payment Approvals";
var ERR0298 = "You do not have rights for Reserve Approvals";
var ERR0299 = "Could not approve all payments. please try again.";
var ERR0300 = "Your authorization level is not high enough to approve this payment.";
var ERR0301 = "Could not approve payment. please try again.";
var ERR0302 = "Could not reviewe payment. please try again.";
var ERR0303 = "You can not save Reduce Earning records until EDI Event record is saved";
var ERR0304 = "Could not approve Reserve Worksheet. please try again.";
var ERR0305 = "Could not reject Reserves Worksheet. please try again.";
var ERR0306 = "At least one contact must be selected to continue.";
var ERR0307 = "Could not delete attorney firm membership details. please try again.";
var ERR0308 = "Please select contact information from claim rolodex search screen.";
var ERR0309 = "This record is used at claim level, you cannot delete this record";
var ERR0310 = "Death date should be greater than Birth date.";
var ERR0311 = "Birth Date cannot be greater than Death Date.";
var ERR0312 = "To change the master vendor, please click on Search below.";
var ERR0313 = "Parameter value '100.00' is out of range.";
var ERR0314 = "Billing Contact Info should not be blank.";
var ERR0315 = "Please Insert Corret Data";
var ERR0316 = "You do not have rights to Calendar.";
var ERR0317 = "You must pick an Insurer and a RRE before you can create an export.";
var ERR0318 = "You must pick an Insurer and a RRE before proceeding.";
var ERR0319 = "Could not delete legal cause of action details. please try again.";
var ERR0320 = "Could not delete legal litigation and event details. please try again.";
var ERR0321 = "Could not delete legal attorny and party details. please try again.";
var ERR0322 = "Select party : Required.";
var ERR0323 = "Approved amount cannot exceed than Requested amount.";
var ERR0324 = "Please select party to create new subrogation record.";
var ERR0325 = "Provider : Required.";
var ERR0326 = "Full Prior Acts cannot be checked if Retro Date is also filled in.";
var ERR0327 = "Individual IRMP values cannot exceed the range specified.";
var ERR0328 = "Closed claims are locked and cannot be modified.";
var ERR0329 = "Please select file to be import";
var ERR0330 = "Cannot set the Reported Date prior to the Incident Date.";
var ERR0331 = "The value '#' is not valid for End Date.";

//ALL WARNING MESSAGE
var WAR0001 = "Save claim to update the state detail panel.";
var WAR0002 = "Claims are assigned to this user. If you proceed with this change the Examiner field on Claim Detail will be blank for each claim assigned to this user.";
var WAR0003 = "Do you want to delete selected record ? You will be redirected to the Home Page.";
var WAR0004 = "Does the MMR Page need to be updated?";
var WAR0005 = "Transferring a claim from one Insured to another Insured is not allowed. ";
var WAR0006 = "This action will check your reserve authority. If you have sufficient authority, the reserves will be created from this worksheet! Save as Incomplete if this worksheet is not ready. Are you sure you wish to save?";
var WAR0007 = "There are no payments to approve at the users authorization level.";
//ALL INFO MESSAGE
var INFO0001 = "Record updated successfully.";
var INFO0002 = "Selected record deleted successfully.";
var INFO0003 = "Record saved successfully.";
var INFO0004 = "Upload Succeeded";
var INFO0005 = "Data was successfully imported!";
var INFO0006 = "There were no errors - ready for export."
var INFO0007 = "There were errors - some records can be exported.";
var INFO0008 = "There were no Valid items - no records can be exported.";
var INFO0009 = "There were errors - no records can be exported.";
var INFO0010 = "The batch was successfully reset to Valid.";
var INFO0011 = "Copying questionnaire successfully.";
var INFO0012 = "Member Type updated successfully.";
var INFO0013 = "Copying program Saved successfully.";
var INFO0014 = "Logged In Successfully";
var INFO0015 = "Location Note record deleted successfully.";
var INFO0016 = "Location Note details updated successfully.";
var INFO0017 = "Organization Exposure details saved successfully.";
var INFO0018 = "Compensation Rate details deleted successfully.";
var INFO0019 = "Initiatives updated successfully.";
var INFO0020 = "Confidential information updated successfully.";
var INFO0021 = "Address details deleted successfully.";
var INFO0022 = "The claim has been saved successfully!";
var INFO0023 = "Claim Detail Updated successfully.";
var INFO0024 = "LossType details updated successfully.";
var INFO0025 = "Status details updated successfully.";
var INFO0026 = "Phone details deleted successfully.";
var INFO0027 = "The claim has been Transferred successfully!";
var INFO0028 = "Compensation details updated successfully.";
var INFO0029 = "Lien Event updated successfully.";
var INFO0030 = "Lien Settlement Negotiation History updated successfully.";
var INFO0031 = "Occurrence Detail Updated successfully.";
var INFO0032 = "Alternate Claimant added successfully.";
var INFO0033 = "Organization added successfully.";
var INFO0034 = "Person added successfully.";
var INFO0035 = "Attachment deleted successfully.";
var INFO0036 = "Email Sent Successfully.";
var INFO0037 = "The email was successfully saved to the claim.";
var INFO0038 = "Schedule Payment Adjustment added successfully.";
var INFO0039 = "The selected line item was deleted.";
var INFO0040 = "Paid Amount update successfully.";
var INFO0041 = "Scheduled Payment Adjustment deleted successfully.";
var INFO0042 = "Only $ %1% remains available for this bank account. Please continue with the available amount, and then enter a new payment for the remainder."
var INFO0043 = "Record Uploaded successfully.";
var INFO0044 = "Record Downloaded successfully.";
var INFO0045 = "Saved Successfully";
var INFO0046 = "There were %1% claims with  matches in the match file out of a total 1 claims";
var INFO0047 = "Claims Updated";
var INFO0048 = "Settlement negotiation details deleted successfully.";
var INFO0049 = "Configuration valid - select Create Batch to load.";
var INFO0050 = "Batch Loaded... ";
var INFO0051 = "The new Coverage  were successfully added.";
var INFO0052 = "Coverage updated successfully.";
var INFO0053 = "The new Policy was successfully added.";
var INFO0054 = "The new  Coverages  were successfully added.";
var INFO0055 = "Premium updated successfully.";
var INFO0056 = "The changes were saved successfully.";
var INFO0057 = "Update to Entities Control names not needed.";
var INFO0058 = "Re-Computing total vendor payments complete. Re-Creating 1099B File complete";
var INFO0059 = "Selected record updated successfully.";
var INFO0060 = "Approve all payments successfully.";
var INFO0061 = "Payment approved successfully.";
var INFO0062 = "Payment reviewed successfully.";
var INFO0063 = "Reduced Earning Details updated successfully.";
var INFO0064 = "Reduced Earnnings details deleted successfully.";
var INFO0065 = "You do not have authority to approve this worksheet.";
var INFO0066 = "Reserve Worksheet is approved successfully.";
var INFO0067 = "Reserve Worksheet is rejected successfully.";
var INFO0068 = "Attorney firm membership details deleted successfully.";
var INFO0069 = "Attorney Members List details deleted successfully.";
var INFO0070 = "Legal Budget Row deleted successfully.";
var INFO0071 = "Legal cause of action details deleted successfully.";
var INFO0072 = "Legal litigation and event details deleted successfully.";
var INFO0073 = "Legal attorny and party details deleted successfully.";
var INFO0074 = "Event details deleted successfully.";
var INFO0075 = "Party details deleted successfully.";
var INFO0076 = "Treatment deleted successfully.";
var INFO0077 = "Settlement benefit details deleted successfully.";
var INFO0078 = " Value must be between ";
var INFO0079 = " must be between ";
