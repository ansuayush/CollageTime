var _phoneFlag = '';
$(document).ready(function () {
    
    $.validator.unobtrusive.parse("#PersonPhoneNumberForm");
    var appBaseUrl = $("#appBaseUrl").val();

    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });
    $("#btnSavePersonPhoneNumberAjax").click(function (e) {
        e.preventDefault();
        var form = $("#PersonPhoneNumberForm");
        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var serialized_Object = form.serializeObject();
        serialized_Object.PhoneTypeDescription = $("#PhoneTypeDescription").data("kendoComboBox").text();

        formService.isSavingInProgress = true;
    //     toastr.success(formService.messages.saving);
        $.ajax({

            url: appBaseUrl + "PersonPhoneNumbers/PersonPhoneNumbersSaveAjax",
            type: "POST",

            data: { personPhoneNumberVm: serialized_Object },

            success: function (response) {
                if (response.succeed == false) {
                    formService.isSavingInProgress = false;
                    toastr.error(response.Message);
                }
                else {
                    var _oldpersonPhoneNumberId = $("#hiddenPersonPhoneNumberId").val();                    
                    refreshPersonPhoneNumberFormData(response.personPhoneNumberVm);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    if (_oldpersonPhoneNumberId != "0") {
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

    $("#btnClearPersonPhoneNumberAjax").click(function (e) {

        e.preventDefault();

        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonPhoneNumberForm");
        $.ajax({

            url: appBaseUrl + "PersonPhoneNumbers/PersonPhoneNumbersIndexChangedAjax",

            type: "POST",

            data: {

                personPhoneNumberIdDdl: 0,
                personId: personId,
            },

            success: function (response) {
                formService.reset(form);
                refreshPersonPhoneNumberFormData(response);
                if (_phoneFlag == 'CLEAR') {
                    _phoneFlag = '';
                    //Custom Logic
                } else {
                    toastr.success(formService.messages.cleared); 
                }
                return;
            }

        });

    });

    $("#btnDeletePersonPhoneNumberAjax").click(function (e) {

        e.preventDefault();     
        confirmDialog("", function () {
    appBaseUrl = $("#appBaseUrl").val();
        var personId = $("#hiddenPersonId").val();
        var personPhoneNumberId = $("#hiddenPersonPhoneNumberId").val();

        $.ajax({

            url: appBaseUrl + "PersonPhoneNumbers/PersonPhoneNumbersDeleteAjax",

            type: "POST",

            data: {
                personPhoneNumberIdDdl: personPhoneNumberId,
                personId: personId
            },

            success: function (response) {
                if (response == formService.messages.phoneNumberNotExists) {
                    toastr.error(response);
                    return;
                }
                refreshPersonPhoneNumberFormData(response);
                toastr.success(formService.messages.deleted);
                return;
            }
        });
        });
    });

});

function onSelectPersonPhoneNumberDdl(e) {
   
    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);

    var ddlSelectedPersonPhoneNumberId = dataItem.PersonPhoneNumberId == "" ? 0 : dataItem.PersonPhoneNumberId;

    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();

    $.ajax({

        url: appBaseUrl + "PersonPhoneNumbers/PersonPhoneNumbersIndexChangedAjax",

        type: "POST",

        data: {

            personPhoneNumberIdDdl: ddlSelectedPersonPhoneNumberId,
            personId: personId
           
        },

        success: function (response) {
         
            refreshPersonPhoneNumberFormData(response);
            return;
        }

    });
}

function onDataBoundPersonPhoneNumberDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noPhoneNumberRecordsFound').hide();
    else
        $('#noPhoneNumberRecordsFound').show();
}

function error_handler(e) {
   
    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshPersonPhoneNumberFormData(response) {
    if (response) {
        $("#hiddenPersonId").val(response.PersonId);
        $("#hiddenPersonPhoneNumberId").val(response.PersonPhoneNumberId);
        $('#formDiv input[type=radio]').prop('checked', false);
        $('#formDiv input:checkbox').prop('checked', false);

        if (response.PhoneTypeDescription != null)
            $("#PhoneTypeDescription").data("kendoComboBox").value(response.PhoneTypeDescription);
        else
            $("#PhoneTypeDescription").data("kendoComboBox").value("");

        $("#PhoneNumber").val(response.PhoneNumber == null ? "" : response.PhoneNumber);
        $("#Extension").val(response.Extension == null ? "" : response.Extension);

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

        var dropdownlist = $("#PersonPhoneNumberIdDdl").data("kendoDropDownList");
        dropdownlist.refresh();
        dropdownlist.dataSource.read();
        if (response.PersonPhoneNumberId != 0) {
            dropdownlist.value(response.PersonPhoneNumberId);
        }
        else {
            dropdownlist.select(0);
        }

        $(".validation-summary-errors").empty();

        if ($("#hiddenPersonPhoneNumberId").val() > 0) {
            var form = $("#PersonPhoneNumberForm");

            form.validate();
            if (!form.valid())
                toastr.error(formService.messages.invalidMessage);
        }

        appBaseUrl = $("#appBaseUrl").val();
        $.ajax({
            url: appBaseUrl + "PersonPhoneNumbers/GetPersonPhoneNumbersList",
            type: "POST",
            success: function (response2) {

                $("#PersonPhoneNumberIdDdl").data("kendoDropDownList").value(response.PersonPhoneNumberId);
            }
        });
        $(".validation-summary-errors").empty();
        return;
    }
    else {
        _phoneFlag = 'CLEAR';
        $("#btnClearPersonPhoneNumberAjax").trigger('click');
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

        $('#gridDdlMedicalPhoneNumberType').data('kendoGrid').dataSource.read();
        $('#gridDdlMedicalPhoneNumberType').data('kendoGrid').refresh();
    }
}


function onClickPhoneList(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openEditpopup(data.PersonPhoneNumberId);
}
function openEditpopup(_PersonPhoneNumberId) {
    var formURL = $("#ApplicationUrl").val() + '/PersonPhoneNumbers/PersonPhoneNumbersDetail?personPhoneNumberId=' + _PersonPhoneNumberId;
    $("#PhoneDetailAddModal .modal-content").html("");
    $("#PhoneDetailAddModal .modal-content").load(formURL);
    $("#PhoneDetailAddModal").modal({ show: true });

}
function addPersonPhoneNumber() {
    openEditpopup(0);
}
function onClickDeleteList(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { personPhoneNumberIdDdl: data.PersonPhoneNumberId };
    var formURL = $("#ApplicationUrl").val() + "PersonPhoneNumbers/PersonPhoneNumbersDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPhoneNumberList");
                LoadMenu("Person", "Person")
            }
        });
    });
}