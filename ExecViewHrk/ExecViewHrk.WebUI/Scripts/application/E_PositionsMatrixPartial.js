$(document).ready(function () {
    $.validator.unobtrusive.parse("#E_PositionForm");
    var appBaseUrl = $("#appBaseUrl").val();
    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });

    $("#btnSaveE_PositionAjax").click(function (e) {
        e.preventDefault();
        var form = $("#E_PositionForm");
        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var serialized_Object = form.serializeObject();
        serialized_Object.EmployeeId = $("#EmployeeIdDdl").val();
        if (serialized_Object.EmployeeId === "") {
            toastr.error("Please Select Employment Numbers.");
            return;
        }
        serialized_Object.PositionDescription = $("#PositionDescription").data("kendoComboBox").text();
        serialized_Object.PayFrequencyDescription = $("#PayFrequencyDescription").data("kendoComboBox").text();
        serialized_Object.RateTypeDescription = $("#RateTypeDescription").data("kendoComboBox").text();
        serialized_Object.PrimaryPosition = $("#PrimaryPosition")["0"].checked;
        $.ajax({

            url: appBaseUrl + "E_Positions/E_PositionsSaveAjax",

            type: "POST",

            data: {
                e_PositionVm: serialized_Object,
            },

            success: function (response) {
                if (response.succeed === false)
                    toastr.error(response.Message);
                else {
                    var _olde_PositionId = $("#hiddenE_PositionId").val();
                    refreshE_PositionFormData(response.e_PositionVm);
                    if (_olde_PositionId !== "0")
                        toastr.success("Record has been updated");
                    else {
                        toastr.success("Record has been saved");
                    }
                }
                return;
            }
        });
    });

    $("#btnClearE_PositionAjax").click(function (e) {

        e.preventDefault();

        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();

        $.ajax({

            url: appBaseUrl + "E_Positions/E_PositionsIndexChangedAjax",

            type: "POST",

            data: {
                e_PositionIdDdl: 0,
                personId: personId,
            },

            success: function (response) {
                refreshE_PositionFormData(response);
                toastr.success("Record has been cleared.");
                return;
            }

        });

    });

    $("#btnDeleteE_PositionAjax").click(function (e) {
        e.preventDefault();
        confirmDialog("", function () {
            appBaseUrl = $("#appBaseUrl").val();
            var personId = $("#hiddenPersonId").val();
            var e_PositionId = $("#hiddenE_PositionId").val();

            $.ajax({

                url: appBaseUrl + "E_Positions/E_PositionsDeleteAjax",

                type: "POST",

                data: {
                    e_PositionIdDdl: e_PositionId,
                    personId: personId
                },

                success: function (response) {


                    if (response == "The Employee Position does not exist") {
                        toastr.error(response);
                        return;
                    }

                    refreshE_PositionFormData(response);
                    toastr.success("Record has been deleted");
                    return;
                }
            });
        });
    });
});

function onSelectE_PositionDdl(e) {

    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1);

    var ddlSelectedE_PositionId = dataItem.EmployeePositionId == "" ? 0 : dataItem.EmployeePositionId;

    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();
    var employeeId = $("#EmployeeIdDdl").val();

    $.ajax({

        url: appBaseUrl + "E_Positions/E_PositionsIndexChangedAjax",

        type: "POST",

        data: {
            e_PositionIdDdl: ddlSelectedE_PositionId,
            personId: personId,
            //employeeId: employeeId,
        },

        success: function (response) {

            refreshE_PositionFormData(response);
            return;
        }

    });
}

function onDataBoundE_PositionDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noE_PositionRecordsFound').hide();
    else
        $('#noE_PositionRecordsFound').show();
}

function onCascadeEmployeePositionsDdl(e) {
    formService.clear();
    var personId = $("#ddlPersonId").data("kendoDropDownList").value();
    var name = $("#ddlPersonId").data("kendoDropDownList").text();
    $("#hiddenPersonId").val(personId);
    $("#PersonName").val(name);
}

