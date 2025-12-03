function gridPositionDetailEdit(e) {
    e.preventDefault();
    var row = $(e.target).closest("tr");
    var grid = $("#" + e.delegateTarget.id).data("kendoGrid");
    var dataItem = grid.dataItem(row);
    var data = { positionID: dataItem.PositionId };
    LoadDetailView('PositionSetupDetail', 'PositionSetupDetail', data);
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