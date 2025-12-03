var _employeeFlag = '';
$(document).ready(function () {
    $.validator.unobtrusive.parse("#PersonEmployeeForm");
    var appBaseUrl = $("#appBaseUrl").val();

    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });

    $("#btnSavePersonEmployeeAjax").click(function (e) {

        e.preventDefault();
        var form = $("#PersonEmployeeForm");

        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }

        var newvalue = $("#FedExemptions").val();
        if (isNaN(newvalue) || newvalue.length < 1 || newvalue.length > 2) {
            toastr.error("Federal Exemptions is not in the correct format. Must be between 0-99");
            return false;
        }

        var serialized_Object = form.serializeObject();
        serialized_Object.EmployeeTypeDescription = $("#EmployeeTypeDescription").data("kendoComboBox").text();
        serialized_Object.EmploymentStatusDescription = $("#EmploymentStatusDescription").data("kendoComboBox").text();
        serialized_Object.PayFrequencyDescription = $("#PayFrequencyDescription").data("kendoComboBox").text();
        serialized_Object.MaritalStatusDescription = $("#MaritalStatusDescription").data("kendoComboBox").text();
        serialized_Object.WorkedStateTitle = $("#WorkedStateTitle").data("kendoComboBox").text();
        serialized_Object.RateTypeDescription = $("#RateTypeDescription").data("kendoComboBox").text();
        serialized_Object.FedExemptions = $("#FedExemptions").val();

        formService.isSavingInProgress = true;
        toastr.success(formService.messages.saving);
        $.ajax({

            url: appBaseUrl + "PersonEmployees/PersonEmployeesSaveAjax",
            type: "POST",
            data: { personEmployeeVm: serialized_Object },

            success: function (response) {
                if (response.succeed == false) {
                    formService.isSavingInProgress = false;
                    toastr.error(response.Message);
                }
                else {
                    var _oldemployeeid = $("#hiddenPersonEmployeeId").val();
                    refreshPersonEmployeeFormData(response.personEmployeeVm);
                    formService.isLoadingInProgress = true;
                    killTimer();
                    if (_oldemployeeid != "0") {
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

    $("#btnClearPersonEmployeeAjax").click(function (e) {
        e.preventDefault();

        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var form = $("#PersonEmployeeForm");
        $.ajax({
            url: appBaseUrl + "PersonEmployees/PersonEmployeesIndexChangedAjax",

            type: "POST",
            data: {
                personEmployeeIdDdl: 0,
                personId: personId,
            },
            success: function (response) {
                formService.reset(form);
                refreshPersonEmployeeFormData(response);
                if (_employeeFlag == 'CLEAR') {
                    _employeeFlag = '';
                }
                else {
                    toastr.success(formService.messages.cleared);
                }
                return;
            }
        });
    });

    $("#btnDeletePersonEmployeeAjax").click(function (e) {
        e.preventDefault();
        confirmDialog("", function () {
            appBaseUrl = $("#appBaseUrl").val();
            var personId = $("#hiddenPersonId").val();
            var personEmployeeId = $("#hiddenPersonEmployeeId").val();
            $.ajax({
                url: appBaseUrl + "PersonEmployees/PersonEmployeesDeleteAjax",
                type: "POST",
                data: {
                    personEmployeeIdDdl: personEmployeeId,
                    personId: personId
                },
                success: function (response) {
                    if (response.succeed == false) {
                        toastr.error(response.Message);
                        return;
                    }
                    refreshPersonEmployeeFormData(response.personEmployeeVm);
                    toastr.success(formService.messages.deleted);
                    return;
                }
            });
        });
    });
});
function onSelectPersonEmployeeDdl(e) {
    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);
    var ddlSelectedPersonEmployeeId = dataItem.PersonEmployeeId == "" ? 0 : dataItem.PersonEmployeeId;
    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();
    $.ajax({
        url: appBaseUrl + "PersonEmployees/PersonEmployeesDeleteAjax",
        type: "POST",
        data: {
            personEmployeeIdDdl: ddlSelectedPersonEmployeeId,
            personId: personId
        },
        success: function (response) {
            refreshPersonEmployeeFormData(response);
            return;
        }
    });
}
function gridPersonTestDelete1(e) {
    alert();
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { personTestId: data.PersonTestId };
    var formURL = $("#ApplicationUrl").val() + "PersonEmployees/PersonEmployeesDeleteAjax";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPersonTest");
            }
        });
    });
}

