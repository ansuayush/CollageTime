
$(document).ready(function () {
    $.validator.unobtrusive.parse("#PersonInnoculationForm");
    var appBaseUrl = $("#appBaseUrl").val();

    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });

    $("#btnSavePersonInnoculationAjax").click(function (e) {
        e.preventDefault();
        var form = $("#PersonInnoculationForm");
        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var serialized_Object = form.serializeObject();
        serialized_Object.InnoculationDescription = $("#InnoculationDescription").data("kendoComboBox").text();
        formService.isSavingInProgress = true;
    //     toastr.success(formService.messages.saving);
        $.ajax({
            url: appBaseUrl + "PersonInnoculations/PersonInnoculationsSaveAjax",
            type: "POST",
            data: { personInnoculationVm: serialized_Object },
            success: function (response) {
                if (response.succeed == false) {
                    toastr.error(response.Message);
                    formService.isSavingInProgress = false;
                }
                else {
                    var _oldPersonInnoculationId = $("#hiddenPersonInnoculationId").val();
                    refreshPersonInnoculationFormData(response.personInnoculationVm);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    if (_oldPersonInnoculationId != "0") {
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

    $("#btnClearPersonInnoculationAjax").click(function (e) {
        e.preventDefault();
        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonInnoculationForm");
        $.ajax({
            //url: "@Url.Action("PersonAdaIndexChangedAjax", "Person")",
            url: appBaseUrl + "PersonInnoculations/PersonInnoculationsIndexChangedAjax",
            type: "POST",
            data: {
                personInnoculationIdDdl: 0,
                personId: personId,
                //requestType: requestType
            },
            success: function (response) {
                formService.reset(form);
                refreshPersonInnoculationFormData(response);
                toastr.success(formService.messages.cleared);
                return;
            }
        });
    });

    $("#btnDeletePersonInnoculationAjax").click(function (e) {
        e.preventDefault();
        var personInnoculationId = $("#hiddenPersonInnoculationId").val();
        if (personInnoculationId == "0") {
            toastr.error(formService.messages.innoculationNotExists);
            return;
        } else {
            confirmDialog("", function () {
                var personId = $("#hiddenPersonId").val();
                $.ajax({
                    url: appBaseUrl + "PersonInnoculations/PersonInnoculationsDeleteAjax",
                    type: "POST",
                    data: {
                        personInnoculationIdDdl: personInnoculationId,
                        personId: personId
                    },
                    success: function (response) {

                        if (response == formService.messages.innoculationNotExists) {
                            toastr.error(response);
                            return;
                        }
                        refreshPersonInnoculationFormData(response);
                        toastr.success(formService.messages.deleted);
                        return;
                    }
                });

            });
        }
    });
});

function onSelectPersonInnoculationDdl(e) {
    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);
    var ddlSelectedPersonInnoculationId = dataItem.PersonInnoculationId == "" ? 0 : dataItem.PersonInnoculationId;
    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();
    $.ajax({
        //url: "@Url.Action("PersonAdaIndexChangedAjax", "Person")",
        url: appBaseUrl + "PersonInnoculations/PersonInnoculationsIndexChangedAjax",
        type: "POST",
        data: {
            personInnoculationIdDdl: ddlSelectedPersonInnoculationId,
            personId: personId
            //,requestType: requestType
        },
        success: function (response) {
            refreshPersonInnoculationFormData(response);
            return;
        }
    });
}

function onDataBoundPersonInnoculationDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noInnoculationRecordsFound').hide();
    else
        $('#noInnoculationRecordsFound').show();
}

function error_handler(e) {
    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshPersonInnoculationFormData(response) {
    $("#hiddenPersonId").val(response.PersonId);
    $("#hiddenPersonInnoculationId").val(response.PersonInnoculationId);
    //$("#formDiv input:text").val("");
    $('#formDiv input[type=radio]').prop('checked', false);
    $('#formDiv input:checkbox').prop('checked', false);
    if (response.InnoculationDescription != null)
        $("#InnoculationDescription").data("kendoComboBox").value(response.InnoculationDescription);
    else
        $("#InnoculationDescription").data("kendoComboBox").value("");
    var datepicker = $("#InnoculationDate").data("kendoDatePicker");
    if (response.InnoculationDate != null) {
        var date = new Date(parseInt(response.InnoculationDate.substr(6)));
        $("#InnoculationDate").val(date.toLocaleDateString());
        datepicker.value(date);
    }
    else
        datepicker.value("");
    var datepicker2 = $("#ExpirationDate").data("kendoDatePicker");
    if (response.ExpirationDate != null) {
        var date2 = new Date(parseInt(response.ExpirationDate.substr(6)));
        $("#ExpirationDate").val(date2.toLocaleDateString());
        datepicker2.value(date2);
    }
    else
        datepicker2.value("");
    $("#enteredByLabel").text(response.EnteredBy == null ? "" : response.EnteredBy);
    if (response.EnteredDate != null) {
        $("#enteredDateLabel").text(new Date(parseInt(response.EnteredDate.substr(6))).toLocaleString());
    }
    else
        $("#enteredDateLabel").text("");
    $("#modifiedByLabel").text(response.ModifiedBy == null ? "" : response.ModifiedBy);
    if (response.ModifiedDate != null) {
        $("#modifiedDateLabel").text(new Date(parseInt(response.ModifiedDate.substr(6))).toLocaleString());
    }
    else
        $("#modifiedDateLabel").text("");
    $("#personNameLabel").html(" - " + response.PersonName);
    $("#Notes").val(response.Notes);
    var dropdownlist = $("#PersonInnoculationIdDdl").data("kendoDropDownList");
    dropdownlist.refresh();
    dropdownlist.dataSource.read();
    if (response.PersonInnoculationId != 0) {
        dropdownlist.value(response.PersonInnoculationId);
    }
    else {
        dropdownlist.select(0);
    }
    $(".validation-summary-errors").empty();
    if ($("#hiddenPersonInnoculationId").val() > 0) {
        var form = $("#PersonInnoculationForm");
        form.validate();
        if (!form.valid())
            toastr.error(formService.messages.invalidMessage);
    }
    appBaseUrl = $("#appBaseUrl").val();
    $.ajax({
        url: appBaseUrl + "PersonInnoculations/GetPersonInnoculationsList",
        type: "POST",
        success: function (response2) {
            $("#PersonInnoculationIdDdl").data("kendoDropDownList").value(response.PersonInnoculationId);
        }
    });
    return;
}
function gridPersonInnoculationEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openInnoculationEditpopup(data.PersonInnoculationId, data.PersonId);
}
function gridPersonInnoculationDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { personInnoculationId: data.PersonInnoculationId };
    var formURL = $("#ApplicationUrl").val() + "PersonInnoculations/PersonInnoculationsDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPersonInnoculation");
            }
        });
    });
}
function addInnoculation() {
    openInnoculationEditpopup(0, $("#SelectedPersonID").val());
}

function openInnoculationEditpopup(_personInnoculationId, _personId) {
    
    var formURL = $("#ApplicationUrl").val() + '/PersonInnoculations/PersonInnoculationsDetail?personInnoculationId=' + _personInnoculationId + '&personId=' + _personId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
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
        $('#gridDdlInnoculationType').data('kendoGrid').dataSource.read();
        $('#gridDdlInnoculationType').data('kendoGrid').refresh();
    }
}
