var _licenseFlag = '';
$(document).ready(function () {

    $.validator.unobtrusive.parse("#PersonLicenseForm");
    var appBaseUrl = $("#appBaseUrl").val();
    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });
    $("#btnSavePersonLicenseAjax").click(function (e) {
        e.preventDefault();
        var form = $("#PersonLicenseForm");
        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var serialized_Object = form.serializeObject();
        serialized_Object.LicenseDescription = $("#LicenseDescription").data("kendoComboBox").text();
        serialized_Object.StateTitle = $("#StateTitle").data("kendoComboBox").text();
        serialized_Object.CountryDescription = $("#CountryDescription").data("kendoComboBox").text();
       
        formService.isSavingInProgress = true;
    //     toastr.success(formService.messages.saving);
        $.ajax({

            url: appBaseUrl + "PersonLicenses/PersonLicensesSaveAjax",
            type: "POST",
            data: { personLicenseVm: serialized_Object },
            success: function (response) {
                if (response.succeed == false){
                    formService.isSavingInProgress = false;
                    toastr.error(response.Message);
                }
                else {
                    var _oldpersonLicenseId = $("#hiddenPersonLicenseId").val();
                    refreshPersonLicenseFormData(response.personLicenseVm);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    if (_oldpersonLicenseId != "0")
                    {
                        timeOutFn = setTimeout(function () {
                            formService.isSavingInProgress = false;
                            toastr.success(formService.messages.updated);
                            formService.isLoadingInProgress = false;
                        },0);
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
         error: function(){
            formService.isSavingInProgress = false;
        }
        });
    });

    $("#btnClearPersonLicenseAjax").click(function (e) {
        e.preventDefault();
        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonLicenseForm");
        $.ajax({

            url: appBaseUrl + "PersonLicenses/PersonLicensesIndexChangedAjax",
            type: "POST",
            data: {
                personLicenseIdDdl: 0,
                personId: personId,
            },

            success: function (response) {
                formService.reset(form);
                refreshPersonLicenseFormData(response);
                if (_licenseFlag == 'CLEAR') {
                    _licenseFlag = '';
                } else {
                    toastr.success(formService.messages.cleared);
                }
                return;
            }
        });
    });


    $("#btnDeletePersonLicenseAjax").click(function (e) {
        e.preventDefault();
        confirmDialog("", function () {
            appBaseUrl = $("#appBaseUrl").val();
            var personId = $("#hiddenPersonId").val();
            var personLicenseId = $("#hiddenPersonLicenseId").val();
            $.ajax({
                url: appBaseUrl + "PersonLicenses/PersonLicensesDeleteAjax",
                type: "POST",
                data: {
                    personLicenseIdDdl: personLicenseId,
                    personId: personId
                },
                success: function (response) {
                    if (response == formService.messages.licenceNotExists) {
                        toastr.error(response);
                        return;
                    }
                    refreshPersonLicenseFormData(response);
                    toastr.success(formService.messages.deleted);
                    return;
                }
            });
        });
    });

});
function onSelectPersonLicenseDdl(e) {

    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);

    var ddlSelectedPersonLicenseId = dataItem.PersonLicenseId == "" ? 0 : dataItem.PersonLicenseId;

    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();

    $.ajax({

        url: appBaseUrl + "PersonLicenses/PersonLicensesIndexChangedAjax",
        type: "POST",
        data: {
            personLicenseIdDdl: ddlSelectedPersonLicenseId,
            personId: personId
        },
        success: function (response) {
            refreshPersonLicenseFormData(response);
            return;
        }
    });
}

function onDataBoundPersonLicenseDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noLicenseRecordsFound').hide();
    else
        $('#noLicenseRecordsFound').show();
}

