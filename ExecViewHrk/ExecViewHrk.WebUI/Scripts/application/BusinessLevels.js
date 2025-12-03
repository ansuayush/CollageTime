function gridBusinessLevelsEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);

    LoadDetailView("BusinessLevelsDetailMatrixPartial", "BusinessLevels", { BusinessLevelNbr: data.BusinessLevelNbr })

  
}
function gridDeleteBusinessLevels(e) {
  
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { BusinessLevelNbr: data.BusinessLevelNbr };
    var formURL = $("#ApplicationUrl").val() + "BusinessLevels/PositionBusinessLevelsList_Destroy";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPositionBusinessLevels");
            }
        });
    });
}
function gridPositionDetailExpand(e) {
    e.preventDefault();
    var grid = e.sender;
    grid.tbody.find("tr.k-master-row").each(function () {
        var selectedRow = e.masterRow;
        var row = $(this);
        var isSelectedRowClicked = (row[0] == selectedRow[0]);
        if (!isSelectedRowClicked) {
            grid.collapseRow(row)
        };
    })
}
function addBusinessLevels() {
    openBusinessLevelsEditpopup(0);
}

function openBusinessLevelsEditpopup(_businessLevelsId) {
    var formURL = $("#ApplicationUrl").val() + '/BusinessLevels/BusinessLevelsPartial?BusinessLevelNbr=' + _businessLevelsId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}
function AddDDLJobsetup(e, type) {
    e.preventDefault();
    var formURL = "";

    formURL = $("#ApplicationUrl").val() + 'JobSetup/DDLPartialJobSetup?type=' + type;
    if (formURL != "") {
        $("#DDLBusinesslevelModal .modal-content").html("");
        $("#DDLBusinesslevelModal .modal-content").load(formURL);
        $("#DDLBusinesslevelModal").modal({ show: true });
    }
}
function AddDDLEIN(e, type) {
    e.preventDefault();
    var formURL = "";

    formURL = $("#ApplicationUrl").val() + 'BusinessLevels/AddDDLBusinessLevels?type=' + type;
    if (formURL != "") {
        $("#DDLBusinesslevelModal .modal-content").html("");
        $("#DDLBusinesslevelModal .modal-content").load(formURL);
        $("#DDLBusinesslevelModal").modal({ show: true });
    }
}
//function AddPopupFedralEIN() {
  
//    var formURL = $("#ApplicationUrl").val() + '/LookupTables/AddPopupFedralEINforDDL?FedralEINNbr=' + 0;
//    $("#DDLBusinesslevelModal .modal-content").html("");
//    $("#DDLBusinesslevelModal .modal-content").load(formURL);
//    $("#DDLBusinesslevelModal").modal({ show: true });
//}
