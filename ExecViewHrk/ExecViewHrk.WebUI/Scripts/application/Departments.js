


function gridDepartmentEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openDepartmentDetailEditpopup(data.DepartmentId);
}


function gridDepartmentDelete(e) {
    
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { DepartmentId: data.DepartmentId };
    var formURL = $("#ApplicationUrl").val() + "Departments/DepartmentsList_Destroy";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridDepartments");
            }
        });
    });
}

function addDepartment() {
    openDepartmentDetailEditpopup(0);
}

function openDepartmentDetailEditpopup(_DepartmentId) {
    var formURL = $("#ApplicationUrl").val() + '/Departments/DepartmentDetail?departmentId=' + _DepartmentId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}