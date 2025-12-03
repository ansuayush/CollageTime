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
    RefreshCalendar(TodayDate.getFullYear(), TodayDate.getMonth()+1);
      
});


function RefreshCalendar(year, month) {
        
    //$('#calendar').data('kendoCalendar').destroy();
    //$('#calendar').empty();

    $.validator.unobtrusive.parse("#TimeOffRequestsFormDdl");
    var appBaseUrl = $("#appBaseUrl").val();
    $.ajax({
        url: appBaseUrl + "TimeOffRequests/GetTimeOffRequestsDB_Ajax",
        type: "POST",       
        data: {
            selectedYear: year,
            selectedMonth: month,
        },
        success: function (response) {
            events = [];
            var today = new Date();

            if (response.EmpTimeOffRequest != null)
            {
                $.each(response.EmpTimeOffRequest, function (TRindex, TRmodel) {                
                    //convert json dates to javascript format
                    response.EmpTimeOffRequest[TRindex].timeOffRequestDate = new Date(parseInt(response.EmpTimeOffRequest[TRindex].timeOffRequestDate.replace('/Date(', '')));               
                });
                events = response.EmpTimeOffRequest;
            }
            //$.each(response, function (TRindex, TRmodel) {
            //    //convert json dates to javascript format
            //    response.TimeOffDate = new Date(parseInt(response.TimeOffDate.replace('/Date(', '')));
            //});
            //events = response.EmpTimeOffRequest;
            //events.push(new Date(today.getFullYear(), today.getMonth(), 8));
            //events.push(new Date(today.getFullYear(), today.getMonth(), 12));

            var Existingcalendar = $('#calendar').data('kendoCalendar');
            if (Existingcalendar != null) {
                $('#calendar').data('kendoCalendar').destroy();
                $('#calendar').empty();
            }

            $("#calendar").kendoCalendar({
                //value: new Date().toJSON().slice(0, 10).replace(/-/g, '/'), 
                value: new Date(year, month-1, 1),
                month: {                   
                    content: "# var test = isInArray(data.date, events); #" +                           
                            "# if(test.equal == 0){#" +
                                    //"# toastr.info(\"Pending \" + test.pending + \"approved \" + test.approved + \"disapproved \" + test.disapproved); #" +
                                    "#if(test.pending == 1){ #" +
                                        "<div class=\"pending\">#= data.value #</div>" +    
                                    "#} else if(test.approved == 2){#" +
                                        "<div class=\"approved\">#= data.value #</div>" +
                                    "#} else if(test.disapproved == 3){#" +
                                        "<div class=\"disapproved\">#= data.value #</div>" +
                                    "#}#" +
                            "#} else {#"+
                                "<div>#= data.value #</div>"+
                             "#}#"
                   
                },
               
            });
           
            return;
        },
        complete: function () {
            
            var calendar = $("#calendar").data("kendoCalendar");
            calendar.setOptions({
                change: function (e) {
                    e.preventDefault();
                    openRangeSelectionDialog();
                }
            });

            calendar.bind("navigate", function(e) {
                e.preventDefault();
                navigate_Calendar();
            });           
         }
    });
}



function navigate_Calendar() {    
    var month = $("#calendar").data("kendoCalendar")._current.getMonth() + 1;
    var year = $("#calendar").data("kendoCalendar")._current.getFullYear();
    // toastr.info(month + " " + year);
    RefreshCalendar(year, month);
    return;    
}

function isInArray(date, events) {
    //
    // (0 "true") equal , (1)pending, (2) approved
    for (var i = 0; i < events.length; i++) {
      
        if (date.getFullYear() == events[i].timeOffRequestDate.getFullYear() && date.getMonth() == events[i].timeOffRequestDate.getMonth() && date.getDate() == events[i].timeOffRequestDate.getDate()) {
            if (events[i].statusOfTimeOffRequest == 0)
                return { equal: 0, pending: 1 };
            else if (events[i].statusOfTimeOffRequest == 1)
                return { equal: 0, approved: 2 };
            else if (events[i].statusOfTimeOffRequest == 2)
                return { equal: 0, disapproved: 3 };
        }
       
    }
    return { equal: 1 }; 
}


