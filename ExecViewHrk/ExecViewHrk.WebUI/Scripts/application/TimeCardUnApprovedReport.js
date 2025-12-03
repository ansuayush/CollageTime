/* TimeCardUnApprovedReport.js */

function ViewTimecardDetails(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var companyCodeId = $("#CompanyCodeIdDdl").val();
    var payPeriodId = $("#PayPeriodIdDdl").val();
    var postData = { companyCodeId: companyCodeId, departmentId: data.DepartmentId, employeeId: data.EmployeeId, payPeriodId: payPeriodId };
    LoadContents("TimeCardsMatrixPartialFromUnapproval", "TimeCardMatrix", postData);
}