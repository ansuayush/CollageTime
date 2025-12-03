
$(document).ready(function () {

    $.validator.unobtrusive.parse("#PersonAdditionalForm");
    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        resetForm();
   });
      
    var appBaseUrl = $("#appBaseUrl").val();
  

    $("#btnSavePersonAdditionalAjax").click(function (e) {
        e.preventDefault();
        var form = $("#PersonAdditionalForm");
        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var serialized_Object = form.serializeObject();
        serialized_Object.NormalRetirementDate = getFormattedDate('NormalRetirementDate');
        serialized_Object.ExpectedRetirementDate = getFormattedDate('ExpectedRetirementDate');
        serialized_Object.EarlyRetirementDate = getFormattedDate('EarlyRetirementDate');
        serialized_Object.DateOfDeath = getFormattedDate('DateOfDeath');
        serialized_Object.CitizenshipDate = getFormattedDate('CitizenshipDate');
        formService.isSavingInProgress = true;
    //     toastr.success(formService.messages.saving);
       
        $.ajax({

            url: appBaseUrl + "PersonAdditionals/PersonAdditionalsSaveAjax",

            type: "POST",
            data: { personAdditionalVm: serialized_Object },

            success: function (response) {
                if (response.succeed == false) {
                    toastr.error(response.Message);
                    formService.isSavingInProgress = false;
                }
                else {
                    refreshPersonAdditionalFormData(response);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    timeOutFn = setTimeout(function () {
                        formService.isSavingInProgress = false;
                        toastr.success(formService.messages.updated);
                        resetForm();
                        formService.isLoadingInProgress = false;
                    }, 2000);
                }

            },
            error: function(){
                formService.isSavingInProgress = false;
            }
        })
    });

    $("#btnClearPersonAdditionalAjax").click(function (e) {
        e.preventDefault();

        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();

        $.ajax({

            url: appBaseUrl + "PersonAdditionals/PersonAdditionalsIndexChangedAjax",

            type: "POST",

            data: {

                personAdditionalIdDdl: 0,
                personId: personId,
            },

            success: function (response) {
                refreshPersonAdditionalFormData(response);
                toastr.success(formService.messages.cleared);
                return;
            }

        });

    });

    $("#btnDeletePersonAdditionalAjax").click(function (e) {
        e.preventDefault();
        var personId = $("#hiddenPersonId").val();
        var personAdditionalId = $("#hiddenPersonAdditionalId").val();

        $.ajax({
            url: appBaseUrl + "PersonaAdditionals/PersonAdditionalsDeleteAjax",

            type: "POST",

            data: {
                personAdditionalIdDdl: personAdditionalId,
                personId: personId
            },

            success: function (response) {

                if (response == formService.messages.additionalNotExists) {
                    toastr.error(response);
                    return;
                }

                refreshPersonAdditionalFormData(response);
                toastr.success(formService.messages.deleted);
                return;
            }
        });
    });

   

});

function onSelectPersonAdditionalDdl(e) {

    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);

    var ddlSelectedPersonAdditionalId = dataItem.PersonAdditionalId == "" ? 0 : dataItem.PersonAdditionalId;

    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();

    $.ajax({

        url: appBaseUrl + "PersonAdditionals/PersonAdditionalsIndexChangedAjax",

        type: "POST",

        data: {

            personAdditionalIdDdl: ddlSelectedPersonAdditionalId,
            personId: personId
        },

        success: function (response) {
            refreshPersonAdditionalFormData(response);
            return;
        }

    });
}

function onDataBoundPersonAdditionalDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noAdditionalRecordsFound').hide();
    else
        $('#noAdditionalRecordsFound').show();
}

function error_handler(e) {

    if (e.errors) {
        toastr.error(e.errors);
    }
}

function resetForm() {
    refreshKenoComboBox('CitizenshipDescription');
    refreshKenoComboBox('ApplicantDescription');
    refreshKenoComboBox('HospitalDescription');
    refreshKenoComboBox('EeoDescription');
    formService.initialize();
}

