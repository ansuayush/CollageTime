var _propertyFlag = '';
$(document).ready(function () {

    $.validator.unobtrusive.parse("#PersonPropertyForm");
    var appBaseUrl = $("#appBaseUrl").val();

    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });


    $("#btnSavePersonPropertyAjax").click(function (e) {

        e.preventDefault();

        var form = $("#PersonPropertyForm");
        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var serialized_Object = form.serializeObject();
        serialized_Object.PropertyTypeDescription = $("#PropertyTypeDescription").data("kendoComboBox").text();
        formService.isSavingInProgress = true;
    //     toastr.success(formService.messages.saving);

        $.ajax({

            url: appBaseUrl + "PersonProperties/PersonPropertiesSaveAjax",
            type: "POST",

            data: { personPropertyVm: serialized_Object },

            success: function (response) {
                if (response.succeed == false) {
                    toastr.error(response.Message);
                    formService.isSavingInProgress = false;
                }
                else {
                    var _oldpersonPropertyTypeId = $("#hiddenPersonPropertyTypeId").val();
                    refreshPersonPropertyFormData(response.personPropertyVm);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    if (_oldpersonPropertyTypeId != "0") {
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

    $("#btnClearPersonPropertyAjax").click(function (e) {

        e.preventDefault();

        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonPropertyForm");
        $.ajax({

            url: appBaseUrl + "PersonProperties/PersonPropertiesIndexChangedAjax",
            type: "POST",

            data: {

                personPropertyTypeIdDdl: 0,
                personId: personId,
            },

            success: function (response) {
                formService.reset(form);
                refreshPersonPropertyFormData(response);
                if (_propertyFlag == 'CLEAR') {
                    _propertyFlag = '';
                }
                else {
                    toastr.success(formService.messages.cleared);
                }
                return;
            }

        });

    });

    $("#btnDeletePersonPropertyAjax").click(function (e) {
        e.preventDefault();
        confirmDialog("", function () {
            appBaseUrl = $("#appBaseUrl").val();
            var personId = $("#hiddenPersonId").val();
            var personPropertyTypeId = $("#hiddenPersonPropertyTypeId").val();
            $.ajax({
                url: appBaseUrl + "PersonProperties/PersonPropertiesDeleteAjax",
                type: "POST",

                data: {
                    personPropertyTypeIdDdl: personPropertyTypeId,
                    personId: personId

                },

                success: function (response) {

                    if (response == formService.messages.propertyNotExists) {
                        toastr.error(response);
                        return;
                    }
                    refreshPersonPropertyFormData(response);
                    toastr.success(formService.messages.deleted);
                    return;
                }
            });
        });
    });

});
function onSelectPersonPropertyDdl(e) {
    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);

    var ddlSelectedPersonPropertyTypeId = dataItem.PersonPropertyTypeId == "" ? 0 : dataItem.PersonPropertyTypeId;

    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();

    $.ajax({

        url: appBaseUrl + "PersonProperties/PersonPropertiesIndexChangedAjax",

        type: "POST",

        data: {

            personPropertyTypeIdDdl: ddlSelectedPersonPropertyTypeId,
            personId: personId

        },

        success: function (response) {

            refreshPersonPropertyFormData(response);
            return;
        }

    });
}

function onDataBoundPersonPropertyDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noPropertyRecordsFound').hide();
    else
        $('#noPropertyRecordsFound').show();
}

function error_handler(e) {

    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshPersonPropertyFormData(response) {
    if (response) {
    $("#hiddenPersonId").val(response.PersonId);
    $("#hiddenPersonPropertyTypeId").val(response.PersonPropertyTypeId);
    $('#formDiv input[type=radio]').prop('checked', false);
    $('#formDiv input:checkbox').prop('checked', false);

    if (response.PropertyTypeDescription != null)
        $("#PropertyTypeDescription").data("kendoComboBox").value(response.PropertyTypeDescription);
    else
        $("#PropertyTypeDescription").data("kendoComboBox").value("");

    var datepicker = $("#AcquiredDate").data("kendoDatePicker");
    if (response.AcquiredDate != null) {
        var date = new Date(parseInt(response.AcquiredDate.substr(6)));
        $("#AcquiredDate").val(date.toLocaleDateString());
        datepicker.value(date);
    }
    else
        datepicker.value("");

    var datepicker2 = $("#ReleaseDate").data("kendoDatePicker");
    if (response.ReleaseDate != null) {
        var date2 = new Date(parseInt(response.ReleaseDate.substr(6)));
        $("#ReleaseDate").val(date2.toLocaleDateString());
        datepicker2.value(date2);
    }
    else
        datepicker2.value("");

    if (response.EstimatedValue != null)
        $("#EstimatedValue").data("kendoNumericTextBox").value(response.EstimatedValue);
    else
        $("#EstimatedValue").data("kendoNumericTextBox").value("");

    $("#AssetNumber").val(response.AssetNumber == null ? "" : response.AssetNumber);

    $("#PropertyDescription").val(response.PropertyDescription == null ? "" : response.PropertyDescription);

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

    var dropdownlist = $("#PersonPropertyTypeIdDdl").data("kendoDropDownList");
    dropdownlist.refresh();
    dropdownlist.dataSource.read();
    if (response.PersonPropertyTypeId != 0) {
        dropdownlist.value(response.PersonPropertyTypeId);
    }
    else {
        dropdownlist.select(0);
    }

   

    if ($("#hiddenPersonPropertyTypeId").val() > 0) {
        var form = $("#PersonPropertyForm");

        form.validate();
        if (!form.valid())
            toastr.error(formService.messages.invalidMessage);
    }

    appBaseUrl = $("#appBaseUrl").val();
    $.ajax({
        url: appBaseUrl + "PersonProperties/GetPersonPropertiesList",
        type: "POST",
        success: function (response2) {

            $("#PersonPropertyTypeIdDdl").data("kendoDropDownList").value(response.PersonPropertyTypeId);
        }
    });
    $(".validation-summary-errors").empty();
    return;
    } else {
        _propertyFlag = 'CLEAR';
        $("#btnClearPersonPropertyAjax").trigger('click');
    }
}

function gridPersonPropertyEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openPropertyEditpopup(data.PersonPropertyTypeId, data.PersonId);
}
function gridPersonPropertyDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { PersonPropertyTypeId: data.PersonPropertyTypeId };
    var formURL = $("#ApplicationUrl").val() + "PersonProperties/PersonPropertiesDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPersonProperty");
            }
        });
    });
}
function addProperty() {
    openPropertyEditpopup(0, $("#SelectedPersonID").val());
}

function openPropertyEditpopup(_personPropertyId, _personId) {
    var formURL = $("#ApplicationUrl").val() + '/PersonProperties/PersonPropertiesDetail?PersonPropertyTypeId=' + _personPropertyId + '&personId=' + _personId;
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

        $('#gridDdlPropertyType').data('kendoGrid').dataSource.read();
        $('#gridDdlPropertyType').data('kendoGrid').refresh();
    }
}


