function GetProviderDashboard() {
    event.preventDefault();

    showLoader();

    var physicianId = $('#sessionId').val();
    var lastStatus = $('#lastStatus').val();


    $.ajax({
        methd: "GET",
        url: "/Provider/GetProviderDashboard",
        data: { physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }

            $('#provider-home-tab-pane').html(result);

            if (lastStatus != 1) {
                $('#new-tab').removeClass("active");
            }
            if (lastStatus == 2) {
                $('#Pending-tab').addClass("active");
            }
            if (lastStatus == 4) {
                $('#Active-tab').addClass("active");
            }
            if (lastStatus == 6) {
                $('#Conclude-tab').addClass("active");
            }

            setTimeout(function () {
                hideLoader();
            }, 300);
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function ProviderTableRecords(status, requesttypeid) {

    var physicianId = $('#sessionId').val();
    $('#lastStatus').val(status[0]);

    $.ajax({
        method: "POST",
        url: "/Provider/ProviderTableRecords",
        data: { status: status, requesttypeid: requesttypeid, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#tableRecords').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function ProviderFilterTable(status, requesttypeid) {

    if (requesttypeid == null) {
        requesttypeid = $('#reqTypeValueId').val();
    }
    if (requesttypeid == 0) {
        requesttypeid = null;
    }

    var physicianId = $('#sessionId').val();
    $('#lastStatus').val(status[0]);

    $.ajax({
        method: "POST",
        url: "/Provider/ProviderFilterTable",
        data: { status: status, requesttypeid: requesttypeid, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#providerRequestTable').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}



// ************************* Accept Case ******************************
function AcceptCaseModal(requestid) {

    $.ajax({
        methd: "GET",
        url: "/Provider/AcceptCaseModal",
        data: { requestid: requestid },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#acceptModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function AcceptCase(requestId) {
    event.preventDefault();

    var physicianId = $('#sessionId').val();


    $.ajax({
        method: "POST",
        url: "/Provider/AcceptCase",
        data: { requestId: requestId, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#acceptModalId').modal('hide');
            Swal.fire("Hurrah", "Case Accepted", "success");

            GetProviderDashboard();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}



// ************************** Transfer Case ***************************
function Transfer(requestid) {

    var physicianId = $('#sessionId').val();

    $.ajax({
        method: "GET",
        url: "/Provider/Transfer",
        data: { requestid: requestid, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#providerTransferModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

function TransferCase() {
    event.preventDefault();

    if ($('#providerTransferCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Provider/TransferCase",
            data: $('#providerTransferCaseForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#providerTransferModalId').modal('hide');
                Swal.fire("Hurrah", "Case Transfer To Admin", "success");

                GetProviderDashboard();
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}



// **************************** Encounter *****************************
function EncounterModal(requestid) {

    var physicianId = $('#sessionId').val();

    $.ajax({
        method: "GET",
        url: "/Provider/EncounterModal",
        data: { requestid: requestid, physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#EncounterModalBox').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

function PostEncounterCare() {
    event.preventDefault();

    if ($('#EncounterModalForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Provider/PostEncounterCare",
            data: $('#EncounterModalForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#EncounterModalBox').modal('hide');
                GetProviderDashboard();
                if (result) {
                    Swal.fire("Hurrah", "Care type will be House call", "success");
                }
                else {
                    Swal.fire("Hurrah", "Care type will be Conclude", "success");
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");

            }
        });
    }
}

function finalizeEncounterModal(requestId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Provider/FinalizeEncounterModal",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#EncounterFinalizeModal').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

function finalizeEncounter(requestId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Provider/FinalizeEncounter",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#EncounterFinalizeModal').modal('hide');
            Swal.fire("Hurrah", "Case Finalized", "success");
            GetProviderDashboard();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

function HouseCall(requestId) {

    $.ajax({
        method: "GET",
        url: "/Provider/HouseCall",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#houseCallModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

function HouseCallPost(requestId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Provider/HouseCallPost",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#houseCallModalId').modal('hide');
            Swal.fire("Hurrah", "Case House Called", "success");
            GetProviderDashboard();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

function DownloadEncounter(requestId) {

    $.ajax({
        method: "GET",
        url: "/Provider/DownloadEncounter",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showProviderCaseModal').html(result);
            $('#DownloadFinalizeModal').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

function FinalizeDownload(requestid) {
    event.preventDefault();

    window.location.href = './GeneratePDF?requestId=' + requestid;

    $('#DownloadFinalizeModal').modal('hide');
    Swal.fire("Hurrah", "Form Downloaded", "success");

}



// *************************** Conclude Case ***************************

function GetConcludeCare(requestid) {
    showLoader();

    $.ajax({
        methd: "GET",
        url: "/Provider/GetConcludeCare",
        data: { requestid: requestid },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#provider-home-tab-pane').html(result);

            setTimeout(function () {
                hideLoader();
            }, 300);

        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function UploadConcludeDocument() {
    event.preventDefault();

    var formdata = new FormData($('#concludeDocumentFormId')[0]);

    $.ajax({
        method: "POST",
        url: "/Provider/UploadConcludeDocument",
        data: formdata,
        processData: false,
        contentType: false,

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            Swal.fire("Hurrah", "Document Uploaded", "success");

            GetConcludeCare(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function DeleteConcludeFile(requestwisefileid, requestid) {
    event.preventDefault();

    $.ajax({
        methd: "POST",
        url: "/Provider/DeleteConcludeFile",
        data: { requestwisefileid: requestwisefileid },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            Swal.fire("Hurrah", "Document Deleted", "success");

            GetConcludeCare(requestid);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    })
}

function ConcludeCare() {
    event.preventDefault();

    if ($('#ConcludeFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/Provider/ConcludeCare",
            data: $('#ConcludeFormId').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                Swal.fire("Hurrah", "Case Concluded", "success");
                GetProviderDashboard();
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");

            }
        });
    }
}



// ************************* Request To Admin *************************

function SendRequestToAdmin() {
    event.preventDefault();

    if ($('#requestToAdminFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/Provider/SendRequestToAdmin",
            data: $('#requestToAdminFormId').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#requestToAdminModal').modal('hide');
                Swal.fire("Hurrah", "Requested sent to Admin", "success");

                GetProviderDashboard();
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}



// ***************************** Invoicing *****************************

function GetProviderInvoicing() {
    showLoader();

    var dateSelected = $('#searchByTimeperiod').val();

    if (dateSelected == undefined) {
        var currentDate = new Date();
        var currentDay = currentDate.getDate();
        var currentMonth = currentDate.getMonth() + 1;
        dateSelected = currentMonth + "-" + (currentDay <= 15 ? "1" : "2");
    }

    $.ajax({
        methd: "GET",
        url: "/Provider/GetProviderInvoicing",
        data: { dateSelected: dateSelected },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#provider-invoicing-tab-pane').html(result);
            if (dateSelected != undefined) {
                $('#searchByTimeperiod').val(dateSelected);
            }
            setTimeout(function () {
                hideLoader();
            }, 300);

        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function OpenFinalizeTimeSheet(dateSelected) {
    showLoader();

    if (dateSelected == null) {
        dateSelected = $('#searchByTimeperiod').val();
    }

    $.ajax({
        methd: "GET",
        url: "/Provider/OpenFinalizeTimeSheet",
        data: { dateSelected: dateSelected },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#provider-invoicing-tab-pane').html(result);

            setTimeout(function () {
                hideLoader();
            }, 300);

        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function PostFinalizeTimesheet(callId) {
    event.preventDefault();

    var formData = $('#finalizeSheetForm').serializeArray();
    
    $.ajax({
        method: "POST",
        url: "/Provider/PostFinalizeTimesheet",
        data: formData,

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (result) {
                Swal.fire("Hurrah", "Timesheet Updated", "success");
                if (callId == 1) {
                    GetAdminFinalizeTimeSheet(formData[0].value, formData[1].value);
                }
                if (callId == 2) {
                    OpenFinalizeTimeSheet(formData[0].value);
                }
            }
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function GetAddReceipts(timeSheetDetailId, callId) {

    $.ajax({
        methd: "GET",
        url: "/Provider/GetAddReceipts",
        data: { timeSheetDetailId: timeSheetDetailId, callId: callId },
        traditional: true,

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#AddReceiptsContainer').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function PostAddReceipt(counter) {
    event.preventDefault();
    debugger
    var formData = new FormData();

    var timeSheetDetailId = $(`#inputTimeSheetDetailId${counter}`).val();
    var item = $(`#inputItem${counter}`).val();
    var amount = $(`#inputAmount${counter}`).val();
    var file = null;

    if ($(`#fileValue${counter}`).val() == "" || $(`#fileValue${counter}`).val() == null) {
        file = $(`#inputFile${counter}`)[0].files[0];
    }

    formData.append("timeSheetDetailId", timeSheetDetailId);
    formData.append("item", item);
    formData.append("amount", amount);
    formData.append("file", file);

    $.ajax({
        method: "POST",
        url: "/Provider/PostAddReceipt",
        data: formData,
        processData: false,
        contentType: false,

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "Success",
                text: "Receipt Updated",
                icon: "success",
            })
            $('#AddRecieptBtn').click();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function DeleteReceipt(timeSheetDetailId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Provider/DeleteReceipt",
        data: { timeSheetDetailId: timeSheetDetailId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "Success",
                text: "Receipt Deleted",
                icon: "success",
            })
            $('#AddRecieptBtn').click();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function ConfirmFinalizeTimeSheet(timeSheetId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Provider/ConfirmFinalizeTimeSheet",
        data: { timeSheetId: timeSheetId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "Success",
                text: "Timesheet Finalized",
                icon: "success",
            })
            GetProviderInvoicing();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
