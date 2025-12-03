
$(document).ready(function () {

    $.validator.unobtrusive.parse("#PersonEducationForm");
    var appBaseUrl = $("#appBaseUrl").val();

    $("#btnSavePersonEducationAjax").click(function (e) {
        e.preventDefault();



        var form = $("#PersonEducationForm");

        form.validate();
        if (!form.valid()) {
            toastr.error("form is invalid");
            return;
        }

        var serialized_Object = form.serializeObject();
        // serialized_Object.AccommodationProvided = $('input[name=AccommodationProvided]').val(); // for some reason checkboxes are always set to false

        // these x lines work
        //@*var personAdaVm = @Html.Raw(Json.Encode(Model));
        //personAdaVm.AccomodationTypeDescription =  $("#AccommodationDescription").data("kendoComboBox").value();
        //personAdaVm.RequestedDate = $("#RequestedDate").data("kendoDatePicker").value();
        //personAdaVm.ProvidedDate = $("#ProvidedDate").data("kendoDatePicker").value();
        //personAdaVm.EstimatedCost = $("#EstimatedCost").data("kendoNumericTextBox").value();
        //personAdaVm.AccommodationProvided = $('input[name=AccommodationProvided]').val();*@



        //var requestType = $("#requestType").val();
        //var personId = $("#hiddenPersonId").val();

        $.ajax({

            url: appBaseUrl + "PersonEducations/PersonEducationsSaveAjax",
            //url: "http://localhost/ExecViewHrk/PersonEducations/PersonEducationsSaveAjax",

            type: "POST",

            //data: { personEducationVm: serialized_Object, requestType: requestType },
            data: { personEducationVm: serialized_Object },
            //data: { personAdaVm: personAdaVm, requestType : requestType },

            success: function (response) {
                refreshPersonEducationFormData(response);
                toastr.success("Record has been saved");
                return;
            }
        });
    });

    $("#btnClearPersonEducationAjax").click(function (e) {
        e.preventDefault();

        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();

        $.ajax({

            //url: "@Url.Action("PersonAdaIndexChangedAjax", "Person")",
            url: appBaseUrl + "PersonEducations/PersonEducationsIndexChangedAjax",

            type: "POST",

            data: {

                personEducationIdDdl: 0,
                personId: personId,
                // requestType: requestType
            },

            success: function (response) {
                refreshPersonEducationFormData(response);
                toastr.success("Record has been cleared.");
                return;
            }

        });

    });

    $("#btnDeletePersonEducationAjax").click(function (e) {
        e.preventDefault();

        //var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var personEducationId = $("#hiddenPersonEducationId").val();

        $.ajax({

            //url: "@Url.Action("PersonAdaDeleteAjax", "Person")",
            url: appBaseUrl + "PersonEducations/PersonEducationsDeleteAjax",

            type: "POST",

            data: {
                personEducationIdDdl: personEducationId,
                personId: personId
                //,requestType: requestType
            },

            success: function (response) {

                if (response == "The person Education Record does not exist") {
                    toastr.error(response);
                    return;
                }

                refreshPersonEducationFormData(response);
                toastr.success("Record has been deleted");
                return;
            }
        });
    });

});

function onSelectPersonEducationDdl(e) {
    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);

    var ddlSelectedPersonEducationId = dataItem.PersonEducationId == "" ? 0 : dataItem.PersonEducationId;

    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();

    $.ajax({

        //url: "@Url.Action("PersonAdaIndexChangedAjax", "Person")",
        url: appBaseUrl + "PersonEducations/PersonEducationsIndexChangedAjax",

        type: "POST",

        data: {

            personEducationIdDdl: ddlSelectedPersonEducationId,
            personId: personId
            //,requestType: requestType
        },

        success: function (response) {
            refreshPersonEducationFormData(response);
            return;
        }

    });
}

function onDataBoundPersonEducationDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noEducationRecordsFound').hide();
    else
        $('#noEducationRecordsFound').show();
}

