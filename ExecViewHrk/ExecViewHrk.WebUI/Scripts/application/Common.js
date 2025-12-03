/*
Toastr Implementation
---------------------
toastr.success('Message'); // Display a success toast
toastr.error('Message'); // Display an error toast
toastr.info('Message'); // Display an info toast
toastr.warning('Message','Title') // Display a warning toast, with a title

toastr.remove() // Remove toast
toastr.clear() // Remove current toasts using animation
 */
var _lookUpTablesObject = [];
$(document).ready(function () {
    setDefaults();
    pageSetup();



    $("#LookupDivSave").click(function () {
        if (_lookUpTablesObject.grid) {
            if (!ValidateForm("frmLookupCommon")) {
                loading(false);
                return;
            }
            _lookUpTablesObject.model.Active = $("#chkLookUpTableActive").prop('checked');
            _lookUpTablesObject.model.Code = $("#txtLookUpTableCode").val();
            _lookUpTablesObject.model.Description = $("#txtLookUpTableDescription").val();
            _lookUpTablesObject.model.dirty = true;
            if (_lookUpTablesObject.row) {
                _lookUpTablesObject.grid.trigger("save", { container: _lookUpTablesObject.row, model: _lookUpTablesObject.model });
            } else {
                _lookUpTablesObject.grid.dataSource.insert(_lookUpTablesObject.model);
            }
             _lookUpTablesObject.grid.saveChanges();
        }
    });
})


function pageSetup() {
    $.fn.datepicker && $(".datepicker").each(function () {
        var a = $(this),
            b = a.attr("data-dateformat") || "mm/dd/yy",
            c = a.attr("data-maxdate") || null;
        a.datepicker({
            changeMonth: true,
            changeYear: true,
            yearRange: "1900:+83",
            "dateFormat": b,
            "prevText": '<', "nextText": '>',
            maxDate: c
            //onSelect: function (dateText, inst) {
            //    $(this).trigger('change');
            //},
            //onClose: function (dateText, inst) {
            //    if (detectIE()) {
            //        $(this).focusNextInputField();
            //    } else {
            //        this.focus();
            //    }
            //}
        }),
            a = null
    });

    $(".md-close").unbind("click");
    $(".md-close").click(function (e) {
        e.preventDefault();
        if ((_lookUpTablesObject.grid) && (_lookUpTablesObject.grid.dataSource.hasChanges()) && $('#LookupTablesDiv').hasClass("md-show")) {
            _lookUpTablesObject.grid.cancelChanges();
            //_lookUpTablesObject.grid.read();
            _lookUpTablesObject.grid.dataSource.sync()
        }
        $(this).parents(".EditSlideRight").toggle('slide', { direction: 'right' }, 500).removeClass("md-show");
    });

}
function setDefaults() {
    //Default settings for toastr
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": true,
        "progressBar": false,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "3000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
    $.validator.methods.date = function (value, element) {
        return this.optional(element) || /(?:[1-9]|1[0-2])\/(?:[1-9]|[12][0-9]|3[01])\/(?:19|20\d{2})/.test(value);
    }

}
// Setup check boxes 
function pageInitCheckBox() {

    $('input:checkbox').change(function () {
        var name = $(this).attr('id');
        var element = $('input:hidden[name= ' + name + ']')[0];
        if (element) {
            element.value = $(this).prop("checked")
        }
    });
}

function convertUTCDateToLocalDate(date) {
    if (date == "Invalid Date") return "";
    return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes(), date.getSeconds())).toLocaleString();
}

function convertLocalDatetoUTCDate(date) {
    if (date == "Invalid Date") return "";
    return new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(), date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds()).toLocaleString();
}

function confirmDialog(dFirstButton, dFirstButtonFunc, dSecondButton, dSecondButtonFunc, dTitle, dContent) {
    if (dTitle == null || dTitle == "") { dTitle = "Confirm"; }
    if (dContent == null || dContent == "") { dContent = "Do you want to delete selected record ?"; }
    if (dFirstButton == null || dFirstButton == "") { dFirstButton = " Yes "; }
    if (dSecondButton == null || dSecondButton == "") { dSecondButton = " No "; }

    $("#dialog-confirm").html(dContent);
    // Define the Dialog and its properties.
    $("#dialog-confirm").dialog({
        resizable: false,
        modal: true,
        title: dTitle,
        width: 550,
        position: { top: -10, left: 0 },
        closeText: "",
        draggable: false,
        open: function () {
            $('.ui-widget-overlay').on('click', function (e) {
                e.preventDefault();
                return false;
            });
        },
        buttons: [{
            html: dFirstButton,
            "class": "btn btn-primary",
            click: function () {
                $(this).dialog("close");
                if (typeof (dFirstButtonFunc) != "undefined") dFirstButtonFunc();
            }
        }, {
            html: dSecondButton,
            "class": "btn btn-default",
            click: function () {
                $(this).dialog("close");
                if (typeof (dSecondButtonFunc) != "undefined") dSecondButtonFunc();
            }
        }],
        responsive: true
    });

}