function gridPersonEmployeeMatrixEdit(e) {   
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openPersonEmployeeEditPopup(data.EmployeeId, data.PersonId);
}
function addPersonEmployee() {
    openPersonEmployeeEditPopup(0,0);
}

function openPersonEmployeeEditPopup(_employeeId, _personId) {   
    loading(true);
    var formURL = $("#ApplicationUrl").val() + '/PersonEmployees/PersonEmployees?employeeId=' + _employeeId + '&personId=' + _personId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}
function addPersonEmployees() {
    openPersonEmployeesPopup(0, 0);
}
function openPersonEmployeesPopup(_employeeId, _personId) {
    loading(true);
    var formURL = $("#ApplicationUrl").val() + '/PersonEmployees/PersonEmployeesList?employeeId=' + _employeeId + '&personId=' + _personId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}

function onDataBoundPersonEmployeeDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noEmployeeRecordsFound').hide();
    else
        $('#noEmployeeRecordsFound').show();
}

function refreshPersonEmployeeFormData(response) {
    if (response) {
        $("#hiddenPersonId").val(response.PersonId);
        $("#hiddenPersonEmployeeId").val(response.EmployeeId);
        $('#formDiv input[type=radio]').prop('checked', false);
        $('#formDiv input:checkbox').prop('checked', false);

        if (response.EmploymentStatusDescription != null)
            $("#EmploymentStatusDescription").data("kendoComboBox").value(response.EmploymentStatusDescription);
        else
            $("#EmploymentStatusDescription").data("kendoComboBox").value("");

        if (response.EmployeeTypeDescription != null)
            $("#EmployeeTypeDescription").data("kendoComboBox").value(response.EmployeeTypeDescription);
        else
            $("#EmployeeTypeDescription").data("kendoComboBox").value("");

        if (response.PayFrequencyDescription != null)
            $("#PayFrequencyDescription").data("kendoComboBox").value(response.PayFrequencyDescription);
        else
            $("#PayFrequencyDescription").data("kendoComboBox").value("");

        if (response.MaritalStatusDescription != null)
            $("#MaritalStatusDescription").data("kendoComboBox").value(response.MaritalStatusDescription);
        else
            $("#MaritalStatusDescription").data("kendoComboBox").value("");
        if (response.LenghtOfEmployment != null)
            $("#LenghtOfEmployment").val(response.LenghtOfEmployment);
        else
            $("#LenghtOfEmployment").val("");
        if (response.WorkedStateTitle != null)
            $("#WorkedStateTitle").data("kendoComboBox").value(response.WorkedStateTitle);
        else
            $("#WorkedStateTitle").data("kendoComboBox").value("");

        if (response.RateTypeDescription != null)
            $("#RateTypeDescription").data("kendoComboBox").value(response.RateTypeDescription);
        else
            $("#RateTypeDescription").data("kendoComboBox").value("");

        if (response.TimeCardTypeDescription != null)
            $("#TimeCardTypeDescription").data("kendoComboBox").value(response.TimeCardTypeDescription);
        else
            $("#TimeCardTypeDescription").data("kendoComboBox").value("");

        var datepicker = $("#HireDate").data("kendoDatePicker");
        if (response.HireDate != null) {
            var date = new Date(parseInt(response.HireDate.substr(6)));
            $("#HireDate").val(date.toLocaleDateString());
            datepicker.value(date);
        }
        else
            datepicker.value("");

        var datepicker2 = $("#TerminationDate").data("kendoDatePicker");
        if (response.TerminationDate != null) {
            var date2 = new Date(parseInt(response.TerminationDate.substr(6)));
            $("#TerminationDate").val(date2.toLocaleDateString());
            datepicker2.value(date2);
        }
        else
            datepicker2.value("");

        var datepicker3 = $("#PlannedServiceStartDate").data("kendoDatePicker");
        if (response.PlannedServiceStartDate != null) {
            var date3 = new Date(parseInt(response.PlannedServiceStartDate.substr(6)));
            $("#PlannedServiceStartDate").val(date3.toLocaleDateString());
            datepicker3.value(date3);
        }
        else
            datepicker3.value("");

        var datepicker4 = $("#ActualServiceStartDate").data("kendoDatePicker");
        if (response.ActualServiceStartDate != null) {
            var date4 = new Date(parseInt(response.ActualServiceStartDate.substr(6)));
            $("#ActualServiceStartDate").val(date4.toLocaleDateString());
            datepicker4.value(date4);
        }
        else
            datepicker4.value("");

        var datepicker5 = $("#ProbationEndDate").data("kendoDatePicker");
        if (response.ProbationEndDate != null) {
            var date5 = new Date(parseInt(response.ProbationEndDate.substr(6)));
            $("#ProbationEndDate").val(date5.toLocaleDateString());
            datepicker5.value(date5);
        }
        else
            datepicker5.value("");

        var datepicker6 = $("#TrainingEndDate").data("kendoDatePicker");
        if (response.TrainingEndDate != null) {
            var date6 = new Date(parseInt(response.TrainingEndDate.substr(6)));
            $("#TrainingEndDate").val(date6.toLocaleDateString());
            datepicker6.value(date6);
        }
        else
            datepicker6.value("");

        var datepicker7 = $("#SeniorityDate").data("kendoDatePicker");
        if (response.SeniorityDate != null) {
            var date7 = new Date(parseInt(response.SeniorityDate.substr(6)));
            $("#SeniorityDate").val(date7.toLocaleDateString());
            datepicker7.value(date7);
        }
        else
            datepicker7.value("");

        $("#CompanyCode").val(response.CompanyCode == null ? "" : response.CompanyCode);
        $("#FileNumber").val(response.FileNumber == null ? "" : response.FileNumber);
        $("#FedExemptions").val(response.FedExemptions == null ? "" : response.FedExemptions);
        $("#EmploymentNumber").val(response.EmploymentNumber == null ? "" : response.EmploymentNumber);

        if (response.Rate != null)
            $("#Rate").data("kendoNumericTextBox").value(response.Rate);
        else
            $("#Rate").data("kendoNumericTextBox").value("");

        if (response.Hours != null)
            $("#Hours").data("kendoNumericTextBox").value(response.Hours);
        else
            $("#Hours").data("kendoNumericTextBox").value("");

        $("#enteredByLabel").text(response.EnteredBy == null ? "" : response.EnteredBy);

        if (response.EnteredDate != null) {
            $("#enteredDateLabel").text(new Date(parseInt(response.EnteredDate.substr(6))).toLocaleString());
        }
        else
            $("#enteredDateLabel").text("");

        $("#modifiedByLabel").text(response.ModifiedBy == null ? "" : response.ModifiedBy);

        if (response.ModifiedDate != null) {
            $("#modifiedDateLabel").text(new Date(parseInt(response.ModifiedDate.substr(6))).toLocaleString());
        }
        else
            $("#modifiedDateLabel").text("");

        $("#personNameLabel").html(" - " + response.PersonName);

        setkendoDropDownList("EmployeeId", response.EmployeeId)
        var dropdownTimeCardType = $("#TimeCardTypeDescription").data("kendoComboBox");
        dropdownTimeCardType.refresh();
        dropdownTimeCardType.dataSource.read();
        if (response.TimeCardTypeId > 0) {
            dropdownTimeCardType.value(response.TimeCardTypeId);
            dropdownTimeCardType.text(response.TimeCardTypeDescription);
        }
        else {
            dropdownTimeCardType.select(0);
        }

        if ($("#hiddenPersonEmployeeId").val() > 0) {
            var form = $("#PersonEmployeeForm");

            form.validate();
            if (!form.valid())
                toastr.error(formService.messages.invalidMessage);
        }

        $(".validation-summary-errors").empty();
        return;
    } else {
        _employeeFlag = 'CLEAR';
        $("#btnClearPersonEmployeeAjax").trigger('click');
    }
}