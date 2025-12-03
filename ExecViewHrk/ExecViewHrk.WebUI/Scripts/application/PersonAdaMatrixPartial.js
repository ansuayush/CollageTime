
$(document).ready(function () {    

    $.validator.unobtrusive.parse("#PersonAdaForm");

    var appBaseUrl = $("#appBaseUrl").val();
    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });
    $("#btnSavePersonAdaAjax").click(function (e) {
        e.preventDefault();
        var form = $("#PersonAdaForm");
        form.validate();
        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var serialized_Object = form.serializeObject();
        // serialized_Object.AccommodationProvided = $('input[name=AccommodationProvided]').val(); // for some reason checkboxes are always set to false
        serialized_Object.AccommodationDescription = $("#AccommodationDescription").data("kendoComboBox").text();
        serialized_Object.AssociatedDisabilityDescription = $("#AssociatedDisabilityDescription").data("kendoComboBox").text();
        var isOk = formService.CompareDates("RequestedDate", "ProvidedDate", 'Provided Date', 'Requested Date');
        if (!isOk) { return };
        formService.isSavingInProgress = true;
    //     toastr.success(formService.messages.saving);
        $.ajax({
            url: appBaseUrl + "PersonAda/PersonAdaSaveAjax",
            type: "POST",
            data: { personAdaVm: serialized_Object },
            success: function (response) {
                if (response.succeed == false)
                    toastr.error(response.Message);
                else
                {
                     var _oldpersonAdaId = $("#hiddenPersonAdaId").val();
                     refreshPersonAdaFormData(response.personAdaVm);
                     formService.isLoadingInProgress = true;
                     killTimer();
                     if (_oldpersonAdaId != "0") {
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
                return; 
            },
            error: function () {
                formService.isSavingInProgress = false;
            }
        });
    });

    $("#btnClearPersonAdaAjax").click(function (e) {
        e.preventDefault();
        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonAdaForm");
        $.ajax({
            url: appBaseUrl+"PersonAda/PersonAdaIndexChangedAjax",
            type: "POST",
            data: {
                personAdaIdDdl: 0,
                personId: personId
            },
            success: function (response) {
                formService.reset(form);
                refreshPersonAdaFormData(response);
                toastr.success(formService.messages.cleared);
                return;
            }

        });

    });

    $("#btnDeletePersonAdaAjax").click(function (e) {
        e.preventDefault();
        var personAdaId = $("#hiddenPersonAdaId").val();
        if (personAdaId == "0") {
            formService.messages.adaNotExists
            return;
        }
        else {
            confirmDialog("", function () {
                var personId = $("#hiddenPersonId").val();
                var personAdaId = $("#hiddenPersonAdaId").val();

                $.ajax({

                    url: appBaseUrl + "PersonAda/PersonAdaDeleteAjax",

                    type: "POST",

                    data: {
                        personAdaIdDdl: personAdaId,
                        personId: personId
                    },

                    success: function (response) {
                        if (response == formService.messages.adaNotExists) {
                            toastr.error(response);
                            return;
                        }
                        refreshPersonAdaFormData(response);
                        toastr.success(formService.messages.deleted);
                        return;
                    }
                });
            });
        }
    });

 });


function onSelectPersonAdaDdl(e) {
    appBaseUrl = $("#appBaseUrl").val(); // dpr - added 5/27/2016; 
    var dataItem = this.dataItem(e.item.index() + 1);

    var ddlSelectedPersonAdaId = dataItem.PersonAdaId == "" ? 0 : dataItem.PersonAdaId;

    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();

    $.ajax({

        url: appBaseUrl + "PersonAda/PersonAdaIndexChangedAjax",

        type: "POST",

        data: {
            personAdaIdDdl: ddlSelectedPersonAdaId,
            personId: personId
        },

        success: function (response) {
            refreshPersonAdaFormData(response);
            return;
        }

    });
}

function onDataBoundPersonAdaDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noAdaRecordsFound').hide();
    else
        $('#noAdaRecordsFound').show();
}

function error_handler(e) {
    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshPersonAdaFormData(response) {
    $("#hiddenPersonId").val(response.PersonId);
    $("#hiddenPersonAdaId").val(response.PersonAdaId);
    //$("#formDiv input:text").val("");
    $('#formDiv input[type=radio]').prop('checked', false);
    $('#formDiv input:checkbox').prop('checked', false);


    if (response.AccommodationDescription != null)
        //$("#AccommodationDescription").data("kendoComboBox").value(response.AccommodationTypeId);
        $("#AccommodationDescription").data("kendoComboBox").value(response.AccommodationDescription);
    else
        $("#AccommodationDescription").data("kendoComboBox").value(""); // dpr - added 5/27/2016; 

    if (response.AssociatedDisabilityDescription != null)
        $("#AssociatedDisabilityDescription").data("kendoComboBox").value(response.AssociatedDisabilityDescription);
    else
        $("#AssociatedDisabilityDescription").data("kendoComboBox").value(""); // dpr - added 5/27/2016; 

    // dpr - 5/27/2016 modified
    var datepicker = $("#RequestedDate").data("kendoDatePicker");
    if (response.RequestedDate != null) {
        var date = new Date(parseInt(response.RequestedDate.substr(6)));
        $("#RequestedDate").val(date.toLocaleDateString());
        datepicker.value(date);
    }
    else
        datepicker.value("");


    // dpr - 5/27/2016 modified
    var datepicker2 = $("#ProvidedDate").data("kendoDatePicker");
    if (response.ProvidedDate != null) {
        var date2 = new Date(parseInt(response.ProvidedDate.substr(6)));
        $("#ProvidedDate").val(date2.toLocaleDateString());
        datepicker2.value(date2);
    }
    else
        datepicker2.value("");


    if (response.EstimatedCost != null)
        $("#EstimatedCost").data("kendoNumericTextBox").value(response.EstimatedCost);
    else
        $("#EstimatedCost").data("kendoNumericTextBox").value("");
    if (response.ActualCost != null)
        $("#ActualCost").data("kendoNumericTextBox").value(response.ActualCost);
    else
        $("#ActualCost").data("kendoNumericTextBox").value("");
    $('input[name=AccommodationProvided]').val([response.AccommodationProvided]);

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
    else {
        $("#modifiedDateLabel").text("");
    }
    $("#personNameLabel").html(" - " + response.PersonName);

    $("#Notes").val(response.Notes);

    var dropdownlist = $("#PersonAdaIdDdl").data("kendoDropDownList");
    dropdownlist.refresh();
    dropdownlist.dataSource.read();
    if (response.PersonAdaId != 0) {
        dropdownlist.value(response.PersonAdaId);
    }
    else {
        dropdownlist.select(0);
    }

    $(".validation-summary-errors").empty();

    if ($("#hiddenPersonAdaId").val() > 0) {
        var form = $("#PersonAdaForm");
        form.validate();
        if (!form.valid())
            toastr.error(formService.messages.invalidMessage);
    }



    // dpr - added 5/27/2016
    appBaseUrl = $("#appBaseUrl").val(); 
    $.ajax({
        url: appBaseUrl + "PersonAda/GetPersonAdaList",
        type: "POST",
        success: function (response2) {
           
            $("#PersonAdaIdDdl").data("kendoDropDownList").value(response.PersonAdaId);
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

        $('#gridDdlAccommodationType').data('kendoGrid').dataSource.read();
        $('#gridDdlAccommodationType').data('kendoGrid').refresh();
    }

}