function gridDelete(e) {
    
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    confirmDialog("", function () {
        if ((grid) && (row)) { _lookUpTablesObject = [];grid._removeRow(row); }
    });
}

function gridAdd(_grid) {
    var grid = $("#" + _grid).data("kendoGrid");
    var data = grid.dataSource._createNewModel();
    $("#chkLookUpTableActive").prop('checked', data.Active);
    $("#txtLookUpTableCode").val(data.Code);
    $("#txtLookUpTableDescription").val(data.Description);
    if (_grid == "gridDdlStates") {
        $("#chkLookUpTableActive").hide();
        $("#lblLookUpTableActive").hide();
    }
    else {
        $("#chkLookUpTableActive").show();
        $("#lblLookUpTableActive").show();
    }

    _lookUpTablesObject.grid = grid;
    _lookUpTablesObject.row = null;
    _lookUpTablesObject.model = data;

    $('#LookupTablesDiv').toggle('slide', { direction: 'right' }, 500).addClass('md-show');
    //2460-Enable Code Field during Add New
    $("#txtLookUpTableCode").prop("disabled", false);
}

function gridEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    $("#chkLookUpTableActive").prop('checked', data.Active);
    $("#txtLookUpTableCode").val(data.Code);
    $("#txtLookUpTableDescription").val(data.Description);

    _lookUpTablesObject.grid = grid;
    _lookUpTablesObject.row = row;
    _lookUpTablesObject.model = data;

    $('#LookupTablesDiv').toggle('slide', { direction: 'right' }, 500);
    $('#LookupTablesDiv').addClass('md-show');
    //2460 - Disable Code Field only for Employee Type
    if (grid._cellId == "gridDdlEmployeeType_active_cell")
        $("#txtLookUpTableCode").prop("disabled", true);
}
function gridEditEmpStatus(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    $("#chkLookUpTableActive").prop('checked', data.Active);
    $("#txtLookUpTableCode").val(data.Code);
    $("#txtLookUpTableDescription").val(data.Description);

    _lookUpTablesObject.grid = grid;
    _lookUpTablesObject.row = row;
    _lookUpTablesObject.model = data;
    if (data.Description != "Active" && data.Description !="On International Assignment" && data.Description !="Casual" && data.Description !=  "On Long Term Disability"&& data.Description !="Sick Leave" && data.Description !="Surviving Spouse" && data.Description !="No Longer Employed"
        && data.Description !="Blocked"  && data.Description !="Inactive" && data.Description !="Transferred" && data.Description != "On Sabbatical" && data.Description !=  "On Leave"&& data.Description != "Multiple Positions" && data.Description !="New Employee" && data.Description !="Lay Off"
        && data.Description != "Part Time" && data.Description != "Deaceased" && data.Description != "Retired" && data.Description != "Separated" && data.Description != "Terminated")
    {
        $('#LookupTablesDiv').toggle('slide', { direction: 'right' }, 500);
        $('#LookupTablesDiv').addClass('md-show');
    }
}
function gridDeleteEmpStatus(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    if (data.Description != "Active" && data.Description != "On International Assignment" && data.Description != "Casual" && data.Description != "On Long Term Disability" && data.Description != "Sick Leave" && data.Description != "Surviving Spouse" && data.Description != "No Longer Employed"
        && data.Description != "Blocked" && data.Description != "Inactive" && data.Description != "Transferred" && data.Description != "On Sabbatical" && data.Description != "On Leave" && data.Description != "Multiple Positions" && data.Description != "New Employee" && data.Description != "Lay Off"
        && data.Description != "Part Time" && data.Description != "Deaceased" && data.Description != "Retired" && data.Description != "Separated" && data.Description != "Terminated")
    {
        confirmDialog("", function () {
            if ((grid) && (row)) { _lookUpTablesObject = []; grid._removeRow(row); }
        });
    }
}

