//AFAIK jQuery has no function defined as serializeObject in its core.
//This function is a way to serialize a form (otherwise get an error if login through EmployeeHome page -> Timecard)
$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

$(document).ready(function () {
    //Date 
    //
    //$("#Approved").click(function (e) {
    //    $.validator.unobtrusive.parse("#TimeCardFormDdl");
    //    var appBaseUrl = $("#appBaseUrl").val();

    //    var form = $("#TimeCardFormDdl");
    //    var serialized_Object = form.serializeObject();

    //    if ($("#Approved").is(":checked")) {
    //        serialized_Object.Approved = true;
    //    }

    //    $.ajax({
    //        url: appBaseUrl + "LookupTables/TimeCard_Approved_Ajax",
    //        type: "POST",
    //        data: {
    //            timeCardVm: serialized_Object,
    //        },
    //        success: function (response) {
    //            if ($("#Approved").is(":checked")) {
    //                toastr.success("Pay period has been approved");
    //            }
    //            return;
    //        }
    //    });
    //});       

    $(function DisplayClock() {
        var currentDateTime = new Date(-5.00);
        //  var currentDateTime = new Date();
        var browserdate = currentDateTime.getMonth() + 1 + "/" + currentDateTime.getDate() + "/" + currentDateTime.getFullYear();
        currentDateTime.setMinutes(currentDateTime.getMinutes() + 30);
        var hh = currentDateTime.getHours();
        var mm = currentDateTime.getMinutes();
        var ss = currentDateTime.getSeconds();

        //pad 0 if digit < 10 for minutes and seconds
        mm = (mm < 10 ? "0" : "") + mm;
        ss = (ss < 10 ? "0" : "") + ss;
        var AMorPM = (hh < 12) ? "AM" : "PM";

        //Hours greater than 12
        hh = (hh > 12 ? hh - 12 : hh);
        hh = (hh == 0 ? 12 : hh);  //For 00 AM
        //$("#TimeIn").val(browserdate + " " + hh + ":" + mm + ":" + ss + " " + AMorPM);

        $("#txtTimeIn").val(browserdate + " " + hh + ":" + mm + ":" + ss + " " + AMorPM);
        $("#txtLunchOut").val(browserdate + " " + hh + ":" + mm + ":" + ss + " " + AMorPM);
        $("#txtLunchBack").val(browserdate + " " + hh + ":" + mm + ":" + ss + " " + AMorPM);
        $("#txtTimeOutAfterLunchBack").val(browserdate + " " + hh + ":" + mm + ":" + ss + " " + AMorPM);
        $("#txtTimeOut").val(browserdate + " " + hh + ":" + mm + ":" + ss + " " + AMorPM);
        setTimeout(function () { DisplayClock() }, 1000);

        var isstudent = $("#hdnIsStudent").val();
        if (isstudent == "True") {
            //Student
            setTimeCardReadOnly();
        }
    });
});

function handleGroups(groups) {
    for (var i = 0; i < groups.length; i++) {
        var gr = groups[i];
        offsetDateFields(gr); //handle the Key variable as well
        if (gr.HasSubgroups) {
            handleGroups(gr.Items)
        } else {
            loopRecords(gr.Items);
        }
    }
}

function loopRecords(persons) {
    for (var i = 0; i < persons.length; i++) {
        var person = persons[i];
        offsetDateFields(person);
    }
}

function offsetDateFields(obj) {
    for (var name in obj) {
        var prop = obj[name];
        if (typeof (prop) === "string" && prop.indexOf("/Date(") == 0) {
            obj[name] = prop.replace(/\d+/, function (n) {
                var offsetMiliseconds = new Date(parseInt(n)).getTimezoneOffset() * 60000;
                return parseInt(n) + offsetMiliseconds
            });
        }
    }
}

function filterByArchive() {
    return {
        IsArchived: $("#hiddenIsArchived").val()
    }
}

/*Returns Company ID To Load Departments*/
function filterDepartmentsByCompanyCode() {
    var dropdownlist = $("#DepartmentIdDdl").data("kendoDropDownList");
    var filterInput = "";
    if (dropdownlist != null)
        filterInput = dropdownlist.filterInput[0].value;
    return {
        CompanyCodeIdDdl: $("#CompanyCodeIdDdl").val(),
        IsArchived: $("#hiddenIsArchived").val(),
        filterInput: filterInput
    };
}

/*Returns Department Id To Load Employees*/
function filterEmployeesByDepartment() {
    var dropdownlist = $("#EmployeeIdDdl").data("kendoDropDownList");
    var filterInput = "";
    var isChecked = $('#IsActive').is(":checked");
    if (dropdownlist != null)
        filterInput = dropdownlist.filterInput[0].value;
    return {
        DepartmentIdDdl: $("#DepartmentIdDdl").val(),
        filterInput: filterInput,
        isActive: isChecked
    };
}

/*Returns Employee Id To Load PayPeriods*/
function filterPayperiodByPayfrequency() {
    return {
        EmployeeIdDdl: $("#EmployeeIdDdl").val(),
        IsArchived: $("#hiddenIsArchived").val()
    };
}

/*Returns Employee Id To Load Employee Projects Grid*/
function filterEmployeeProjects() {
    return {
        employeeIdDdl: $("#EmployeeIdDdl").val(),
    };
}

/*Returns Employee Id To Load Employee Positions in the Grid*/
function filterEmployeePositions() {
    return {
        employeeIdDdl: $("#EmployeeIdDdl").val(),
        payPeriodId: $("#PayPeriodIdDdl").val(),
    };
}

/*Returns For Loading the Time Cards Grid*/
function filterTimeCardRecords() {
    return {
        employeeIdDdl: $("#EmployeeIdDdl").val(),
        companyCodeIdDdl: $("#CompanyCodeIdDdl").val(),
        payPeriodId: $("#PayPeriodIdDdl").val(),
        departmentId: $("#DepartmentIdDdl").val(),
        IsArchived: $("#hiddenIsArchived").val()
    };
}

/*Loading All Grid in Employee Change in Time card and In and out */
function OnChangeEmployee(e) {
    if ($("#EmployeeIdDdl").val() != "") {
        //$("#GridProjectsassignedEmployee").data("kendoGrid").dataSource.read();
        $("#GridTimeoffSummary").data("kendoGrid").dataSource.read();

        $("#GridTimeCard").data("kendoGrid").dataSource.read();
        $("#GridDisplayWeeklyTimeCardTotal").data("kendoGrid").dataSource.read();

        $("#btnSendeMail").prop('disabled', false);
        $("#btnSendSMS").prop('disabled', false);
    }
    else {
        $("#btnSendeMail").prop('disabled', true);
        $("#btnSendSMS").prop('disabled', true);
    }

    if ($("#PayPeriodIdDdl").val() != "") {
        $("#GridTimeCard").data("kendoGrid").dataSource.read();
        $("#GridDisplayWeeklyTimeCardTotal").data("kendoGrid").dataSource.read();

    }
    if ($("#PayPeriodIdDdl").val() == "" || $("#EmployeeIdDdl").val() == "") {
        $("#btnIsApproved").prop('disabled', true);
    }
    else {
        $("#btnIsApproved").prop('disabled', false);
    }
    GetApprovalStatus(); //Retus the Approval Status and sets the Approve Button.
};

