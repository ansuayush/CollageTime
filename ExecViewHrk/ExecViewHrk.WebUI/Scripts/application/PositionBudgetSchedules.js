function gridPositionBudgetSchedulesEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    LoadDetailView("PositionBudgetSchedulesDetails", "PositionBudgetSchedules", { ID: data.ID }); 
}

function gridPositionBudgetSchedulesDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { ID: data.ID };
    var formURL = $("#ApplicationUrl").val() + "PositionBudgetSchedules/PositionBudgetSchedulesDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPositionBudgetSchedules");
            }
        });
    });
}
function AddNewBudgetSchedule() {
    openBudgetSchedulePopup(0);
}

function openBudgetSchedulePopup(Id) {
    var formURL = $("#ApplicationUrl").val() + '/PositionBudgetSchedules/PositionBudgetSchedulesAddEdit?ID=' + Id;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}