function addSkill() {
    openskillEditpopup(0);
}

function openskillEditpopup(SkillId) {
    var formURL = $("#ApplicationUrl").val() + '/DdlSkills/skillEdit?skillId=' + SkillId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}
function gridSkillEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var formURL = $("#ApplicationUrl").val() + '/DdlSkills/skillEdit?skillId=' + data.SkillId;
    $("#EditDetailDiv .md-content").html("");
    $("#EditDetailDiv .md-content").load(formURL);
    $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
    $('#EditDetailDiv').addClass('md-show');
}
function gridRequestEnd(e) {
    var operationType = e.type;
    if (operationType == "read" && e.response.Errors == null) {
        loading(false);
    }
    else if (operationType == "destroy" && e.response.Errors == null) {
        toastr.success('Record deleted successfully.');
        if (e.sender.data().length == 0) {
            e.sender.page(1);
        } else {
            e.sender.read();
        }
        loading(false);
    }
    else if (operationType == "create" && e.response.Errors == null) {
        toastr.success('Record created successfully.');
        if ((_lookUpTablesObject.grid) && $('#LookupTablesDiv').hasClass("md-show")) {
            $('#LookupTablesDiv').toggle('slide', { direction: 'right' }, 500).removeClass("md-show");
            _lookUpTablesObject = [];
        }
        e.sender.read();
        //loading(false);
    }
    else if (operationType == "update" && e.response.Errors == null) {
        toastr.success('Record updated successfully.');
        if ((_lookUpTablesObject.grid) && $('#LookupTablesDiv').hasClass("md-show")) {
            $('#LookupTablesDiv').toggle('slide', { direction: 'right' }, 500).removeClass("md-show");
            _lookUpTablesObject = [];
        }
        loading(false);
    }


    if (operationType == "create" && e.response.Errors != null) {
        e.sender.cancelChanges();
    }
}
function gridDataBound(e) {
    $(".fa-pencil").parent().attr("data-toggle", "tooltip").attr("data-placement", "top").attr("title", "Edit");
    $(".fa-trash-o").parent().attr("data-toggle", "tooltip").attr("data-placement", "top").attr("title", "Delete");
    $(".fa-eye").parent().attr("data-toggle", "tooltip").attr("data-placement", "top").attr("title", "View Salary");
    $('[data-toggle="tooltip"]').tooltip();

}

function refreshGrid(e) {
    var ds = e.sender;
    if (e.errors) {
        var message = "";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "<br />";
                });
            }
        });
        if ((_lookUpTablesObject.grid) && $('#LookupTablesDiv').hasClass("md-show")) {
            //TODO: Add logic for lookup tables
            //if (ds.hasChanges()) {
            //    ds.cancelChanges();
            //}
        } else {
            if (ds.hasChanges()) {
                ds.cancelChanges();
            }
            ds.read();
        }
        
        loading(false)
        toastr.error(message);
    }
}

function resetGrid(name) {
    $('#' + name).data('kendoGrid').dataSource.cancelChanges();
    $('#' + name).data('kendoGrid').dataSource.read();
    $('#' + name).data('kendoGrid').refresh();
    $('#' + name).data('kendoGrid').dataSource.page(1);
}

function errorHandlerCitizenships(e) {
    return refreshGrid(e);
}

function errorHandleApplicantSource(e) {
    return refreshGrid(e);
}

function errorHandlerHospitals(e) {
    return refreshGrid(e);
}

function errorhandlerEeoTypes(e) {
    return refreshGrid(e);
}

// Refresh Combobox For All
var timeOutFn = null;
function resetKenoComboBox(name) {
    var dropdownlist = $('#' + name).data("kendoComboBox")
    var selectedValue = dropdownlist.value();
    var selectedtext = dropdownlist.text();
    dropdownlist.refresh();
    dropdownlist.dataSource.read();
    killTimer();
    timeOutFn = setTimeout(function () {
        dropdownlist.text(selectedtext);
        var index = dropdownlist.selectedIndex;
        //check selected index by seletected  text
        if (index == -1) {
            dropdownlist.value(selectedValue);
            index = dropdownlist.selectedIndex;
        }
        //check selected index by seletected value
        if (index == -1) {
            //clear Items
            dropdownlist.select(-1);
            dropdownlist.value('');
        }
    }, 1000)

}

