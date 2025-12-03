
$(document).ready(function () {   
    $.validator.unobtrusive.parse("#PersonSkillForm");
    var appBaseUrl = $("#appBaseUrl").val();

    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });

    $("#btnSavePersonSkillAjax").click(function (e) {
        e.preventDefault();

        var form = $("#PersonSkillForm");
        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var serialized_Object = form.serializeObject();
        serialized_Object.SkillDescription = $("#SkillDescription").data("kendoComboBox").text();
        serialized_Object.SkillLevelDescription = $("#SkillLevelDescription").data("kendoComboBox").text();
        formService.isSavingInProgress = true;
    //     toastr.success(formService.messages.saving);

        $.ajax({

            url: appBaseUrl + "PersonSkills/PersonSkillsSaveAjax",

            type: "POST",

            data: { personSkillVm: serialized_Object },

            success: function (response) {
                if (response.succeed == false) {
                    toastr.error(response.Message);
                    formService.isSavingInProgress = false;
                }
                else {
                    var _oldpersonSkillId = $("#hiddenPersonSkillId").val();
                    refreshPersonSkillFormData(response.personSkillVm);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    if (_oldpersonSkillId != "0") {
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

    $("#btnClearPersonSkillAjax").click(function (e) {
     
        e.preventDefault();

        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonSkillForm");
        $.ajax({

           
            url: appBaseUrl + "PersonSkills/PersonSkillsIndexChangedAjax",

            type: "POST",

            data: {

                personSkillIdDdl: 0,
                personId: personId,
                
            },

            success: function (response) {
                formService.reset(form);
                refreshPersonSkillFormData(response);
                toastr.success(formService.messages.cleared);
                return;
            }

        });

    });

    $("#btnDeletePersonSkillAjax").click(function (e) {
        
        e.preventDefault();
        confirmDialog("", function () {       
        var personId = $("#hiddenPersonId").val();
        var personSkillId = $("#hiddenPersonSkillId").val();

        $.ajax({

            
            url: appBaseUrl + "PersonSkills/PersonSkillsDeleteAjax",

            type: "POST",

            data: {
                personSkillIdDdl: personSkillId,
                personId: personId
             
            },

            success: function (response) {
               
                if (response == formService.messages.skillNotExists) {
                    toastr.error(response);
                    return;
                }
                refreshPersonSkillFormData(response);
                toastr.success(formService.messages.deleted);
                return;
            }
        });
        });
    });

});



function onSelectPersonSkillDdl(e) {

    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);

    var ddlSelectedPersonSkillId = dataItem.PersonSkillId == "" ? 0 : dataItem.PersonSkillId;

    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();

    $.ajax({

        url: appBaseUrl + "PersonSkills/PersonSkillsIndexChangedAjax",

        type: "POST",

        data: {

            personSkillIdDdl: ddlSelectedPersonSkillId,
            personId: personId
        },

        success: function (response) {
            
            refreshPersonSkillFormData(response);
            return;
        }

    });
}

function onDataBoundPersonSkillDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noSkillRecordsFound').hide();
    else
        $('#noSkillRecordsFound').show();
}

function error_handler(e) {   
    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshPersonSkillFormData(response) {
    $("#hiddenPersonId").val(response.PersonId);
    $("#hiddenPersonSkillId").val(response.PersonSkillId);
    $('#formDiv input[type=radio]').prop('checked', false);
    $('#formDiv input:checkbox').prop('checked', false);

    if (response.SkillDescription != null)
        $("#SkillDescription").data("kendoComboBox").value(response.SkillDescription);
    else
        $("#SkillDescription").data("kendoComboBox").value("");

    var datepicker = $("#AttainedDate").data("kendoDatePicker");
    if (response.AttainedDate != null) {
        var date = new Date(parseInt(response.AttainedDate.substr(6)));
        $("#AttainedDate").val(date.toLocaleDateString());
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

    var datepicker3 = $("#LastUsedDate").data("kendoDatePicker");
    if (response.LastUsedDate != null) {
        var date3 = new Date(parseInt(response.LastUsedDate.substr(6)));
        $("#LastUsedDate").val(date3.toLocaleDateString());
        datepicker3.value(date3);
    }
    else
        datepicker3.value("");

    var datepicker4 = $("#EffectiveDate").data("kendoDatePicker");
    if (response.EffectiveDate != null) {
        var date4 = new Date(parseInt(response.EffectiveDate.substr(6)));
        $("#EffectiveDate").val(date4.toLocaleDateString());
        datepicker4.value(date4);
    }
    else
        datepicker4.value("");

    $("#enteredByLabel").text(response.EnteredBy == null ? "" : response.EnteredBy);

    if (response.EnteredDate != null) {
        var date = new Date(parseInt(response.EnteredDate.substr(6)));
        $("#enteredDateLabel").text(date.toLocaleString());
    }
    else
        $("#enteredDateLabel").text("");

  
    $("#personNameLabel").html(" - " + response.PersonName);

    $("#Notes").val(response.Notes);

    var dropdownlist = $("#PersonSkillIdDdl").data("kendoDropDownList");
    dropdownlist.refresh();
    dropdownlist.dataSource.read();
    if (response.PersonSkillId != 0) {
        dropdownlist.value(response.PersonSkillId);
    }
    else {
        dropdownlist.select(0);
    }

    $(".validation-summary-errors").empty();

    if ($("#hiddenPersonSkillId").val() > 0) {
        var form = $("#PersonSkillForm");

        form.validate();
        if (!form.valid())
            toastr.error(formService.messages.invalidMessage);
    }

    appBaseUrl = $("#appBaseUrl").val();
    $.ajax({
        url: appBaseUrl + "PersonSkills/GetPersonSkillsList",
        type: "POST",
        success: function (response2) {           
            $("#PersonSkillIdDdl").data("kendoDropDownList").value(response.PersonSkillId);
        }
    });

    return;
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
        $('#gridDdlSkill').data('kendoGrid').dataSource.read();
        $('#gridDdlSkill').data('kendoGrid').refresh();
    }
}


