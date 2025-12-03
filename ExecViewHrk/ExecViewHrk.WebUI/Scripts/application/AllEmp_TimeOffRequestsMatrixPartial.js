$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

$(document).ready(function () {
    //events = [];
    
    var TodayDate = new Date();
    RefreshCalendar(TodayDate.getFullYear(), TodayDate.getMonth() + 1);

});


function RefreshCalendar(year, month) {
    

    $.validator.unobtrusive.parse("#AllTimeOffRequestsFormDdl");
    var appBaseUrl = $("#appBaseUrl").val();
    $.ajax({
        url: appBaseUrl + "TimeOffRequests/GetAllTimeOffRequestsDB_Ajax",
        type: "POST",
        data: {
            selectedYear: year,
            selectedMonth: month,
        },
        success: function (response) {
            timeOffRequestDates = [];
            var today = new Date();

            if (response.TimeOffRequestsList != null && response.TimeOffRequestsList.length != 0) {
                $.each(response.TimeOffRequestsList, function (TRindex, TRmodel) {
                    //convert json dates to javascript format
                    response.TimeOffRequestsList[TRindex].timeOffRequestDate = new Date(parseInt(response.TimeOffRequestsList[TRindex].timeOffRequestDate.replace('/Date(', '')));
                    //timeOffRequestDates[TRindex] = response.TimeOffRequestsList[TRindex].timeOffRequestDate;
                });
                timeOffRequestDates = response.TimeOffRequestsList;
            }

            var Existingcalendar = $('#calendar').data('kendoCalendar');
            if (Existingcalendar != null) {
                $('#calendar').data('kendoCalendar').destroy();
                $('#calendar').empty();
            }

            $("#calendar").kendoCalendar({
                //value: new Date().toJSON().slice(0, 10).replace(/-/g, '/'), 
                value: new Date(year, month - 1, 1),
                month: {
                    content: "# var test = isInArray(data.date, timeOffRequestDates); #" +
                            "# if(test.equal == 0){#" +
                                    //"# toastr.info(\"Pending \" + test.equal + \"  \" + test.request); #" +
                                     //<span style=\"color:blue\">R- #= test.request#</span>,
                                    "<div class=\"requests\"> <br> #= data.value #&nbsp&nbsp <br> R- #= test.pendingReq#,  A- #= test.approvedReq#,  D- #= test.disapprovedReq#&nbsp&nbsp </div>" +
                            "#} else {#" +
                                "<div>#= data.value #</div>" +
                             "#}#"
                },

            });

            return;
        }
        ,
        complete: function () {
            
            var calendar = $("#calendar").data("kendoCalendar");
            calendar.setOptions({
                change: function (e) {
                    e.preventDefault();
                    openDateOffRequestsDialog();
                }
            });

            calendar.bind("navigate", function (e) {
                e.preventDefault();
                navigate_Calendar();
            });
        }
    });
}



function navigate_Calendar() {
    var month = $("#calendar").data("kendoCalendar")._current.getMonth() + 1;
    var year = $("#calendar").data("kendoCalendar")._current.getFullYear();
    //toastr.info(month + " " + year);
    RefreshCalendar(year, month);
    return;
}

function isInArray(date, events) {
    // (0 "true") equal 
    for (var i = 0; i < events.length; i++) {
        if (date.getFullYear() == events[i].timeOffRequestDate.getFullYear() && date.getMonth() == events[i].timeOffRequestDate.getMonth() && date.getDate() == events[i].timeOffRequestDate.getDate()) {
            return { equal: 0, request: events[i].totalRequests, pendingReq: events[i].pendingRequests, approvedReq: events[i].approvedRequests, disapprovedReq: events[i].disapprovedRequests };
        }
    }
    return { equal: 1 };
}


function openDateOffRequestsDialog() {
    debugger
    //e.preventDefault();

    calval = $("#calendar").data("kendoCalendar").value();
    $.validator.unobtrusive.parse("#TimeOffRequestsByDate");
    var appBaseUrl = $("#appBaseUrl").val();
    $("#TimeOffRequestsByDate").kendoWindow({
        width: "50%", //"900px",
        //height: "31%",  //"400px",
        position: { top: 300, left: 120 },
        title: "Time off Requests",
        actions: [
            "Maximize",
            "Close"
        ],
        //scrollable: true,
        modal: true,
        //close: onCloseRangeSelectionWindow,

        content: {
            url: appBaseUrl + "TimeOffRequests/TimeOffRequestsByDatePartial",
            data: {
                selectedDate: kendo.toString(calval, 'd'),
            },           
        }        
    })
    $("#TimeOffRequestsByDate").data("kendoWindow").open().center();

}

function onCloseTimeOffDateWindow() {
    
    $('#TimeOffRequestsByDate').data("kendoWindow").close();
}

