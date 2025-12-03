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

function filterDepartmentsByCompanyCode_SuummaryReport() {
    //
    return {
        CompanyCodeIdDdl: $("#CompanyCodeIdDdl").val()
    };
}

function OnChangeDepartment(e) {
    var dropdown = $("#PayPeriodIdDdl").data("kendoDropDownList");
    dropdown.select(1);
    getApprovalStatus();
    $("#GridPayPeriodSummaryReport").data("kendoGrid").dataSource.read();
}

function filterTimeCardRecords_SummaryReport() {

    return {
        companyCodeId: $("#CompanyCodeIdDdl").val(),
        payPeriodId: $("#PayPeriodIdDdl").val(),
        departmentId: $("#DepartmentIdDdl").val()
    };
}

function OnChange_SummaryReport(e) {
    getApprovalStatus();
    $("#GridPayPeriodSummaryReport").data("kendoGrid").dataSource.read();
};