var _trainingFlag = '';
$(document).ready(function () {
    $.validator.unobtrusive.parse("#PersonTrainingForm");
    var appBaseUrl = $("#appBaseUrl").val();

    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });

    $("#btnSavePersonTrainingAjax").click(function (e) {
        e.preventDefault();

        var form = $("#PersonTrainingForm");

        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var serialized_Object = form.serializeObject();
        serialized_Object.TrainingCourseDescription = $("#TrainingCourseDescription").data("kendoComboBox").text();
        serialized_Object.QualityRelated = $("#QualityRelated")["0"].checked;
        serialized_Object.OshaRelated = $("#OshaRelated")["0"].checked;

        formService.isSavingInProgress = true;
    //     toastr.success(formService.messages.saving);

        $.ajax({

            url: appBaseUrl + "PersonTrainings/PersonTrainingsSaveAjax",

            type: "POST",

            data: { personTrainingVm: serialized_Object },

            success: function (response) {
                if (response.succeed == false) {
                    formService.isSavingInProgress = false;
                    toastr.error(response.Message);
                }
                else {
                    var _oldpersonTrainingId = $("#hiddenPersonTrainingId").val();
                    refreshPersonTrainingFormData(response.personTrainingVm);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    if (_oldpersonTrainingId != "0") {
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

    $("#btnClearPersonTrainingAjax").click(function (e) {
        e.preventDefault();

        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonTrainingForm");
        $.ajax({

            url: appBaseUrl + "PersonTrainings/PersonTrainingsIndexChangedAjax",

            type: "POST",

            data: {

                personTrainingIdDdl: 0,
                personId: personId,
            },

            success: function (response) {
                formService.reset(form);
                refreshPersonTrainingFormData(response);
                if (_trainingFlag == 'CLEAR') {
                    _trainingFlag = '';
                }
                else {
                    toastr.success(formService.messages.cleared);
                }
                return;
            }
        });
    });

    $("#btnDeletePersonTrainingAjax").click(function (e) {
        e.preventDefault();
        confirmDialog("", function () {
            appBaseUrl = $("#appBaseUrl").val();
            var personId = $("#hiddenPersonId").val();
            var personTrainingId = $("#hiddenPersonTrainingId").val();
            $.ajax({
                url: appBaseUrl + "PersonTrainings/PersonTrainingsDeleteAjax",
                type: "POST",
                data: {
                    personTrainingIdDdl: personTrainingId,
                    personId: personId
                },
                success: function (response) {
                    if (response == formService.messages.trainingNotExists) {
                        toastr.error(response);
                        return;
                    }
                    refreshPersonTrainingFormData(response);
                    toastr.success(formService.messages.deleted);
                    return;
                }
            });
        });
    });

});
function onSelectPersonTrainingDdl(e) {
    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);
    var ddlSelectedPersonTrainingId = dataItem.PersonTrainingId == "" ? 0 : dataItem.PersonTrainingId;
    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();
    $.ajax({
        url: appBaseUrl + "PersonTrainings/PersonTrainingsIndexChangedAjax",
        type: "POST",
        data: {
            personTrainingIdDdl: ddlSelectedPersonTrainingId,
            personId: personId
        },
        success: function (response) {
            refreshPersonTrainingFormData(response);
            return;
        }
    });
}

function onDataBoundPersonTrainingDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noTrainingRecordsFound').hide();
    else
        $('#noTrainingRecordsFound').show();
}

function error_handler(e) {
    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshPersonTrainingFormData(response) {
    if (response) {
        $("#hiddenPersonId").val(response.PersonId);
        $("#hiddenPersonTrainingId").val(response.PersonTrainingId);
        $('#formDiv input[type=radio]').prop('checked', false);
        $('#formDiv input:checkbox').prop('checked', false);
        if (response.TrainingCourseDescription != null)
            $("#TrainingCourseDescription").data("kendoComboBox").value(response.TrainingCourseDescription);
        else
            $("#TrainingCourseDescription").data("kendoComboBox").value("");

        var datepicker = $("#RecommendationDate").data("kendoDatePicker");
        if (response.RecommendationDate != null) {
            var date = new Date(parseInt(response.RecommendationDate.substr(6)));
            $("#RecommendationDate").val(date.toLocaleDateString());
            datepicker.value(date);
        }
        else
            datepicker.value("");

        var datepicker2 = $("#RequiredByDate").data("kendoDatePicker");
        if (response.RequiredByDate != null) {
            var date2 = new Date(parseInt(response.RequiredByDate.substr(6)));
            $("#RequiredByDate").val(date2.toLocaleDateString());
            datepicker2.value(date2);
        }
        else
            datepicker2.value("");

        var datepicker3 = $("#ScheduledDate").data("kendoDatePicker");
        if (response.ScheduledDate != null) {
            var date3 = new Date(parseInt(response.ScheduledDate.substr(6)));
            $("#ScheduledDate").val(date3.toLocaleDateString());
            datepicker3.value(date3);
        }
        else
            datepicker3.value("");

        var datepicker4 = $("#CompletedDate").data("kendoDatePicker");
        if (response.CompletedDate != null) {
            var date4 = new Date(parseInt(response.CompletedDate.substr(6)));
            $("#CompletedDate").val(date4.toLocaleDateString());
            datepicker4.value(date2);
        }
        else
            datepicker4.value("");

        $("#Grade").val(response.Grade == null ? "" : response.Grade);
        $("#Venue").val(response.Venue == null ? "" : response.Venue);

        if (response.HotelMealsExpense != null)
            $("#HotelMealsExpense").data("kendoNumericTextBox").value(response.HotelMealsExpense);
        else
            $("#HotelMealsExpense").data("kendoNumericTextBox").value("");

        if (response.ActualCourseCost != null)
            $("#ActualCourseCost").data("kendoNumericTextBox").value(response.ActualCourseCost);
        else
            $("#ActualCourseCost").data("kendoNumericTextBox").value("");

        if (response.TravelCost != null)
            $("#TravelCost").data("kendoNumericTextBox").value(response.TravelCost);
        else
            $("#TravelCost").data("kendoNumericTextBox").value("");

        $('input[name=QualityRelated]').val([response.QualityRelated]);

        $('input[name=OshaRelated]').val([response.OshaRelated]);

        $("#enteredByLabel").text(response.EnteredBy == null ? "" : response.EnteredBy);

        if (response.EnteredDate != null) {
            var date = new Date(parseInt(response.EnteredDate.substr(6)));
            $("#enteredDateLabel").text(date.toLocaleString());
        }
        else
            $("#enteredDateLabel").text("");

        $("#modifiedByLabel").text(response.ModifiedBy == null ? "" : response.ModifiedBy);

        $("#personNameLabel").html(" - " + response.PersonName);

        $("#Notes").val(response.Notes);

        var dropdownlist = $("#PersonTrainingIdDdl").data("kendoDropDownList");
        dropdownlist.refresh();
        dropdownlist.dataSource.read();
        if (response.PersonTrainingId != 0) {
            dropdownlist.value(response.PersonTrainingId);
        }
        else {
            dropdownlist.select(0);
        }


        if ($("#hiddenPersonTrainingId").val() > 0) {
            var form = $("#PersonTrainingForm");

            form.validate();
            form.validate();
            if (!form.valid())
                toastr.error(formService.messages.invalidMessage);
        }

        appBaseUrl = $("#appBaseUrl").val();
        $.ajax({
            url: appBaseUrl + "PersonTrainings/GetPersonTrainingsList",
            type: "POST",
            success: function (response2) {
                $("#PersonTrainingIdDdl").data("kendoDropDownList").value(response.PersonTrainingId);
            }
        });
        $(".validation-summary-errors").empty();
        return;
    }else {
        _trainingFlag = 'CLEAR';
        $("#btnClearPersonTrainingAjax").trigger('click');
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

        $('#gridDdlTrainingCourse').data('kendoGrid').dataSource.read();
        $('#gridDdlTrainingCourse').data('kendoGrid').refresh();
    }
}