/*Loading Time Cards Grid in Payperiod Change*/
function OnChange(child) {
    $("#hdnIsTimeCardChanged").val("false");
    if (child != "childGrid") {
        GetApprovalStatus();
        $("#GridTimeCard").data("kendoGrid").dataSource.read();
        $("#GridDisplayWeeklyTimeCardTotal").data("kendoGrid").dataSource.read();
    }
    if ($("#PayPeriodIdDdl").val() == "" || $("#EmployeeIdDdl").val() == "") {
        $("#btnIsApproved").prop('disabled', true);
    }
    else {
        $("#btnIsApproved").prop('disabled', false);
    }

    GetApprovalStatus(); //Retus the Approval Status and sets the Approve Button.
    //Sets Time Card Read Only/Editable based on Employee Role
    setTimeCardByStudentAndEmployee();
};

/*Getting Approval status and Disable the Approve Button*/
function GetApprovalStatus() {
    $.validator.unobtrusive.parse("#TimeCardFormDdl");
    var appBaseUrl = $("#appBaseUrl").val();
    var form = $("#TimeCardFormDdl");
    //var serialized_Object = form.serializeObject();

    var payPeriodId = $("#PayPeriodIdDdl").val();
    var employeeID = $("#EmployeeIdDdl").val();
    if (employeeID != 0 && payPeriodId != "") {
        $.ajax({
            url: appBaseUrl + "LookupTables/getTimeCardApprovalStatus_Ajax",
            type: "GET",
            async: true,
            // data: { timeCardVm: serialized_Object, employeeID: employeeID },
            data: { employeeID: employeeID, payPeriodId: payPeriodId },
            success: function (response) {
                if (response == true) {
                    $("#btnIsApproved").prop('value', 'Disapprove');
                    $("#hdnApprovalStatus").prop('value', 'Disapprove');
                    // $("#btnIsApproved").val("Disapprove");
                    //   $("#hdnApprovalStatus").val("Disapprove");
                    //$("#GridTimeCard").data("kendoGrid").dataSource.read();
                    setTimeCardReadOnly();
                }
                else {
                    $("#btnIsApproved").prop('value', 'Approve All');
                    $("#hdnApprovalStatus").prop('value', 'Approve');
                    // $("#btnIsApproved").val("Approve");
                    // $("#hdnApprovalStatus").val("Approve");
                    //$("#GridTimeCard").data("kendoGrid").dataSource.read();
                    setTimeCardEditable();
                }
            }
        });
    }
}
//Sets Time Card- Student - Readonly; Employee - Readonly based on Approval
function setTimeCardByStudentAndEmployee() {
    var role = $("#hdnRole").val();
    var isstudent = $("#hdnIsStudent").val();
    if (role == "ClientEmployees") //&& isstudent == "True")
    {
        //alert("Student: " + isstudent + " - Approval Status: " + $("#hdnApprovalStatus").prop("value"));
        if (isstudent == "True") {
            //Student
            setTimeCardReadOnly();
        }
        else {
            //Employee
            if ($("#hdnApprovalStatus").prop("value") == "Approve") {
                //$("#GridTimeCard").data("kendoGrid").dataSource.read();
                //setTimeCardReadOnly();
                setTimeCardEditable();
            }
            else {
                //setTimeCardEditable();
                setTimeCardReadOnly();
            }
        }
    }
    else {
        //OTHER THAN EMPLOYEE ROLES
        if ($("#hdnApprovalStatus").prop("value") == "Approve") {
            //$("#GridTimeCard").data("kendoGrid").dataSource.read();
            //setTimeCardReadOnly();
            setTimeCardEditable();
        }
        else {
            //setTimeCardEditable();
            setTimeCardReadOnly();
        }
    }
}

//Sets the Time Card Read Only
function setTimeCardReadOnly() {
    $(".k-grid-add").css("display", "none");
    $(".btnNestedgrid").css("display", "none");
    $(".k-grid-save-changes").css("display", "none");
    $(".k-grid-cancel-changes").css("display", "none");
    var grid = $("#GridTimeCard").data("kendoGrid");
    grid.hideColumn(0);
    $(".chkbx").prop("disabled", true);
    //grid.hideColumn(1);
    //$("#btnDelete").hide();    
    $("#btnSubmit").hide();
}

//Sets the Time Card Editable
function setTimeCardEditable() {
    var superAdmin = $("#hdnSupervisorandAdmin").val();
    if (superAdmin == "SupervisorAdmin") {
        $(".k-grid-add").css("display", "none");
    }
    else {
        $(".k-grid-add").show();
    }
    $(".btnNestedgrid").show();
    $(".k-grid-save-changes").show();
    $(".k-grid-cancel-changes").show();
    var grid = $("#GridTimeCard").data("kendoGrid");
    grid.showColumn(0);
    //grid.showColumn(1);
    //$("#btnDelete").show();
    //$(".btnDelete").show();
    $("#btnSubmit").show();
}

function setTimeCardApprovedCheckBox() {
    var isstudent = $("#hdnIsStudent").val();
    if ($("#hdnApprovalStatus").prop("value") == "Disapprove" || isstudent == "True") {
        return "<input type ='checkbox' class='chkbx'  disabled ='disabled'/>"
    }
    else {
        return "<input type ='checkbox' class='chkbx' />"
    }
}


function setApprovedCheckBox(isapproved) {
    var role = $("#hdnRole").val();
    var isstudent = $("#hdnIsStudent").val();
    //if (isapproved) {
    if ((isapproved) || ((role == "ClientEmployees") && (isstudent == "True"))) {
        return "<input type ='checkbox' class='chkbx' checked='checked' disabled ='disabled'/>"
    }
    else {
        //return "<input type ='checkbox' />"  /*onclick= 'setRowOnApproval(this)'*/
        return " #if(ShowLineApprovedActive){# <input type='checkbox' #=IsLineApproved ? checked='checked' :'' # class='chkbx' />#} else{# <input type='checkbox' disabled='disabled' #=IsLineApproved ? checked='checked' :'' # class='chkbx' />#}#"
    }
}

function onGridEdit(e) {
    var isstudent = $("#hdnIsStudent").val();
    if ($("#hdnApprovalStatus").prop("value") == "Disapprove" || isstudent == "True" || $("#hdnPayPeriodLock").val() == "true") {
        var grid = this.wrapper.closest("[data-role=grid]")[0].id;
        $("#" + grid).data("kendoGrid").closeCell(e.container);
    }

    //Disables the Inline Approved Punch.
    //debugger;
    var fieldName = e.container.find("input").attr("name");
    if (!isEditable(fieldName, e.model)) {
        this.closeCell(); // prevent editing
    }
}

//Returns false for the fields specified in the condition
function isEditable(fieldName, model) {
    //debugger;
    if (model.IsLineApproved == true) {
        if ((fieldName === "TimeIn") ||
            (fieldName === "TimeOut") ||
            (fieldName === "LunchOut") ||
            (fieldName === "LunchBack") ||
            (fieldName === "PositionId")
        ) {
            // condition for the field "SomeForeignKeyID" 
            // (default to true if defining property doesn't exist)
            //return model.hasOwnProperty("TimeIn") && model.IsFkEnabled;
            return model.hasOwnProperty("IsFKEnabled") && model.IsFkEnabled;
        }
        // additional checks, e.g. to only allow editing unsaved rows:
        // if (!model.isNew()) { return false; }       
    }
    return true; // default to editable
}

