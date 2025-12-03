
$(document).ready(function () {
    $.validator.unobtrusive.parse("#EmployeeI9DocumentForm");
    var appBaseUrl = $("#appBaseUrl").val();
    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });
    $("#btnSaveEmployeeI9DocumentAjax").click(function (e) {
        e.preventDefault();
        var form = $("#EmployeeI9DocumentForm");
        
        var staus = formService.getStatus();
        if (staus) {
            return;
        }
        var errorMessage = formService.validate(form);
        if (errorMessage) {
            return;
        }
        var employeeId = $("#EmployeeIdDdl").val();
        if (!employeeId) { toastr.error(formService.messages.selectEmploymentNumber); return } 
        var serialized_Object = form.serializeObject();
        serialized_Object.EmployeeId = employeeId;
        serialized_Object.PresentedDate = getFormattedDate('PresentedDate');
        serialized_Object.ExpirationDate = getFormattedDate('ExpirationDate');
        var isOk = formService.CompareDates("PresentedDate", "ExpirationDate",'Date Delivered', 'Expires Date');
        if (!isOk) { return };
        formService.isSavingInProgress = true;
    //     toastr.success(formService.messages.saving);
        $.ajax({

            url: appBaseUrl + "EmployeeI9Documents/EmployeeI9DocumentsSaveAjax",

            type: "POST",

            data: {
                employeeI9DocumentVm: serialized_Object,
                //employeeId: employeeId,
            },

            success: function (response) {
                var _oldpersonEmployeeI9DocumentId = $("#hiddenEmployeeI9DocumentId").val();
                refreshEmployeeI9DocumentFormData(response);
                formService.isLoadingInProgress = true;
                killTimer();
                if (_oldpersonEmployeeI9DocumentId != "0") {
                    timeOutFn = setTimeout(function () {
                        formService.isSavingInProgress = false;
                        toastr.success(formService.messages.updated);
                        formService.isLoadingInProgress = false;
                    }, 0);
                }
                else {
                    timeOutFn = setTimeout(function () {
                        formService.isSavingInProgress = false;
                        toastr.success(formService.messages.saved);
                        formService.isLoadingInProgress = false;
                    }, 0);
                }
          },
        error: function () {
            formService.isSavingInProgress = false;
        }
        });
    });

    $("#btnClearEmployeeI9DocumentAjax").click(function (e) {
        e.preventDefault();

        var requestType = $("#requestType").val();
        var personId = $("#hiddenPersonId").val();
        var employeeId = $("#EmployeeIdDdl").val()? $("#EmployeeIdDdl").val():0;
        var form = $("#EmployeeI9DocumentForm");
        $.ajax({

            url: appBaseUrl + "EmployeeI9Documents/EmployeeI9DocumentsIndexChangedAjax",

            type: "POST",

            data: {

                employeeI9DocumentIdDdl: 0,
                personId: personId,
                employeeId: employeeId
            },

            success: function (response) {
                formService.reset(form);
                refreshEmployeeI9DocumentFormData(response);
                toastr.success(formService.messages.cleared);
                return;
            }

        });

    });

    $("#btnDeleteEmployeeI9DocumentAjax").click(function (e) {
        e.preventDefault
        confirmDialog("", function () {
            var personId = $("#hiddenPersonId").val();
            var employeeI9DocumentId = $("#hiddenEmployeeI9DocumentId").val();

            $.ajax({

                url: appBaseUrl + "EmployeeI9Documents/EmployeeI9DocumentsDeleteAjax",

                type: "POST",

                data: {
                    employeeI9DocumentIdDdl: employeeI9DocumentId,
                    personId: personId
                },

                success: function (response) {
                    if (response == formService.messages.employeeI9DocumentNotExists) {
                        toastr.error(response);
                        return;
                    }
                    refreshEmployeeI9DocumentFormData(response);
                    toastr.success(formService.messages.deleted);
                    return;

                }
            });
        });
    });

});
function onSelectEmployeeI9DocumentDdl(e) {
    appBaseUrl = $("#appBaseUrl").val();
    var dataItem = this.dataItem(e.item.index() + 1)
    var ddlSelectedEmployeeI9DocumentId = 0
    if (e.item.html() != '- select one -') {
        ddlSelectedEmployeeI9DocumentId = (dataItem.EmployeeI9DocumentId == "" ) ? 0 : dataItem.EmployeeI9DocumentId;
    }  
    var requestType = $("#requestType").val();
    var personId = $("#hiddenPersonId").val();
    var employeeId = $("#EmployeeIdDdl").val() ? $("#EmployeeIdDdl").val():0;
    formService.isLoadingInProgress = true;
    $.ajax({
        url: appBaseUrl + "EmployeeI9Documents/EmployeeI9DocumentsIndexChangedAjax",
        type: "POST",
        data: {
            employeeI9DocumentIdDdl: ddlSelectedEmployeeI9DocumentId,
            personId: personId,
            employeeId: employeeId,
        },

        success: function (response) {
            refreshEmployeeI9DocumentFormData(response);
             formService.isLoadingInProgress = false;
        }

    });
}
function onCascadeEmployeeI9DocumentDdl(e) {
    formService.clear();
}  
function onDataBoundEmployeeI9DocumentDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noEmployeeI9DocumentRecordsFound').hide();
    else
        $('#noEmployeeI9DocumentRecordsFound').show();
}


