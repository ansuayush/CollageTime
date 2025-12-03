$(document).ready(function () {
    $.validator.unobtrusive.parse("#PerformanceReviewForm");
    var appBaseUrl = $("#appBaseUrl").val();
    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });
});
function gridPerformanceReviewSetupEdit(e) {
    
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openPerformanceReviewDetailEditpopup(data.ID);
}
function openPerformanceReviewDetailEditpopup(_Id) {
    
    var formURL = $("#ApplicationUrl").val() + '/PerformanceReviewSetup/EditPerformanceReviewSetup?Id=' + _Id;
    $.post(formURL, function (data, status) {
        if (data.succeed == false) {
            toastr.error(data.Message);
        } else {
            var $modal = $('#PerformanceEditModal');
            $modal.html(data);
            $modal.modal("show");
        }
    });
}

function gridPerformanceReviewSetupDelete(e) {
   
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { Id: data.ID };
    var formURL = $("#ApplicationUrl").val() + "PerformanceReviewSetup/PerProfileDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPerformanceReviewSetup");
            }
        });
    });
}
function addPerformanceProfile() {
    openPerformanceProfilepopup(0);
}
function openPerformanceProfilepopup(_PerProfileId) {
    var formURL = $("#ApplicationUrl").val() + '/PerformanceReviewSetup/PerformanceReviewSetupDetail?PerProfileId=' + _PerProfileId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}
function gridPerformanceProfileEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    openPerformanceProfileEditpopup(data.PerProfileId);
}
function openPerformanceProfileEditpopup(_PerProfileId) {
    var formURL = $("#ApplicationUrl").val() + '/PerformanceReviewSetup/PerformanceReviewSetupDetail?PerProfileId=' + _PerProfileId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}
function gridPerformanceProfileDelete(e) {
    
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { PerProfileId: data.PerProfileId};
    var formURL = $("#ApplicationUrl").val() + "PerformanceReviewSetup/PerformanceProfileDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPerformanceProfile");
            }
        });
    });
}