//function setRowOnApproval(e) {
//    //alert("Line Approval");
//    //var grid = $("#GridTimeCard").data("kendoGrid");
//    var row = $(e.currentTarget).closest("tr");
//    //var data = grid.dataItem(row);
//    //var row = e.closest("tr");
//    grid = $("#GridTimeCard").data("kendoGrid");
//    dataItem = grid.dataItem(row);
//}


/*Returns Fund Id To Load Projects In Grid*/
function filterProjectsByfund(e) {
    return {
        fundDdl: $("#FundIdDdl").val(),
    };
}

/*Returns Projects Id To Load Funds In Grid*/
function OnFundsChange(e) {
    $("#ProjectIdDdl").val(this.value());
}

/*Setting Fund Id To Hiden field in Fund Change*/
function OnfundsChange(e) {
    $("#FundIdDdl").val(this.value());
}

/*Returns Projects Id To Load Funds In Grid*/
function filterFundsByProject(e) {
    if ($("#ProjectIdDdl").val() == "") {
        return {
            projectIdDdl: 3
        };
    }
    return {
        projectIdDdl: $("#ProjectIdDdl").val()
    };
}

/*Setting Projects Id To Hiden field in Projects Change*/
function OnProjectsChange(e) {
    $("#ProjectIdDdl").val(this.value());
}