function refreshKenoComboBox(name) {
    var dropdownlist = $('#' + name).data("kendoComboBox")
    var selectedValue = dropdownlist.value();
    var selectedtext = dropdownlist.text();
    dropdownlist.refresh();
    dropdownlist.text(selectedtext);
    var index = dropdownlist.selectedIndex;
    //check selected index by seletected  text
    if (index == -1) {
        dropdownlist.value(selectedValue);
        index = dropdownlist.selectedIndex;
    }
    //check selected index by seletected value
    if (index == -1) {
        //clear Items
        dropdownlist.select(-1);
        dropdownlist.value('');
    }
}

function setkendoDropDownList(name, selectedValue) {
    var dropdownlist = $('#' + name).data("kendoDropDownList")
    dropdownlist.refresh();
    dropdownlist.dataSource.read();
    killTimer();
    timeOutFn = setTimeout(function () {
        dropdownlist.value(selectedValue);
        var index = dropdownlist.selectedIndex;
        //check selected index by seletected  text
        if (index == -1) {
            //dropdownlist.value(selectedValue);
            index = dropdownlist.selectedIndex;
        }
        //check selected index by seletected value
        if (index == -1) {
            //clear Items
            dropdownlist.select(-1);
            dropdownlist.value('');
        }
    }, 1000)

}

//start
function resetKenoDropDownList(name) {
    var dropdownlist = $("#" + name).data("kendoDropDownList");
    var selectedValue = dropdownlist.value();
    var selectedtext = dropdownlist.text();
    dropdownlist.refresh();
    dropdownlist.dataSource.read();
    killTimer();
    timeOutFn = setTimeout(function () {
        dropdownlist.text(selectedtext);
        var index = dropdownlist.selectedIndex;
        //check selected index by seletected  text
        if (index == -1) {
            dropdownlist.value(selectedValue);
            index = dropdownlist.selectedIndex;
        }
        //check selected index by seletected value
        if (index == -1) {
            //clear Items
            dropdownlist.select(-1);
            dropdownlist.value('');
        }
    }, 1000)

}
//end

function killTimer() {
    if (timeOutFn) {
        timeOutFn = null;
    }
}
//  Date Time Formatting through Moment
// usages  getFormattedDate(selector :  Name of the Input Picker)
function getFormattedDate(selector) {
    var selectedDate = $("#" + selector)[0].value;
    if (selectedDate && (selectedDate != '' || typeof (selectedDate) != undefined)) {
        var formattedDate = new Date(moment.utc(new Date(selectedDate), 'MM/DD/YYYY'));
        return moment(formattedDate, "MM/DD/YYYY").format('MM/DD/YYYY');
    } else {
        return selectedDate;
    }

}


function onDatechange() {
    var dt = this;
    var value = this.value();

    if (value === null) {
        value = kendo.parseDate(dt.element.val(), dt.options.parseFormats);
    }

    if (value < dt.min()) {
        dt.value(dt.min());
    } else if (value > dt.max()) {
        dt.value(dt.max());
    }
}

function validateDate(element) {
    var dt = $(element).data("kendoDatePicker").options;
    var value = element.val();

    if (Boolean(value)) {
        if (value.length < 8) {
            $(element).addClass("input-validation-error");
            return false;
        } else if (new Date(value) < dt.min) {
            $(element).addClass("input-validation-error");
            return false;
        } else if (new Date(value) > dt.max) {
            $(element).addClass("input-validation-error");
            return false;
        }
        $(element).removeClass("input-validation-error");
        return true;
    }
    $(element).removeClass("input-validation-error");
    return true;
}