//function openRangeSelectionDialog() {
//    //$.validator.unobtrusive.parse("#RangeSelection");
//    //var appBaseUrl = $("#appBaseUrl").val();

//    //var appBaseUrl = "@Url.Content("~/")";
//    //appBaseUrl = appBaseUrl + '/Views/TimeOffRequests/RangeSelectionPartial.cshtml';
//    //Views / TimeOffRequests / RangeSelectionPartial.cshtml
//    //var path = window.location.pathname;
//    //window.open("@(Url.Action("Index", "Home", new { id= 1}))", "My test page")
//    //window.open('/TimeOffRequests/RangeSelectionPartial', '_blank', "height=200,width=200")
//    //$.validator.unobtrusive.parse("#RangeSelectionFormDdl");
//    //var appBaseUrl = $("#appBaseUrl").val();

//    //appBaseUrl = appBaseUrl + "TimeOffRequests/RangeSelectionPartial";
//    //window.open(appBaseUrl, "RangeSelection","height=400,width=400");
//    //window.open('@Url.Action("DdlAccommodationTypesPartial","LookupTables")');
//    //document.location = '@Url.Action("RangeSelectionPartial", "TimeOffRequests")'; 
//    //toastr.info("open new window");
//    //var kcalendar = $("#calendar").data("kendoCalendar");
//    //var val = kcalendar.toString(this.value(), 'd');

//    //var calendar = $("#calendar").data("kendoCalendar");
//    //calendar.value(new Date());
//    $.validator.unobtrusive.parse("#TimeOffRequestsFormDdl");
//    var appBaseUrl = $("#appBaseUrl").val();
//    //var appBaseUrl = "/ExecViewHrk/";

//    //var valcalendar = kendo.toString(this.value(), 'd');
//    $("#RangeSelection").dialog({
//        autoOpen: true,
//        title: "Time off Date range",
//        position: { my: "center", at: "top+350", of: window },
//        width: 500,
//        resizable: false,
//        modal: true,
//        open: function (event, ui) {
//            $(this).load(appBaseUrl + "/TimeOffRequests/RangeSelectionPartial");
//            //$('#RangeSelection').html();
//        },
//        buttons: {
//            "Save": function () {
//                AddTimeOffRequest();
//                //$(this).dialog("close");
//                //RefreshCalendar();
//            },
//            Cancel: function () {
//                $(this).dialog("close");
//            }
//        }
//    });




//$.ajax({
//            url: appBaseUrl + "TimeOffRequests/RangeSelectionPartial",
//            type: "POST",         
//            data: {
//                selectedDate: kendo.toString(this.value(), 'd'),
//            },

//            success: function (response) {
//                $('#RangeSelection').html(response);                    
//                $("#RangeSelection").dialog("open");                    
//                return;
//            }
//        });

//$("#RangeSelection").load('@Url.Action("RangeSelectionPartial")', function() {
//    $(this).dialog("open");
//});
//$("#RangeSelection").dialog("open");

//$("#RangeSelection").attr("start").data("kendoCalendar").value(valcalendar)
//calendar.value($(valcalendar).val());
//}


function AddTimeOffRequest() {
    
    $.validator.unobtrusive.parse("#TimeOffRequestsByDateFormDdl");
    var appBaseUrl = $("#appBaseUrl").val();

    var form = $("#TimeOffRequestsByDateFormDdl");
    var serialized_Object = form.serializeObject();

    $.ajax({
        url: appBaseUrl + "TimeOffRequests/AddResposeTimeOffRequest_Ajax",
        type: "POST",

        data: {
            //timeOffVm: serialized_Object
            //tjson: JSON.stringify({ 'TimeOffEmpVm': serialized_Object }),
            TimeOffEmpVm: JSON.stringify([serialized_Object])
        },

        //contentType: "application/json",
        //async: true,
        //dataType: 'json',

        //cache: false,
        //contentType: "application/jsonrequest; charset=utf-8",
        success: function (response) {
            //RefreshCalendar(appBaseUrl);
            //RefreshCalendar();
            // $('#RangeSelection').data("kendoWindow").close();
            if (response.succeed == false) {
                //$('#RangeSelection').data("kendoWindow").close();
                toastr.error(response.Message);
            }
            else {
                $('#TimeOffRequestsByDate').data("kendoWindow").close();
                toastr.success("Timeoff request submitted");
                //toastr.success(response.Message);
            }
            return;
        }
    });
}


//function RefreshCalendar(appBaseUrl)
//{
//    $.ajax({
//        url: appBaseUrl + "TimeOffRequests/GetTimeOffRequestsDB_Ajax",
//        type: "POST",
//        data: {
//            timeOffDateRangeVm: serialized_Object,
//        },
//        success: function (response) {           
//            return;
//        }
//    });
//}