function error_handler(e) {
    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshPersonLicenseFormData(response) {
    if (response) {
        $("#hiddenPersonId").val(response.PersonId);
        $("#hiddenPersonLicenseId").val(response.PersonLicenseId);
        $('#formDiv input[type=radio]').prop('checked', false);
        $('#formDiv input:checkbox').prop('checked', false);

        if (response.LicenseDescription != null)
            $("#LicenseDescription").data("kendoComboBox").value(response.LicenseDescription);
        else
            $("#LicenseDescription").data("kendoComboBox").value("");

        if (response.StateTitle != null)
            $("#StateTitle").data("kendoComboBox").value(response.StateTitle);
        else
            $("#StateTitle").data("kendoComboBox").value("");

        if (response.CountryDescription != null)
            $("#CountryDescription").data("kendoComboBox").value(response.CountryDescription);
        else
            $("#CountryDescription").data("kendoComboBox").value("");

        var datepicker = $("#ExpirationDate").data("kendoDatePicker");
        if (response.ExpirationDate != null) {
            var date = new Date(parseInt(response.ExpirationDate.substr(6)));
            $("#ExpirationDate").val(date.toLocaleDateString());
            datepicker.value(date);
        }
        else
            datepicker.value("");

        var datepicker2 = $("#RevokedDate").data("kendoDatePicker");
        if (response.RevokedDate != null) {
            var date2 = new Date(parseInt(response.RevokedDate.substr(6)));
            $("#RevokedDate").val(date2.toLocaleDateString());
            datepicker2.value(date2);
        }
        else
            datepicker2.value("");

        var datepicker3 = $("#ReinstatedDate").data("kendoDatePicker");
        if (response.ReinstatedDate != null) {
            var date3 = new Date(parseInt(response.ReinstatedDate.substr(6)));
            $("#ReinstatedDate").val(date3.toLocaleDateString());
            datepicker3.value(date3);
        }
        else
            datepicker3.value("");

        $("#LicenseNumber").val(response.LicenseNumber == null ? "" : response.LicenseNumber);
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

        var dropdownlist = $("#PersonLicenseIdDdl").data("kendoDropDownList");
        dropdownlist.refresh();
        dropdownlist.dataSource.read();
        if (response.PersonLicenseId != 0) {
            dropdownlist.value(response.PersonLicenseId);
        }
        else {
            dropdownlist.select(0);
        }


        if ($("#hiddenPersonLicenseId").val() > 0) {
            var form = $("#PersonLicenseForm");

            form.validate();
            if (!form.valid())
                toastr.error(formService.messages.invalidMessage);
        }

        appBaseUrl = $("#appBaseUrl").val();
        $.ajax({
            url: appBaseUrl + "PersonLicenses/GetPersonLicensesList",
            type: "POST",
            success: function (response2) {
                $("#PersonLicenseIdDdl").data("kendoDropDownList").value(response.PersonLicenseId);
            }
        });
        $(".validation-summary-errors").empty();
        return;

    }
    else {
        _licenseFlag = 'CLEAR';
        $("#btnClearPersonLicenseAjax").trigger('click');
    }
    initializeForm();
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
        $('#gridDdlLicenseType').data('kendoGrid').dataSource.read();
        $('#gridDdlLicenseType').data('kendoGrid').refresh();
    }
}
function gridPersonLicenseEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);

    openPersonLicensePopup(data.PersonLicenseId, data.PersonId);
}


function gridPersonLicenseDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { personLicenseId: data.PersonLicenseId };
    var formURL = $("#ApplicationUrl").val() + "PersonLicenses/PersonLicenseDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPersonLicense");
            }
        });
    });
}

function addPersonLicense() {
    openPersonLicensePopup(0, $("#SelectedPersonID").val());
}

function openPersonLicensePopup(_personLicenseId, _personId) {
    loading(true);
    var formURL = $("#ApplicationUrl").val() + '/PersonLicenses/PersonLicenseDetail?personLicenseId=' + _personLicenseId + '&personId=' + _personId;

    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);

    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');

}




