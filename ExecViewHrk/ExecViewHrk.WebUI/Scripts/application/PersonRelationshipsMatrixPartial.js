var _relationFlag = '';
$(document).ready(function () {
    $.validator.unobtrusive.parse("#PersonRelationshipForm");
    var appBaseUrl = $("#appBaseUrl").val();

    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });

    $("#btnSavePersonRelationshipAjax").click(function (e) {
        e.preventDefault();
        var form = $("#PersonRelationshipForm");
        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }

        var serialized_Object = form.serializeObject();
        serialized_Object.Dependent = $("#Dependent")["0"].checked;
        serialized_Object.EmergencyContact = $("#EmergencyContact")["0"].checked;
        serialized_Object.Garnishment = $("#Garnishment")["0"].checked;
        serialized_Object.RelationshipDescription = $("#RelationshipDescription").data("kendoComboBox").text();
        serialized_Object.RelationPersonName = $("#RelationPersonName").data("kendoComboBox").text();

        formService.isSavingInProgress = true;
    //     toastr.success(formService.messages.saving);

        var requestType = $("#requestType").val();

        $.ajax({
            url: appBaseUrl + "PersonRelationships/PersonRelationshipsSaveAjax",
            type: "POST",
            data: { personRelationshipVm: serialized_Object },
            success: function (response) {
                if (response.succeed == false) {
                    formService.isSavingInProgress = false;
                    toastr.error(response.Message);
                }
                else {
                    var _oldPersonRelationshipId = $("#hiddenPersonRelationshipId").val();
                    refreshPersonRelationshipFormData(response.personRelationshipVm);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    if (_oldPersonRelationshipId != "0") {
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

    $("#btnClearPersonRelationshipAjax").click(function (e) {

        e.preventDefault();

        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonRelationshipForm");
        $.ajax({
            url: appBaseUrl + "PersonRelationships/PersonRelationshipsIndexChangedAjax",

            type: "POST",

            data: {

                personRelationshipIdDdl: 0,
                personId: personId,
            },

            success: function (response) {
                formService.reset(form);
                refreshPersonRelationshipFormData(response);
                toastr.success(formService.messages.cleared); 
                return;
            }

        });

    });

    $("#btnDeletePersonRelationshipAjax").click(function (e) {
        e.preventDefault();
        confirmDialog("", function () {
            appBaseUrl = $("#appBaseUrl").val();
            var personId = $("#hiddenPersonId").val();
            var personRelationshipId = $("#hiddenPersonRelationshipId").val();

            $.ajax({
                url: appBaseUrl + "PersonRelationships/PersonRelationshipsDeleteAjax",
                type: "POST",
                data: {
                    personRelationshipIdDdl: personRelationshipId,
                    personId: personId
                },
                success: function (response) {
                    if (response == formService.messages.relationshipNotExists) {
                        toastr.error(response);
                        return;
                    }
                    refreshPersonRelationshipFormData(response);
                    toastr.success(formService.messages.deleted);
                    return;
                }
            });
        });
    });

});
function onSelectPersonRelationshipDdl(e) {

    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);

    var ddlSelectedPersonRelationshipId = dataItem.PersonRelationshipId == "" ? 0 : dataItem.PersonRelationshipId;

    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();

    $.ajax({

        url: appBaseUrl + "PersonRelationships/PersonRelationshipsIndexChangedAjax",

        type: "POST",

        data: {

            personRelationshipIdDdl: ddlSelectedPersonRelationshipId,
            personId: personId
        },

        success: function (response) {

            refreshPersonRelationshipFormData(response);
            return;
        }

    });
}

function onDataBoundPersonRelationshipDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noRelationshipRecordsFound').hide();
    else
        $('#noRelationshipRecordsFound').show();
}

function error_handler(e) {

    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshPersonRelationshipFormData(response) {

    $("#hiddenPersonId").val(response.PersonId);
    $("#hiddenPersonRelationshipId").val(response.PersonRelationshipId);
    $('#formDiv input[type=radio]').prop('checked', false);
    $('#formDiv input:checkbox').prop('checked', false);


    if (response.RelationshipDescription != null)
        $("#RelationshipDescription").data("kendoComboBox").value(response.RelationshipDescription);
    else
        $("#RelationshipDescription").data("kendoComboBox").value("");

    if (response.RelationPersonName != null)
        $("#RelationPersonName").data("kendoComboBox").value(response.RelationPersonName);
    else
        $("#RelationPersonName").data("kendoComboBox").value("");

    $('input[name=Dependent]').val([response.Dependent]);

    $('input[name=EmergencyContact]').val([response.EmergencyContact]);

    $('input[name=Garnishment]').val([response.Garnishment]);

    $("#enteredByLabel").text(response.EnteredBy == null ? "" : response.EnteredBy);

    if (response.EnteredDate != null) {
        var date = new Date(parseInt(response.EnteredDate.substr(6)));
        $("#enteredDateLabel").text(date.toLocaleString());
    }
    else
        $("#enteredDateLabel").text("");

    $("#personNameLabel").html(" - " + response.PersonName);

    $("#Notes").val(response.Notes);

    var dropdownlist = $("#PersonRelationshipIdDdl").data("kendoDropDownList");
    dropdownlist.refresh();
    dropdownlist.dataSource.read();
    if (response.PersonRelationshipId != 0) {
        dropdownlist.value(response.PersonRelationshipId);
    }
    else {
        dropdownlist.select(0);
    }

    $(".validation-summary-errors").empty();

    if ($("#hiddenPersonRelationshipId").val() > 0) {
        var form = $("#PersonRelationshipForm");
        form.validate();
        if (!form.valid())
            toastr.error(formService.messages.invalidMessage);
    }

    appBaseUrl = $("#appBaseUrl").val();
    $.ajax({
        url: appBaseUrl + "PersonRelationships/GetPersonRelationshipsList",
        type: "POST",
        success: function (response2) {

            $("#PersonRelationshipIdDdl").data("kendoDropDownList").value(response.PersonRelationshipId);
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

        $('#gridDdlRelationshipType').data('kendoGrid').dataSource.read();
        $('#gridDdlRelationshipType').data('kendoGrid').refresh();
    }
}

function addPersonRelationship() {
    openPersonRelationshipPopup(0, $("#SelectedPersonID").val());
}

function openPersonRelationshipPopup(_personRelationShipID,_personId) {
    loading(true);
    var formURL = $("#ApplicationUrl").val() + '/PersonRelationships/PersonRelationshipsDetail?personRelationShipID=' + _personRelationShipID + '&personId=' + _personId;

    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);

    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');

}
function gridRelationshipEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openPersonRelationshipPopup(data.PersonRelationshipId ,data.PersonId);
}

function gridRelationshipDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { personRelationshipIdDdl: data.PersonRelationshipId, personId: data.PersonId };
    var formURL = $("#ApplicationUrl").val() + "PersonRelationships/PersonRelationshipsDeleteAjax";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPersonRelationship");
            }
        });
    });
}

