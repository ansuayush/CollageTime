


function gridEarningsCodeEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openaddEarningsCodeDetailEditpopup(data.EarningsCodeId);
}


function gridEarningsCodeDelete(e) {
    
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { EarningsCodeId: data.EarningsCodeId };
    var formURL = $("#ApplicationUrl").val() + "EarningsCodes/EarningsCodesList_Destroy";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridEarningsCode");
            }
        });
    });
}

function addEarningsCode() {
    openaddEarningsCodeDetailEditpopup(0);
}

function openaddEarningsCodeDetailEditpopup(_earningsCodeId) {
    var formURL = $("#ApplicationUrl").val() + '/EarningsCodes/GetEarningsCodeDetails?earningsCodeId=' + _earningsCodeId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}