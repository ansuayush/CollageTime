(function () {
    var $root = $("#prStartRoot");
    if (!$root.length) return;
    function url(n) { return $root.attr("data-" + n) || ""; }

    function load() {
        $.getJSON(url("list-url"), function (r) {
            var $tb = $("#prStartTable tbody").empty();
            ((r && r.data) || []).forEach(function (x) {
                var canStart = x.Status === "Draft" || x.Status === "Ready" || x.Status === "InProgress";
                $tb.append("<tr><td>" + (x.ReviewName || "") + "</td><td>" + (x.Status || "") + "</td><td>" + (x.FromDate || "—") +
                    "</td><td>" + ((x.EmployeeNames || []).join(", ")) +
                    "</td><td>" + (canStart ? "<button type='button' class='btn btn-xs btn-primary btn-start-review' data-id='" + x.ReviewId + "'>Start</button>" : "") + "</td></tr>");
            });
        });
    }

    $(document).on("click", ".btn-start-review", function () {
        var id = $(this).attr("data-id");
        if (!confirm("Start this review and notify the first approver(s)?")) return;
        $.post(url("start-url"), { reviewId: id }, function (res) {
            alert((res && res.message) || (res && res.success ? "Started" : "Failed"));
            load();
        });
    });

    load();
})();
