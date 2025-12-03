// For DesignateSupervisorController

function DeleteDesignatedSupervisorJs(e, url, managerId) {
    confirmDialog("", function () {
        loading(true);
        e.preventDefault();
        $.ajax({
            type: "GET",
            url: url,
            data: { ManagerPersonId: managerId },
            cache: false,
            success: function (data) {
                if (data != null) {
                    loading(false);
                    toastr.success("Record deleted successfully");
                    $("#gridDesignatedSupervisor").data("kendoGrid").dataSource.read();
                }
            },
            onerror: function (ex) {
                loading(false);
                toastr.error(ex.Message);
            }
        });
    });
}

function addDesignatedSupervisor(e) {
    e.preventDefault();
    var formURL = $("#ApplicationUrl").val() + "DesignateSupervisor/_NewDesignatedSupervisor";
    $("#AddDesignatedSupervisorModal .modal-content").html("");
    $("#AddDesignatedSupervisorModal .modal-content").load(formURL);
    $("#AddDesignatedSupervisorModal").modal({ show: true });
}