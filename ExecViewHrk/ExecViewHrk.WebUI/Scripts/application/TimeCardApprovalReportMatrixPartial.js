//AFAIK jQuery has no function defined as serializeObject in its core.
//This function is a way to serialize a form (otherwise get an error if login through EmployeeHome page)
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

    $("#Approved_status").click(function (e) {
        $.validator.unobtrusive.parse("#TimeCardApprovalReportFormDdl");
        var appBaseUrl = $("#appBaseUrl").val();

        var form = $("#TimeCardApprovalReportFormDdl");
        var serialized_Object = form.serializeObject();

        if ($("#Approved_status").is(":checked")) {
            serialized_Object.Approved = true;
        }

        $.ajax({
            url: appBaseUrl + "TimeCardApprovalReport/PayPeriod_Approved_Ajax",
            type: "POST",
            data: {
                timeCardApprovalReportVm: serialized_Object,
            },
            success: function (response) {
                if ($("#Approved_status").is(":checked")) {                   
                    toastr.success("Pay period has been approved");
                }
                $("#GridPayPeriodApprovalReport").data("kendoGrid").dataSource.read();
                return;
            }
        });
    });

    $(function () {
        $('#GridPayPeriodApprovalReport').on('click', '.chkbx', function () {
            var checked = $(this).is(':checked');
            var grid = $('#GridPayPeriodApprovalReport').data().kendoGrid;
            var dataItem = grid.dataItem($(this).closest('tr'));
            dataItem.set('IsLineApproved', checked);
        });
    });

    //$("#exportButtonDiv").click(function (e) {
    //    $.validator.unobtrusive.parse("#TimeCardApprovalReportFormDdl");
    //    var appBaseUrl = $("#appBaseUrl").val();

    //    var form = $("#TimeCardApprovalReportFormDdl");
    //    var serialized_Object = form.serializeObject();
       
    //    $.ajax({
    //        url: appBaseUrl + "TimeCardApprovalReport/ExportTimeCardApprovalReportByDept_Ajax",
    //        type: "POST",
    //        data: {
    //             timeCardApprovalReportVm: serialized_Object,
    //            //companyCodeId: dataItem.CompanyCodeId,
    //            //payPeriodId: dataItem.PayPeriodId,
    //        },
    //        success: function (response) {
    //            if(response.succeed == false)
    //                toastr.error("Please select company, department and pay period");
    //            else if (response.fileName)
    //                document.location = appBaseUrl + "TimeCardApprovalReport/Download?fileName=" + response.fileName;
    //            else
    //                toastr.error("No employees record exist.");
    //            //window.location = '@Url.Action("Download", "PayPeriods", new { file =' + response.fileName + '})';
    //            //window.location = 'PayPeriods/Download?file=' + response.fileName;
    //            //if (response.allApproved) {
    //            //    toastr.success("File has been created");
    //            //}
    //            //else {
    //            //    toastr.error("All records are not approved");
    //            //}
    //            return;
    //        },
    //        error: function (response) {
    //            toastr.error("error!");
    //        }
    //    });
    //});
    
});

function filterDepartmentsByCompanyCode_ApprovalReport() {  
    var dropdownlist = $("#DepartmentIdDdl").data("kendoDropDownList");
    var filterInput = "";
    if (dropdownlist != null)
        filterInput = dropdownlist.filterInput[0].value;
    return {
        CompanyCodeIdDdl: $("#CompanyCodeIdDdl").val(),
        filterInput: filterInput
    };
}

function OnChangeDepartments(e) {
    var dropdown = $("#PayPeriodIdDdl").data("kendoDropDownList");
    dropdown.select(1);
    getApprovalStatus();
    $("#GridPayPeriodApprovalReport").data("kendoGrid").dataSource.read();
}

function filterTimeCardRecords_ApprovalReport() {

    return {       
        companyCodeId: $("#CompanyCodeIdDdl").val(),
        payPeriodId: $("#PayPeriodIdDdl").val()
        //departmentId: $("#DepartmentIdDdl").val()
    };
}


function OnChange_ApprovalReport(e) {   
    getApprovalStatus();
    $("#GridPayPeriodApprovalReport").data("kendoGrid").dataSource.read();   
};


function getApprovalStatus() {
    $.validator.unobtrusive.parse("#TimeCardApprovalReportFormDdl");
    var appBaseUrl = $("#appBaseUrl").val();
    var form = $("#TimeCardApprovalReportFormDdl");
    var serialized_Object = form.serializeObject();

    $.ajax({

        url: appBaseUrl + "TimeCardApprovalReport/getPayPeriodApprovalStatus_Ajax",

        type: "POST",

        data: { timeCardApprovalReportVm: serialized_Object, },

        success: function (response) {
            $("#Approved_status").prop("checked", response);
            return;
        }
    });
}