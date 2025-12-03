var formService = {};
var formServiceFn =  (function () {
    $(document).ready(function () {
        var form = $(this).closest("form");
        formService = {
            isLoadingInProgress: false,
            isSavingInProgress: false,
            isUpdatingInProgress: false,
            isDeletingInProgress: false,
            initialize: initializeForm,
            validate: validateForm,
            validateDates: validateDates,
            validateDate: isDateValid,
            getStatus: checkformService,
            getDirty: checkFormDirty,
            compare: compareObjects,
            clear: clearScreen,
            reset: formReset,
            CompareDates:CompareDates,
            activeForm: null,
            messages:  messeageFn(),
        };
        var status = {
            type: '',
            text: ''
        }

    })

    function initializeForm(form) {
        // killTimer();
        formService.isLoadingInProgress = true;
        formService.isSavingInProgress = false;
        formService.isUpdatingInProgress = false;
        formService.isDeletingInProgress = false;
        // return timeOutFn = setTimeout(function () {
        formService.activeForm = getSerializedData(form);
        formService.isLoadingInProgress = false;
        pageInitCheckBox();
        //},0);
    }

    function validateForm(form) {
        var isFormDirty = formService.getDirty(form.serializeObject());
        var invalid = validateDates(form);
        form.validate();
        if (invalid) {
            return invalid;
        }
        if (!form.valid()) {
            toastr.error(formService.messages.invalidMessage);
            return formService.messages.invalidMessage;
        }

        if (!isFormDirty) {
            formService.isSavingInProgress = false;
            toastr.success(formService.messages.updated);
            return formService.messages.updated;
        }
        if (formService.isSavingInProgress) {
            toastr.info(formService.messages.savingInProgress);
            return formService.messages.savingInProgress;
        } else {
            return formService.messages.defaultMessage;
        }
    }

    function checkformService() {
        if (formService.isLoadingInProgress == true) {
            toastr.info(formService.messages.loadingInProgress);
            return formService.messages.loadingInProgress;

        } else if (formService.isSavingInProgress == true) {
            toastr.info(formService.messages.savingInProgress);
            return formService.messages.savingInProgress;

        } else if (formService.isUpdatingInProgress == true) {
            toastr.info(formService.messages.updatingInProgress);
            return formService.messages.updatingInProgress;

        } else if (formService.isDeletingInProgress == true) {
            toastr.info(formService.messages.deletingInProgress);
            return formService.messages.deletingInProgress;

        } else {

            return formService.messages.defaultMessage;
        }

    }

    function checkFormDirty(data) {
        var isDirty = compareObjects(formService.activeForm, data);
        return true;//isDirty;

    }

    function getSerializedData(form) {
        return (form) ? $(form).serializeObject() : $('form :last').serializeObject();
    }

    function compareObjects(obj1, obj2) {
        var isDirty = false;
        var values = obj1 ? Object.keys(obj1).map(function (i) { return obj1[i]; }) : [];
        var valuesToCompare = obj2 ? Object.keys(obj2).map(function (i) { return obj2[i]; }) : [];
        for (var i = 0 ; i < valuesToCompare.length ; i++) {
            if (values[i] !== valuesToCompare[i]) {
                isDirty = true;
                break;
            } else {
                continue
            }
        }
        return isDirty;
    }

    function validateDates(form) {
        var datePickerList = $("input[data-role='datepicker']");
        var invalidDate = false, name = '';
        if (datePickerList) {
            for (var i = 0 ; i < datePickerList.length; i++) {
                name = datePickerList[i].name;
                invalidDate = isDateValid(name);
                if (invalidDate) {
                    break
                } else {
                    continue;
                }
            }
        }
        return invalidDate;
    }
    function isDateValid(name) {
        var dateSelected = $("#" + name).val();
        var invalid = true
        if (typeof (dateSelected) == "undefined" || dateSelected == "") return !invalid;
        if (!moment(dateSelected, "MM/DD/YYYY")._isValid) {
            toastr.error("The " + name.replace('Date','') + " Date value is not valid.");
            return invalid
        } else {
            return !invalid;
        }
    }
    function formReset(form) {
        var form = (form) ? form : $('form :last');
        form.validate().resetForm();
        // form[0].reset();
    }

    function clearScreen(form) {
        var form = (form) ? form : $('form :last');
        if (form && form.length) {
            for (var i = 0 ; i < form.length; i++) {
                clear(form[i].id);
            }
        } else {
            clear(form.id)
        }

    }

    function CompareDates(startDateField, endDateField, startDateLabel, endDateLabel) {
        var first = getTimeFn(startDateField);
        var last = getTimeFn(endDateField);
        var isOk = true;
        if (first != null && last != null) {
            var dateDifference = Math.ceil((first - last) / (1000 * 3600 * 24));
            if (dateDifference > 0) {
                toastr.error(endDateLabel + " should not be less than " + startDateLabel + " .");
                isOk = false;
            }
        }
        return isOk;
    }

    function getTimeFn(selector) {
        var selectedDate = $("#" + selector)[0].value;
        if (selectedDate && (selectedDate != '' || typeof (selectedDate) != undefined)) {
            var time = new Date(moment.utc(new Date(selectedDate), 'MM/DD/YYYY')).getTime();
            return time;
        } else {
            return  null;
        }

    }
    function clear(form) {
        $('#' + form).find(':input , label[id]').each(function () {
            if (this.type == 'submit' || this.type == 'button' || this.type == 'hidden') {
                //do nothing
            } else if (this.type == 'checkbox' || this.type == 'radio') {
                this.checked = false;
            } else if (this.nodeName == 'label' || this.tagName == 'LABEL') {
                var control = $(this);
                control.html('');
                control.replaceWith(control = control.clone(true));
            } else if (this.type == 'file' || this.nodeName == 'label' || this.tagName == 'LABEL') {
                var control = $(this);
                control.replaceWith(control = control.clone(true));
            } else {
                $(this).val('');
            }
        });
    }
    return formService;
});

var formService = formServiceFn();

