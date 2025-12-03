//(document).ready(function () {
function gridPositionBudgetEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    LoadDetailView("PositionBudgetsDetail", "PositionBudgets", { ID: data.ID });
}

function gridPositionBudgetDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { ID: data.ID };
    var formURL = $("#ApplicationUrl").val() + "PositionBudgets/PositionBudgetDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("grid_PositionBudgets");
            }
        });
    });
}
function gridPositionBudgetListEdit(e) {
    var lstitem = "";
    e.preventDefault();
    //$('#ViewAllocationModal').modal('show');
    var row = $(e.target).closest("tr");
    var grid = $("#" + e.delegateTarget.id).data("kendoGrid");
    var dataItem = grid.dataItem(row);
    if (dataItem != null) {
        lstitem = 'ID :' + dataItem.ID;
        lstitem = lstitem + ',FTE :' + dataItem.FTE;
        lstitem = lstitem + ',BudgetYear :' + dataItem.BudgetYear;
        lstitem = lstitem + ',BudgetMonth :' + dataItem.BudgetMonth;
        lstitem = lstitem + ',BudgetAmount :' + dataItem.BudgetAmount;
    }
    var formURL = $("#ApplicationUrl").val() + "PositionSetupDetail/PositionBudgetEditModalPartial?positionBudgetID="+dataItem.ID;
    if (formURL != "") {
        $('#Editpositionbudget_ID').val(lstitem);
        $("#EditPositionBudgetModal .modal-content").html("");
        $("#EditPositionBudgetModal .modal-content").load(formURL);
        $("#EditPositionBudgetModal").modal({ show: true });
    }
    
    
}


function gridPositionBudgetDeleteFromTab(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
   // var positionID = $('#PositionId').val();
    var postData = { ID: data.ID };
    //var formURL = $("#ApplicationUrl").val() + "PositionBudgets/PositionBudgetDelete";
    var formURL = $("#ApplicationUrl").val() + "PositionSetupDetail/PositionBudgetDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                if (data) {
                    $("#PositionBudget").html("");
                    $("#PositionBudget").html(data);
                }
            }
        });
    });
}

function gridPositionBudgetListEditRefresh() {
    var positionBudgetID = $('#PositionBudgetID').val();
    var formURL = $("#ApplicationUrl").val() + "PositionSetupDetail/PositionBudgetEditModalPartial?positionBudgetID=" + positionBudgetID;
    if (formURL != "") {
        $("#EditPositionBudgetModal .modal-content").html("");
        $("#EditPositionBudgetModal .modal-content").load(formURL);
        loading(false);
        $("#EditPositionBudgetModal").modal({ show: true });
    }

}

function gridPositionBudgetListAdd(e) {
    e.preventDefault();
    var positionID = $('#PositionId').val();
    var formURL = $("#ApplicationUrl").val() + "PositionSetupDetail/PositionBudgetAddModalPartial?positionID="+ positionID;
    if (formURL != "") {
        $("#EditPositionBudgetModal .modal-content").html("");
        $("#EditPositionBudgetModal .modal-content").load(formURL);
        $("#EditPositionBudgetModal").modal({ show: true });
    }


}



function gridPositionAdd(e) {
    e.preventDefault();
    var formURL = $("#ApplicationUrl").val() + "Positions/PositionAddModalPartial";
    if (formURL != "") {
        $("#AddPositionModal .modal-content").html("");
        $("#AddPositionModal .modal-content").load(formURL);
        $("#AddPositionModal").modal({ show: true });
    }


}


function addPositionBudgetAllocation(e) {
    e.preventDefault();
    $("#EditFundFlag").val("False");
    var positionBudgetID = $('#PositionBudgetID').val()
    var formURL = $("#ApplicationUrl").val() + "PositionSetupDetail/PositionAddAllocationModalPartial?positionBudgetID=" + positionBudgetID;
    if (formURL != "") {
        $("#AddAllocationsModal .modal-content").html("");
        $("#AddAllocationsModal .modal-content").load(formURL);
        $("#AddAllocationsModal").modal({ show: true });
    }

}

function gridFundDefinitionAdd(e) {
    e.preventDefault();
    var formURL = $("#ApplicationUrl").val() + "PositionSetupDetail/PositionAddFundDefinitionModalPartial";
    if (formURL != "") {
        $("#FundDefinitionModal .modal-content").html("");
        $("#FundDefinitionModal .modal-content").load(formURL);
        $("#FundDefinitionModal").modal({ show: true });
    }

}
function EditPositionFundingList(e) {
    var row = $(e.target).closest("tr");
    var grid = $("#" + e.delegateTarget.id).data("kendoGrid");
    var dataItem = grid.dataItem(row);
    $('#EditPositionFundingSourceID').val(dataItem.id);
    $('#EditPercentage').val(dataItem.FundPercentage);
    $("#EditFundCodeID option:selected").text(dataItem.FundCode);
}