function quickAdd(e, type) {
    e.preventDefault();
    var formURL = "";
    if (type == "Suffix") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddSuffix";
    }
    else if (type == "Prefix") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddPrefix";
    }
    else if (type == "Gender") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddGender";
    }
    else if (type == "MaritalStatus") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddMaritalStatus";
    }
    else if (type == "EvaluationTest") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddEvaluationTest";
    }
    else if (type == "MedicalExaminationType") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddMedicalExaminationType";
    }
    else if (type == "ProfessionalBody") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddProfessionalBody";
    }
    else if (type == "RegionalChapter") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddRegionalChapter";
    }
    else if (type == "AddressType") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddAddressType";
    }
    else if (type == "State") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddState";
    }
    else if (type == "Country") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddCountry";
    }
    else if (type == "PayFrequency") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddPayFrequency";
    }
    else if (type == "PositionGrade") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddPositionGrade";
    }
    else if (type == "PositionCategory") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddPositionCategory";
    }
    else if (type == "PositionType") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddPositionType";
    }
    else if (type == "EmploymentStatusId") {
        formURL = $("#ApplicationUrl").val() + "PersonEmployees/AddEmploymentStatus";
    }
    else if (type == "EmployeeTypeID") {
        formURL = $("#ApplicationUrl").val() + "PersonEmployees/AddEmployeeType";
    }
    else if (type == "PersonsList") {
        formURL = $("#ApplicationUrl").val() + "PersonEmployees/AddPersons";
    }
    else if (type == "InnoculationTypes") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddInnoculationTypes";
    }
    else if (type == "PersonRelationShipTypes") {
        formURL = $("#ApplicationUrl").val() + "PersonRelationships/AddDdlRelationshipTypes";
    }
    else if (type == "LicenseType") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddLicenseType";
    }
    else if (type == "PropertyTypes") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddPropertiesTypes";
    }
    else if (type == "Disability") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddDisability";
    }
    else if (type == "Accomodation") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddAccomodation";
    }
    else if (type == "SalaryComponent") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddSalaryComponentType";
    }
    else if (type == "payType") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddPaytype";
    }
    else if (type == "EducationEstablishment") {
        formURL = $("#ApplicationUrl").val() + "LookupTables/AddEducationEstablishment";
    }
    if (formURL != "") {
        $("#QuickAddModal .modal-content").html("");
        $("#QuickAddModal .modal-content").load(formURL);
        $("#QuickAddModal").modal({ show: true });
    }
}

//Common Custom Functions For Whole Application
function ShowValidationMessage(elementId, message) {
    $("span[data-valmsg-for='" + elementId + "']").css("display", "block").removeClass("field-validation-valid").addClass("field-validation-error").html("<span id='" + elementId + "-error'>" + message + "</span>");
}

function LoadMenu(_view, _controller) {
   
    //IMPLEMENT: Custom logic for menu of required
    $("#HD_VIEW").val(_view);
    $("#HD_CONTROLLER").val(_controller);
    LoadContents(_view, _controller);
}

function LoadDetailView(_view, _controller, data) {
    //IMPLEMENT: Custom logic for menu of required
    $("#HD_VIEW").val(_view);
    $("#HD_CONTROLLER").val(_controller);
    LoadContents(_view, _controller, data);
}

function LoadContents(viewname, controllername, data, tdiv, isAsync) {
    if (typeof (isAsync) == "undefined") isAsync = true;
    //this is fire after than no more async task.
    //$(document).ajaxStop(function () {
    //    loading(false);
    //});
    var urlval = $("#ApplicationUrl").val() + controllername + "/" + viewname;

    var targetdiv = "content";
    if (tdiv) targetdiv = tdiv;

    $.ajax({
        type: "GET",
        url: urlval,
        data: data,
        beforeSend: function () {
            loading(true);

        },
        success: function (data) {
            try {
                var obj = JSON.parse(data);
                if (obj.Message != null) {
                    DisplayNotification(obj.Message, "error");
                } else {
                    $("#" + targetdiv).html(obj);
                }
            }
            catch (e) {
                $("#" + targetdiv).html(data);
            }

            //pageSetup();
        },
        complete: function () {
            loading(false);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            toastr.error(errorThrown, "error");
            loading(false);
        },
        dataType: "html",
        async: isAsync,
        cache: false
    });

}

function loading(show) {
    if (show) {
        if (!$("#enlarge").is(":visible")) {
            $("#enlarge").show();
        }
    }
    if (!show) {
        $("#enlarge").hide();
    }
}

function toggleFullScreen() {
    if (!document.fullscreenElement &&    // alternative standard method
        !document.mozFullScreenElement && !document.webkitFullscreenElement) {  // current working methods
        if (document.documentElement.requestFullscreen) {
            document.documentElement.requestFullscreen();
        } else if (document.documentElement.mozRequestFullScreen) {
            document.documentElement.mozRequestFullScreen();
        } else if (document.documentElement.webkitRequestFullscreen) {
            document.documentElement.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
        }
    } else {
        if (document.cancelFullScreen) {
            document.cancelFullScreen();
        } else if (document.mozCancelFullScreen) {
            document.mozCancelFullScreen();
        } else if (document.webkitCancelFullScreen) {
            document.webkitCancelFullScreen();
        }
    }
}
function deactivateThis() {
    this.refresh();
}