/*Time Grid Events Start*/
function TimeGridRequestStart(e) {
    var role = $("#hdnRole").val();
    if (e.type == "create") {

        //For two alert
        var data2 = $("#GridTimeCard").data("kendoGrid").dataSource._data;
        for (var i = 0; i < data2.length; i++) {
            if (data2[i].dirty) {
                if (data2[i].UserId != null) {
                    return false;
                }
            }
        }

        if (role != "ClientEmployees") {
            if ($("#CompanyCodeIdDdl").val() == "") {
                toastr.error("The Company Code field is Required </br> The Department field is Required </br> The Employee field is Required  </br> The Pay Period field is Required");
                e.stopPropagation();
                return false;
            }
            if ($("#DepartmentIdDdl").val() == "") {
                toastr.error("The Department field is Required  </br> The Employee field is Required  </br> The Payperiod field is Required");
                e.stopPropagation();
                return false;
            }
            if ($("#EmployeeIdDdl").val() == "") {
                toastr.error("The Employee field is Required </br> The Payperiod field is Required");
                e.stopPropagation();
                return false;
            }
            if ($("#PayPeriodIdDdl").val() == "") {
                toastr.error("The Payperiod field is Required");
                e.stopPropagation();
                return false;
            }
        }
        if (role == "ClientEmployees") {
            if ($("#PayPeriodIdDdl").val() == "") {
                toastr.error("The Payperiod field is Required");
                e.stopPropagation();
                return false;
            }
        }
    }
    if (e.type == "update" || e.type == "create") {
        var maxhours = $("#hdnMaxhours").val();
        var payperioddates = PayperiodDates();

        var data = $("#GridTimeCard").data("kendoGrid").dataSource._data;
        var total = 0;
        var dailyhours = 0;
        var codedhours = 0;
        var timeout = 0;
        var timein = 0;
        var lunchback = 0;
        var lunchout = 0;
        var payperiodweekonetotal = 0;
        var payperiodweektwototal = 0;
       
        //if (data[0].items[0].Falsacode != null) {
        //    falsacode = data[0].items[0].Falsacode;
        //}

        for (i = 0; i < data.length; i++) {
            //Position Required Valiation           
            if ((data[i].TimeOut != null) || (data[i].TimeIn != null)) {
                if (data[i].PositionId == null) {
                    toastr.error('Position is required ');
                    e.preventDefault();
                    e.stopPropagation();
                    return false;
                }
            }

            if ((data[i].TimeOut != null || data[i].LunchOut != null) && (data[i].TimeIn == null)) {
                toastr.error('Time In is required ');
                e.preventDefault();
                e.stopPropagation();
                return false;
            }

            if ((data[i].LunchBack != null) && (data[i].LunchOut == null)) {
                toastr.error('Lunch Out is required ');
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            if (data[i].Hours != null) {
                if (data[i].HoursCodeId == null) {
                    toastr.error("Please select Hour Code.");
                    e.preventDefault();
                    e.stopPropagation();
                    return false;
                }
            }

            //Time and Max Session Hours Validation.
            if (data[i].TimeIn != null && data[i].TimeIn != 0) {
                var dailyhours = 0;

                if (data[i].Hours != null && data[i].Hours != 0)
                    codedhours = data[i].Hours;
                else
                    codedhours = 0;
                if ((data[i].TimeOut != null) && (data[i].TimeIn != null)) {
                    timeout = data[i].TimeOut;
                    timein = data[i].TimeIn;
                    dailyhours = timeDiffCal(timein, timeout);
                }
                else {
                    if (data[i].DailyHours != null && data[i].DailyHours != 0)
                        dailyhours = data[i].DailyHours;
                }
                if ((data[i].LunchOut != null) && (data[i].LunchBack != null)) {                   
                    lunchback = data[i].LunchBack;
                    lunchout = data[i].LunchOut;
                }              

                //24 hours validation functionality
                var hiddenDailyHours = 0;
                var lineTotal = 0;
                if (data[i].DailyHours != null && data[i].DailyHours != 0) {
                    hiddenDailyHours = data[i].DailyHours;
                }
                if (data[i].LineTotal != null && data[i].LineTotal != 0) {
                    lineTotal = data[i].LineTotal;
                }
                var hours = (dailyhours - timeDiffCal(lunchout, lunchback)) + codedhours;
                var totalDailyHours = hours + lineTotal - hiddenDailyHours;
                if (totalDailyHours >= 24) {
                    toastr.error('Total Hours should not be greater than 24 Hours at row position ' + (i + 1));
                    $("#GridTimeCard").data("kendoGrid").dataSource.read();
                    e.stopPropagation();
                    return false;
                }
            }
        }

        //Session Validation per week.
        //RETURNS CURRENT PAGE
        var timecardgrid = $("#GridTimeCard").data("kendoGrid");
        var currentPage = timecardgrid.dataSource.page();

        var filterdata = filterTimeCardRecords();
        var appBaseUrl = $("#appBaseUrl").val();
        $.ajax({
            url: appBaseUrl + "TimeCardMatrix/GetSumofWeekWiseApprovedTotal",
            type: "GET",
            async: false,
            data: {
                employeeIdDdl: filterdata.employeeIdDdl,
                payPeriodId: filterdata.payPeriodId,
                IsArchived: filterdata.IsArchived,
                maxhours: maxhours
            },
            success: function (weeklytotal) {
                payperiodweekonetotal = weeklytotal.weekOneTotalHours;
                payperiodweektwototal = weeklytotal.weekTwoTotalHours;
            }
        });

        if (currentPage == 1) {
            payperiodweekonetotal += eval($("#hdnWeekOneApprovedHours").val());
            payperiodweekonetotal -= eval($("#hdnWeekOneUnApprovedHours").val());
            //alert("Week One Total Approved Hours:" + eval($("#hdnWeekOneApprovedHours").val()));
            //alert("Week One Total UnApproved Hours:" + eval($("#hdnWeekOneUnApprovedHours").val()));
            //alert("Main Grid Week One Total Approved Hours: " + payperiodweekonetotal + "/" + "Session Time:" + maxhours);
            if (e.type == "create") {
                //For two alert
                var data1 = $("#GridTimeCard").data("kendoGrid").dataSource._data;
                for (var i = 0; i < data1.length; i++) {
                    if (data1[i].dirty) {
                        if (data1[i].UserId != null) {
                            return false;
                        }
                    }
                }
            }
            if (payperiodweekonetotal > maxhours) {
                //toastr.error('Hours exceeded with session limit per week. Session Limit is:  ' + maxhours + ' Hours.');
                //Confirm dialogbox to approve more than session limit        
                $("#hdnSessionLimitExceeded").val("true");
                var result = confirm('Hours exceeded with Session Limit per week ' + maxhours + '. Do you want to Approve?');
                if (result == false) {
                    $("#GridTimeCard").data("kendoGrid").dataSource.read();
                    $("#hdnWeekOneApprovedHours").val(0);
                    $("#hdnWeekOneUnApprovedHours").val(0);
                    e.preventDefault();
                    e.stopPropagation();
                    return false;
                }
                else {
                    //Refresh to clear Approved Sum
                    $("#GridTimeCard").data("kendoGrid").dataSource.read();
                    $("#hdnWeekOneApprovedHours").val(0);
                    $("#hdnWeekOneUnApprovedHours").val(0);
                }
            }
        }
        if (currentPage == 2) {
            payperiodweektwototal += eval($("#hdnWeekTwoApprovedHours").val());
            payperiodweektwototal -= eval($("#hdnWeekTwoUnApprovedHours").val());
            //alert("Week Two Total Approved Hours:" + eval($("#hdnWeekTwoApprovedHours").val()));
            //alert("Week Two Total UnApproved Hours:" + eval($("#hdnWeekTwoUnApprovedHours").val()));
            //alert("Main Grid Week Two Total Approved Hours: " + payperiodweektwototal + "/" + "Session Time:" + maxhours);
            if (e.type == "create") {
                //For two alert
                var data3 = $("#GridTimeCard").data("kendoGrid").dataSource._data;
                for (var i = 0; i < data3.length; i++) {
                    if (data3[i].dirty) {
                        if (data3[i].UserId != null) {
                            return false;
                        }
                    }
                }
            }
            if (payperiodweektwototal > maxhours) {
                //toastr.error('Hours exceeded with session limit per week. Session Limit is:  ' + maxhours + ' Hours.');
                //Confirm dialogbox to approve more than session limit
                $("#hdnSessionLimitExceeded").val("true");
                result = confirm('Hours exceeded with Session Limit per week ' + maxhours + '. Do you want to Approve?');
                if (result == false) {
                    $("#GridTimeCard").data("kendoGrid").dataSource.read();
                    $("#hdnWeekOneApprovedHours").val(0);
                    $("#hdnWeekOneUnApprovedHours").val(0);
                    e.preventDefault();
                    e.stopPropagation();
                    return false;
                }
                else {
                    //Refresh to clear Approved Sum
                    $("#GridTimeCard").data("kendoGrid").dataSource.read();
                    $("#hdnWeekOneApprovedHours").val(0);
                    $("#hdnWeekOneUnApprovedHours").val(0);
                }
            }
        } 
        
    }
}

function SessionValidation(maxhours) {
    //var timecardgrid = $("#GridTimeCard").data("kendoGrid");
    //var currentPage = timecardgrid.dataSource.page();
    var week1 = 0;
    var week2 = 0;
    var filterdata = filterTimeCardRecords();
    var appBaseUrl = $("#appBaseUrl").val();
    $.ajax({
        url: appBaseUrl + "TimeCardMatrix/GetSumofTotalApprovedTotal",
        type: "GET",
        async: false,
        data: {
            employeeIdDdl: filterdata.employeeIdDdl,
            payPeriodId: filterdata.payPeriodId,
            IsArchived: filterdata.IsArchived
        },
        success: function (d) {
            week1 = d.week1;
            week2 = d.week2;
        }
    });

    if (week1 > maxhours || week2 > maxhours) {   
            $("#hdnSessionLimitExceeded").val("true");
            var result = confirm('Hours exceeded with Session Limit per week ' + maxhours + '. Do you want to Approve?');
            if (result == false) {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
            else {
                return true;
            }
        }
};

//Time difference between calculation
function timeDiffCal(TimeIn, TimeOut) {
    var diffHours = 0;
    if ((TimeIn != null && TimeIn != 0) && (TimeOut != null && TimeOut != 0)) {
        var time2 = (TimeOut.getHours() + ":" + TimeOut.getMinutes() + ":" + TimeOut.getSeconds());
        var time1 = (TimeIn.getHours() + ":" + TimeIn.getMinutes() + ":" + TimeIn.getSeconds());
        diffHours = (new Date("2000-1-1 " + time2) - new Date("2000-1-1 " + time1)) / 1000 / 60 / 60;
    }
    return diffHours;
}

//overlap hours
function timeOverlap(TimeIn, TimeOut, subTimeIn, subTimeOut) {
    var overlap = true;
    if (TimeOut == null) {
        var time3 = (subTimeOut.getHours() + ":" + subTimeOut.getMinutes());
        var time2 = (subTimeIn.getHours() + ":" + subTimeIn.getMinutes());
        var time1 = (TimeIn.getHours() + ":" + TimeIn.getMinutes());
        //time1 = parseInt(time1);
        //time2 = parseInt(time2);
        //time3 = parseInt(time3)
        time1 = new Date("2000-1-1 " + time1);
        time2 = new Date("2000-1-1 " + time2);
        time3 = new Date("2000-1-1 " + time3);
        if ((time1 > time2 && time1 >= time3) || (time1 < time2 && time1 <= time3)) {
            overlap = false;
        }
    }
    else {
       if (TimeOut != null) {
            var time4 = (subTimeOut.getHours() + ":" + subTimeOut.getMinutes());
            var time3 = (subTimeIn.getHours() + ":" + subTimeIn.getMinutes());
            var time2 = (TimeOut.getHours() + ":" + TimeOut.getMinutes());
            var time1 = (TimeIn.getHours() + ":" + TimeIn.getMinutes());
            //time1 = parseInt(time1);
            //time2 = parseInt(time2);
            //time3 = parseInt(time3)
            time1 = new Date("2000-1-1 " + time1);
            time2 = new Date("2000-1-1 " + time2);
            time3 = new Date("2000-1-1 " + time3);
            time4 = new Date("2000-1-1 " + time4);
            if (time1 < time3 && time1 < time4) {
                if (time2 <= time3 && time2 < time4) {
                    overlap = false;
                }
            }
            else {
                if (time2 >= time3 && time2 > time4) {
                    overlap = false;
                }
            }
        } // end of checking null values.         
    }
    return overlap;
}

//Returns the difference between 2 times.
function timeDiff(time1, time2) {
    var Datediff = new Date();
    Datediff.setTime(time2 - time1);
    var timediff = Datediff.getTime();
    var timediffhours = timediff / (60 * 60 * 1000);
    return timediffhours;
}

/*Time Grid Events End*/
function timegridRequestEnd(e) {
    if (e.type == "create") {
       
       var isvalid = $("hdnSessionIsEffectiveDateafter").val();
        //For two alert
        var data1 = $("#GridTimeCard").data("kendoGrid").dataSource._data;
        for (var i = 0; i < data1.length; i++) {
            //alert(data1[i].dirty + " - " + data1[i].IsLineApproved);
            var isSessionLimitExceeded = $("#hdnSessionLimitExceeded").val();
            if (data1[i].dirty || data1[i].IsLineApproved || isSessionLimitExceeded == "true") {
                if (data1[i].UserId != null) {
                    $("#hdnSessionLimitExceeded").val("false");
                    //return false;
                }
            }
            if (data1[i].dirty && data1[i].UserId == null && isvalid ==false) {
                
                toastr.success('Record Created successfully.');
            }
        }

        if (!e.response.Errors) {
            //toastr.success('Record Created successfully.');
            e.sender.read();
            $("#GridDisplayWeeklyTimeCardTotal").data("kendoGrid").dataSource.read();
            $("#GridTimeoffSummary").data("kendoGrid").dataSource.read();
            //GetApprovalStatus();
        }
    }


    if (e.response.Data && e.response.Data.length) {
        var data = e.response.Data;
        if (this.group().length && e.type == "read") {
            handleGroups(data);
        } else {
            loopRecords(data);
        }
    }
    if (e.type == "update") {
        if (!e.response.Errors) {
            toastr.success('Record Updated successfully.');
            e.sender.read();
            $("#GridDisplayWeeklyTimeCardTotal").data("kendoGrid").dataSource.read();
            $("#GridTimeoffSummary").data("kendoGrid").dataSource.read();

            //Reset Approved and UnApproved Hours after Updation
            $("#hdnWeekOneApprovedHours").val(0);
            $("#hdnWeekOneUnApprovedHours").val(0);
            $("#hdnWeekTwoApprovedHours").val(0);
            $("#hdnWeekTwoUnApprovedHours").val(0);
            GetApprovalStatus();
        }
    }
    //if (e.type == "create") {
    //    if (!e.response.Errors) {
    //        toastr.success('Record Created successfully.');
    //        e.sender.read();
    //        $("#GridDisplayWeeklyTimeCardTotal").data("kendoGrid").dataSource.read();
    //        $("#GridTimeoffSummary").data("kendoGrid").dataSource.read();
    //        //GetApprovalStatus();
    //    }
    //}   
}

function OnCreateUpdate(e) {
    if (e.type == "create" || e.type == "update" || e.type == "destroy") {
        $("#GridTimeCard").data("kendoGrid").dataSource.read();
        $("#GridDisplayWeeklyTimeCardTotal").data("kendoGrid").dataSource.read();
    }
}

/*Delete For Time Cards In and Out*/
function gridDeleteRow(e) {
    e.preventDefault();
    //$("#GridTimeCard").data("kendoGrid").removeRow($(e.currentTarget).closest("tr"));

    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { timecardId: data.TimeCardId };
    var formURL = $("#ApplicationUrl").val() + "TimeCardMatrix/TimecardsDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
                resetGrid("GridTimeCard");
                $("#GridDisplayWeeklyTimeCardTotal").data("kendoGrid").dataSource.read();
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("GridTimeCard");
                $("#GridDisplayWeeklyTimeCardTotal").data("kendoGrid").dataSource.read();
            }
        });
    });
}

