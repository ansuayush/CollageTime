function gridFedralEINEdit(e) {    
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var formURL = $("#ApplicationUrl").val() + '/LookupTables/AddEditPopupFedralEIN?FedralEINNbr=' + data.FedralEINNbr;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}

function gridFedralEINDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { FedralEINNbr: data.FedralEINNbr };
    var formURL = $("#ApplicationUrl").val() + "LookupTables/FedralEINDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridFedralEIN");
            }
        });
    });
}
function AddEditPopupFedralEIN(_fedralEINNbr) {    
    var formURL = $("#ApplicationUrl").val() + '/LookupTables/AddEditPopupFedralEIN?FedralEINNbr=' + _fedralEINNbr;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}

function AddPopupFedralEIN() {
    var formURL = $("#ApplicationUrl").val() + '/LookupTables/AddPopupFedralEINforDDL?FedralEINNbr=' + 0;
    $("#DDLBusinesslevelModal .modal-content").html("");
    $("#DDLBusinesslevelModal .modal-content").load(formURL);
    $("#DDLBusinesslevelModal").modal({ show: true });
}