function openRangeSelectionDialog() {
    debugger
    //e.preventDefault();

    calval = $("#calendar").data("kendoCalendar").value();
    $.validator.unobtrusive.parse("#RangeSelection");
    var appBaseUrl = $("#appBaseUrl").val();
    $("#RangeSelection").kendoWindow({
        width: "30%", //"900px",
        height: "61%",  //"400px",
        position: { top: 300, left: 120 },
        title: "Time off Date selection",
        actions: [
            //"Maximize",
            "Close"
        ],
        //scrollable: true,
        modal: true,
        //close: onCloseRangeSelectionWindow,
        
        content:{
            url:appBaseUrl + "TimeOffRequests/RangeSelectionPartial",
            data: { selectedDate: kendo.toString(calval, 'd') }
        }
    })
    $("#RangeSelection").data("kendoWindow").open().center();   
}


function onCloseRangeSelectionWindow() {
    
    $('#RangeSelection').data("kendoWindow").close();    
}



function AddTimeOffRequest()
{
    
    $.validator.unobtrusive.parse("#RangeSelectionFormDdl");
    var appBaseUrl = $("#appBaseUrl").val();

    var form = $("#RangeSelectionFormDdl");
    var serialized_Object = form.serializeObject();
   // $('#RangeSelection').data("kendoWindow").close();

    $.ajax({
        url: appBaseUrl + "TimeOffRequests/AddTimeOffRequest_Ajax",
        type: "POST",
        data: {
            timeOffRequestVm: serialized_Object,
        },
        success: function (response) {           
            //RefreshCalendar(appBaseUrl);
            //RefreshCalendar();
            // $('#RangeSelection').data("kendoWindow").close();
            if (response.succeed == false){
                //$('#RangeSelection').data("kendoWindow").close();
                toastr.error(response.Message);
            }
            else {                
                $('#RangeSelection').data("kendoWindow").close();                
                toastr.success("Timeoff request submitted");
                var today = new Date(serialized_Object.start);
                RefreshCalendar(today.getFullYear(), today.getMonth() + 1);
                //toastr.info(response.Message);
            }
            return;
        }
    });
}


function DeleteTimeOffRequest()
{
    
    $.validator.unobtrusive.parse("#RangeSelectionFormDdl");
    var appBaseUrl = $("#appBaseUrl").val();

    var form = $("#RangeSelectionFormDdl");
    var serialized_Object = form.serializeObject();
    $.ajax({
        url: appBaseUrl + "TimeOffRequests/DeleteTimeOffRequest_Ajax",
        type: "POST",
        data: {
            timeOffRequestVm: serialized_Object,
        },
        success: function (response) {
            //RefreshCalendar(appBaseUrl);
            //RefreshCalendar();
            // $('#RangeSelection').data("kendoWindow").close();
            if (response.succeed == false) {
                //$('#RangeSelection').data("kendoWindow").close();
                toastr.error(response.Message);
            }
            else {
                var str = "";
                var deletedDate;
                
                for (var i = 0; i < response.Data.length; i++) {                   
                    deletedDate = new Date(parseInt(response.Data[0][i].TimeOffDate.replace('/Date(', '')));
                    str = str + "  " + (deletedDate.getMonth()+1) + "/" + deletedDate.getDate() + "/" + deletedDate.getFullYear();
                    //str = str + " " + response.Data[i][i].TimeOffDate ;
                }

                if (response.Data.length > 0) {
                    str = "Following request(s) deleted:\n" + " " + str;
                    deletedDate = new Date(parseInt(response.Data[0][0].TimeOffDate.replace('/Date(', '')));
                }
                else {
                    deletedDate = new Date();
                }

                $('#RangeSelection').data("kendoWindow").close();
                toastr.info(str);
                
                RefreshCalendar(deletedDate.getFullYear(), deletedDate.getMonth() + 1);              
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

//function openRangeSelectionDialog() {
//    //$.validator.unobtrusive.parse("#RangeSelection");
//    //var appBaseUrl = $("#appBaseUrl").val();
//    
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