function gridPositionFundingSourceEdit(e) {
    
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var formURL = $("#ApplicationUrl").val() + "PositionSetupDetail/EditPositionFundingSource?FundingSourceID=" + data.ID;
    if (formURL != "") {
        $("#PositionSalaryGradeDetailsEditModal .modal-content").html("");
        $("#PositionSalaryGradeDetailsEditModal .modal-content").load(formURL);
        $("#PositionSalaryGradeDetailsEditModal").modal({ show: true });
    }
}

function getPercentage(elementID)
{
    var eleID = "#" + elementID;
    if (eleID == "#" || eleID == "#undefined") return true;
    if ($(eleID).val() != "") {
        var strNumber = '';
        var num = parseFloat($(eleID).val(), 10).toFixed(2);
        if (num > 100)
            strNumber = "100" ;
        else
            strNumber = num ;
        $(eleID).val(strNumber.replace('.00', ''));
    }
}

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
function gridPositionSalarySourceeEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var formURL = $("#ApplicationUrl").val() + "PositionSetupDetail/EditPositionSalaryDetailGrad?SalaryGradeGridID=" + data.ID;
    if (formURL != "") {
        $("#PositionSalaryGradeDetailsEditModal .modal-content").html("");
        $("#PositionSalaryGradeDetailsEditModal .modal-content").load(formURL);
        $("#PositionSalaryGradeDetailsEditModal").modal({ show: true });
    }
}
function SelectedBudgetYear(options) {
    return { BudgetYear: $("#budgetYear").val(), Month: $("#budgetMonth").val()  }
}

function gridFundCodeEdit(e) {
    e.preventDefault();
    var grid = this;
    var row = $(e.currentTarget).closest("tr");
    var data = grid.dataItem(row);
    var formURL = $("#ApplicationUrl").val() + "LookupTables/AddEditFundCode?fundCodeId=" + data.ID;
    if (formURL != "") {
        $("#EditDetailDiv .md-content").html("");
        $("#EditDetailDiv .md-content").load(formURL);
        $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
        $('#EditDetailDiv').addClass('md-show');
    }
}
function gridAddFundCode() {
    var formURL = $("#ApplicationUrl").val() + "LookupTables/AddEditFundCode?fundCodeId=0";
    if (formURL != "") {
        $("#EditDetailDiv .md-content").html("");
        $("#EditDetailDiv .md-content").load(formURL);
        $('#EditDetailDiv').toggle('slide', { direction: 'right' }, 500);
        $('#EditDetailDiv').addClass('md-show');
    }
}

$.pushMenu = {
    activate: function (toggleBtn) {
        
        //Enable sidebar toggle
        $(toggleBtn).on('click', function (e) {
            e.preventDefault();

            //Enable sidebar push menu
            if ($(window).width() > (767)) {
                if ($("body").hasClass('sidebar-collapse')) {
                    $("body").removeClass('sidebar-collapse').trigger('expanded.pushMenu');
                } else {
                    $("body").addClass('sidebar-collapse').trigger('collapsed.pushMenu');
                }
            }
                //Handle sidebar push menu for small screens
            else {
                if ($("body").hasClass('sidebar-open')) {
                    $("body").removeClass('sidebar-open').removeClass('sidebar-collapse').trigger('collapsed.pushMenu');
                } else {
                    $("body").addClass('sidebar-open').trigger('expanded.pushMenu');
                }
            }
            if ($('body').hasClass('fixed') && $('body').hasClass('sidebar-mini') && $('body').hasClass('sidebar-collapse')) {
                $('.sidebar').css("overflow", "visible");
                $('.main-sidebar').find(".slimScrollDiv").css("overflow", "visible");
            }
            if ($('body').hasClass('only-sidebar')) {
                $('.sidebar').css("overflow", "visible");
                $('.main-sidebar').find(".slimScrollDiv").css("overflow", "visible");
            };
        });

        $(".content-wrapper").on('click', function () {
            //Enable hide menu when clicking on the content-wrapper on small screens
            if ($(window).width() <= (767) && $("body").hasClass("sidebar-open")) {
                $("body").removeClass('sidebar-open');
            }
        });
    }
};