/*Delete For Time Cards*/
function deleteTimecardsRow(e) {
    //e.preventDefault();
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);

    //Deletion of Approved Record
    if (data.IsLineApproved == true) {
        toastr.error('Approved record cannot be deleted!');
        return false;
    }

    var employeeId = $("#EmployeeIdDdl").val();
    if (employeeId != 0 && data.TimeCardId != 0) {
        var postData = { timecardId: data.TimeCardId };
        var formURL = $("#ApplicationUrl").val() + "TimeCardMatrix/TimecardsDelete";
        confirmDialog("", function () {
            $.post(formURL, postData, function (data, status) {
                if (data.succeed == false) {
                    toastr.error(data.Message);
                    //resetGrid("GridTimeCard");
                } else {
                    toastr.success('Record deleted successfully.');
                    resetGrid("GridTimeCard");
                    $("#GridDisplayWeeklyTimeCardTotal").data("kendoGrid").dataSource.read();
                }
            });
        });
    }
}

/*Delete For Employee Projects*/
function deleteProjectPercentage(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { projectPercentageID: data.ProjectPercentageID };
    var formURL = $("#ApplicationUrl").val() + "TimeCardMatrix/ProjectsPercentage_Delete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
                resetGrid("GridProjectsassignedEmployee");
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("GridProjectsassignedEmployee");
            }
        });
    });
}

//Timecard Configuation
function gridTimeCardConfigEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);

    openTimeCardConfigPopup(data.TimeCardTypeId);
}

function openTimeCardConfigPopup(_TimeCardTypeId) {

    loading(true);
    var formURL = $("#ApplicationUrl").val() + '/TimeCardConfigurations/GetTimeCardColumnsDetails?timeCardTypeId=' + _TimeCardTypeId;

    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);

    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}

/*Loading Time Cards,Weekly Aggregate and Employee Projects Grid in Employee Change For Time Cards Archive */
function OnChangeEmployeeforArchive(e) {
    //GetApprovalStatus();
    if ($("#EmployeeIdDdl").val() != "") {
        $("#GridTimeoffSummary").data("kendoGrid").dataSource.read();
    }
    if ($("#PayPeriodIdDdl").val() != "") {
        $("#GridTimeCardArchive").data("kendoGrid").dataSource.read();
        $("#GridDisplayWeeklyTimeCardTotal").data("kendoGrid").dataSource.read();
    }
    //if ($("#PayPeriodIdDdl").val() == "" || $("#EmployeeIdDdl").val() == "") {
    //    $('#Approved').prop('disabled', true);
    //}
    //else {
    //    $('#Approved').prop('disabled', false);
    //}
};

function OnChangePayPeriodforArchive(e) {
    //GetApprovalStatus();
    
    if ($("#PayPeriodIdDdl").val() != "") {
        $("#GridTimeCardArchive").data("kendoGrid").dataSource.read();
        $("#GridDisplayWeeklyTimeCardTotal").data("kendoGrid").dataSource.read();
    }
    //if ($("#PayPeriodIdDdl").val() == "" || $("#EmployeeIdDdl").val() == "") {
    //    $('#Approved').prop('disabled', true);
    //}
    //else {
    //    $('#Approved').prop('disabled', false);
    //}
};


function PayperiodDates() {
    var payPeriod = $("#PayPeriodIdDdl").data("kendoDropDownList").text();
    var startDate = new Date(payPeriod.substr(0, payPeriod.indexOf("-")).trim());
    var endDate = new Date(payPeriod.substr(payPeriod.indexOf("-") + 1, payPeriod.length).trim());

    var timeDiff = Math.abs(endDate.getTime() - startDate.getTime());
    var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
    return diffDays;

}

function disableDatesNotInPayPeriod(date) {
    var payPeriod = $("#PayPeriodIdDdl").data("kendoDropDownList").text();
    var startDate = new Date(payPeriod.substr(0, payPeriod.indexOf("-")).trim());
    var endDate = new Date(payPeriod.substr(payPeriod.indexOf("-") + 1, payPeriod.length).trim());

    if (date && compareDates(date, startDate, endDate)) {
        return false;
    } else {
        return true;
    }
}

function compareDates(date, startDate, endDate) {
    for (var iDate = startDate; iDate <= endDate; iDate.setDate(iDate.getDate() + 1)) {
        if (iDate.getDate() == date.getDate() && iDate.getMonth() == date.getMonth() && iDate.getYear() == date.getYear()) {
            return true;
        }
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

        $('#GridTimeCard').data('kendoGrid').dataSource.read();
        $('#GridTimeCard').data('kendoGrid').refresh();
    }
}

