function addNewManager() {
    openManagerpopup(0);
}
function openManagerpopup(_managerId) {
    var formURL = $("#ApplicationUrl").val() + '/Managers/AddNewManager?managerId=' + _managerId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}
function gridManagerEdit(e) {
    
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openManagerListDetailEditpopup(data.ManagerId);
}
function openManagerListDetailEditpopup(_ManagerId) {
    
    var formURL = $("#ApplicationUrl").val() + '/Managers/EditManager?managerId=' + _ManagerId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}
function gridManagerDelete(e) {
   
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { managerId: data.ManagerId };
    var formURL = $("#ApplicationUrl").val() + "Managers/ManagerDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridManager");
            }
        });
    });
}