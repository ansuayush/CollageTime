window.PerformanceReviewForm = (function () {

    function esc(s) {
        return String(s == null ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;");
    }

    function formatRoleLabel(role) {
        var r = String(role || "").toUpperCase();
        if (r === "APPROVER1") return "Manager";
        if (r === "APPROVER2") return "Manager 2";
        if (r === "APPROVER3") return "Manager 3";
        if (r === "HR") return "HR";
        if (r === "EMPLOYEE") return "Self";
        return role || "Reviewer";
    }

    function taskProp(task, name) {
        if (!task) return null;
        if (task[name] != null && task[name] !== "") return task[name];
        var camel = name.charAt(0).toLowerCase() + name.slice(1);
        if (task[camel] != null && task[camel] !== "") return task[camel];
        return null;
    }

    function formatReviewerDisplay(task) {
        var roleLabel = taskProp(task, "ReviewerLabel") || formatRoleLabel(taskProp(task, "ReviewerRole") || task.ReviewerRole);
        var reviewerName = taskProp(task, "ReviewerName");
        if (reviewerName) return roleLabel.indexOf(reviewerName) >= 0 ? roleLabel : (roleLabel + " — " + reviewerName);
        return roleLabel;
    }

    function groupCriteria(criteria) {
        var types = [];
        var typeMap = {};
        (criteria || []).forEach(function (c) {
            var typeName = c.CriteriaTypeName || "Criteria";
            if (!typeMap[typeName]) {
                typeMap[typeName] = { name: typeName, sections: [], sectionMap: {} };
                types.push(typeMap[typeName]);
            }
            var sectionName = c.SectionName || "General";
            if (!typeMap[typeName].sectionMap[sectionName]) {
                var section = { name: sectionName, items: [] };
                typeMap[typeName].sectionMap[sectionName] = section;
                typeMap[typeName].sections.push(section);
            }
            typeMap[typeName].sectionMap[sectionName].items.push(c);
        });
        return types;
    }

    function renderToolbar(task, extraClass) {
        var rejectBtn = task.CanReject
            ? "<button type='button' class='btn btn-pr-reject pr-rf-reject'>Reject</button>"
            : "";
        return "<div class='pr-review-toolbar" + (extraClass ? " " + extraClass : "") + "'>" +
            "<button type='button' class='btn btn-pr-submit pr-rf-submit'>Submit</button>" +
            rejectBtn +
            "<button type='button' class='btn btn-pr-save pr-rf-save'>Save</button>" +
            "<button type='button' class='btn btn-pr-cancel pr-rf-cancel' data-dismiss='modal'>Cancel</button>" +
            "</div>";
    }

    function renderRejectModal(task) {
        if (!task.CanReject) return "";
        var options = (task.RejectTargets || []).map(function (t) {
            return "<option value='" + esc(t.Value) + "'>" + esc(t.Label) + "</option>";
        }).join("");
        return "<div id='prRejectOverlay' class='pr-reject-overlay' style='display:none;'>" +
            "<div class='pr-reject-panel'>" +
            "<div class='pr-reject-header'>" +
            "<h4 class='pr-reject-title'>Reject Review</h4>" +
            "<button type='button' class='close pr-reject-close' aria-label='Close'><span aria-hidden='true'>&times;</span></button>" +
            "</div>" +
            "<div class='pr-reject-body'>" +
            "<div class='form-group'><label>Send back to</label>" +
            "<select class='form-control' id='prRejectTarget'>" + options + "</select></div>" +
            "<div class='form-group'><label>Reason for Rejection <span class='text-danger'>*</span></label>" +
            "<input type='text' class='form-control' id='prRejectReason' placeholder='Reason for rejection' /></div>" +
            "<div class='form-group'><label>Comments</label>" +
            "<textarea class='form-control' id='prRejectComments' rows='4' placeholder='Explain what needs to be corrected...'></textarea></div>" +
            "</div>" +
            "<div class='pr-reject-footer'>" +
            "<button type='button' class='btn btn-default pr-reject-close'>Cancel</button>" +
            "<button type='button' class='btn btn-danger' id='prRejectConfirm'>Submit Rejection</button>" +
            "</div></div></div>";
    }

    function renderReworkBanner(task) {
        var reason = taskProp(task, "ReworkReason");
        var comments = taskProp(task, "ReworkComments");
        if (!reason && !comments) return "";
        return "<div class='pr-review-rework-banner'>" +
            "<strong><i class='fa fa-exclamation-triangle'></i> Rework Required</strong>" +
            (reason ? "<div><strong>Reason:</strong> " + esc(reason) + "</div>" : "") +
            (comments ? "<div><strong>Feedback:</strong> " + esc(comments) + "</div>" : "") +
            "</div>";
    }

    function renderAnswerInput(c, opts) {
        var code = (c.ResponseTypeCode || "").toLowerCase();
        var ansClass = opts.ansClass || "pr-rf-ans";
        var cid = c.ReviewCriteriaId;
        if (code === "text") {
            return "<textarea class='form-control " + ansClass + "' rows='4' data-cid='" + cid + "'>" + esc(c.Answer) + "</textarea>";
        }
        if (code === "numeric" || c.ResponseTypeId === 3) {
            return "<input type='number' step='0.01' class='form-control " + ansClass + "' data-cid='" + cid + "' value='" + esc(c.Answer) + "'/>";
        }
        var html = "<select class='form-control " + ansClass + "' data-cid='" + cid + "'><option value=''>--Select--</option>";
        (c.ScaleOptions || []).forEach(function (s) {
            html += "<option value='" + s.Id + "' data-score='" + s.ItemValue + "'" +
                (String(c.Answer) === String(s.Id) ? " selected" : "") + ">" + esc(s.ItemName) + "</option>";
        });
        html += "</select>";
        return html;
    }

    function renderCriteriaSections(criteria, opts, readonly) {
        if (!criteria || !criteria.length) {
            return "<div class='pr-review-empty'>No criteria assigned to this step.</div>";
        }
        var html = "";
        groupCriteria(criteria).forEach(function (type) {
            html += "<div class='pr-review-section'><h3 class='pr-review-section-title'>" + esc(type.name) + "</h3>";
            type.sections.forEach(function (section) {
                html += "<div class='pr-review-subsection'><h4 class='pr-review-subsection-title'>" + esc(section.name) + "</h4>";
                section.items.forEach(function (c, idx) {
                    var num = idx + 1;
                    var rowClass = "pr-criteria-row" + (readonly ? " pr-criteria-readonly" : "");
                    html += "<div class='" + rowClass + "'>" +
                        "<div class='pr-criteria-desc'>" + num + ". " + esc(c.Description) + "</div>" +
                        "<div class='pr-criteria-rating'><label>Rating</label>";
                    if (readonly) {
                        html += "<input type='text' class='form-control pr-readonly-field' readonly value='" + esc(c.Answer || "") + "'/>";
                    } else {
                        html += renderAnswerInput(c, opts);
                    }
                    html += "</div><div class='pr-criteria-comments'><label>Comments</label>";
                    if (readonly) {
                        html += "<textarea class='form-control pr-readonly-field' rows='4' readonly>" + esc(c.Comments || "") + "</textarea>";
                    } else {
                        html += "<textarea class='form-control " + (opts.cmtClass || "pr-rf-cmt") + "' rows='4' data-cid='" + c.ReviewCriteriaId + "'>" + esc(c.Comments) + "</textarea>";
                    }
                    html += "</div></div>";
                });
                html += "</div>";
            });
            html += "</div>";
        });
        return html;
    }

    function renderPriorStepSummary(p, task) {
        var score = p.Score != null ? esc(p.Score) : "---";
        var reviewer = p.ReviewerName || formatRoleLabel(p.ReviewerRole);
        var status = p.Status === "Submitted" ? "Submitted by " + reviewer : (p.Status || "Submitted");
        var date = p.SubmittedDate || task.ReviewDate || "";
        return "<div class='pr-review-summary pr-review-prior-summary'>" +
            "<div class='pr-review-summary-head'>Reviewee: <strong>" + esc(task.EmployeeName) + "</strong></div>" +
            "<div class='pr-review-summary-body'><div class='row'>" +
            "<div class='col-md-4 pr-review-meta'>" +
            "<div><strong>Manager:</strong> " + esc(task.ManagerName || "—") + "</div>" +
            "<div><strong>Date:</strong> " + esc(date) + "</div>" +
            "<div><strong>Status:</strong> " + esc(status) + "</div>" +
            "<div><strong>Reviewer:</strong> " + esc(reviewer) + "</div>" +
            "</div>" +
            "<div class='col-md-4 pr-review-score-tile'>" +
            "<div class='pr-review-score-value pr-prior-score'>" + score + "</div>" +
            "<div class='pr-review-score-label'>Total Score by this Reviewer</div>" +
            "</div>" +
            "<div class='col-md-4 pr-review-hr-box'><div class='pr-review-hr-placeholder'>For use by Human Resources</div></div>" +
            "</div></div></div>";
    }

    function renderPriorStepBlock(p, task) {
        var title = formatRoleLabel(p.ReviewerRole) + " Review";
        if (p.ReviewerName) title += " — " + p.ReviewerName;
        return "<div class='pr-review-prior-block pr-readonly'>" +
            "<div class='pr-review-prior-block-header'>" + esc(title) + " <span class='label label-default'>Read only</span></div>" +
            renderPriorStepSummary(p, task) +
            renderCriteriaSections(p.Criteria, {}, true) +
            renderReviewerCommentsBlock(p.Comments, true) +
            "</div>";
    }

    function renderPriorSteps(prior, task, collapsed) {
        if (!prior || !prior.length) return "";
        var hiddenClass = collapsed ? " pr-review-prior-collapsed" : "";
        var toggleBtn = collapsed
            ? "<div class='pr-review-view-previous-bar'>" +
              "<button type='button' class='btn btn-link pr-view-previous-btn'><i class='fa fa-eye'></i> View Previous</button>" +
              "</div>"
            : "";
        return toggleBtn +
            "<div class='pr-review-prior-wrap" + hiddenClass + "'>" +
            prior.map(function (p) { return renderPriorStepBlock(p, task); }).join("") +
            (collapsed ? "<div class='pr-review-view-previous-bar pr-review-hide-previous-bar' style='display:none;'>" +
             "<button type='button' class='btn btn-link pr-hide-previous-btn'><i class='fa fa-eye-slash'></i> Hide Previous</button></div>" : "") +
            "</div>";
    }

    function renderSummary(t, opts) {
        var score = t.Score != null ? esc(t.Score) : "---";
        var hrBox = "<div class='pr-review-hr-placeholder'>For use by Human Resources</div>";
        return "<div class='pr-review-summary'>" +
            "<div class='pr-review-summary-head'>Reviewee: <strong>" + esc(t.EmployeeName) + "</strong></div>" +
            "<div class='pr-review-summary-body'><div class='row'>" +
            "<div class='col-md-4 pr-review-meta'>" +
            "<div><strong>Manager:</strong> " + esc(t.ManagerName || "—") + "</div>" +
            "<div><strong>Date:</strong> " + esc(t.ReviewDate || "") + "</div>" +
            "<div><strong>Status:</strong> " + esc(t.ReviewStatusLabel || t.Status || "") + "</div>" +
            "<div><strong>Reviewer:</strong> " + esc(formatReviewerDisplay(t)) + "</div>" +
            "</div>" +
            "<div class='col-md-4 pr-review-score-tile'>" +
            "<div class='pr-review-score-value' id='prRfScoreValue'>" + score + "</div>" +
            "<div class='pr-review-score-label'>Total Score by this Reviewer</div>" +
            "</div>" +
            "<div class='col-md-4 pr-review-hr-box'>" + hrBox + "</div>" +
            "</div></div></div>";
    }

    function renderHrComments(task, opts) {
        opts = opts || {};
        var commentsId = opts.commentsId || "prRfComments";
        return "<div class='pr-review-hr-comments-section'>" +
            "<div class='pr-review-hr-comments-header'>HR COMMENTS</div>" +
            "<div class='pr-review-hr-comments-body'>" +
            "<textarea id='" + commentsId + "' class='form-control pr-review-hr-comments-input' rows='3' placeholder='Enter HR comments...'>" +
            esc(task.Comments || "") + "</textarea>" +
            "</div></div>";
    }

    function renderReviewerCommentsBlock(comments, readonly, opts) {
        opts = opts || {};
        var commentsId = opts.commentsId || "prRfComments";
        var body;
        if (readonly) {
            body = "<textarea class='form-control pr-review-reviewer-textarea pr-readonly-field' rows='6' readonly>" + esc(comments || "") + "</textarea>";
        } else {
            body = "<textarea id='" + commentsId + "' class='form-control pr-review-reviewer-textarea' rows='8' placeholder='Enter reviewer comments...'>" +
                esc(comments || "") + "</textarea>";
        }
        var attachHtml = readonly ? "" :
            "<div class='col-md-3 pr-review-reviewer-attach-col'>" +
            "<div class='pr-review-attach-row'>" +
            "<input type='text' class='form-control pr-review-attach-filename' id='prRfAttachName' readonly placeholder='No file selected' />" +
            "<button type='button' class='btn btn-default pr-review-attach-btn' id='prRfAttachBtn'>Attach File</button>" +
            "<input type='file' class='pr-review-attach-input' id='prRfAttachInput' style='display:none;' />" +
            "</div></div>" +
            "<div class='col-md-3 pr-review-reviewer-files-col'>" +
            "<div class='pr-review-attachments-panel'>" +
            "<div class='pr-review-attachments-title'>Attachments</div>" +
            "<div class='pr-review-attachments-list' id='prRfAttachList'><span class='text-muted'>No attachments</span></div>" +
            "</div></div>";
        return "<div class='pr-review-reviewer-section'>" +
            "<div class='pr-review-reviewer-header'>Reviewer Comments</div>" +
            "<div class='pr-review-reviewer-body'><div class='row'>" +
            "<div class='" + (readonly ? "col-md-12" : "col-md-6") + " pr-review-reviewer-comments-col'>" + body + "</div>" +
            attachHtml +
            "</div></div></div>";
    }

    function renderReviewerComments(task, opts) {
        return renderReviewerCommentsBlock(task.Comments, false, opts);
    }

    function render(task, opts) {
        opts = opts || {};
        var isHr = task.IsHrFinal;
        var currentTitle = isHr ? "HR Final Approval" : ("Your Review — " + formatReviewerDisplay(task));
        var collapsePrior = task.CollapsePriorSteps !== false && !isHr && task.PriorSteps && task.PriorSteps.length;

        var html = "<div class='pr-review-screen'>" +
            "<div class='pr-review-topbar'>" +
            "<div class='pr-review-topbar-title'>PERFORMANCE REVIEW</div>" +
            "<div class='pr-review-topbar-actions'>" +
            "<a href='javascript:void(0)' class='pr-rf-print' title='Print'><i class='fa fa-print'></i></a>" +
            "<button type='button' class='close pr-rf-cancel' data-dismiss='modal' aria-label='Close'><span aria-hidden='true'>&times;</span></button>" +
            "</div></div>" +
            renderToolbar(task);

        if (task.PriorSteps && task.PriorSteps.length) {
            html += renderPriorSteps(task.PriorSteps, task, collapsePrior);
        }

        html += "<div class='pr-review-current-block'>" +
            "<div class='pr-review-current-header'>" + esc(currentTitle) + "</div>" +
            renderReworkBanner(task);

        if (!isHr) {
            html += renderSummary(task, opts);
        } else if (task.PriorSteps && task.PriorSteps.length) {
            html += "<div class='pr-review-section'><p class='text-muted' style='margin:0 16px 8px;'>Review prior approver responses above, then add HR comments and submit final approval.</p></div>";
        }

        if (isHr) {
            html += renderHrComments(task, opts);
        } else {
            html += "<div class='pr-review-editable'>" + renderCriteriaSections(task.Criteria, opts, false) + "</div>";
            html += "<div class='pr-review-editable'>" + renderReviewerComments(task, opts) + "</div>";
        }

        html += "</div>" +
            renderToolbar(task, "pr-review-toolbar-bottom") +
            renderRejectModal(task) +
            "</div>";
        return html;
    }

    function bindViewPrevious($root) {
        $root.off("click.prViewPrior").on("click.prViewPrior", ".pr-view-previous-btn", function () {
            var $wrap = $root.find(".pr-review-prior-wrap");
            $wrap.removeClass("pr-review-prior-collapsed");
            $root.find(".pr-review-view-previous-bar").first().hide();
            $root.find(".pr-review-hide-previous-bar").show();
        });
        $root.off("click.prHidePrior").on("click.prHidePrior", ".pr-hide-previous-btn", function () {
            var $wrap = $root.find(".pr-review-prior-wrap");
            $wrap.addClass("pr-review-prior-collapsed");
            $root.find(".pr-review-hide-previous-bar").hide();
            $root.find(".pr-review-view-previous-bar").first().show();
        });
    }

    function hideRejectOverlay() {
        $("#prRejectOverlay").hide();
    }

    function showRejectOverlay($root) {
        var $overlay = $("#prRejectOverlay");
        if (!$overlay.length) {
            $overlay = $root.find("#prRejectOverlay");
            if ($overlay.length) {
                $overlay.appendTo("body");
            }
        }
        if (!$overlay.length) return;
        $overlay.find("#prRejectReason, #prRejectComments").val("");
        $overlay.show();
    }

    function bindReject($root, onReject) {
        $root.off("click.prReject").on("click.prReject", ".pr-rf-reject", function (e) {
            e.preventDefault();
            e.stopPropagation();
            showRejectOverlay($root);
        });
        $(document).off("click.prRejectClose").on("click.prRejectClose", ".pr-reject-close", function (e) {
            e.preventDefault();
            hideRejectOverlay();
        });
        $(document).off("click.prRejectConfirm", "#prRejectConfirm").on("click.prRejectConfirm", "#prRejectConfirm", function () {
            var $overlay = $("#prRejectOverlay");
            if (!$overlay.length) return;
            var target = $overlay.find("#prRejectTarget").val();
            var reason = ($overlay.find("#prRejectReason").val() || "").trim();
            var comments = ($overlay.find("#prRejectComments").val() || "").trim();
            if (!reason) { alert("Reason for rejection is required."); return; }
            if (typeof onReject === "function") onReject(target, comments, reason);
        });
    }

    function bindAttachFile($root) {
        $root.off("click.prAttach").on("click.prAttach", "#prRfAttachBtn", function () {
            $root.find("#prRfAttachInput").click();
        });
        $root.off("change.prAttach").on("change.prAttach", "#prRfAttachInput", function () {
            var file = this.files && this.files[0];
            if (!file) return;
            $root.find("#prRfAttachName").val(file.name);
            var $list = $root.find("#prRfAttachList").empty();
            $list.append("<div class='pr-review-attachment-item'><i class='fa fa-paperclip'></i> " + esc(file.name) +
                " <span class='text-muted'>(" + Math.round(file.size / 1024) + " KB)</span></div>");
        });
    }

    function collectAnswers(ansClass, cmtClass) {
        var map = {};
        $(".pr-review-editable ." + (ansClass || "pr-rf-ans")).each(function () {
            var cid = $(this).attr("data-cid");
            map[cid] = map[cid] || { ReviewCriteriaId: parseInt(cid, 10), Answer: "", Comments: "" };
            map[cid].Answer = $(this).val();
        });
        $(".pr-review-editable ." + (cmtClass || "pr-rf-cmt")).each(function () {
            var cid = $(this).attr("data-cid");
            map[cid] = map[cid] || { ReviewCriteriaId: parseInt(cid, 10), Answer: "", Comments: "" };
            map[cid].Comments = $(this).val();
        });
        return Object.keys(map).map(function (k) { return map[k]; });
    }

    function bindLiveScore($root) {
        $root.off("change.prScore").on("change.prScore", ".pr-review-editable .pr-rf-ans", function () {
            var total = 0;
            var count = 0;
            $root.find(".pr-review-editable .pr-rf-ans").each(function () {
                var $el = $(this);
                if ($el.is("select")) {
                    var val = parseFloat($el.find("option:selected").attr("data-score"));
                    if (!isNaN(val)) { total += val; count++; }
                } else if ($el.attr("type") === "number") {
                    var n = parseFloat($el.val());
                    if (!isNaN(n)) { total += n; count++; }
                }
            });
            $("#prRfScoreValue").text(count ? total.toFixed(2) : "---");
        });
    }

    function bindPrint($root) {
        $root.off("click.prPrint").on("click.prPrint", ".pr-rf-print", function (e) {
            e.preventDefault();
            window.print();
        });
    }

    function sumApproverScores(steps) {
        if (!steps || !steps.length) return null;
        var total = 0;
        var count = 0;
        steps.forEach(function (s) {
            var role = String(s.ReviewerRole || "").toUpperCase();
            if (role !== "APPROVER1" && role !== "APPROVER2" && role !== "APPROVER3") return;
            if (s.Status !== "Submitted" || s.Score == null || s.Score === "") return;
            var n = parseFloat(s.Score);
            if (isNaN(n)) return;
            total += n;
            count++;
        });
        return count ? total : null;
    }

    function formatFinalScore(value) {
        if (value == null || value === "") return "—";
        var n = parseFloat(value);
        if (isNaN(n)) return esc(value);
        return Number.isInteger(n) ? String(n) : n.toFixed(2);
    }

    function renderCompletedReview(detail) {
        var task = {
            EmployeeName: detail.EmployeeName,
            ManagerName: detail.ManagerName,
            ReviewDate: detail.ReviewDate,
            ReviewStatusLabel: detail.Status,
            ReviewName: detail.ReviewName
        };
        var finalScore = detail.FinalScore != null ? detail.FinalScore : sumApproverScores(detail.Steps);
        var html = "<div class='pr-review-screen pr-completed-view'>" +
            "<div class='pr-review-topbar'>" +
            "<div class='pr-review-topbar-title'>PERFORMANCE REVIEW</div>" +
            "<div class='pr-review-topbar-actions'>" +
            "<a href='javascript:void(0)' class='pr-rf-print' title='Print'><i class='fa fa-print'></i></a>" +
            "<button type='button' class='close pr-completed-close' data-dismiss='modal' aria-label='Close'><span aria-hidden='true'>&times;</span></button>" +
            "</div></div>" +
            "<div class='pr-review-section' style='margin:16px;'>" +
            "<h3 class='pr-review-section-title'>" + esc(detail.ReviewName || "Performance Review") + "</h3>" +
            "<div class='pr-review-meta pr-completed-meta'>" +
            "<div><strong>Employee:</strong> " + esc(detail.EmployeeName) + "</div>" +
            "<div><strong>Manager:</strong> " + esc(detail.ManagerName || "—") + "</div>" +
            "<div><strong>Initiated:</strong> " + esc(detail.ReviewDate || "") + "</div>" +
            "<div><strong>Status:</strong> " + esc(detail.Status || "") + "</div>" +
            "</div>" +
            "<div class='pr-completed-final-score'>" +
            "<div class='pr-completed-final-score-label'>Final Score</div>" +
            "<div class='pr-completed-final-score-value'>" + formatFinalScore(finalScore) + "</div>" +
            "<div class='pr-completed-final-score-hint'>Sum of Total Score by Approver 1, Approver 2, and Approver 3</div>" +
            "</div></div>";
        if (detail.Steps && detail.Steps.length) {
            html += renderPriorSteps(detail.Steps, task, false);
        } else {
            html += "<div class='pr-review-empty' style='margin:16px;'>No reviewer responses recorded yet.</div>";
        }
        html += "</div>";
        return html;
    }

    return {
        esc: esc,
        render: render,
        renderCompletedReview: renderCompletedReview,
        collectAnswers: collectAnswers,
        bindLiveScore: bindLiveScore,
        bindPrint: bindPrint,
        bindAttachFile: bindAttachFile,
        bindViewPrevious: bindViewPrevious,
        bindReject: bindReject,
        hideRejectOverlay: hideRejectOverlay
    };

})();