function error_handler(e) {

    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshE_PositionFormData(response) {

    $("#hiddenPersonId").val(response.PersonId);
    $("#hiddenE_PositionId").val(response.E_PositionId);

    $('#formDiv input[type=radio]').prop('checked', false);
    $('#formDiv input:checkbox').prop('checked', false);


    if (response.PositionDescription != null)
        $("#PositionDescription").data("kendoComboBox").value(response.PositionDescription);
    else
        $("#PositionDescription").data("kendoComboBox").value("");

    if (response.PayFrequencyDescription != null)
        $("#PayFrequencyDescription").data("kendoComboBox").value(response.PayFrequencyDescription);
    else
        $("#PayFrequencyDescription").data("kendoComboBox").value("");

    if (response.RateTypeDescription != null)
        $("#RateTypeDescription").data("kendoComboBox").value(response.RateTypeDescription);
    else
        $("#RateTypeDescription").data("kendoComboBox").value("");

    var datepicker = $("#StartDate").data("kendoDatePicker");
    if (response.StartDate != null) {
        var date = new Date(parseInt(response.StartDate.substr(6)));
        $("#StartDate").val(date.toLocaleDateString());
        datepicker.value(date);
    }
    else
        datepicker.value("");

    var datepicker2 = $("#EndDate").data("kendoDatePicker");
    if (response.EndDate != null) {
        var date2 = new Date(parseInt(response.EndDate.substr(6)));
        $("#EndDate").val(date2.toLocaleDateString());
        datepicker2.value(date2);
    }
    else
        datepicker2.value("");

    $('input[name=PrimaryPosition]').val([response.PrimaryPosition]);

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

    var dropdownlist = $("#EmployeeIdDdl").data("kendoDropDownList");
    dropdownlist.refresh();
    dropdownlist.dataSource.read();
    if (response.EmployeeId != 0) {
        dropdownlist.value(response.EmployeeId);
    }
    else {
        dropdownlist.select(0);
    }

    var dropdownlist1 = $("#E_PositionIdDdl").data("kendoDropDownList");
    dropdownlist1.refresh();
    dropdownlist1.dataSource.read();
    if (response.E_PositionId != 0) {
        dropdownlist1.value(response.E_PositionId);
    }
    else {
        dropdownlist1.select(0);
    }

    $(".validation-summary-errors").empty();

    if ($("#hiddenE_PositionId").val() > 0) {
        var form = $("#E_PositionForm");

        form.validate();
        if (!form.valid())
            toastr.error("form is invalid");
    }

    appBaseUrl = $("#appBaseUrl").val();

    $.ajax({
        url: appBaseUrl + "EmployeeI9Documents/GetEmployeeEmploymentNumberList",
        type: "POST",
        success: function (response2) {
            $("#EmployeeIdDdl").data("kendoDropDownList").value(response.EmployeeId);
        }
    });

    $.ajax({
        url: appBaseUrl + "E_Positions/GetE_PositionsList",
        type: "POST",
        success: function (response3) {
            $("#E_PositionIdDdl").data("kendoDropDownList").value(response.E_PositionId);
        }
    });
    return;
}

function filterE_PositionsbyEmploymentNumber() {
    return {
        EmployeeIdDdl: $("#EmployeeIdDdl").val()
    };
}

function filterByE_PositionId() {
    return { e_PositionIdDdl: $("#E_PositionIdDdl").val() };
}

function onEdit(e) {
    if (e.model.isNew()) {
        e.model.set("E_PositionId", $("#hiddenE_PositionId").val());
    } else {
        toastr.error("Select Position to add data");
        return;
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

        $('#gridE_Position').data('kendoGrid').dataSource.read();
        $('#gridE_Position').data('kendoGrid').refresh();
    }
}
function gridEmployeePositionEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openEPositionDetailEditpopup(data.E_PositionId, data.PersonId);
}
function gridEmployeePositionDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { e_PositionIdDdl: data.E_PositionId, personId: data.PersonId };
    var formURL = $("#ApplicationUrl").val() + "E_Positions/E_PositionsDeleteAjax";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                //window.location.reload();
                resetGrid("gridEmployeePosition");
            }
        });
    });
}

