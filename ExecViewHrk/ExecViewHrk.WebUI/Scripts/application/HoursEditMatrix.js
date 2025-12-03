
function gridHourEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openhoursCodeEditpopup(data.HoursCodeId);
}

function addHoursCode() {
    openhoursCodeEditpopup(0);
};
function openhoursCodeEditpopup(_HoursCodeId) {
    var formURL = $("#ApplicationUrl").val() + '/HoursCodes/HoursEditMatrix?HoursCodeId=' + _HoursCodeId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}



function gridHoursDelete(e) {
    //debugger;
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { HoursCodeId: data.HoursCodeId };
    var formURL = $("#ApplicationUrl").val() + "HoursCodes/HoursCodesDeleteAjax";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridHoursCode");
            }
        });
    });
}




//$("#btnDeletePersonEmployeeAjax").click(function (e) {
//    e.preventDefault();
//    confirmDialog("", function () {
//        appBaseUrl = $("#appBaseUrl").val();
//        var personId = $("#hiddenPersonId").val();
//        var personEmployeeId = $("#hiddenPersonEmployeeId").val();
//        $.ajax({
//            url: appBaseUrl + "PersonEmployees/PersonEmployeesDeleteAjax",
//            type: "POST",
//            data: {
//                personEmployeeIdDdl: personEmployeeId,
//                personId: personId
//            },
//            success: function (response) {
//                if (response.succeed == false) {
//                    toastr.error(response.Message);
//                    return;
//                }
//                refreshPersonEmployeeFormData(response.personEmployeeVm);
//                toastr.success(formService.messages.deleted);
//                return;
//            }
//        });
//    });
//});