function GetTimecrdbyEmployee(IsArchived) {
    loading(true);
    //var formURL = $("#ApplicationUrl").val() + '/TimeCards/TimeCardsMatrixPartial?IsArchived=' + IsArchived;

    //$("#EditDetailDiv .md-content").html("");
    //$("#EditDetailDiv .md-content").load(formURL);

    //$('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    //$('#EditDetailDiv').addClass('md-show');

    $.ajax({

        url: $("#ApplicationUrl").val() + '/TimeCards/TimeCardsMatrixPartial',

        type: "GET",

        data: { IsArchived: IsArchived },

        success: function (data) {
            $("#timecardData").html(data);
            //return;
        }
    });
}

//Send Email OpenPopup
function sendEmailtoEmp() {
    $.validator.unobtrusive.parse("#frmEmpSendEmail");
    var appBaseUrl = $("#appBaseUrl").val();

    var form = $("#frmEmpSendEmail");
    var serialized_Object = form.serializeObject();
    $.ajax({
        url: appBaseUrl + "TimeCardMatrix/SendEmailtoEmployee",
        type: "POST",
        async: true,
        data: {
            MailToAddress: serialized_Object.txtEmail.trim(),
            message: serialized_Object.txtMessage,
        },
        success: function (response) {

            if (response.success == true) {
                toastr.success("Mail sent successfully");
                $("#eMailModal").modal('hide');
            }
            else {
                toastr.error("Error in sending mail..");
            }
        },
        error: function (data) {
            toastr.error("Error in sending mail");
        }
    });
}

//Send Email OpenPopup
function sendSMStoEmp() {

    $.validator.unobtrusive.parse("#frmEmpSendSMS");
    var appBaseUrl = $("#appBaseUrl").val();

    var form = $("#frmEmpSendSMS");
    var serialized_Object = form.serializeObject();
    var number = serialized_Object.txtNumber + '@' + serialized_Object.txtGateway;
    $.ajax({
        url: appBaseUrl + "TimeCardMatrix/SendSMStoEmployee",
        type: "POST",
        async: true,
        data: {
            Number: number,
            message: serialized_Object.txtSMS,
        },
        success: function (response) {

            if (response.success = true) {
                toastr.success("SMS sent successfully");
                $("#SMSModal").modal('hide');
            }
            else {
                toastr.error("Error in sending sms");
            }
        },
        error: function (data) {
            toastr.error("Error in sending sms");
        }
    });
}

//Timecard Notes
function getNotes(Id) {
    //var d = new Date(Id);
    //var str = $.datepicker.formatDate('yy-mm-dd', d);
    var appBaseUrl = $("#appBaseUrl").val();
    if (Id != 0) {
        $.ajax({
            url: appBaseUrl + "TimeCardMatrix/GetTimecardNotes",
            type: "GET",
            async: true,
            data: {
                timecardId: Id
            },
            success: function (data) {
                if (data != null) {
                    $("#timecardnoteBody").html(data);
                    $("#timecardNotesModal").modal('show');
                }
                else {
                    toastr.error("Error");
                }
            },
            error: function (data) {
                toastr.error("Error");
            }
        });
    }
    else {
        toastr.error("No time card data exist for this Date");
        return false;
    }
};

//Timecard Archive Notes
function getArchiveNotes(Id) {
    //var d = new Date(Id);
    //var str = $.datepicker.formatDate('yy-mm-dd', d);
    var appBaseUrl = $("#appBaseUrl").val();
    if (Id != 0 && Id != "null") {
        $.ajax({
            url: appBaseUrl + "TimeCardArchive/GetTimecardArchiveNotes",
            type: "GET",
            async: true,
            data: {
                timecardId: Id
            },
            success: function (data) {
                if (data != null) {
                    $("#timecardnoteBody").html(data);
                    $("#timecardNotesModal").modal('show');
                }
                else {
                    toastr.error("Error");
                }
            },
            error: function (data) {
                toastr.error("Error");
            }
        });
    }
    else {
        toastr.error("Notes doesn't exist for this Date");
        return false;
    }
};

function TimeGridCellChange(e) {
    if (e.field == "TimeIn") {
    }
}

///VALIDATE TIME
function isDateValid(strDate) {
    var r, regExp;
    regExp = new RegExp("^(((0?[1-9]|1[012])((:[0-5]\\d){0,1}((\\x20)?([AaPp]([Mm])?)?))?)|([01]\\d|2[0-3])(:[0-5]\\d){1,2})$");	//Create regular expression object.
    var matches = regExp.exec(strDate);
    return (matches != null && strDate == matches[0]);
}

