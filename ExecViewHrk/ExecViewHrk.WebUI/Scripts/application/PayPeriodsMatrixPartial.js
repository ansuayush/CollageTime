$(document).ready(function () {

});


function gridPayPeriodPreview(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var stdt = data.PayPeriodStartDate;
    var enddt = data.PayPeriodEndDate;
    var paygrp = data.PayGroupCode;
    var paynum = data.PayPeriodNumber;
    var postData = { payPeriodId: data.PayPeriodId, startDate: data.PayPeriodStartDate, endDate: data.PayPeriodEndDate, payGroupId: data.PayGroupCode, payPeriodNumber: data.PayPeriodNumber };
    var formURL = $("#ApplicationUrl").val() + "PayPeriods/ExportPreviewPayPeriod1";
    $.ajax({
        url: formURL,
        type: "POST",
        data: {
            startDate: data.PayPeriodStartDate,
            endDate: data.PayPeriodEndDate,
            payGroupId: data.PayGroupCode,
            payPeriodNumber: data.PayPeriodNumber,
        },
        success: function (response) {
            if (response != null) {
                
                $("#divExportPreview").html(response);
                $('#modalPreview').modal();
            }
            else
                toastr.error("No employees record exist.");
        },
        error: function (response) {
            toastr.error("error!");
        }
    });
};

//function Export_Employees_TimeCard(e) {

//    e.preventDefault();
//    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
//    companyCodeId = dataItem.CompanyCodeId;
//    payPeriodId = dataItem.PayPeriodId;

//    //var url = "PayPeriods/ExportEmployeeTimeCardDetails_Ajax?companyCodeId=" + dataItem.CompanyCodeId + "&&payPeriodId=" + dataItem.PayPeriodId;
//    //document.location = url;

//    window.location = "PayPeriods/ExportEmployeeTimeCardDetails_Ajax?companyCodeId=" + dataItem.CompanyCodeId + "&payPeriodId=" + dataItem.PayPeriodId;
//    //window.location = ""
//}

function addPayPeriods() {
    openPayPeriodPopup(0);
}

function gridPayPeriodEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);

    openPayPeriodPopup(data.PayPeriodId);
}

function openPayPeriodPopup(_payPeriodId) {

    loading(true);
    var formURL = $("#ApplicationUrl").val() + '/PayPeriods/PayPeriodDetails?payPeriodId=' + _payPeriodId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}

function gridPayPeriodDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { payPeriodId: data.PayPeriodId };
    var formURL = $("#ApplicationUrl").val() + "PayPeriods/PayPeriodDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPayPeriod");
            }
        });
    });
}
function Export_Employees_TimeCard(e) {

    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

    $.validator.unobtrusive.parse("#PayPeriodFormDdl");
    var appBaseUrl = $("#appBaseUrl").val();

    $.ajax({
        url: appBaseUrl + "PayPeriods/ExportEmployeeTimeCardDetails_Ajax",
        type: "POST",
        data: {
            companyCodeId: dataItem.CompanyCodeId,
            payPeriodId: dataItem.PayPeriodId,
        },
        success: function (response) {
            if (response.fileName)
                document.location = appBaseUrl + "PayPeriods/Download?fileName=" + response.fileName;
            else
                toastr.error("No employees record exist.");
            //window.location = '@Url.Action("Download", "PayPeriods", new { file =' + response.fileName + '})';
            //window.location = 'PayPeriods/Download?file=' + response.fileName;
            //if (response.allApproved) {
            //    toastr.success("File has been created");
            //}
            //else {
            //    toastr.error("All records are not approved");
            //}
            return;
        },
        error: function (response) {
            toastr.error("error!");
        }
    });
}


