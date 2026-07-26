window.EmployeeDocumentsUI = (function ($) {
    var cfg = {};
    var selectedEmployee = null;
    // { file: File, rotation: 0|90|180|270 }
    var pageItems = [];
    var signCanvas = null;
    var signCtx = null;
    var drawing = false;
    var hasStroke = false;
    var isSelfService = false;

    function flag($root, name) {
        var v = $root.attr("data-" + name);
        if (v == null || v === "") v = $root.data(name);
        return v === 1 || v === "1" || v === true;
    }

    function initFromDom() {
        var $root = $("#employeeDocumentsRoot");
        if (!$root.length) return;

        isSelfService = flag($root, "selfservice");

        var base = ($("#ApplicationUrl").val() || $("#appBaseUrl").val() || "/");
        if (base.slice(-1) !== "/") base += "/";

        cfg = {
            listUrl: $root.attr("data-list-url") || $root.data("list-url") || (base + "EmployeeDocuments/GetDocuments"),
            uploadUrl: $root.attr("data-upload-url") || $root.data("upload-url") || (base + "EmployeeDocuments/UploadAndSave"),
            deleteUrl: $root.attr("data-delete-url") || $root.data("delete-url") || (base + "EmployeeDocuments/DeleteDocument"),
            downloadUrl: $root.attr("data-download-url") || $root.data("download-url") || (base + "EmployeeDocuments/Download"),
            signUrl: $root.attr("data-sign-url") || $root.data("sign-url") || (base + "EmployeeDocuments/SignDocument"),
            canSign: true
        };

        // Identity employee from login — no search
        var empId = parseInt($root.attr("data-employee-id") || "0", 10);
        var empName = $root.attr("data-employee-name") || "";
        if (empId > 0) {
            selectedEmployee = {
                EmployeeId: empId,
                PersonName: empName
            };
            $("#selectedEmployeeId").val(empId);
        } else {
            selectedEmployee = null;
        }

        bindEvents();
        initSignCanvas();
        renderPreview();
        if (selectedEmployee) {
            loadDocuments(selectedEmployee.EmployeeId);
        }
    }

    function bindEvents() {
        $(document).off(".employeeDocs");

        $(document).on("change.employeeDocs", "#docPageFiles", function (e) {
            addFiles(e.target.files);
            this.value = "";
        });

        $(document).on("click.employeeDocs", "#btnClearPages", function () {
            pageItems = [];
            renderPreview();
        });

        $(document).on("click.employeeDocs", "#btnSaveDocument", saveDocument);
        $(document).on("click.employeeDocs", "#btnRefreshDocs", function () {
            if (selectedEmployee) loadDocuments(selectedEmployee.EmployeeId);
        });

        $(document).on("change.employeeDocs", "#docSignAfterSave", function () {
            $("#docSignAfterSaveFields").toggle(this.checked);
        });

        $(document).on("change.employeeDocs", "#docSignatureFile", function () {
            var file = this.files && this.files[0];
            if (!file) {
                clearUploadSignaturePreview();
                return;
            }
            if (!(file.type && file.type.indexOf("image/") === 0)) {
                alert("Please choose an image file for the signature.");
                this.value = "";
                clearUploadSignaturePreview();
                return;
            }
            var url = URL.createObjectURL(file);
            $("#docSignaturePreviewImg").attr("src", url);
            $("#docSignaturePreview").show();
        });

        $(document).on("click.employeeDocs", "#btnClearSignatureFile", function () {
            $("#docSignatureFile").val("");
            clearUploadSignaturePreview();
        });

        $(document).on("click.employeeDocs", ".btn-sign-doc", function () {
            openSignModal($(this).attr("data-id") || $(this).data("id"));
        });

        $(document).on("click.employeeDocs", ".btn-rotate-ccw", function () {
            rotatePage(parseInt($(this).attr("data-index"), 10), -90);
        });
        $(document).on("click.employeeDocs", ".btn-rotate-cw", function () {
            rotatePage(parseInt($(this).attr("data-index"), 10), 90);
        });

        $(document).on("click.employeeDocs", "#btnCancelSign, .employee-docs__modal-backdrop", closeSignModal);
        $(document).on("click.employeeDocs", "#btnConfirmSign", confirmSign);
        $(document).on("click.employeeDocs", "#btnClearSignCanvas", clearSignCanvas);
    }

    function initSignCanvas() {
        var canvas = document.getElementById("docSignCanvas");
        if (!canvas) return;
        signCanvas = canvas;
        signCtx = canvas.getContext("2d");
        clearSignCanvas();

        function pos(e) {
            var rect = canvas.getBoundingClientRect();
            var src = e.touches && e.touches[0] ? e.touches[0] : e;
            return {
                x: (src.clientX - rect.left) * (canvas.width / rect.width),
                y: (src.clientY - rect.top) * (canvas.height / rect.height)
            };
        }

        function start(e) {
            e.preventDefault();
            drawing = true;
            var p = pos(e);
            signCtx.beginPath();
            signCtx.moveTo(p.x, p.y);
        }
        function move(e) {
            if (!drawing) return;
            e.preventDefault();
            var p = pos(e);
            signCtx.lineTo(p.x, p.y);
            signCtx.stroke();
            hasStroke = true;
        }
        function end(e) {
            if (!drawing) return;
            e.preventDefault();
            drawing = false;
        }

        canvas.onmousedown = start;
        canvas.onmousemove = move;
        canvas.onmouseup = end;
        canvas.onmouseleave = end;
        canvas.ontouchstart = start;
        canvas.ontouchmove = move;
        canvas.ontouchend = end;
    }

    function clearSignCanvas() {
        hasStroke = false;
        if (!signCanvas || !signCtx) return;
        signCtx.fillStyle = "#ffffff";
        signCtx.fillRect(0, 0, signCanvas.width, signCanvas.height);
        signCtx.strokeStyle = "#1c232b";
        signCtx.lineWidth = 2;
        signCtx.lineCap = "round";
        signCtx.lineJoin = "round";
    }

    function clearUploadSignaturePreview() {
        $("#docSignaturePreview").hide();
        $("#docSignaturePreviewImg").attr("src", "");
    }

    function openSignModal(documentId) {
        $("#docSignDocumentId").val(documentId);
        $("#docSignName").val("");
        $("#docSignFile").val("");
        clearSignCanvas();
        $("#docSignModal").show().attr("aria-hidden", "false");
    }

    function closeSignModal() {
        $("#docSignModal").hide().attr("aria-hidden", "true");
        $("#docSignDocumentId").val("");
        $("#docSignFile").val("");
    }

    function getSignatureImageBase64() {
        if (!hasStroke || !signCanvas) return null;
        return signCanvas.toDataURL("image/png");
    }

    function readFileAsDataUrl(file) {
        return new Promise(function (resolve, reject) {
            if (!file) {
                resolve(null);
                return;
            }
            var reader = new FileReader();
            reader.onload = function () { resolve(reader.result); };
            reader.onerror = reject;
            reader.readAsDataURL(file);
        });
    }

    function confirmSign() {
        var documentId = $("#docSignDocumentId").val();
        var signerRole = $("#docSignRole").val();
        var signatureName = $.trim($("#docSignName").val() || "");
        var fileInput = document.getElementById("docSignFile");
        var file = fileInput && fileInput.files && fileInput.files[0] ? fileInput.files[0] : null;
        var drawn = getSignatureImageBase64();

        if (!documentId) return;
        if (!signatureName && !file && !drawn) {
            alert("Provide a typed name and/or a signature image (upload or draw). Or cancel — signing is optional.");
            return;
        }
        if (!signatureName) signatureName = "Signed";

        $("#btnConfirmSign").prop("disabled", true).text("Signing...");
        var imagePromise = file ? readFileAsDataUrl(file) : Promise.resolve(drawn);

        imagePromise.then(function (imageBase64) {
            return $.ajax({
                url: cfg.signUrl,
                type: "POST",
                dataType: "json",
                data: {
                    documentId: documentId,
                    signerRole: signerRole,
                    signatureName: signatureName,
                    signatureImageBase64: imageBase64
                }
            });
        }).then(function (res) {
            if (res && res.success) {
                closeSignModal();
                if (selectedEmployee) {
                    loadDocuments(selectedEmployee.EmployeeId);
                } else if (isSelfService) {
                    location.reload();
                }
                if (window.toastr) toastr.success(res.message);
                else alert(res.message);
            } else {
                alert((res && res.message) || "Sign failed.");
            }
        }, function (xhr) {
            alert((xhr && xhr.responseText) || "Sign failed.");
        }).then(function () {
            $("#btnConfirmSign").prop("disabled", false).text("Sign document");
        }, function () {
            $("#btnConfirmSign").prop("disabled", false).text("Sign document");
        });
    }

    function signDocumentAfterUpload(documentId, signerRole, signatureName, signatureImageBase64) {
        return $.ajax({
            url: cfg.signUrl,
            type: "POST",
            dataType: "json",
            data: {
                documentId: documentId,
                signerRole: signerRole,
                signatureName: signatureName || "Signed",
                signatureImageBase64: signatureImageBase64 || null
            }
        });
    }

    function addFiles(fileList) {
        if (!fileList || !fileList.length) return;
        for (var i = 0; i < fileList.length; i++) {
            pageItems.push({ file: fileList[i], rotation: 0 });
        }
        renderPreview();
    }

    function rotatePage(index, degrees) {
        if (index < 0 || index >= pageItems.length) return;
        var item = pageItems[index];
        if (!(item.file.type && item.file.type.indexOf("image/") === 0)) {
            alert("Only image pages can be rotated (not PDF).");
            return;
        }
        item.rotation = ((item.rotation + degrees) % 360 + 360) % 360;
        renderPreview();
    }

    function renderPreview() {
        var $preview = $("#docPreview");
        if (!$preview.length) return;
        $preview.empty();
        if (!pageItems.length) {
            $preview.append("<span class='text-muted'>No pages yet. Add scanner output images or one PDF. Use ↺ / ↻ to rotate images.</span>");
            return;
        }
        pageItems.forEach(function (item, index) {
            var file = item.file;
            var $thumb = $("<div class='employee-docs__thumb'/>");
            if (file.type && file.type.indexOf("image/") === 0) {
                var url = URL.createObjectURL(file);
                var $img = $("<img/>").attr("src", url).attr("alt", "Page " + (index + 1));
                if (item.rotation) {
                    $img.css("transform", "rotate(" + item.rotation + "deg)");
                }
                $thumb.append($img);
            } else {
                $thumb.append($("<div/>").css({ padding: "28px 0", fontWeight: 600 }).text("PDF"));
            }
            $thumb.append($("<span/>").text((index + 1) + ". " + file.name + (item.rotation ? (" (" + item.rotation + "°)") : "")));
            if (file.type && file.type.indexOf("image/") === 0) {
                var $actions = $("<div class='employee-docs__thumb-actions'/>");
                $actions.append(
                    $("<button type='button' class='btn btn-default btn-rotate-ccw' title='Rotate left'/>")
                        .attr("data-index", index)
                        .text("↺")
                );
                $actions.append(
                    $("<button type='button' class='btn btn-default btn-rotate-cw' title='Rotate right'/>")
                        .attr("data-index", index)
                        .text("↻")
                );
                $thumb.append($actions);
            }
            $preview.append($thumb);
        });
    }

    function loadImage(file) {
        return new Promise(function (resolve, reject) {
            var img = new Image();
            img.onload = function () { resolve(img); };
            img.onerror = reject;
            img.src = URL.createObjectURL(file);
        });
    }

    function rotateFileToBlob(file, rotation) {
        rotation = ((rotation % 360) + 360) % 360;
        if (!rotation) {
            return Promise.resolve(file);
        }
        return loadImage(file).then(function (img) {
            var canvas = document.createElement("canvas");
            var swap = rotation === 90 || rotation === 270;
            canvas.width = swap ? img.height : img.width;
            canvas.height = swap ? img.width : img.height;
            var ctx = canvas.getContext("2d");
            ctx.translate(canvas.width / 2, canvas.height / 2);
            ctx.rotate(rotation * Math.PI / 180);
            ctx.drawImage(img, -img.width / 2, -img.height / 2);
            URL.revokeObjectURL(img.src);
            return new Promise(function (resolve) {
                canvas.toBlob(function (blob) {
                    var name = file.name || "page.jpg";
                    if (!/\.jpe?g$/i.test(name) && !/\.png$/i.test(name)) name += ".jpg";
                    resolve(new File([blob], name, { type: blob.type || "image/jpeg" }));
                }, "image/jpeg", 0.92);
            });
        });
    }

    function prepareUploadFiles() {
        return Promise.all(pageItems.map(function (item) {
            if (item.file.type && item.file.type.indexOf("image/") === 0) {
                return rotateFileToBlob(item.file, item.rotation);
            }
            return Promise.resolve(item.file);
        }));
    }

    function signatureCellHtml(d) {
        var isSigned = !!(d.IsSigned || d.isSigned);
        if (!isSigned) return "<span class='text-muted'>Not signed</span>";
        var role = d.SignerRole || d.signerRole || "";
        var name = d.SignatureName || d.signatureName || "";
        var when = d.SignedDate || d.signedDate;
        var whenText = when ? (" · " + new Date(parseDate(when)).toLocaleDateString()) : "";
        return "<span class='employee-docs__signed'>Signed (" + role + ") — " + $("<div/>").text(name).html() + whenText + "</span>";
    }

    function loadDocuments(employeeId) {
        $.ajax({
            url: cfg.listUrl,
            type: "GET",
            dataType: "json",
            data: { employeeId: employeeId },
            cache: false
        }).done(function (docs) {
            var $body = $("#employeeDocsBody").empty();
            if (!docs || !docs.length) {
                $("#employeeDocsEmpty").show();
                return;
            }
            $("#employeeDocsEmpty").hide();
            $.each(docs, function (_, d) {
                var documentId = d.DocumentId || d.documentId;
                var fileName = d.FileName || d.fileName || "";
                var uploadedBy = d.UploadedBy || d.uploadedBy || "";
                var uploadedDate = d.UploadedDate || d.uploadedDate;
                var when = uploadedDate ? new Date(parseDate(uploadedDate)).toLocaleString() : "";
                var isSigned = !!(d.IsSigned || d.isSigned);

                var $tr = $("<tr/>").attr("data-id", documentId);
                $tr.append($("<td/>").text(fileName));
                $tr.append($("<td/>").text(uploadedBy));
                $tr.append($("<td/>").text(when));
                $tr.append($("<td/>").html(signatureCellHtml(d)));

                var $actions = $("<td/>");
                $actions.append(
                    $("<a class='btn btn-sm btn-default m-r-5'/>")
                        .attr("href", cfg.downloadUrl + "?documentId=" + documentId)
                        .text("View / Download")
                );
                if (!isSigned) {
                    $actions.append(
                        $("<button type='button' class='btn btn-sm btn-primary m-r-5 btn-sign-doc'/>")
                            .attr("data-id", documentId)
                            .text("Sign")
                    );
                }
                if (!isSelfService) {
                    $actions.append(
                        $("<button type='button' class='btn btn-sm btn-danger'/>")
                            .text("Delete")
                            .on("click", function () { deleteDocument(documentId); })
                    );
                }
                $tr.append($actions);
                $body.append($tr);
            });
        }).fail(function (xhr) {
            var msg = xhr.responseText || xhr.statusText || "Could not load documents.";
            if (/Invalid column name|IsSigned/i.test(msg)) {
                msg = "Signature columns missing. Run App_Data/Sql/EmployeeDocuments_AddSignature.sql on the client database.";
            }
            alert(msg);
        });
    }

    function parseDate(value) {
        if (typeof value === "string" && value.indexOf("/Date(") === 0) {
            return parseInt(value.replace(/\/Date\((\d+)\)\//, "$1"), 10);
        }
        return value;
    }

    function saveDocument() {
        if (!selectedEmployee || !selectedEmployee.EmployeeId) {
            alert("Select an employee first.");
            return;
        }
        if (!pageItems.length) {
            alert("Add scanned pages or a PDF first.");
            return;
        }

        var title = $.trim($("#docTitle").val() || "");
        if (!title) {
            title = prompt("Enter a document name for this scan:", "Scan_" + new Date().toISOString().slice(0, 19).replace(/[:T]/g, "") + ".pdf");
            if (!title || !$.trim(title)) {
                alert("Document name is required.");
                $("#docTitle").focus();
                return;
            }
            title = $.trim(title);
            if (!/\.pdf$/i.test(title)) title += ".pdf";
            $("#docTitle").val(title);
        } else if (!/\.pdf$/i.test(title)) {
            title += ".pdf";
            $("#docTitle").val(title);
        }

        var signAfter = $("#docSignAfterSave").is(":checked");
        var uploadSignerRole = $("#docUploadSignerRole").val();
        var uploadSignatureName = $.trim($("#docUploadSignatureName").val() || "");
        var sigInput = document.getElementById("docSignatureFile");
        var signatureFile = sigInput && sigInput.files && sigInput.files[0] ? sigInput.files[0] : null;

        // Signing is optional. If checked, need name and/or image.
        if (signAfter && !uploadSignatureName && !signatureFile) {
            alert("Optional sign is checked: add a typed name and/or upload a signature image, or uncheck to upload without signing.");
            return;
        }
        if (signAfter && !uploadSignatureName) {
            uploadSignatureName = (selectedEmployee && selectedEmployee.PersonName) || "Signed";
        }
        if (signAfter && !signatureFile) {
            alert("To place a signature on the last page of the PDF, please upload a signature image.");
            $("#docSignatureFile").focus();
            return;
        }

        $("#btnSaveDocument").prop("disabled", true).text("Saving...");

        prepareUploadFiles().then(function (files) {
            var formData = new FormData();
            formData.append("employeeId", selectedEmployee.EmployeeId);
            formData.append("documentTitle", title);
            if (signAfter) {
                formData.append("signerRole", uploadSignerRole || "Employee");
                formData.append("signatureName", uploadSignatureName || "Signed");
                if (signatureFile) {
                    formData.append("signatureFile", signatureFile, signatureFile.name);
                }
            }
            files.forEach(function (file) {
                formData.append("files", file, file.name);
            });

            return $.ajax({
                url: cfg.uploadUrl,
                type: "POST",
                data: formData,
                processData: false,
                contentType: false
            });
        }).then(function (res) {
            if (res && res.success) {
                pageItems = [];
                renderPreview();
                $("#docTitle").val("");
                $("#docSignAfterSave").prop("checked", false);
                $("#docSignAfterSaveFields").hide();
                $("#docUploadSignatureName").val("");
                $("#docSignatureFile").val("");
                clearUploadSignaturePreview();
                loadDocuments(selectedEmployee.EmployeeId);
                if (window.toastr) toastr.success(res.message);
                else alert(res.message);
            } else {
                alert((res && res.message) || "Save failed.");
            }
        }, function (xhr) {
            alert((xhr && xhr.responseText) || "Save failed.");
        }).then(function () {
            $("#btnSaveDocument").prop("disabled", false).text("4. Save as PDF & upload");
        }, function () {
            $("#btnSaveDocument").prop("disabled", false).text("4. Save as PDF & upload");
        });
    }

    function deleteDocument(documentId) {
        if (!confirm("Delete this document?")) return;
        $.post(cfg.deleteUrl, { documentId: documentId })
            .done(function (res) {
                if (res && res.success) {
                    if (selectedEmployee) loadDocuments(selectedEmployee.EmployeeId);
                } else {
                    alert((res && res.message) || "Delete failed.");
                }
            });
    }

    return { initFromDom: initFromDom };
})(jQuery);

$(document).ready(function () {
    if (window.EmployeeDocumentsUI) {
        EmployeeDocumentsUI.initFromDom();
    }
});