function SetValidTime(obj) {
    //obj.id = "TimeIn" / "TimeOut"
    //obj.name = "TimeIn" / "TimeOut"
    // check to see if date is in correct format
    //if (m_InvalidObj != null && m_InvalidObj != obj)
    //    return;

    if (obj.id == "TimeIn" || obj.id == "TimeOut" || obj.id == "LunchOut" || obj.id == "LunchBack") {
        if (obj.value.length > 0) {
            bIsValidDate = isDateValid(obj.value);
            if (!bIsValidDate) {
                //alert("Time is not in the correct format.");
                //obj.focus();
                //////m_InvalidObj = obj;  
                var time = obj.value;
                var timetype = "PM";
                var strDate;
                if (obj.id == "TimeIn") {
                    timetype = "AM";
                }
                //LOGIC FOR INVALID FORMAT TIME INPUT. EX. 830, 1130
                if (obj.value.length == 3) {
                    if (time[0] != '0') {
                        strDate = '0' + time[0] + ':' + time[1] + time[2] + ' ' + timetype;
                    }
                    else {
                        strDate = time[0] + ':' + time[1] + time[2] + ' ' + timetype;
                    }
                    return strDate;
                }
                if (obj.value.length == 4) {
                    if (time[0] != '0') {
                        strDate = time[0] + time[1] + ':' + time[2] + time[3] + ' ' + timetype;
                    }
                    else {
                        strDate = time[0] + time[1] + ':' + time[2] + time[3] + ' ' + timetype;
                    }
                    return strDate;
                }
            }
            else {
                m_InvalidObj = null;
                // if correct format - fill in with long value - i.e convert 8a to 8:00 AM
                var strDate;
                var bIsExplicitAMPM = false;
                var strArray = new Array(8);
                var strChar;
                var nState = 0;

                strDate = obj.value;
                // see if the user explicitly specified time of day - AM or PM and retain for later
                var re = /[ap]/i;            //Create regular expression pattern.
                if (strDate.search(re) != -1)
                    bIsExplicitAMPM = true;

                // loop through the FINITE STATE AUTOMATON - STATE MACHINE FOLLOWS
                for (i = 0; i < strDate.length; i++) {
                    if (nState == 9)
                        break;
                    strChar = strDate.charAt(i);
                    switch (nState) {
                        case 0: if (strChar == "0") {
                            strArray[0] = "0";
                            nState = 1;
                        }
                        else if (parseInt(strChar, 10) > 1 && parseInt(strChar, 10) <= 9) {
                            if (i == strDate.length - 1) {
                                strArray[0] = "0";
                                strArray[1] = strChar;
                                strArray[2] = ":"; strArray[3] = "0"; strArray[4] = "0";
                                if (obj.id == "TimeIn") {
                                    strArray[5] = " "; strArray[6] = "A"; strArray[7] = "M";
                                }
                                else { //if (obj.id == "TimeOut")
                                    strArray[5] = " "; strArray[6] = "P"; strArray[7] = "M";
                                }

                            }
                            else {
                                strArray[0] = strChar;
                                nState = 4;
                            }
                        }
                        else if (parseInt(strChar, 10) == 1) {
                            if (i == strDate.length - 1) {
                                strArray[0] = "0";
                                strArray[1] = strChar;
                                strArray[2] = ":"; strArray[3] = "0"; strArray[4] = "0";
                                //strArray[5] = " "; strArray[6] = "A"; strArray[7] = "M";
                                if (obj.id == "TimeIn") {
                                    strArray[5] = " "; strArray[6] = "A"; strArray[7] = "M";
                                }
                                else { //if (obj.id == "TimeOut") 
                                    strArray[5] = " "; strArray[6] = "P"; strArray[7] = "M";
                                }
                            }
                            else {
                                strArray[0] = "1";
                                nState = 3;
                            }
                        }
                            break;
                        case 1: if (parseInt(strChar, 10) > 0 && parseInt(strChar, 10) <= 9) {
                            strArray[1] = strChar;
                            if (i == strDate.length - 1) {
                                strArray[2] = ":"; strArray[3] = "0"; strArray[4] = "0";
                                //strArray[5] = " "; strArray[6] = "A"; strArray[7] = "M";
                                if (obj.id == "TimeIn") {
                                    strArray[5] = " "; strArray[6] = "A"; strArray[7] = "M";
                                }
                                else {//if (obj.id == "TimeOut")
                                    strArray[5] = " "; strArray[6] = "P"; strArray[7] = "M";
                                }
                            }
                            else
                                nState = 2;
                        }
                        else
                            alert("Error in state 2 - second character isn't 1-9");
                            break;

                        case 2: if (strChar == ":") {
                            strArray[2] = strChar;
                            nState = 5;
                        }
                        else if (strChar == "a" | strChar == "A" | strChar == "p" | strChar == "P") {
                            strArray[2] = ":"; strArray[3] = "0"; strArray[4] = "0";
                            strArray[5] = " "; strArray[6] = strChar.toUpperCase(); strArray[7] = "M";
                            nState = 9;
                        }
                        else if (strChar == " ") {
                            strArray[2] = ":"; strArray[3] = "0"; strArray[4] = "0"; strArray[5] = " ";
                            nState = 8;
                        }
                            break;
                        case 3: if (strChar == 0 | strChar == 1 | strChar == 2) {
                            strArray[1] = strChar;
                            if (i == strDate.length - 1) {
                                strArray[2] = ":"; strArray[3] = "0"; strArray[4] = "0";
                                strArray[5] = " "; strArray[6] = "A"; strArray[7] = "M";
                            }
                            else
                                nState = 2;
                        }
                        else if (strChar == ":") {
                            strArray[1] = strArray[0];
                            strArray[0] = "0";
                            strArray[2] = ":";
                            nState = 5;
                        }
                        else if (strChar == "a" | strChar == "A" | strChar == "p" | strChar == "P") {
                            strArray[1] = strArray[0];
                            strArray[0] = "0";
                            strArray[2] = ":"; strArray[3] = "0"; strArray[4] = "0";
                            strArray[5] = " "; strArray[6] = strChar.toUpperCase(); strArray[7] = "M";
                            nState = 9;
                        }
                        else if (strChar == " ") {
                            strArray[1] = strArray[0];
                            strArray[0] = "0";
                            strArray[2] = ":"; strArray[3] = "0"; strArray[4] = "0"; strArray[5] = " ";
                            nState = 8;
                        }
                            break;
                        case 4: if (strChar == ":") {
                            strArray[1] = strArray[0];
                            strArray[0] = "0";
                            strArray[2] = ":";
                            nState = 5;
                        }
                        else if (strChar == "a" | strChar == "A" | strChar == "p" | strChar == "P") {
                            strArray[1] = strArray[0];
                            strArray[0] = "0";
                            strArray[2] = ":"; strArray[3] = "0"; strArray[4] = "0";
                            strArray[5] = " "; strArray[6] = strChar.toUpperCase(); strArray[7] = "M";
                            nState = 9;
                        }
                        else if (strChar == " ") {
                            strArray[1] = strArray[0];
                            strArray[0] = "0";
                            strArray[2] = ":"; strArray[3] = "0"; strArray[4] = "0"; strArray[5] = " ";
                            nState = 8;
                        }
                            break;
                        case 5: if (parseInt(strChar, 10) >= 0 && parseInt(strChar, 10) <= 5) // minutes (10's)
                        {
                            strArray[3] = strChar;
                            nState = 6;
                        }
                            break;
                        case 6: if (parseInt(strChar, 10) >= 0 && parseInt(strChar, 10) <= 9) // minutes (1's)
                        {
                            strArray[4] = strChar;
                            if (i == strDate.length - 1) {
                                //strArray[5] = " "; strArray[6] = "A"; strArray[7] = "M";
                                if (obj.id == "TimeIn") {
                                    strArray[5] = " "; strArray[6] = "A"; strArray[7] = "M";
                                }
                                else { //if (obj.id == "TimeOut")
                                    strArray[5] = " "; strArray[6] = "P"; strArray[7] = "M";
                                }
                            }
                            nState = 7;
                        }
                            break;
                        case 7: if (strChar == " ") {
                            strArray[5] = strChar;
                            nState = 7;
                        }
                        else if (strChar == "a" | strChar == "A" | strChar == "p" | strChar == "P") {
                            strArray[5] = " "; strArray[6] = strChar.toUpperCase(); strArray[7] = "M";
                            nState = 9;
                        }
                            break;
                        case 8: strArray[6] = strChar.toUpperCase();
                            strArray[7] = "M";
                            nState = 9;
                            break;
                        default: alert("error in time convert switch");
                    }
                }

                // save expanded result back to strDate
                strDate = "";
                for (i = 0; i < 8; i++) {
                    strDate = strDate + strArray[i];
                }

                var strTimeID = obj.name;
                //var strTimeType = strTimeID.substr(strTimeID.length-1,1); // 3 = in, 4 = Lunch out, 5 = lunch back, 6 = time out
                var strTimeType = strTimeID.substr(strTimeID.lastIndexOf("-") + 1);

                var bIsAM = true;
                if (strDate.substr(6, 1) == "P")
                    bIsAM = false;

                var dateToday = new Date();
                var bCurrentTimeIsAM = true;
                if (dateToday.getHours() > 11)
                    bCurrentTimeIsAM = false;

                return strDate;
            }
        }

    }
}

function gridDataBound(e) {
    var data = filterTimeCardRecords();
    var appBaseUrl = $("#appBaseUrl").val();

    var view = this.dataSource.view();
    for (var i = 0; i < view.length; i++) {
        if (view[i].IsLineApproved) {
            var currentUid = view[i].uid;
            this.tbody.find("tr[data-uid='" + currentUid + "']")
                .addClass("k-state-selected")
                .find(".chkbx")
                .attr("checked", "checked");
        }
    }

    if (data.payPeriodId != "" && e.sender._data.length != 0) {
        if (data.employeeIdDdl == "") {
            data.employeeIdDdl = 0;
        }
        if (data.departmentId == "") {
            data.departmentId = 0;
        }
        $.ajax({
            url: appBaseUrl + "TimeCardMatrix/GetTimeCardsList",
            type: "GET",
            data: {
                employeeIdDdl: data.employeeIdDdl,
                payPeriodId: data.payPeriodId,
                IsArchived: data.IsArchived,
                departmentId: data.departmentId
            },
            success: function (response) {
                for (var i = 0; i < response.length; i++) {
                    if (response[i].Count > 1) {
                        var grid = $("#GridTimeCard").data("kendoGrid").items();
                        var closesttr = $(grid).find('td:contains(' + response[i].ActualDate + ')').closest('tr');
                        var td = $(closesttr).find("td:eq(0) a");
                        $(td).css({
                            //'background-image': "url('../images/up-arrow-child.png') !important",
                            //'background - position': 'center center !important',
                            //'background - repeat': 'no - repeat!important',
                            'opacity': '1',
                            'background-color': '#67b4ff'
                        });
                    }
                }
            }
        });
    }
};


