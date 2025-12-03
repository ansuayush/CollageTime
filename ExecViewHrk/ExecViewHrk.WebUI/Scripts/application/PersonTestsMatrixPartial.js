$(document).ready(function () {
     // TODO
});

//function refreshPersonTestFormData(response) {
//    if (response.PersonId) {
//        $("#hiddenPersonId").val(response.PersonId);
//    } else {
//        $("#hiddenPersonId").val("");
//    }

//    $("#hiddenPersonTestId").val(response.PersonTestId);
//    //$("#PersonTestForm input:text").val("");
//    $('#PersonTestForm input[type=radio]').prop('checked', false);
//    $('#PersonTestForm input:checkbox').prop('checked', false);

//    //
//    if (response.EvaluationTestDescription != null)
//        $("#EvaluationTestDescription").data("kendoComboBox").value(response.EvaluationTestDescription);
//    else
//        $("#EvaluationTestDescription").data("kendoComboBox").value("");

//    var datepicker = $("#TestDate").data("kendoDatePicker");
//    if (response.TestDate != null) {
//        var date = new Date(parseInt(response.TestDate.substr(6)));
//        $("#TestDate").val(date.toLocaleDateString());
//        datepicker.value(date);
//    }
//    else
//        datepicker.value("");


//    $("#Score").val(response.Score == null ? "" : response.Score);
//    $("#Grade").val(response.Grade == null ? "" : response.Grade);
//    $("#Administrator").val(response.Administrator == null ? "" : response.Administrator);

//    $("#enteredByLabel").text(response.EnteredBy == null ? "" : response.EnteredBy);

//    if (response.EnteredDate != null && response.EnteredBy != null) {
//        var date = new Date(parseInt(response.EnteredDate.substr(6)));
//        $("#enteredDateLabel").text(date.toLocaleString());
//    }
//    else
//        $("#enteredDateLabel").text("");

//    //$("#modifiedByLabel").text(response.ModifiedBy == null ? "" : response.ModifiedBy);

//    //if (response.ModifiedDate != null) {
//    //    var date = new Date(parseInt(response.ModifiedDate.substr(6)));
//    //    $("#modifiedDateLabel").text(date.toLocaleString());
//    //}
//    //else
//    //    $("#modifiedDateLabel").text("");

//    $("#personNameLabel").html(" - " + response.PersonName);

//    $("#Notes").val(response.Notes);

//    var dropdownlist = $("#PersonTestIdDdl").data("kendoDropDownList");
//    dropdownlist.refresh();
//    dropdownlist.dataSource.read();
//    if (response.PersonTestId != 0) {
//        dropdownlist.value(response.PersonTestId);
//    }
//    else {
//        dropdownlist.select(0);
//    }

//    $(".validation-summary-errors").empty();

//    if ($("#hiddenPersonTestId").val() > 0) {
//        var form = $("#PersonTestForm");

//        form.validate();
//        if (!form.valid())
//            toastr.error(formService.messages.invalidMessage);
//    }

//    appBaseUrl = $("#appBaseUrl").val();
//    $.ajax({
//        url: appBaseUrl + "PersonTests/GetPersonTestsList",
//        type: "POST",
//        success: function (response2) {
//            //
//            $("#PersonTestIdDdl").data("kendoDropDownList").value(response.PersonTestId);
//        }
//    });

//    return;
//}

function gridPersonTestEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);

    openPersonTestPopup(data.PersonTestId, data.PersonId);
}

function gridPersonTestDelete(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var postData = { personTestId: data.PersonTestId };
    var formURL = $("#ApplicationUrl").val() + "PersonTests/PersonTestsDelete";
    confirmDialog("", function () {
        $.post(formURL, postData, function (data, status) {
            if (data.succeed == false) {
                toastr.error(data.Message);
            } else {
                toastr.success('Record deleted successfully.');
                resetGrid("gridPersonTest");
            }
        });
    });
}

function addPersonTest() {
    openPersonTestPopup(0, $("#SelectedPersonID").val());
}

function openPersonTestPopup(_personTestId, _personId) {
    loading(true);
    var formURL = $("#ApplicationUrl").val() + '/PersonTests/PersonTestsDetail?personTestId=' + _personTestId + '&personId=' + _personId;

    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);

    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');

}