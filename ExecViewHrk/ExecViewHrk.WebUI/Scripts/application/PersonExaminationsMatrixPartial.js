$(document).ready(function () {
    $.validator.unobtrusive.parse("#PersonExaminationForm");
    var appBaseUrl = $("#appBaseUrl").val();
    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });
    $("#btnSavePersonExaminationAjax").click(function (e) {
        e.preventDefault();
        var form = $("#PersonExaminationForm");
        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var serialized_Object = form.serializeObject();
        serialized_Object.MedicalExaminationDescription = $("#MedicalExaminationDescription").data("kendoComboBox").text();

        formService.isSavingInProgress = true;
    //     toastr.success(formService.messages.saving);
        $.ajax({
            url: appBaseUrl + "PersonExaminations/PersonExaminationsSaveAjax",
            type: "POST",
            data: { personExaminationVm: serialized_Object },
            success: function (response) {
                if (response.succeed == false)
                    toastr.error(response.Message);
                else {
                    var _oldexaminationid = $("#hiddenPersonExaminationId").val();
                    refreshPersonExaminationFormData(response.personExaminationVm);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    if (_oldexaminationid != "0") {
                        timeOutFn = setTimeout(function () {
                            formService.isSavingInProgress = false;
                            toastr.success(formService.messages.updated);
                            formService.isLoadingInProgress = false;
                        }, 0);
                    }
                    else {
                        timeOutFn = setTimeout(function () {
                            formService.isSavingInProgress = false;
                            toastr.success(formService.messages.saved);
                            formService.isLoadingInProgress = false;
                        }, 0);
                    }
                }
            },
            error: function () {
                formService.isSavingInProgress = false;
            }
        });
    });

    $("#btnClearPersonExaminationAjax").click(function (e) {
        e.preventDefault();
        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonExaminationForm");
        $.ajax({
            url: appBaseUrl + "PersonExaminations/PersonExaminationsIndexChangedAjax",
            type: "POST",
            data: {
                personExaminationIdDdl: 0,
                personId: personId,
            },
            success: function (response) {
                formService.reset(form);
                refreshPersonExaminationFormData(response);
                toastr.success(formService.messages.cleared);;
                return;
            }
        });
    });

    $("#btnDeletePersonExaminationAjax").click(function (e) {
        e.preventDefault();
        var personExaminationId = $("#hiddenPersonExaminationId").val();
        if (personExaminationId == "0") {
            formService.messages.examinationNotExists
            return;
        } else {
            confirmDialog("", function () {
                var personId = $("#hiddenPersonId").val();
                var personExaminationId = $("#hiddenPersonExaminationId").val();
                $.ajax({
                    url: appBaseUrl + "PersonExaminations/PersonExaminationsDeleteAjax",
                    type: "POST",
                    data: {
                        personExaminationIdDdl: personExaminationId,
                        personId: personId
                    },
                    success: function (response) {
                        if (response == formService.messages.examinationNotExists) {
                            toastr.error(response);
                            return;
                        }
                        refreshPersonExaminationFormData(response);
                        toastr.success(formService.messages.deleted);
                        return;
                    }
                });
            });
        }
    });
});

function onSelectPersonExaminationDdl(e) {
    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);
    var ddlSelectedPersonExaminationId = dataItem.PersonExaminationId == "" ? 0 : dataItem.PersonExaminationId;
    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();
    $.ajax({
        url: appBaseUrl + "PersonExaminations/PersonExaminationsIndexChangedAjax",
        type: "POST",
        data: {
            personExaminationIdDdl: ddlSelectedPersonExaminationId,
            personId: personId
        },
        success: function (response) {
            refreshPersonExaminationFormData(response);
            return;
        }
    });
}

function onDataBoundPersonExaminationDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noExaminationRecordsFound').hide();
    else
        $('#noExaminationRecordsFound').show();
}

function refreshPersonExaminationFormData(response) {
    $("#hiddenPersonId").val(response.PersonId);
    $("#hiddenPersonExaminationId").val(response.PersonExaminationId);
    $('#formDiv input[type=radio]').prop('checked', false);
    $('#formDiv input:checkbox').prop('checked', false);
    if (response.MedicalExaminationDescription != null)
        $("#MedicalExaminationDescription").data("kendoComboBox").value(response.MedicalExaminationDescription);
    else
        $("#MedicalExaminationDescription").data("kendoComboBox").value("");
    var datepicker = $("#ExaminationDate").data("kendoDatePicker");
    if (response.ExaminationDate != null) {
        var date = new Date(parseInt(response.ExaminationDate.substr(6)));
        $("#ExaminationDate").val(date.toLocaleDateString());
        datepicker.value(date);
    }
    else
        datepicker.value("");
    var datepicker2 = $("#NextScheduledExamination").data("kendoDatePicker");
    if (response.NextScheduledExamination != null) {
        var date2 = new Date(parseInt(response.NextScheduledExamination.substr(6)));
        $("#NextScheduledExamination").val(date2.toLocaleDateString());
        datepicker2.value(date2);
    }
    else
        datepicker2.value("");
    $("#Examiner").val(response.Examiner == null ? "" : response.Examiner);

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
    var dropdownlist = $("#PersonExaminationIdDdl").data("kendoDropDownList");
    dropdownlist.refresh();
    dropdownlist.dataSource.read();
    if (response.PersonExaminationId != 0) {
        dropdownlist.value(response.PersonExaminationId);
    }
    else {
        dropdownlist.select(0);
    }
    $(".validation-summary-errors").empty();
    if ($("#hiddenPersonExaminationId").val() > 0) {
        var form = $("#PersonExaminationForm");
        form.validate();
        if (!form.valid())
            toastr.error(formService.messages.invalidMessage);
    }
    appBaseUrl = $("#appBaseUrl").val();
    $.ajax({
        url: appBaseUrl + "PersonExaminations/GetPersonExaminationsList",
        type: "POST",
        success: function (response2) {
            $("#PersonExaminationIdDdl").data("kendoDropDownList").value(response.PersonExaminationId);
        }
    });
    return;
}
function gridPersonExaminationEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openExamEditpopup(data.PersonExaminationId, data.PersonId);
}
function gridPersonExaminationDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { personExaminationId: data.PersonExaminationId };
    var formURL = $("#ApplicationUrl").val() + "PersonExaminations/PersonExaminationsDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPersonExamination");
            }
        });
    });
}
function addMedicalExamination() {
    openExamEditpopup(0, $("#SelectedPersonID").val());
}

function openExamEditpopup(_personExaminationId, _personId) {
    var formURL = $("#ApplicationUrl").val() + '/PersonExaminations/PersonExaminationsDetail?personExaminationId=' + _personExaminationId + '&personId=' + _personId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}