function ArchiveEmployeesTimeCard(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

    $.validator.unobtrusive.parse("#PayPeriodFormDdl");
    var appBaseUrl = $("#appBaseUrl").val();

    $.ajax({
        url: appBaseUrl + 'PayPeriods/BeforeArchive',
        type: 'POST',
        data: {
            companyCodeId: dataItem.CompanyCodeId,
            payPeriodId: dataItem.PayPeriodId
        },
        //dataType: "json",
        success: function (d) {
            if (d.IsArchived) {
                toastr.error("Pay Period already Archived");
                return false;
            }
            else if (d.IsEndDate) {
                toastr.error("Pay Period should be Closed");
                return false;
            }
            else if (!d.LockoutEmployees) {
                toastr.error("Please Lockout Employees");
                return false;
            }
            else if (!d.LockoutManagers) {
                toastr.error("Please Lockout Managers");
                return false;
            }
            else if (d.IsEndDate) {
                toastr.error("Pay Period must be Closed.");
                return false;
            }
            else if (d.TimecardCount == 0) {
                toastr.error("No Time Card Records to Archive");
                return false;
            }
            else if (d.TimecardUnApprovedCount > 0) {
                var result = confirm("There are unapproved time card record in Pay Period. Are you sure still you want to Archive this Pay Period?");
                if (result) {
                    $.ajax({
                        url: appBaseUrl + "PayPeriods/ArchiveEmployeesTimeCard_Ajax",
                        type: "POST",
                        //cache: false,
                        async: false,
                        //dataType: "json",
                        data: {
                            companyCodeId: dataItem.CompanyCodeId,
                            payPeriodId: dataItem.PayPeriodId,

                        },
                        success: function (response) {
                            if (response.result == true) {
                                toastr.success('Record Archived successfully.');
                            }
                            else {
                                toastr.error("No Time Card Records to Archive");
                            }
                            RefreshGrid();
                            return;
                        },
                        error: function (response) {
                            toastr("error!");
                        }
                    })
                }
                else {
                    return false;
                }
            }
            else {
                var result = confirm("Are you sure you want to Archive this Pay Period?");
                if (result) {
                    $.ajax({
                        url: appBaseUrl + "PayPeriods/ArchiveEmployeesTimeCard_Ajax",
                        type: "POST",
                        //cache: false,
                        async: false,
                        //dataType: "json",
                        data: {
                            companyCodeId: dataItem.CompanyCodeId,
                            payPeriodId: dataItem.PayPeriodId,

                        },
                        success: function (response) {
                            if (response.result == true) {
                                toastr.success('Record Archived successfully.');
                            }
                            else {
                                toastr.error("No Time Card Records to Archive");
                            }
                            RefreshGrid();
                            return;
                        },
                        error: function (response) {
                            toastr("error!");
                        }
                    })
                }
            }
        },
        error: function (e) {
            toastr("error!");
        }
    });

}

//Global Archive for all Companies
function ArchiveEmployeesGlobalTimeCard(e) {
    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

    $.validator.unobtrusive.parse("#PayPeriodFormDdl");
    var appBaseUrl = $("#appBaseUrl").val();

    $.ajax({
        //url: appBaseUrl + 'PayPeriods/BeforeArchive',
        //type: 'POST',
        //data: {
        //    companyCodeId: dataItem.CompanyCodeId,
        //    payPeriodId: dataItem.PayPeriodId
        //},
        url: appBaseUrl + 'PayPeriods/BeforeGlobalArchive',
        type: 'POST',
        data: {
            payPeriodId: dataItem.PayPeriodId
        },
        //dataType: "json",
        success: function (d) {
            //debugger;
            if (d.IsArchived) {
                toastr.error("Pay Period already Archived");
                return false;
            }
            else if (d.IsEndDate) {
                toastr.error("Pay Period should be Closed");
                return false;
            }
            else if (!d.LockoutEmployees) {
                toastr.error("Please Lockout Employees");
                return false;
            }
            else if (!d.LockoutManagers) {
                toastr.error("Please Lockout Managers");
                return false;
            }
            else if (d.IsEndDate) {
                toastr.error("Pay Period must be Closed.");
                return false;
            }
            else if (d.TimecardCount == 0) {
                toastr.error("No Time Card Records to Archive");
                return false;
            }
            else if (d.Isexported == false)
            {
                toastr.error("Please Export EPIP Before Archive");
                return false;
            }
            else if (d.TimecardUnApprovedCount > 0) {
                var result = confirm("There are unapproved time card records in Pay Period. Are you sure still you want to Archive this Pay Period?");
                if (result) {
                    $.ajax({
                        url: appBaseUrl + "PayPeriods/ArchiveEmployeesGlobalTimeCard_Ajax",
                        type: "POST",
                        //cache: false,
                        async: false,
                        //dataType: "json",
                        data: {
                            payPeriodId: dataItem.PayPeriodId,

                        },
                        success: function (response) {
                            if (response.result == true) {
                                toastr.success('Record Archived successfully.');
                            }
                            else {
                                toastr.error("No Time Card Records to Archive");
                            }
                            RefreshGrid();
                            return;
                        },
                        error: function (response) {
                            toastr("error!");
                        }
                    })
                }
                else {
                    return false;
                }
            }
            else {
                var result = confirm("Are you sure you want to Archive this Pay Period?");
                if (result) {
                    $.ajax({
                        url: appBaseUrl + "PayPeriods/ArchiveEmployeesGlobalTimeCard_Ajax",
                        type: "POST",
                        //cache: false,
                        async: false,
                        //dataType: "json",
                        data: {
                            payPeriodId: dataItem.PayPeriodId
                        },
                        success: function (response) {
                            if (response.result == true) {
                                toastr.success('Record Archived successfully.');
                            }
                            else {
                                toastr.error("No Time Card Records to Archive");
                            }
                            RefreshGrid();
                            return;
                        },
                        error: function (response) {
                            toastr("error!");
                        }
                    })
                }
            }
        },
        error: function (e) {
            toastr("error!");
        }
    });
}

function RefreshGrid() {

    $("#gridPayPeriod").data("kendoGrid").dataSource.read();
}