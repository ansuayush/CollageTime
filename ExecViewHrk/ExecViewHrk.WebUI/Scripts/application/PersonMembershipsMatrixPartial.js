var _memberFlag = '';
$(document).ready(function () {

    $.validator.unobtrusive.parse("#PersonMembershipForm");
    var appBaseUrl = $("#appBaseUrl").val();
    formService.isLoadingInProgress = true;

    $(document).ajaxStop(function () {
        formService.initialize();
    });

    $("#btnSavePersonMembershipAjax").click(function (e) {
        e.preventDefault();
        var form = $("#PersonMembershipForm");

        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var serialized_Object = form.serializeObject();
        serialized_Object.ProfessionalBodyDescription = $("#ProfessionalBodyDescription").data("kendoComboBox").text();
        serialized_Object.RegionalChapterDescription = $("#RegionalChapterDescription").data("kendoComboBox").text();

        formService.isSavingInProgress = true;
    //     toastr.success(formService.messages.saving);

        $.ajax({
            url: appBaseUrl + "PersonMemberships/PersonMembershipsSaveAjax",
            type: "POST",
            data: { personMembershipVm: serialized_Object },

            success: function (response) {
                if (response.succeed == false) {
                    toastr.error(response.Message);
                    formService.isSavingInProgress = false;
                }
                else {
                    var _oldpersonMembershipId = $("#hiddenPersonMembershipId").val();
                    refreshPersonMembershipFormData(response.personMembershipVm);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    if (_oldpersonMembershipId != "0") {
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


    $("#btnClearPersonMembershipAjax").click(function (e) {
        e.preventDefault();
        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonMembershipForm");
        $.ajax({

            url: appBaseUrl + "PersonMemberships/PersonMembershipsIndexChangedAjax",
            type: "POST",
            data: {
                personMembershipIdDdl: 0,
                personId: personId,
            },

            success: function (response) {
                formService.reset(form);
                refreshPersonMembershipFormData(response);
                if (_memberFlag == 'CLEAR') {
                    _memberFlag = '';
                }
                else {
                    toastr.success(formService.messages.cleared);
                }
                return;
            }
        });
    });

    $("#btnDeletePersonMembershipAjax").click(function (e) {
        e.preventDefault();
        confirmDialog("", function () {
            appBaseUrl = $("#appBaseUrl").val();
            var personId = $("#hiddenPersonId").val();
            var personMembershipId = $("#hiddenPersonMembershipId").val();
            $.ajax({
                url: appBaseUrl + "PersonMemberships/PersonMembershipsDeleteAjax",
                type: "POST",
                data: {
                    personMembershipIdDdl: personMembershipId,
                    personId: personId
                },
                success: function (response) {

                    if (response == formService.messages.membershipNotExists) {
                        toastr.error(response);
                        return;
                    }
                    refreshPersonMembershipFormData(response);
                    toastr.success(formService.messages.deleted);
                    return;
                }
            });
        });
    });
});
function onSelectPersonMembershipDdl(e) {
    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);
    var ddlSelectedPersonMembershipId = dataItem.PersonMembershipId == "" ? 0 : dataItem.PersonMembershipId;
    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();
    $.ajax({

        url: appBaseUrl + "PersonMemberships/PersonMembershipsIndexChangedAjax",

        type: "POST",
        data: {
            personMembershipIdDdl: ddlSelectedPersonMembershipId,
            personId: personId
        },

        success: function (response) {

            refreshPersonMembershipFormData(response);
            return;
        }
    });
}

function onDataBoundPersonMembershipDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noMembershipRecordsFound').hide();
    else
        $('#noMembershipRecordsFound').show();
}


function error_handler(e) {
    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshPersonMembershipFormData(response) {
    if (response) {
        $("#hiddenPersonId").val(response.PersonId);
        $("#hiddenPersonMembershipId").val(response.PersonMembershipId);
        $('#formDiv input[type=radio]').prop('checked', false);
        $('#formDiv input:checkbox').prop('checked', false);

        if (response.ProfessionalBodyDescription != null)
            $("#ProfessionalBodyDescription").data("kendoComboBox").value(response.ProfessionalBodyDescription);
        else
            $("#ProfessionalBodyDescription").data("kendoComboBox").value("");

        if (response.RegionalChapterDescription != null)
            $("#RegionalChapterDescription").data("kendoComboBox").value(response.RegionalChapterDescription);
        else
            $("#RegionalChapterDescription").data("kendoComboBox").value("");

        var datepicker = $("#StartDate").data("kendoDatePicker");
        if (response.StartDate != null) {
            var date = new Date(parseInt(response.StartDate.substr(6)));
            $("#RequestedDate").val(date.toLocaleDateString());
            datepicker.value(date);
        }
        else
            datepicker.value("");

        var datepicker2 = $("#RenewalDate").data("kendoDatePicker");
        if (response.RenewalDate != null) {
            var date2 = new Date(parseInt(response.RenewalDate.substr(6)));
            $("#RenewalDate").val(date2.toLocaleDateString());
            datepicker2.value(date2);
        }
        else
            datepicker2.value("");

        $("#Number").val(response.Number == null ? "" : response.Number);

        if (response.Fee != null)
            $("#Fee").data("kendoNumericTextBox").value(response.Fee);
        else
            $("#Fee").data("kendoNumericTextBox").value("");

        var datepicker3 = $("#FeePaidDate").data("kendoDatePicker");
        if (response.FeePaidDate != null) {
            var date3 = new Date(parseInt(response.FeePaidDate.substr(6)));
            $("#FeePaidDate").val(date3.toLocaleDateString());
            datepicker3.value(date3);
        }
        else
            datepicker3.value("");

        $("#ProfessionalTitle").val(response.ProfessionalTitle == null ? "" : response.ProfessionalTitle);

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

        var dropdownlist = $("#PersonMembershipIdDdl").data("kendoDropDownList");
        dropdownlist.refresh();
        dropdownlist.dataSource.read();
        if (response.PersonMembershipId != 0) {
            dropdownlist.value(response.PersonMembershipId);
        }
        else {
            dropdownlist.select(0);
        }

        if ($("#hiddenPersonMembershipId").val() > 0) {
            var form = $("#PersonMembershipForm");
            form.validate();
            if (!form.valid())
                toastr.error(formService.messages.invalidMessage);
        }

        appBaseUrl = $("#appBaseUrl").val();
        $.ajax({
            url: appBaseUrl + "PersonMemberships/GetPersonMembershipsList",
            type: "POST",
            success: function (response2) {
                $("#PersonMembershipIdDdl").data("kendoDropDownList").value(response.PersonMembershipId);
            }
        });
        $(".validation-summary-errors").empty();
        return;

    } else {
        _memberFlag = 'CLEAR';
        $("#btnClearPersonMembershipAjax").trigger('click');
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

        $('#gridDdlAccommodationType').data('kendoGrid').dataSource.read();
        $('#gridDdlAccommodationType').data('kendoGrid').refresh();
    }
}
function gridPersonMembershipEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openMembershipEditpopup(data.PersonMembershipId, data.PersonId);
}

function gridPersonMembershipDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { personMembershipId: data.PersonMembershipId };
    var formURL = $("#ApplicationUrl").val() + "PersonMemberships/PersonMembershipsDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPersonMembership");
            }
        });
    });
}

function addPersonMembership() {
    openMembershipEditpopup(0, $("#SelectedPersonID").val());
}

function openMembershipEditpopup(_personMembershipId, _personId) {
    var formURL = $("#ApplicationUrl").val() + '/PersonMemberships/PersonMembershipsDetail?personMembershipId=' + _personMembershipId + '&personId=' + _personId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}