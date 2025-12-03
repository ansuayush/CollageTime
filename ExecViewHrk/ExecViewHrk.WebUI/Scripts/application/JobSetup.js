function gridJobSetuptEdit(e) {
 
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    LoadDetailView("JobSetupDetail", "JobSetup", { jobid: data.Jobid })
}


function gridJobSetuptNew() {
    openJobSetuptNewpopup(0);
}

function openJobSetuptNewpopup(JobId) {
    var formURL = $("#ApplicationUrl").val() + '/JobSetup/AddNewJob?JobId=' + JobId;
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
        $("#DDLAddModal .modal-content").html("");
        $("#DDLAddModal .modal-content").load(formURL);
        $("#DDLAddModal").modal({ show: true });
}
}
//$("#BtnNewJobSave").click(function (e) {
//    e.preventDefault();

//    var form = $("#JobSetupForm");
//    var staus = formService.getStatus();
//    if (staus) {
//        return;
//    }
//    var errorMessage = formService.validate(form);
//    if (errorMessage) {
//        return;
//    }
//    var appBaseUrl = $("#appBaseUrl").val();
//    var postData = $("#JobSetupForm").serializeArray();

//    var formURL = appBaseUrl + "JobSetup/JobSaveAjax";
//    $.post(formURL, postData, function (data, status) {
//        if (status == "success") {
//            
//            if (data.recordIsNew == true) {
//                LoadMenu("JobSetupList", "JobSetup")
//                formService.isLoadingInProgress = true;
//                killTimer();
//                timeOutFn = setTimeout(function () {
//                    formService.isSavingInProgress = false;
//                    toastr.success(formService.messages.saved);
//                    formService.isLoadingInProgress = false;
//                }, 0);
//            }
//            else {
//                var Jobdata = { jobid: data.JobSetUp.Jobid };
//                LoadDetailView("JobSetupDetail", "JobSetup", Jobdata)
//                formService.isLoadingInProgress = true;
//                killTimer();
//                timeOutFn = setTimeout(function () {
//                    formService.isSavingInProgress = false;
//                    toastr.success(formService.messages.updated);
//                    formService.isLoadingInProgress = false;
//                }, 0);
//            }

//        }
//    });
//});