function error_handler(e) {
    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshPersonEducationFormData(response) {
    $("#hiddenPersonId").val(response.PersonId);
    $("#hiddenPersonEducationId").val(response.PersonEducationId);
    //$("#formDiv input:text").val("");
    $('#formDiv input[type=radio]').prop('checked', false);
    $('#formDiv input:checkbox').prop('checked', false);

    if (response.QualificationTypeDescription != null)
        $("#QualificationTypeDescription").data("kendoComboBox").value(response.QualificationTypeDescription);
    else
        $("#QualificationTypeDescription").data("kendoComboBox").value("");

    if (response.MajorDescription != null)
        $("#MajorDescription").data("kendoComboBox").value(response.MajorDescription);
    else
        $("#MajorDescription").data("kendoComboBox").value("");

    if (response.MinorDescription != null)
        $("#MinorDescription").data("kendoComboBox").value(response.MinorDescription);
    else
        $("#MinorDescription").data("kendoComboBox").value("");

    if (response.EducationEstablishmentDescription != null)
        $("#EducationEstablishmentDescription").data("kendoComboBox").value(response.EducationEstablishmentDescription);
    else
        $("#EducationEstablishmentDescription").data("kendoComboBox").value("");

    if (response.LevelDescription != null)
        $("#LevelDescription").data("kendoComboBox").value(response.LevelDescription);
    else
        $("#LevelDescription").data("kendoComboBox").value("");

    var datepicker = $("#PlannedStart").data("kendoDatePicker");
    if (response.PlannedStart != null) {
        var date = new Date(parseInt(response.PlannedStart.substr(6)));
        $("#PlannedStart").val(date.toLocaleDateString());
        datepicker.value(date);
    }
    else
        datepicker.value("");

    var datepicker2 = $("#PlannedCompletion").data("kendoDatePicker");
    if (response.PlannedCompletion != null) {
        var date2 = new Date(parseInt(response.PlannedCompletion.substr(6)));
        $("#PlannedCompletion").val(date2.toLocaleDateString());
        datepicker2.value(date2);
    }
    else
        datepicker2.value("");


    var datepicker3 = $("#ActualCompletion").data("kendoDatePicker");
    if (response.ActualCompletion != null) {
        var date3 = new Date(parseInt(response.ActualCompletion.substr(6)));
        $("#ActualCompletion").val(date3.toLocaleDateString());
        datepicker3.value(date2);
    }
    else
        datepicker3.value("");


    $("#Grade").val(response.Grade == null ? "" : response.Grade);
    $("#Gpa").val(response.Gpa == null ? "" : response.Gpa);
    $("#CreditsEarned").val(response.CreditsEarned == null ? "" : response.CreditsEarned);

    $("#enteredByLabel").text(response.EnteredBy == null ? "" : response.EnteredBy);

    if (response.EnteredDate != null) {
        var date = new Date(parseInt(response.EnteredDate.substr(6)));
        $("#enteredDateLabel").text(date.toLocaleString());
    }
    else
        $("#enteredDateLabel").text("");

    $("#modifiedByLabel").text(response.ModifiedBy == null ? "" : response.ModifiedBy);

    if (response.ModifiedDate != null) {
        var date = new Date(parseInt(response.ModifiedDate.substr(6)));
        $("#modifiedDateLabel").text(date.toLocaleString());
    }
    else
        $("#modifiedDateLabel").text("");

    $("#personNameLabel").html(" - " + response.PersonName);

    $("#Notes").val(response.Notes);

    var dropdownlist = $("#PersonEducationIdDdl").data("kendoDropDownList");
    dropdownlist.refresh();
    dropdownlist.dataSource.read();
    if (response.PersonEducationId != 0) {
        dropdownlist.value(response.PersonEducationId);
    }
    else {
        dropdownlist.select(0);
    }

    $(".validation-summary-errors").empty();

    if ($("#hiddenPersonEducationId").val() > 0) {
        var form = $("#PersonEducationForm");

        form.validate();
        if (!form.valid())
            toastr.error("form is invalid");
    }

    appBaseUrl = $("#appBaseUrl").val();
    $.ajax({
        url: appBaseUrl + "PersonEducations/GetPersonEducationsList",
        type: "POST",
        success: function (response2) {
            $("#PersonEducationIdDdl").data("kendoDropDownList").value(response.PersonEducationId);
        }
    });

    return;
}

function error_handler1(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        toastr.error(message);

        $('#gridDdlEducation').data('kendoGrid').dataSource.read();
        $('#gridDdlEducation').data('kendoGrid').refresh();
    }
}


