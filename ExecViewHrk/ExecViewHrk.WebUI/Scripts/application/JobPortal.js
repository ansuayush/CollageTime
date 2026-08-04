(function () {
    var $r = $("#jobPortalRoot");
    if (!$r.length) return;

    $r.on("click", ".btn-apply", function () {
        var reqId = $(this).data("req");
        $.post($r.data("start-url"), { requisitionId: reqId }, function (res) {
            if (!res.success) {
                alert(res.message || "Unable to start application.");
                return;
            }
            window.open(res.url, "_blank");
        }).fail(function () {
            alert("Unable to start application.");
        });
    });
})();
