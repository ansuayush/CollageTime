
$(document).ready(function () {

    $.validator.unobtrusive.parse("#PersonAddressForm");
    var appBaseUrl = $("#appBaseUrl").val();

    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });

    $("#btnSavePersonAddressAjax").click(function (e) {
        e.preventDefault();
        var form = $("#PersonAddressForm");
        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var serialized_Object = form.serializeObject();
        serialized_Object.AddressDescription = $("#AddressDescription").data("kendoComboBox").text();
        serialized_Object.StateTitle = $("#StateTitle").data("kendoComboBox").text();
        serialized_Object.CountryDescription = $("#CountryDescription").data("kendoComboBox").text();
        serialized_Object.CheckPayrollAddress = $("#CheckPayrollAddress")["0"].checked;
        serialized_Object.CorrespondenceAddress = $("#CorrespondenceAddress")["0"].checked;
        var requestType = $("#requestType").val();
        formService.isSavingInProgress = true;
    //     toastr.success(formService.messages.saving);
        $.ajax({

            url: appBaseUrl + "PersonAddresses/PersonAddressesSaveAjax",
            type: "POST",

           
            data: { personAddressVm: serialized_Object },

            success: function (response) {
                if (response.succeed == false) {
                    toastr.error(response.Message);
                    formService.isSavingInProgress = false;
                }
                else {
                    var _oldpersonAddressId = $("#hiddenPersonAddressId").val();
                    refreshPersonAddressFormData(response.personAddressVm);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    if (_oldpersonAddressId != "0") {
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

    $("#btnClearPersonAddressAjax").click(function (e) {
        e.preventDefault();
        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonAddressForm");
        $.ajax({
            url: appBaseUrl + "PersonAddresses/PersonAddressesIndexChangedAjax",
            type: "POST",
            data: {
                personAddressIdDdl: 0,
                personId: personId
            },

            success: function (response) {
                formService.reset(form);
                refreshPersonAddressFormData(response);
                toastr.success(formService.messages.cleared);
                return;
            }

        });

    });

    $("#btnDeletePersonAddressAjax").click(function (e) {
        appBaseUrl = $("#appBaseUrl").val();
        e.preventDefault();
        confirmDialog("", function () {
        var personId = $("#hiddenPersonId").val();
        var personAddressId = $("#hiddenPersonAddressId").val();

        $.ajax({
            url: appBaseUrl + "PersonAddresses/PersonAddressesDeleteAjax",

            type: "POST",

            data: {
                personAddressIdDdl: personAddressId,
                personId: personId
            },

            success: function (response) {
                if (response == formService.messages.addressNotExists) {
                    toastr.error(response);
                    return;
                }
                refreshPersonAddressFormData(response);
                toastr.success(formService.messages.deleted);
                return;
            }
        });
});
});

});

function onSelectPersonAddressDdl(e) {
    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);

    var ddlSelectedPersonAddressId = dataItem.AddressTypeId == "" ? 0 : dataItem.AddressTypeId;

    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();

    $.ajax({

        url: appBaseUrl + "PersonAddresses/PersonAddressesIndexChangedAjax",

        type: "POST",

        data: {

            personAddressIdDdl: ddlSelectedPersonAddressId,
            personId: personId
        },

        success: function (response) {

            refreshPersonAddressFormData(response);
            return;
        }

    });
}

function onDataBoundPersonAddressDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noAddressRecordsFound').hide();
    else
        $('#noAddressRecordsFound').show();
}

function error_handler(e) {
    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshPersonAddressFormData(response) {
  
    $("#hiddenPersonId").val(response.PersonId);
    $("#hiddenPersonAddressId").val(response.PersonAddressId);
    //$("#formDiv input:text").val("");
    $('#formDiv input[type=radio]').prop('checked', false);
    $('#formDiv input:checkbox').prop('checked', false);

    if (response.AddressDescription != null)
        $("#AddressDescription").data("kendoComboBox").value(response.AddressDescription);
    else
        $("#AddressDescription").data("kendoComboBox").value("");

    if (response.StateTitle != null)
        $("#StateTitle").data("kendoComboBox").value(response.StateTitle);
    else
        $("#StateTitle").data("kendoComboBox").value("");

    if (response.CountryDescription != null)
        $("#CountryDescription").data("kendoComboBox").value(response.CountryDescription);
    else
        $("#CountryDescription").data("kendoComboBox").value("");



    $("#AddressLineOne").val(response.AddressLineOne);

    $("#AddressLineTwo").val(response.AddressLineTwo);

    $("#City").val(response.City);

    $("#ZipCode").val(response.ZipCode);

    $('input[name=CheckPayrollAddress]').val([response.CheckPayrollAddress]);

    $('input[name=CorrespondenceAddress]').val([response.CorrespondenceAddress]);

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

    var dropdownlist = $("#PersonAddressIdDdl").data("kendoDropDownList");
    dropdownlist.refresh();
    dropdownlist.dataSource.read();
    if (response.PersonAddressId != 0) {
        dropdownlist.value(response.PersonAddressId);
    }
    else {
        dropdownlist.select(0);
    }

    $(".validation-summary-errors").empty();

    if ($("#hiddenPersonAddressId").val() > 0) {
        var form = $("#PersonAddressForm");
        form.validate();
        if (!form.valid())
            toastr.error(formService.messages.invalidMessage);
    }

    appBaseUrl = $("#appBaseUrl").val();
    $.ajax({
        url: appBaseUrl + "PersonAddresses/GetPersonAddressesList",
        type: "POST",
        success: function (response2) {
            if (response.AddressTypeId != 0) {
                $("#PersonAddressIdDdl").data("kendoDropDownList").value(response.PersonAddressId);
            }
            else {
                $("#PersonAddressIdDdl").data("kendoDropDownList").value(0);
            }
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

        $('#gridDdlAddressType').data('kendoGrid').dataSource.read();
        $('#gridDdlAdressType').data('kendoGrid').refresh();
    }
}


function grdAddressEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openPersonAddressPopup(data.PersonAddressId);
}

function grdAddressDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { personAddressId: data.PersonAddressId };
    var formURL = $("#ApplicationUrl").val() + "PersonAddresses/PersonAddressDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPersonAddress");
                LoadMenu("Person", "Person")
            }
        });
    });
}

function addPersonAddress() {
    openPersonAddressPopup(0);
}

function openPersonAddressPopup(_PersonAddressId) {
    var formURL = $("#ApplicationUrl").val() + '/PersonAddresses/PersonAddressDetail?personAddressId=' + _PersonAddressId;
    $("#PhoneAddressDetailAddModal .modal-content").html("");
    $("#PhoneAddressDetailAddModal .modal-content").load(formURL);
    $("#PhoneAddressDetailAddModal").modal({ show: true });
}

