function gridEditEmpStatus1(e) {    
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openaddStatusEditpopup(data.EmploymentStatusId);
}
function openaddStatusEditpopup(_EmploymentStatusId) {
    var formURL = $("#ApplicationUrl").val() + '/DdlEmploymentStatuses/StatusesListEditMaintenance?EmploymentStatusId=' + _EmploymentStatusId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}

