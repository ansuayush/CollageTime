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
});

function gridTimeCardSessionConfigEdit(e) {

    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);

    openTimeCardSessionConfigPopup(data.TimeCardSessionId);
}
function openTimeCardSessionConfigPopup(_TimeCardSessionId) {

    loading(true);
    var formURL = $("#ApplicationUrl").val() + '/TimeCardSessionInOutConfig/GetTimeCardsessionInoutDetails?timecardssessionId=' + _TimeCardSessionId;

    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);

    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}

function timecardgridsessionRequestEnd(e) {
    
    if (e.type == "update") {
        if (!e.response.Errors) {
            toastr.success('Record Updated successfully.');
            e.sender.read();
            $("#gridTimeCardSession").data("kendoGrid").dataSource.read();
        }
    }    
}