function addEmployeePosition() {  
    var EmpNumber = $('#EmployeeId').val();
    var empId = $("#EmployeeId option:selected").val();
    var postData = { EmpId: empId, personid: $("#SelectedPersonID").val(), EmpNumber: EmpNumber };
    var formURL = $("#ApplicationUrl").val() + "E_Positions/GetEmpStatus";
    $.post(formURL, postData, function (data, status) {
        if (data.Empstatus != "Terminated") {
            openEPositionEditpopup(0, empId, $("#SelectedPersonID").val(), EmpNumber);
        }
        else {
            toastr.error("You can not assign position for terminated employee");
        }
    });
}
function ViewSalaryDetail(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openSalaryDetailPopup(data.E_PositionId);
}
function openEPositionEditpopup(_ePositionId, _employeeId, _personId, EmpNumber) {
    var formURL = $("#ApplicationUrl").val() + '/E_Positions/E_PositionsDetail?ePositionId=' + _ePositionId + '&employeeId=' + _employeeId + '&personId=' + _personId + '&EmpNumber=' + EmpNumber;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}
function openEPositionDetailEditpopup(_ePositionId, _personId) {
    var formURL = $("#ApplicationUrl").val() + '/E_Positions/EditePositionDetail?ePositionId=' + _ePositionId + '&personId=' + _personId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}
function gridEmployeeSalPositionEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    if (data.EndDate == null) {
        openSalaryDetailEditPopup(data.E_PositionSalaryHistoryId, data.RateTypeId);
    }
}
function openSalaryDetailEditPopup(_EPositionSalaryHistoryId, _RateTypeId) {
    var formURL = $("#ApplicationUrl").val() + '/E_Positions/EditePositionSalaryDetail?ePositionSalHistoryId=' + _EPositionSalaryHistoryId + '&RateTypeId=' + _RateTypeId;
    $.post(formURL, function (data, status) {
        if (data.succeed == false) {
            toastr.error(data.Message);
        } else {
            var $modal = $('#SalaryEditModal');
            $modal.html(data);
            $modal.modal("show");
        }
    });
}



function openSalaryDetailPopup(_ePositionId) {
    var formURL = $("#ApplicationUrl").val() + '/E_Positions/ViewSalaryDetail?e_PositionId=' + _ePositionId;
    $.ajax({
        url: formURL,
        type: "GET",
        contentType: 'application/json',
        dataType: "json",
        data: "{}",
    }).success(function (data) {
        var row = "";
        $.each(data.Data, function (index, item) {
            var ratetype = item.RateTypeDescription;
            var annualSalary = "$" + item.AnnualSalary;
            var payRate = "$" + item.PayRate;
            if (ratetype == "H") {
                item.RateTypeDescription = "Hourly";
            }
            else if (ratetype == "S") {
                item.RateTypeDescription = "Salary";
            }
            
            var effDate = item.EffectiveDate;
            if (effDate != null) {
                var date = new Date(parseInt(effDate.substr(6)));
                item.EffectiveDate = date.toLocaleDateString();
            }
            else {
                item.EffectiveDate = "";
            }
            var actualDate = item.EndDate;
            if (actualDate != null) {
                var adate = new Date(parseInt(actualDate.substr(6)));
                item.EnteredDate = adate.toLocaleDateString();
            }
            else {
                item.EnteredDate = "";
            }
            row += "<tr><td>" + item.RateTypeDescription + "</td><td>" + payRate + "</td><td>" + item.HoursPerPayPeriod + "</td><td>" + annualSalary + "</td><td>" + item.EffectiveDate + "</td><td>" + item.EnteredDate + "</td></tr>";
        });
        $("#salaryDetail").html(row);
        $("#ViewSalaryModal").modal('show');
    });
}







