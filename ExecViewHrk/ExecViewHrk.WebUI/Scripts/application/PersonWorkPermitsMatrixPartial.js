
$(document).ready(function () {
    //
    $.validator.unobtrusive.parse("#PersonWorkPermitForm");
    var appBaseUrl = $("#appBaseUrl").val();
    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });

    $("#btnSavePersonWorkPermitAjax").click(function (e) {
        e.preventDefault();
        var form = $("#PersonWorkPermitForm");
        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var serialized_Object = form.serializeObject();
        serialized_Object.CountryDescription = $("#CountryDescription").data("kendoComboBox").text();
        formService.isSavingInProgress = true;
    //     toastr.success(formService.messages.saving);
        $.ajax({
            url: appBaseUrl + "PersonWorkPermits/PersonWorkPermitsSaveAjax",
            type: "POST",

            data: { personWorkPermitVm: serialized_Object },
            success: function (response) {
                if (response.succeed == false) {
                    toastr.error(response.Message);
                    formService.isSavingInProgress = false;
                }
                else {
                    var _oldPersonWPId = $("#hiddenPersonWorkPermitId").val();
                    refreshPersonWorkPermitFormData(response.personWorkPermitVm);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    if (_oldPersonWPId != "0") {
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

    $("#btnClearPersonWorkPermitAjax").click(function (e) {
        //
        e.preventDefault();
        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonWorkPermitForm");
        $.ajax({
            url: appBaseUrl + "PersonWorkPermits/PersonWorkPermitsIndexChangedAjax",
            type: "POST",
            data: {
                personWorkPermitIdDdl: 0,
                personId: personId,
            },
            success: function (response) {
                formService.reset(form);
                refreshPersonWorkPermitFormData(response);
                toastr.success(formService.messages.cleared);
                return;
            }
        });
    });

    $("#btnDeletePersonWorkPermitAjax").click(function (e) {
        e.preventDefault();
        var personWorkPermitId = $("#hiddenPersonWorkPermitId").val();
        if (personWorkPermitId == "0") {
            toastr.error( formService.messages.workPermitNotExists);
            return;
        } else {
            confirmDialog("", function () {
                var personId = $("#hiddenPersonId").val();
                $.ajax({
                    url: appBaseUrl + "PersonWorkPermits/PersonWorkPermitsDeleteAjax",
                    type: "POST",
                    data: {
                        personWorkPermitIdDdl: personWorkPermitId,
                        personId: personId
                    },
                    success: function (response) {
                        if (response.succeed == true) {
                            refreshPersonWorkPermitFormData(response.personWorkPermitVm);
                            toastr.success(formService.messages.deleted);
                            return;
                        } else {
                            toastr.error(response.Message);
                            return;
                        }
                    }
                });

            });
        }
    });
});

function onSelectPersonWorkPermitDdl(e) {
    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);
    var ddlSelectedPersonWorkPermitId = dataItem.PersonWorkPermitId == "" ? 0 : dataItem.PersonWorkPermitId;
    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();
    $.ajax({
        url: appBaseUrl + "PersonWorkPermits/PersonWorkPermitsIndexChangedAjax",
        type: "POST",
        data: {
            personWorkPermitIdDdl: ddlSelectedPersonWorkPermitId,
            personId: personId
        },
        success: function (response) {
            refreshPersonWorkPermitFormData(response);
            return;
        }
    });
}
function onDataBoundPersonWorkPermitDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noWorkPermitRecordsFound').hide();
    else
        $('#noWorkPermitRecordsFound').show();
}

function error_handler(e) {
    if (e.errors) {
        toastr.error(e.errors);
    }
}
function refreshPersonWorkPermitFormData(response) {
    $("#hiddenPersonId").val(response.PersonId);
    $("#hiddenPersonWorkPermitId").val(response.PersonWorkPermitId);
    $('#formDiv input[type=radio]').prop('checked', false);
    $('#formDiv input:checkbox').prop('checked', false);

    if (response.CountryDescription != null)
        $("#CountryDescription").data("kendoComboBox").value(response.CountryDescription);
    else
        $("#CountryDescription").data("kendoComboBox").value("");

    var datepicker = $("#IssueDate").data("kendoDatePicker");
    if (response.IssueDate != null) {
        var date = new Date(parseInt(response.IssueDate.substr(6)));
        $("#IssueDate").val(date.toLocaleDateString());
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

    var datepicker3 = $("#ExtensionDate").data("kendoDatePicker");

    if (response.ExtensionDate != null) {
        var date3 = new Date(parseInt(response.ExtensionDate.substr(6)));
        $("#ExtensionDate").val(date3.toLocaleDateString());
        datepicker3.value(date3);
    }
    else
        datepicker3.value("");

    $("#WorkPermitNumber").val(response.WorkPermitNumber == null ? "" : response.WorkPermitNumber);
    $("#WorkPermitType").val(response.WorkPermitType == null ? "" : response.WorkPermitType);
    $("#IssuingAuthority").val(response.IssuingAuthority == null ? "" : response.IssuingAuthority);
    $("#enteredByLabel").text(response.EnteredBy == null ? "" : response.EnteredBy);

    if (response.EnteredDate != null) {
        var date = new Date(parseInt(response.EnteredDate.substr(6)));
        $("#enteredDateLabel").text(date.toLocaleString());
    }
    else
        $("#enteredDateLabel").text("");


    $("#personNameLabel").html(" - " + response.PersonName);
    $("#Notes").val(response.Notes);
    var dropdownlist = $("#PersonWorkPermitIdDdl").data("kendoDropDownList");
    dropdownlist.refresh();
    dropdownlist.dataSource.read();

    if (response.PersonWorkPermitId != 0) {
        dropdownlist.value(response.PersonWorkPermitId);
    }
    else {
        dropdownlist.select(0);
    }

    $(".validation-summary-errors").empty();

    if ($("#hiddenPersonWorkPermitId").val() > 0) {
        var form = $("#PersonWorkPermitForm");
        form.validate();
        if (!form.valid())
            toastr.error(formService.messages.invalidMessage);
    }

    appBaseUrl = $("#appBaseUrl").val();
    $.ajax({
        url: appBaseUrl + "PersonWorkPermits/GetPersonWorkPermitsList",
        type: "POST",
        success: function (response2) {
            //
            $("#PersonWorkPermitIdDdl").data("kendoDropDownList").value(response.PersonWorkPermitId);
        }
    });
    return;
}


function grdWorkPermitEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row)
    openPersonWorkPermitPopup(data.PersonWorkPermitId, data.PersonId);
}

function grdWorkPermitDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { personWorkPermitIdDdl: data.PersonWorkPermitId };
    var formURL = $("#ApplicationUrl").val() + "PersonWorkPermits/PersonWorkPermitsDeleteAjax";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPersonWorkPermit");            
            }
        });
    });
}
function addPersonWorkPermitAddress() {
    openPersonWorkPermitPopup(0, $("#SelectedPersonID").val());
}
function openPersonWorkPermitPopup(_personWorkPermitId, _personId) {
    loading(true);
    var formURL = $("#ApplicationUrl").val() + '/PersonWorkPermits/PersonWorkPermitDetails?personWorkPermitId=' + _personWorkPermitId + '&personId=' + _personId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}