function onDataBoundEmployeeDocumentDdl(e) {
    var ds = this.dataSource.data();
    if (ds.length > 0)
        $('#noEmployeeRecordsFound').hide();
    else
        $('#noEmployeeRecordsFound').show();
}

function error_handler(e) {
    if (e.errors) {
        toastr.error(e.errors);
    }
}

function refreshEmployeeI9DocumentFormData(response) {
    $("#hiddenPersonId").val(response.PersonId);
    $("#hiddenEmployeeI9DocumentId").val(response.EmployeeI9DocumentId);
   
    $('#formDiv input[type=radio]').prop('checked', false);
    $('#formDiv input:checkbox').prop('checked', false);

    if (response.I9DocumentTypeDescription != null)
        $("#I9DocumentTypeDescription").data("kendoComboBox").value(response.I9DocumentTypeDescription);
    else
        $("#I9DocumentTypeDescription").data("kendoComboBox").value("");

    var datepicker = $("#PresentedDate").data("kendoDatePicker");
    if (response.PresentedDate != null) {
        var date = new Date(parseInt(response.PresentedDate.substr(6)));
        $("#PresentedDate").val(date.toLocaleDateString());
        datepicker.value(date);
    }
    else
        datepicker.value("");

    var datepicker2 = $("#ExpirationDate").data("kendoDatePicker");
    if (response.ExpirationDate != null) {
        var date2 = new Date(parseInt(response.ExpirationDate.substr(6)));
        $("#ExpirationDate").val(date2.toLocaleDateString());
        datepicker2.value(date2);
    }
    else
        datepicker2.value("");


    $("#enteredByLabel").text(response.EnteredBy == null ? "" : response.EnteredBy);

    if (response.EnteredDate != null &&  response.EnteredBy != null) {
        var date = new Date(parseInt(response.EnteredDate.substr(6)));
        $("#enteredDateLabel").text(date.toLocaleString());
    }
    else
        $("#enteredDateLabel").text("");

    $("#modifiedByLabel").text(response.ModifiedBy == null ? "" : response.ModifiedBy);

    if (response.ModifiedDate != null) {
        var date = new Date(parseInt(response.ModifiedDate.substr(6)));
        $("#modifiedDateLabel").text(date.toLocaleString());
    }
    else
        $("#modifiedDateLabel").text("");

    $("#personNameLabel").html(" - " + response.PersonName);

    $("#Notes").val(response.Notes);

    var dropdownlist = $("#EmployeeIdDdl").data("kendoDropDownList");
    dropdownlist.refresh();
    dropdownlist.dataSource.read();
    formService.isLoadingInProgress = true;
    if (response.EmployeeId != 0) {
        setTimeout(function () {
            dropdownlist.value(response.EmployeeId);
        },500)
       
    }
    else {
        dropdownlist.select(0);
    }

     var dropdownlist1 = $("#EmployeeI9DocumentIdDdl").data("kendoDropDownList");
    dropdownlist1.refresh();
    dropdownlist1.dataSource.read();
    if (response.EmployeeI9DocumentId != 0) {
        formService.isLoadingInProgress = true;
        setTimeout(function () {
            dropdownlist1.value(response.EmployeeI9DocumentId);
            formService.isLoadingInProgress = false;
        }, 1000)
       
    }
    else {
        dropdownlist.select(0);
    }

    $(".validation-summary-errors").empty();

    if ($("#hiddenEmployeeI9DocumentId").val() > 0) {
        var form = $("#EmployeeI9DocumentForm");
        form.validate();
        if (!form.valid())
            toastr.error(formService.messages.invalidMessage);
    }

    return;
}

function error_handler1(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        toastr.error(message);

        $('#gridEmployeeI9Document').data('kendoGrid').dataSource.read();
        $('#gridEmployeeI9Document').data('kendoGrid').refresh();
    }
}


function filterI9Docs() {
    var obj = { EmployeeIdDdl : 0}
        obj.EmployeeIdDdl = $("#EmployeeIdDdl").val();
        return obj
}




//$(document).ready(function () {
//    
//    $.validator.unobtrusive.parse("#EmployeeI9DocumentForm");
//    var appBaseUrl = $("#appBaseUrl").val();
//});


//    function onSelectEmployeeDdl(e) {
//        
//        appBaseUrl = $("#appBaseUrl").val();
//        var dataItem = this.dataItem(e.item.index() + 1);

//        var ddlSelectedEmployeeId = dataItem.EmployeeId == "" ? 0 : dataItem.EmployeeId;

//        var requestType = $("#requestType").val();
//        var personId = $("#hiddenPersonId").val();

//        $.ajax({

//            //url: "@Url.Action("PersonAdaIndexChangedAjax", "Person")",
//            url: appBaseUrl + "EmployeeI9Documents/EmployeeI9DocumentsIndexChangedAjax",

//            type: "POST",

//            data: {

//                employeeIdDdl: ddlSelectedEmployeeId,
//                personId: personId
//                //,requestType: requestType
//            },

//            success: function (response) {
//                

//                //refreshPersonEmployeeFormData(response);
//                return;
//            }

//        });
//    }

//    function onDataBoundEmployeeDdl(e) {
//        var ds = this.dataSource.data();
//        if (ds.length > 0)
//            $('#noEmployeeRecordsFound').hide();
//        else
//            $('#noEmployeeRecordsFound').show();
//    }

//    function error_handler(e) {
//        
//        if (e.errors) {
//            toastr.error(e.errors);
//        }
//    }