function refreshPersonAdditionalFormData(response) {

    //Todo    formService.clear(); This method is able to clean the available filds.
  
    $("#hiddenPersonId").val(response.PersonId);
    $("#hiddenPersonAdditionalId").val(response.PersonAdditionalId);
    //$("#formDiv input:text").val("");
    $('#formDiv input[type=radio]').prop('checked', false);
    $('#formDiv input:checkbox').prop('checked', false);

 
    if (response.EeoDescription != null)
        $("#EeoDescription").data("kendoComboBox").value(response.EeoDescription);
    else
        $("#EeoDescription").data("kendoComboBox").value("");

    if (response.CitizenshipDescription != null)
        $("#CitizenshipDescription").data("kendoComboBox").value(response.CitizenshipDescription);
    else
        $("#CitizenshipDescription").data("kendoComboBox").value("");

    if (response.HospitalDescription != null)
        $("#HospitalDescription").data("kendoComboBox").value(response.HospitalDescription);
    else
        $("#HospitalDescription").data("kendoComboBox").value("");

    if (response.ApplicantDescription != null)
        $("#ApplicantDescription").data("kendoComboBox").value(response.ApplicantDescription);
    else
        $("#ApplicantDescription").data("kendoComboBox").value("");

    var datepicker = $("#EarlyRetirementDate").data("kendoDatePicker");
    if (response.EarlyRetirementDate != null) {
        var date = new Date(parseInt(response.EarlyRetirementDate.substr(6)));
        $("#EarlyRetirementDate").val(date.toLocaleDateString());
        datepicker.value(date);
    }
    else
        datepicker.value("");

    var datepicker2 = $("#NormalRetirementDate").data("kendoDatePicker");
    if (response.NormalRetirementDate != null) {
        var date2 = new Date(parseInt(response.NormalRetirementDate.substr(6)));
        $("#NormalRetirementDate").val(date2.toLocaleDateString());
        datepicker2.value(date2);
    }
    else
        datepicker2.value("");

    var datepicker3 = $("#ExpectedRetirementDate").data("kendoDatePicker");
    if (response.ExpectedRetirementDate != null) {
        var date3 = new Date(parseInt(response.ExpectedRetirementDate.substr(6)));
        $("#ExpectedRetirementDate").val(date3.toLocaleDateString());
        datepicker3.value(date3);
    }
    else
        datepicker3.value("");

    var datepicker4 = $("#DateOfDeath").data("kendoDatePicker");
    if (response.DateOfDeath != null) {
        var date4 = new Date(parseInt(response.DateOfDeath.substr(6)));
        $("#DateOfDeath").val(date4.toLocaleDateString());
        datepicker4.value(date4);
    }
    else
        datepicker4.value("");

    var datepicker5 =  $("#CitizenshipDate").data("kendoDatePicker");
    if (response.CitizenshipDate != null) {
        var date5 = new Date(parseInt(response.CitizenshipDate.substr(6)));
        $("#CitizenshipDate").val(date5.toLocaleDateString());
        datepicker5.value(date5);
    }
    else
        datepicker5.value("");

    $("#BirthPlace").val(response.BirthPlace == null ? "" : response.BirthPlace);

    $("#Veteran").val(response.Veteran == null ? "" : response.Veteran);

    $("#DisabledComments").val(response.DisabledComments == null ? "" : response.DisabledComments);

    $("#Doctor").val(response.Doctor == null ? "" : response.Doctor);

    $('input[name=Disabled]').val([response.Disabled] == null ? "" : [response.Disabled]);
    $('input[name=Vietnam]').val([response.Vietnam] == null ? "" : [response.Vietnam]);
    $('input[name=SpecialDisabled]').val([response.SpecialDisabled] == null ? "" : [response.SpecialDisabled]);
    $('input[name=Other]').val([response.Other] == null ? "" : [response.Other]);


    $('input[name=BloodDonor]').val([response.BloodDonor] == null ? "" : [response.BloodDonor]);

    $('input[name=Smoker]').val([response.Smoker] == null ? "" : [response.Smoker]);

    $('input[name=AdvancedDirective]').val([response.AdvancedDirective] == null ? "" : [response.AdvancedDirective]);

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

    var dropdownlist = $("#PersonAdditionalIdDdl").data("kendoDropDownList");
    if (dropdownlist) {
        dropdownlist.refresh();
        dropdownlist.dataSource.read();
        if (response.PersonAdditionalId != 0) {
            dropdownlist.value(response.PersonAdditionalId);
        } else {
            dropdownlist.select(0);
        }
    }
    
    
    $(".validation-summary-errors").empty();

    if ($("#hiddenPersonAdditionalId").val() > 0) {
        var form = $("#PersonAdditionalForm");

        form.validate();
        if (!form.valid())
            toastr.error(formService.messages.invalidMessage);
    }
    initializeForm();

    return;
}