//Timecard Lockout
function GetPayPeriodLockoutStatus() {
    var data = $("#PayPeriodIdDdl").data("kendoDropDownList");
    var role = $("#hdnUserRole").val();
    var isstudent = $("#hdnIsStudent").val();
    if (role == "ClientManagers") {

        //var datelist = data.text().split("-");
        //var SDate = datelist[0].trim();
        //var EDate = datelist[1].trim();
        //var EmployeeId = $("#EmployeeIdDdl").val();
        //var postData = { PayStartDate: SDate, PayEndDate: EDate, EmployeeId: EmployeeId }
        var payPeriodId = $("#PayPeriodIdDdl").val();
        var employeeID = $("#EmployeeIdDdl").val();
        var postData = { payPeriodId: payPeriodId, employeeID: employeeID }
        var formURL = $("#ApplicationUrl").val() + "TimeCardMatrix/GetPayPeriodLockOutStatus";
        $.post(formURL, postData, function (data, status) {
            if (data != null) {
                if (role == "ClientEmployees") {
                    if (isstudent != "True") {
                        if (data.LockEmployee == true) {
                            setTimeCardReadOnly();
                            $("#btnSendeMail").css("display", "none");
                            $("#btnSendSMS").css("display", "none");
                            $("#btnIsApproved").css("display", "none");
                            $("#hdnPayPeriodLock").val('true');
                        }
                        else {
                            OnChange("maingrid");
                        }
                    }
                }
                if (role == "ClientManagers") {
                    //if (data.LockManger == true) {
                    if ((data.LockManger == true) || data.LockAssignedManager == true) {
                        setTimeCardReadOnly();
                        $("#btnSendeMail").css("display", "none");
                        $("#btnSendSMS").css("display", "none");
                        $("#btnIsApproved").css("display", "none");
                        $("#hdnPayPeriodLock").val('true');
                        //  $("#lblPayPeriodLocked").show();
                        //  $("#lblPayPeriodLocked").html("Pay period is locked!");

                        $("#GridTimeCard").data("kendoGrid").dataSource.read();
                        $("#GridDisplayWeeklyTimeCardTotal").data("kendoGrid").dataSource.read();
                    }
                    else {
                        $("#btnSendeMail").show();
                        $("#btnSendSMS").show();
                        $("#btnIsApproved").show();
                        $("#hdnPayPeriodLock").val('false');
                        //  $("#lblPayPeriodLocked").hide();
                        //  $("#lblPayPeriodLocked").text("");
                        OnChange("maingrid");
                    }
                } else {
                    OnChange("maingrid");
                }
            } else { OnChange("maingrid"); }
        });
    } else {
        OnChange("maingrid");
    }
}
//function GetPayPeriodLockoutStatus() {
//    var data = $("#PayPeriodIdDdl").data("kendoDropDownList");
//    var role = $("#hdnUserRole").val();
//    if (data.value() == "") {
//        $(".k-grid-save-changes", "#GridTimeCard").show();
//        // $("#lblTimesub").hide();
//    }
//    else {
//        var datelist = data.text().split("-");
//        var SDate = datelist[0].trim();
//        var EDate = datelist[1].trim();
//        var EmployeeId = $("#EmployeeIdDdl").val();
//        var postData = { PayStartDate: SDate, PayEndDate: EDate, EmployeeId: EmployeeId }
//        var formURL = $("#ApplicationUrl").val() + "TimeCardMatrix/GetPayPeriodLockOutStatus";
//        $.post(formURL, postData, function (data, status) {
//            if (data.StartDate != null) {
//                $(".k-grid-save-changes", "#GridTimeCard").hide();
//                $("#StartDate").text(data.StartDate);
//                $("#EndDate").text(data.EndDate);
//                //$("#EmpSubDate").text(data.EmpSubDate);
//                //$("#lblTimesub").show();
//                //var MgrApprove = data.MgrApproveDate;
//                //var MgrUnApprove = data.MgrUnApproveDate;
//                //if (MgrApprove > MgrUnApprove) {
//                //    $("#lblApprove").text(', Approved on' + " " + MgrApprove);
//                //    $('#lblApprove').attr('class', 'bg-success text-white');
//                //}
//                //else if (MgrApprove < MgrUnApprove) {
//                //    $("#lblApprove").text(', DisApproved on' + " " + MgrUnApprove);
//                //    $('#lblApprove').attr('class', 'bg-danger text-white');
//                //}
//                //if (MgrApprove == "" && MgrUnApprove == "") {
//                //    $("#btnIsApproved").prop('value', 'Approve');
//                //}
//                $("#btnIsApproved").prop("disabled", false);
//            } else {
//                $(".k-grid-save-changes", "#GridTimeCard").show();
//                //$("#lblTimesub").hide();
//                $('#Approved').prop('disabled', true);
//            }
//            if (role == "ClientEmployees") {
//                if (data.LockEmployee == true) {
//                    $(".k-grid-save-changes", "#GridTimeCard").addClass("k-state-disabled")
//                }
//                else {
//                    $(".k-grid-save-changes", "#GridTimeCard").removeClass("k-state-disabled")
//                }
//            }
//            else if (role == "ClientManagers") {
//                if (data.LockManger == true) {
//                    $(".k-grid-save-changes", "#GridTimeCard").addClass("k-state-disabled")
//                }
//                else {
//                    $(".k-grid-save-changes", "#GridTimeCard").removeClass("k-state-disabled")
//                }
//            }

//            //Apply Background color to grid
//            //if (MgrUnApprove > EmpSubDate) {
//            //    var grid = $("#GridTimeCard").data("kendoGrid");
//            //    var gridData = grid.dataSource.view();
//            //    var gridData = grid.columns;
//            //    $.each(grid.tbody.find('tr'), function () {
//            //        var model = grid.dataItem(this);
//            //        if (model.MgrUnApproveDate == null) {    //if Selected Id(key) and Model Id(key) equals when applying Colour for row
//            //            $("#GridTimeCard" + " tbody tr").removeClass("rowSelected");
//            //            $('[data-uid=' + model.uid + ']').css('background-color', 'skyblue');//.addClass('rowSelected');
//            //        }
//            //    });
//            //}

//        });
//    }
//}

//$("#btnBack").click(alert("Hi"));

$(document).on("click", "#btnBack", function () {
    BackToTimeCardUnApprovalPage();
});

function BackToTimeCardUnApprovalPage() {
    var companyCodeId = $("#CompanyCodeIdDdl").val();
    var payPeriodId = $("#PayPeriodIdDdl").val();
    var postData = { companyCodeId: companyCodeId, payPeriodId: payPeriodId };
    LoadContents("TimeCardUnapprovedReportPartialNew", "TimeCardUnApprovedReport", postData);
}