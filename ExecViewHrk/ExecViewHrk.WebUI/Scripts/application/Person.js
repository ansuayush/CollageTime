$(document).ready(function () {
   
    var appBaseUrl = $("#appBaseUrl").val();
    formService.isLoadingInProgress = true;
    $(document).ajaxStop(function () {
        formService.initialize();
    });
 
    //$("#btnClearPersonAjax").click(function (e) {

    //    e.preventDefault();

    //    var requestType = $("#requestType").val();
    //    var personId = $("#hiddenPersonId").val();

    //    $.ajax({

    //        url: appBaseUrl + "Person/PersonClearAjax",

    //        type: "POST",

    //        data: {

    //            personAdaIdDdl: 0,
    //            personId: personId
    //            //,requestType: requestType
    //        },

    //        success: function (response) {
               
    //            refreshPersonFormData(response);
    //            return;
    //        }

    //    });

    //});

    //$("#btnDeletePersonAjax").click(function (e) {

    //    e.preventDefault();

      
    //    var personId = $("#hiddenPersonId").val();
    //    var personAdaId = $("#hiddenPersonAdaId").val();

    //    $.ajax({

    //        //url: "@Url.Action("PersonAdaDeleteAjax", "Person")",
    //        url: appBaseUrl + "Personal/PersonDeleteAjax",

    //        type: "POST",

    //        data: {
    //            personAdaIdDdl: personAdaId,
    //            personId: personId
    //        },

    //        success: function (response) {

    //            if (response == formService.messages.IdentityNotExits) {
    //                toastr.error(response);
    //                return;
    //            }

    //            refreshPersonFormData(response);
    //            return;
    //        }
    //    });
    //});

  
});


function refreshPersonFormData(response) {
    $("#hiddenPersonId").val(response.PersonId);
    //$('#formDiv input[type=radio]').prop('checked', false);
    //$('#formDiv input:checkbox').prop('checked', false);


    $("#Ssn").val(response.Ssn);
    $("#LastName").val(response.LastName);
    $("#FirstName").val(response.FirstName);
    $("#MiddleName").val(response.MiddleName);
    $("#PreferredName").val(response.PreferredName);
    $("#MaidenName").val(response.MaidenName);
    $("#Email").val(response.Email);
    $("#AlternateEmail").val(response.AlternateEmail);

    var datePicker = $("#DateOfBirth").data("kendoDatePicker");
    if (response.DateOfBirth != null) {
        var date = new Date(parseInt(response.DateOfBirth.substr(6)));
        $("#DateOfBirth").val(date.toLocaleDateString());
        datePicker.value(date);
    }
    else
        datePicker.value("");

    if (response.PrefixId != null)
        $("#PrefixId").data("kendoDropDownList").value(response.PrefixId);
    else
        $("#PrefixId").data("kendoDropDownList").value(0);

    if (response.SuffixId != null)
        $("#SuffixId").data("kendoDropDownList").value(response.SuffixId);
    else
        $("#SuffixId").data("kendoDropDownList").value(0);

    $('input[name="GenderId"]').val([response.GenderId]);


    if (response.MaritalStatusId != null)
        $("#MaritalStatusId").data("kendoDropDownList").value(response.MaritalStatusId);
    else
        $("#MaritalStatusId").data("kendoDropDownList").value(0);

    $('input[name=IsDependent]').val([response.IsDependent]);

    $("#enteredByLabel").text(response.EnteredBy == null ? "" : response.EnteredBy);

    if (response.EnteredDate != null) {
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

    if (response.PersonId == null)
        $("#personNameLabel").html(" ");
    else
        $("#personNameLabel").html(" - " + response.LastName + ", " + response.FirstName);

    var dropdownlist = $("#ddlPersonId").data("kendoDropDownList");
    if (response.PersonId != 0) {
        dropdownlist.refresh();
        dropdownlist.dataSource.read();
        dropdownlist.value(response.PersonId);
        $("#Email").attr("readonly", true);
    }
    else {
        dropdownlist.select(0);
        $("#Email").attr("readonly", false);
    }

    $(".validation-summary-errors").empty();

    if ($("#hiddenPersonId").val() > 0) {
        var form = $("#PersonForm");

        form.validate();
        if (!form.valid())
            toastr.error(formService.messages.invalidMessage);
    }

    return;
}


function editPersonImage() {
    openPersonImagePopup(0);
}

function openPersonImagePopup(_personId) {
    var formURL = $("#ApplicationUrl").val() + '/Person/EditImage?personId=' + _personId;
    $("#QuickAddModal .modal-content").html("");
    $("#QuickAddModal .modal-content").load(formURL);
    $("#QuickAddModal").modal({ show: true });
}




