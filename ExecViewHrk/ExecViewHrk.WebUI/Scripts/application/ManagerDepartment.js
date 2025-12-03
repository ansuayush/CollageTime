
function gridManagerDepartmentEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openManagerDepartmentEditpopup(data.ManagerDepartmentId);
}


function gridManagerDepartmentDelete(e) {    
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { managerDepartmentId: data.ManagerDepartmentId };
    var formURL = $("#ApplicationUrl").val() + "ManagerDepartments/ManagerDepartmentsList_Destroy";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridManagerDepartment");
            }
        });
    });
}

function addManagerDepartment() {
    openManagerDepartmentEditpopup(0);
}

function openManagerDepartmentEditpopup(_managerDepartmentId) {
    var formURL = $("#ApplicationUrl").val() + '/ManagerDepartments/ManagerDepartmentDetail?managerDepartmentId=' + _managerDepartmentId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}