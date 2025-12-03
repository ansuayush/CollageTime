var _PassportFlag = '';
$(document).ready(function () {
    $.validator.unobtrusive.parse("#PersonPassportForm");
    var appBaseUrl = $("#appBaseUrl").val();

    formService.isLoadingInProgress = true;

    $(document).ajaxStop(function () {
        formService.initialize();
    })

    $("#btnSavePersonPassportAjax").click(function (e) {
        e.preventDefault();
        var form = $("#PersonPassportForm");
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
            url: appBaseUrl + "PersonPassports/PersonPassportsSaveAjax",
            type: "POST",
            data: { personPassportVm: serialized_Object },
            success: function (response) {
                if (response.succeed == false) {
                    toastr.error(response.Message);
                    formService.isSavingInProgress = false;
                }
                else {
                    var _oldpersonPassportId = $("#hiddenPersonPassportId").val();
                    refreshPersonPassportFormData(response.personPassportVm);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    if (_oldpersonPassportId != "0") {
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

    $("#btnClearPersonPassportAjax").click(function (e) {
        e.preventDefault();
        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonPassportForm");
        $.ajax({
            url: appBaseUrl + "PersonPassports/PersonPassportsIndexChangedAjax",
            type: "POST",
            data: {

                personPassportIdDdl: 0,
                personId: personId,
            },

            success: function (response) {
                formService.reset(form);
                refreshPersonPassportFormData(response);
                if (_PassportFlag == 'CLEAR') {
                    _PassportFlag = '';
                } else {
                    toastr.success(formService.messages.cleared);
                }
                return;
            }

        });

    });

    $("#btnDeletePersonPassportAjax").click(function (e) {

        e.preventDefault();
        confirmDialog("", function () {
            appBaseUrl = $("#appBaseUrl").val();
            var personId = $("#hiddenPersonId").val();
            var personPassportId = $("#hiddenPersonPassportId").val();

            $.ajax({
                url: appBaseUrl + "PersonPassports/PersonPassportsDeleteAjax",
                type: "POST",
                data: {
                    personPassportIdDdl: personPassportId,
                    personId: personId
                },
                success: function (response) {
                    if (response == formService.messages.passportNotExists) {
                        toastr.error(response);
                        return;
                    }
                    refreshPersonPassportFormData(response);
                    toastr.success(formService.messages.deleted);
                    return;
                }
            });
        });
    });
});

function onSelectPersonPassportDdl(e) {

    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);
    var ddlSelectedPersonPassportId = dataItem.PersonPassportId == "" ? 0 : dataItem.PersonPassportId;
    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();

    $.ajax({
        url: appBaseUrl + "PersonPassports/PersonPassportsIndexChangedAjax",
        type: "POST",
        data: {

            personPassportIdDdl: ddlSelectedPersonPassportId,
            personId: personId
        },

        success: function (response) {

            refreshPersonPassportFormData(response);
            return;
        }

    });
}

function onDataBoundPersonPassportDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noPassportRecordsFound').hide();
    else
        $('#noPassportRecordsFound').show();
}

function error_handler(e) {

    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshPersonPassportFormData(response) {
    if(response) {
        $("#hiddenPersonId").val(response.PersonId);
        $("#hiddenPersonPassportId").val(response.PersonPassportId);
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

        $("#PassportNumber").val(response.PassportNumber == null ? "" : response.PassportNumber);

        $("#PassportStorage").val(response.PassportStorage == null ? "" : response.PassportStorage);

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

        var dropdownlist = $("#PersonPassportIdDdl").data("kendoDropDownList");
        dropdownlist.refresh();
        dropdownlist.dataSource.read();
        if (response.PersonPassportId != 0) {     
            dropdownlist.value(response.PersonPassportId);
        }
        else {
            dropdownlist.select(0);
        }
        if ($("#hiddenPersonPassportId").val() > 0) {
            var form = $("#PersonPassportForm");
            form.validate();
            if (!form.valid())
                toastr.error(formService.messages.invalidMessage);
        }

        appBaseUrl = $("#appBaseUrl").val();
        $.ajax({
            url: appBaseUrl + "PersonPassports/GetPersonPassportsList",
            type: "POST",
            success: function (response2) {
         
                $("#PersonPassportIdDdl").data("kendoDropDownList").value(response.PersonPassportId);
            }
        });
        $(".validation-summary-errors").empty();
        return;
    }
    else
    {
        _PassportFlag = 'CLEAR';
        $("#btnClearPersonPassportAjax").trigger('click');
    }
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

        $('#gridPassport').data('kendoGrid').dataSource.read();
        $('#gridPassport').data('kendoGrid').refresh();
    }
}
function gridPersonPassportEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openPersonPassportPopup(data.PersonPassportId, data.PersonId);
}


function gridPersonPassportDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { personPassportId: data.PersonPassportId };
    var formURL = $("#ApplicationUrl").val() + "PersonPassports/PersonPassportDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPersonPassport");
            }
        });
    });
}

function addPersonPassport() {
    openPersonPassportPopup(0, $("#SelectedPersonID").val());
}

function openPersonPassportPopup(_personPassportId, _personId) {
    loading(true);
    var formURL = $("#ApplicationUrl").val() + '/PersonPassports/PersonPassportDetail?personPassportId=' + _personPassportId + '&personId=' + _personId;

    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);

    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}






