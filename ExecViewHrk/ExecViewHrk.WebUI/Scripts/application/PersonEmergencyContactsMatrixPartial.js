

function showDetails(e) {
    e.preventDefault();
    
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    var wnd = $("#EmegencyPhoneNumbersDetails").data("kendoWindow");
    var appBaseUrl = $("#appBaseUrl").val();
    wnd.title("" + dataItem.PersonName + " : Phone Numbers")
    $.ajax({
        url: appBaseUrl + "PersonPhoneNumbers/GetPersonPhoneNumbersListByPersonId?_personId=" + dataItem.RelationPersonId,
        type: "GET",
        success: function (response) {
            var _html = "<div id='details-container'>";
            _html += "<div>";
            if (response.length > 0) {
                for (var i = 0; i < response.length; i++) {
                    _html += "<p>" + response[i].PersonPhoneType + ":  " + PhoneMasking(response[i].PersonPhoneNumber) + "</p>";
                }
            } else {
                _html += "<p> No phone number available.</p>";
            }
            _html += "</div><div>";
            wnd.content(_html);
            wnd.center().open();
        }
    });

}

