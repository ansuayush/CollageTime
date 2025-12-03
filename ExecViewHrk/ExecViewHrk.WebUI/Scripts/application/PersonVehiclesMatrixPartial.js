var _vehicleFlag = '';
$(document).ready(function () {
    
    $.validator.unobtrusive.parse("#PersonVehicleForm");
    var appBaseUrl = $("#appBaseUrl").val();
    formService.isLoadingInProgress = false;
    $(document).ajaxStop(function () {
        formService.initialize();
    });

    $("#btnSavePersonVehicleAjax").click(function (e) {
        e.preventDefault();

        var form = $("#PersonVehicleForm");
        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var serialized_Object = form.serializeObject();
        serialized_Object.StateTitle = $("#StateTitle").data("kendoComboBox").text();

        $.ajax({
            url: appBaseUrl + "PersonVehicles/PersonVehiclesSaveAjax",
            type: "POST",
            data: { personVehicleVm: serialized_Object },
            success: function (response) {
                if (response.succeed == false) {
                    toastr.error(response.Message);
                    formService.isSavingInProgress = false;
                }
                else {
                    var _oldPersonVehicleId = $("#hiddenPersonVehicleId").val();
                    refreshPersonVehicleFormData(response.personVehicleVm);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    if (_oldPersonVehicleId != "0") {
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

    $("#btnClearPersonVehicleAjax").click(function (e) {
        e.preventDefault();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonVehicleForm");
        $.ajax({
            url: appBaseUrl + "PersonVehicles/PersonVehiclesIndexChangedAjax",
            type: "POST",
            data: {
                personVehicleIdDdl: 0,
                personId: personId,
            },
            success: function (response) {
                formService.reset(form);
                refreshPersonVehicleFormData(response);
                if (_vehicleFlag == 'CLEAR') {
                    _vehicleFlag = '';
                    //Custom Logic
                } else {
                    toastr.success(formService.messages.cleared);
                }
                return;
            }
        });
    });

    $("#btnDeletePersonVehicleAjax").click(function (e) {
        e.preventDefault();
        var personId = $("#hiddenPersonId").val();
        var personVehicleId = $("#hiddenPersonVehicleId").val();
        $.ajax({
            url: appBaseUrl + "PersonVehicles/PersonVehiclesDeleteAjax",
            type: "POST",
            data: {
                personVehicleIdDdl: personVehicleId,
                personId: personId
            },
            success: function (response) {
                if (response == formService.messages.vehicleNotExists) {
                    toastr.error(response);
                    return;
                }
                refreshPersonVehicleFormData(response);
                toastr.success(formService.messages.deleted);
                return;
            }
        });
    });

});

function onSelectPersonVehicleDdl(e) {
    //
    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);

    var ddlSelectedPersonVehicleId = dataItem.PersonVehicleId == "" ? 0 : dataItem.PersonVehicleId;

    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();

    $.ajax({
        url: appBaseUrl + "PersonVehicles/PersonVehiclesIndexChangedAjax",

        type: "POST",

        data: {

            personVehicleIdDdl: ddlSelectedPersonVehicleId,
            personId: personId
            //,requestType: requestType
        },

        success: function (response) {
            //

            refreshPersonVehicleFormData(response);
            return;
        }

    });
}

function onDataBoundPersonVehicleDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noVehicleRecordsFound').hide();
    else
        $('#noVehicleRecordsFound').show();
}

function error_handler(e) {
    //
    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshPersonVehicleFormData(response) {
    if (response) {
        $("#hiddenPersonId").val(response.PersonId);
        $("#hiddenPersonVehicleId").val(response.PersonVehicleId);
        if (response.StateTitle != null)
            $("#StateTitle").data("kendoComboBox").value(response.StateTitle);
        else
            $("#StateTitle").data("kendoComboBox").value("");

        var datepicker = $("#AcquisitionDate").data("kendoDatePicker");
        if (response.AcquisitionDate != null) {
            var date = new Date(parseInt(response.AcquisitionDate.substr(6)));
            $("#AcquisitionDate").val(date.toLocaleDateString());
            datepicker.value(date);
        }
        else {
            datepicker.value("");
        }
        var datepicker2 = $("#SoldDate").data("kendoDatePicker");
        if (response.SoldDate != null) {
            var date2 = new Date(parseInt(response.SoldDate.substr(6)));
            $("#SoldDate").val(date2.toLocaleDateString());
            datepicker2.value(date2);
        }
        else {
            datepicker2.value("");
        }
        $("#LicenseNumber").val(response.LicenseNumber == null ? "" : response.LicenseNumber);

        $("#Make").val(response.Make == null ? "" : response.Make);
        $("#Model").val(response.Model == null ? "" : response.Model);
        $("#Color").val(response.Color == null ? "" : response.Color);

        $("#enteredByLabel").text(response.EnteredBy == null ? "" : response.EnteredBy);

        if (response.EnteredDate != null) {
            var date = new Date(parseInt(response.EnteredDate.substr(6)));
            $("#enteredDateLabel").text(date.toLocaleString());
        }
        else {
            $("#enteredDateLabel").text("");
        }
        $("#modifiedByLabel").text(response.ModifiedBy == null ? "" : response.ModifiedBy);

        if (response.ModifiedDate != null) {
            var date = new Date(parseInt(response.ModifiedDate.substr(6)));
            $("#modifiedDateLabel").text(date.toLocaleString());
        }
        else {
            $("#modifiedDateLabel").text("");
        }
        $("#personNameLabel").html(" - " + response.PersonName);

        $("#Notes").val(response.Notes);

        var dropdownlist = $("#PersonVehicleIdDdl").data("kendoDropDownList");
        dropdownlist.refresh();
        dropdownlist.dataSource.read();
        if (response.PersonVehicleId != 0) {
            dropdownlist.value(response.PersonVehicleId);
        }
        else {
            dropdownlist.select(0);
        }

        if ($("#hiddenPersonVehicleId").val() > 0) {
            var form = $("#PersonVehicleForm");
            form.validate();
            if (!form.valid())
                toastr.error(formService.messages.invalidMessage);
        }

        appBaseUrl = $("#appBaseUrl").val();
        $.ajax({
            url: appBaseUrl + "PersonVehicles/GetPersonVehiclesList",
            type: "POST",
            success: function (response2) {
                //
                $("#PersonVehicleIdDdl").data("kendoDropDownList").value(response.PersonVehicleId);
            }
        });

        $(".validation-summary-errors").empty();
        return;

    } else {
        _vehicleFlag = 'CLEAR';
        $("#btnClearPersonVehicleAjax").trigger('click');
    }
}

function error_handler1(e) {
    //
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

        $('#gridDdlVehicle').data('kendoGrid').dataSource.read();
        $('#gridDdlVehicle').data('kendoGrid').refresh();
    }
}
function gridPersonVehicleEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);

    openPersonVehiclePopup(data.PersonVehicleId, data.PersonId);
}


function gridPersonVehicleDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { personVehicleId: data.PersonVehicleId };
    var formURL = $("#ApplicationUrl").val() + "PersonVehicles/PersonVehicleDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPersonVehicle");
            }
        });
    });
}

function addPersonVehicle() {
    openPersonVehiclePopup(0, $("#SelectedPersonID").val());
}

function openPersonVehiclePopup(_personVehicleId, _personId) {
    loading(true);
    var formURL = $("#ApplicationUrl").val() + '/PersonVehicles/PersonVehicleDetail?personVehicleId=' + _personVehicleId + '&personId=' + _personId;

    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);

    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');

}


