function gridClientConfigurationEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var formURL = $("#ApplicationUrl").val() + '/ClientConfiguration/ClientConfigurationAddEdit?ClientConfigId=' + data.ClientConfigId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}

function AddNewClientConfiguration() {
    openClientConfigurationPopup(0);
}

function openClientConfigurationPopup(ClientConfigId) {
    var formURL = $("#ApplicationUrl").val() + '/ClientConfiguration/ClientConfigurationAddEdit?ClientConfigId=' + ClientConfigId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}
