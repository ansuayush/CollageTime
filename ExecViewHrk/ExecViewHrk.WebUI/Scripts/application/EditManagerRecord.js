$(document).ready(function () {
    pageSetup();
    loading(false);
    SetStyle();
});
$("#left").on('click', function () {
    var array = $("#Managersnotlocked").find('option:selected');
    for (i = 0; i < array.length; i++) {
        $("#Managerslocked").append("<option value='" + array[i].value + "'>" + array[i].text + "</option>");
    }
    $("#Managersnotlocked").find('option:selected').remove();
    SetStyle();
});
$("#right").on('click', function () {
    var array = $("#Managerslocked").find('option:selected');
    for (i = 0; i < array.length; i++) {
        $("#Managersnotlocked").append("<option value='" + array[i].value + "'>" + array[i].text + "</option>");
    }
    $("#Managerslocked").find('option:selected').remove();
    SetStyle();
});
function SetStyle() {
    $('#Managerslocked option, #Managersnotlocked option').removeAttr('style');
    $('#Managerslocked option:even, #Managersnotlocked option:even').css({ 'background-color': '#eeeeee' });
    $('#Departmentlocked option, #Departmentnotlocked option').removeAttr('style');
    $('#Departmentlocked option:even, #Departmentnotlocked option:even').css({ 'background-color': '#eeeeee' });
}
function UpdateRecord() {
    var array = $("#Managersnotlocked option");
    var arrayvalue;
    for (i = 0; i < array.length; i++) {
        if (i == 0) {
            arrayvalue = array[i].value;
        }
        else {
            arrayvalue += "," + array[i].value;
        }
    }

    $("#hdnmanagersnotlocked").val(arrayvalue);
    var lockedarray = $("#Managerslocked option");
    var arraylockedvalue;
    for (i = 0; i < lockedarray.length; i++) {
        if (i == 0) {
            arraylockedvalue = lockedarray[i].value;
        }
        else {
            arraylockedvalue += "," + lockedarray[i].value;
        }
    }
    $("#hdnmanagerslocked").val(arraylockedvalue);
    DepartmentListdata();
    loading(true);
    var postData = $("#EditManagerForm").serializeArray();
    var formURL = $("#ApplicationUrl").val() + '/Managers/SaveManagers';
    $.post(formURL, postData, function (data, status) {
        if (data.succeed == false) {
            toastr.error(data.Message);
            loading(false);
        } else {
            $('#btnCloseManagerReview').click();
            loading(false);
            toastr.success('Record updated successfully.');
            resetGrid("gridManager");
        }
    });
}
$("#leftDepartment").on('click', function () {
    var array = $("#Departmentnotlocked").find('option:selected');
    for (i = 0; i < array.length; i++) {
        $("#Departmentlocked").append("<option value='" + array[i].value + "'>" + array[i].text + "</option>");
    }
    $("#Departmentnotlocked").find('option:selected').remove();
    SetStyle();
});

$("#rightDepartment").on('click', function () {
    var array = $("#Departmentlocked").find('option:selected');
    for (i = 0; i < array.length; i++) {
        $("#Departmentnotlocked").append("<option value='" + array[i].value + "'>" + array[i].text + "</option>");
    }
    $("#Departmentlocked").find('option:selected').remove();
    SetStyle();
});


function DepartmentListdata() {
    var Ptrojectarray = $("#Departmentnotlocked option");
    var Projectsarrayvalue;
    for (i = 0; i < Ptrojectarray.length; i++) {
        if (i == 0) {
            Projectsarrayvalue = Ptrojectarray[i].value;
        }
        else {
            Projectsarrayvalue += "," + Ptrojectarray[i].value;
        }
    }

    $("#hdnDepartmentnotlockedlist").val(Projectsarrayvalue);
    var Projectlockedarray = $("#Departmentlocked option");
    var Projectarraylockedvalue;
    for (i = 0; i < Projectlockedarray.length; i++) {
        if (i == 0) {
            Projectarraylockedvalue = Projectlockedarray[i].value;
        }
        else {
            Projectarraylockedvalue += "," + Projectlockedarray[i].value;
        }
    }
    $("#hdnDepartmentlockedlist").val(Projectarraylockedvalue);
}