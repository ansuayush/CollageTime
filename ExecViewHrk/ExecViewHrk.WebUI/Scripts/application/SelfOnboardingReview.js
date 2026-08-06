(function () {
    var $root = $("#selfOnboardReviewRoot");
    if (!$root.length) return;

    function openHireWizard(hireId) {
        if (typeof LoadDetailView === "function") {
            LoadDetailView("ReviewWizardPartial", "SelfOnboarding", { hireId: hireId });
        } else if (typeof LoadContents === "function") {
            LoadContents("ReviewWizardPartial", "SelfOnboarding", { hireId: hireId });
        } else {
            window.location.href = ($("#ApplicationUrl").val() || "/") + "SelfOnboarding/ReviewWizardPartial?hireId=" + hireId;
        }
    }

    $(document).off("click.soHireName", ".link-hire-name, .btn-view-hire")
        .on("click.soHireName", ".link-hire-name, .btn-view-hire", function (e) {
            e.preventDefault();
            openHireWizard($(this).data("id"));
        });

    $(document).off("click.soHireApprove", ".btn-approve-hire")
        .on("click.soHireApprove", ".btn-approve-hire", function () {
            if (!confirm("Approve this registration as Hired?")) return;
            var id = $(this).data("id");
            $.post($root.data("approve-url") || $root.attr("data-approve-url"), { hireId: id }, function (res) {
                if (!res.success) { alert(res.message || "Failed"); return; }
                if (typeof LoadMenu === "function") LoadMenu("HireReviewPartial", "SelfOnboarding");
            });
        });
})();
