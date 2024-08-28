function showLoader() {
    document.querySelector(".loader-container").style.display = "flex";
    document.querySelector(".backdrop").style.display = "flex";
}

function hideLoader() {
    document.querySelector(".loader-container").style.display = "none";
    document.querySelector(".backdrop").style.display = "none";
}

hideLoader();



// *********************************************************** Get Dashboard data ***********************************************************

function GetDashboard() {
    event.preventDefault();

    showLoader();
    var PrevStatus = $('#PrevStatus').val();

    $.ajax({
        methd: "GET",
        url: "/Admin/GetDashboard",

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#home-tab-pane').html(result);

            if (PrevStatus != 1) {
                $('#new-tab').removeClass("active");
            }
            if (PrevStatus == 1) {
                $('#new-tab').addClass("active");
            }
            if (PrevStatus == 2) {
                $('#Pending-tab').addClass("active");
            }
            if (PrevStatus == 4) {
                $('#Active-tab').addClass("active");
            }
            if (PrevStatus == 6) {
                $('#Conclude-tab').addClass("active");
            }
            if (PrevStatus == 3) {
                $('#ToClose-tab').addClass("active");
            }
            if (PrevStatus == 9) {
                $('#Unpaid-tab').addClass("active");
            }
            if (PrevStatus == "") {
                $('#new-tab').addClass("active");
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

function TableRecords(status, requesttypeid, regionid) {

    $('#PrevStatus').val(status[0]);

    $.ajax({
        method: "POST",
        url: "/Admin/TableRecords",
        data: { status: status, requesttypeid: requesttypeid, regionid: regionid },

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

function FilterTableRecords(status, requesttypeid) {

    var regionid = $('#regionId').val();
    $('#PrevStatus').val(status[0]);

    if (requesttypeid == null) {
        requesttypeid = $('#reqTypeValueId').val();
    }
    if (requesttypeid == 0) {
        requesttypeid = null;
    }

    $.ajax({
        method: "POST",
        url: "/Admin/FilterTableRecords",
        data: { status: status, requesttypeid: requesttypeid, regionid: regionid },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#requestTable').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}



// **************************** Send Link *******************************
function sendLinkModal(status, callId) {
    $.ajax({
        method: "GET",
        url: "/Admin/SendLinkModal",
        data: { status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (callId == 2) {
                $('#showProviderCaseModal').html(result);
            }
            else {
                $('#showCaseModal').html(result);
            }
            $('#sendLinkModal').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

function SendLink(status, callId) {
    event.preventDefault();

    if ($('#sendLinkFormId').valid()) {
        var formdata = $('#sendLinkFormId').serialize();

        $.ajax({
            method: "POST",
            url: "/Admin/SendLink",
            data: formdata,

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#sendLinkModal').modal('hide');
                if (callId == 2) {
                    GetProviderDashboard();
                }
                else {
                    GetDashboard();
                }
                Swal.fire("Hurrah", "Link sent", "success");
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}



// ******************************** Create Request ***********************
function adminCreateRequest(status, callId) {
    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/AdminCreateRequest",
        data: { status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            if (callId == 2) {
                $('#provider-home-tab-pane').html(result);
            }
            else {
                $('#home-tab-pane').html(result);
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

function sendAdminCreateRequest(status, callId) {
    event.preventDefault();

    if ($('#AdminCreateRequestFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/SendAdminCreateRequest",
            data: $('#AdminCreateRequestFormId').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                if (callId == 2) {
                    GetProviderDashboard();
                }
                else {
                    GetDashboard();
                }
                Swal.fire("Hurrah", "Request Created", "success");
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}



// *************************** Export *********************************
function Export(arr, requesttypeid) {
    showLoader();

    var regionid = $('#regionId').val();

    requesttypeid = $('#reqTypeValueId').val();
    if (requesttypeid == 0) {
        requesttypeid = null;
    }

    var arr2 = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];

    if (JSON.stringify(arr) == JSON.stringify(arr2)) {
        regionid = 0;
        requesttypeid = null;
    }

    $.ajax({
        method: "POST",
        url: "/Admin/Export",
        data: { arr: arr, requesttypeid: requesttypeid, regionid: regionid },
        xhrFields: {
            responseType: 'blob'
        },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }

            var blob = new Blob([result], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });

            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = 'RequestData.xlsx';

            document.body.appendChild(link);
            link.click();

            document.body.removeChild(link);
            window.URL.revokeObjectURL(link.href);

            hideLoader();
            Swal.fire("Hurrah", "File Downloaded", "success");
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}



// ************************** Request DTY Support **********************
function RequestSupport() {
    $.ajax({
        method: "GET",
        url: "/Admin/RequestSupport",

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showCaseModal').html(result);
            $('#requestSupportModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function requestDTYPost() {
    event.preventDefault();

    if ($('#requestDTYFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/RequestDTYPost",
            data: $('#requestDTYFormId').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#requestSupportModalId').modal('hide');
                Swal.fire("Hurrah", "Requested Support", "success");
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}



// ******************************************************** New State ******************************************************

// *********************** fetch view case *************************
function ViewCaseRecords(status, requestid, callId) {

    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/ViewCaseRecords",
        data: { status: status, requestid: requestid, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (callId == 2) {
                $('#provider-home-tab-pane').html(result);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
            else if (callId == 1) {

                $('#Patient-tab-pane').html(result);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
            else {
                $('#home-tab-pane').html(result);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }

        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function UpdateViewCaseRecords(status, requestid, callId) {
    event.preventDefault();

    if ($('#viewCaseDataForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/UpdateViewCaseRecords",
            data: $('#viewCaseDataForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                Swal.fire("Hurrah", "Case Updated", "success");
                ViewCaseRecords(status, requestid, callId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}



// ************************ fetch view notes ***********************
function ViewNotes(requestid, status, callId) {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/Admin/ViewNotes",
        data: { requestid: requestid, status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (callId == 2) {
                $('#provider-home-tab-pane').html(result);
                $('#adminNote').val("");
            }
            else {
                $('#home-tab-pane').html(result);
                $('#adminNote').val("");
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

function UpdateViewNotes() {
    event.preventDefault();

    if ($('#viewNotesForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/UpdateViewNotes",
            data: $('#viewNotesForm').serialize(),

            success: function (response) {
                if (response.code == 401) {
                    location.reload();
                }
                ViewNotes(response.reqId, response.status, response.callId);
                Swal.fire("Hurrah", "Notes Updated", "success");
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}



// ************************ show cancel modal ************************
function CancelModal(requestid, status) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/Admin/CancelModal",
        data: { requestid: requestid, status: status },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showCaseModal').html(result);
            $('#cancelModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

function CancelCase(requestId) {
    event.preventDefault();

    if ($('#cancelCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/CancelCase",
            data: $('#cancelCaseForm').serialize(),

            success: function (response) {
                if (response.code == 401) {
                    location.reload();
                }
                $('#cancelModalId').modal('hide');
                Swal.fire("Hurrah", "Case Cancelled", "success");

                GetDashboard();
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");

            }
        });
    }
}



// ************************** show assign modal *************************
function AsignModal(requestid, modalId) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/Admin/AsignModal",
        data: { requestid: requestid, modalId: modalId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showCaseModal').html(result);
            $('#assignModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

function FilterAsignModal(regionid) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/Admin/FilterAsignModal",
        data: { regionid: regionid },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $("#physicianId").empty();
            result.success.forEach(obj => {
                $('#physicianId').append($("<option></option>").attr("value", obj.physicianid).text(obj.firstname + " " + obj.lastname))
            });
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

function AsignCase() {
    event.preventDefault();

    var status = $('#statusForName').val();

    if ($('#asignCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/AsignCase",
            data: $('#asignCaseForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#assignModalId').modal('hide');
                if (status == 1) {
                    Swal.fire("Hurrah", "Case Assigned", "success");

                }
                else {
                    Swal.fire("Hurrah", "Case Transferred", "success");

                }
                GetDashboard();
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");

            }
        });
    }
}



// ************************** show block modal ***************************
function BlockModal(requestid) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/Admin/BlockModal",
        data: { requestid: requestid },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showCaseModal').html(result);
            $('#blockModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

function BlockCase() {
    event.preventDefault();
    if ($('#blockCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/BlockCase",
            //url: $('#blockCaseForm').attr("action"),
            data: $('#blockCaseForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#blockModalId').modal('hide');
                Swal.fire("HUrrah", "Case Blocked", "success");

                GetDashboard();
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}



// *********************************************** Pending State ******************************************************
// *********************** fetch view documents ***************************
function ViewDocuments(requestid, status, callId) {
    showLoader();
    $.ajax({
        methd: "GET",
        url: "/Admin/ViewDocuments",
        data: { requestid: requestid, status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (callId == 2) {
                $('#provider-home-tab-pane').html(result);
            }
            if (callId == 1) {
                $('#Patient-tab-pane').html(result);
            }
            else {
                $('#home-tab-pane').html(result);
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

function UploadDocument(requestid, status, callId) {

    var formdata = new FormData($('#viewDocumentFormId')[0]);
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Admin/UploadDocument",
        data: formdata,
        processData: false,
        contentType: false,

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            Swal.fire("Hurrah", "Document Uploaded", "success");

            ViewDocuments(requestid, status, callId);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function DeleteFile(requestwisefileid, requestid, status, callId) {
    event.preventDefault();

    $.ajax({
        methd: "GET",
        url: "/Admin/DeleteFile",
        data: { requestwisefileid: requestwisefileid, requestid: requestid, status: status },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            else {
                Swal.fire("Hurrah", "Document Deleted", "success");
            }

            ViewDocuments(requestid, status, callId);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    })
}

function SendFile(requestwisefileid, requestid, status, callId) {
    event.preventDefault();

    $.ajax({
        methd: "GET",
        url: "/Admin/SendFile",
        data: { requestwisefileid: requestwisefileid, requestid: requestid, status: status },
        traditional: true,

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (requestwisefileid.length === 0) {
                Swal.fire("Oops", "Please Select Any File To Send Mail", "error");
            }
            else {
                Swal.fire("Hurrah", "Files Sent", "success");
                ViewDocuments(requestid, status, callId);
            }
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}



// *************************** fetch Send agreement **************************
function SendAgreementModal(requestid, requesttypeid, status, callId) {

    $.ajax({
        methd: "GET",
        url: "/Admin/SendAgreementModal",
        data: { requestid: requestid, requesttypeid: requesttypeid, status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (callId == 2) {
                $('#showProviderCaseModal').html(result);
            }
            else {
                $('#showCaseModal').html(result);
            }
            $('#SendAgreementModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function SendAgreement(callId) {
    event.preventDefault();

    var status = $('#statusForNameId').val();

    if ($('#sendAgreementFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/SendAgreement",
            data: $('#sendAgreementFormId').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#SendAgreementModalId').modal('hide');
                Swal.fire("Hurrah", "Agreement Sent", "success");
                if (callId == 2) {
                    GetProviderDashboard();
                }
                else {
                    GetDashboard();
                }

            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}



// ************************ show clear case modal *****************************
function ClearModal(requestid, status) {
    $.ajax({
        methd: "GET",
        url: "/Admin/ClearModal",
        data: { requestid: requestid, status: status },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showCaseModal').html(result);
            $('#clearModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function ClearCase() {
    event.preventDefault();
    var requestId = $('#clearRequestId').val();
    var status = $('#statusForNameId').val();

    $.ajax({
        method: "POST",
        url: "/Admin/ClearCase",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#clearModalId').modal('hide');
            Swal.fire("Hurrah", "Case Cleared", "success");

            GetDashboard();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}



// **************************************************** Active State ***********************************************
// ************************* fetch send orders **********************************
function GetSendOrderData(requestid, status, callId) {
    showLoader();

    $.ajax({
        methd: "GET",
        url: "/Admin/GetSendOrderData",
        data: { requestid: requestid, status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (callId == 2) {
                $('#provider-home-tab-pane').html(result);
            }
            else {
                $('#home-tab-pane').html(result);
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

function FilterBusinessByProfessions(professionid) {
    $.ajax({
        method: "GET",
        url: "/Admin/FilterBusinessByProfessions",
        data: { professionid: professionid },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }

            $("#businessId").empty();
            $('#businessId').append($("<option></option>").attr("value", "").text("Select Business"));
            result.success.forEach(obj => {
                $('#businessId').append($("<option></option>").attr("value", obj.vendorid).text(obj.vendorname))
            });

            $('#BusinessContact').val("");
            $('#BusinessEmail').val("");
            $('#BusinessFax').val("");
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function BusinessData(businessid) {
    $.ajax({
        method: "GET",
        url: "/Admin/BusinessData",
        data: { businessid: businessid },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }

            $('#BusinessContact').val("");
            $('#BusinessEmail').val("");
            $('#BusinessFax').val("");

            var businessData = result.success;
            $('#BusinessContact').val(`${businessData.phonenumber}`);
            $('#BusinessEmail').val(`${businessData.email}`);
            $('#BusinessFax').val(`${businessData.faxnumber}`);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function SendOrderData(callId) {
    event.preventDefault();

    var status = $('#statusForNameId').val();

    if ($('#SendOrderForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/SendOrderData",
            data: $('#SendOrderForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                if (callId == 2) {
                    GetProviderDashboard();
                }
                else {
                    GetDashboard();
                }
                Swal.fire("Hurrah", "Order Sent", "success");
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}



// **************************************************** To Close State **********************************************
// ************************** fetch close case ********************************
function CloseCase(requestid, status) {
    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/CloseCase",
        data: { requestid: requestid, status: status },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            setTimeout(function () {
                hideLoader();
            }, 300);

            $('#home-tab-pane').html(result);
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }

    });
}

function updateCloseCase(requestid, status) {
    event.preventDefault();

    if ($('#closeCaseForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/UpdateCloseCase",
            data: $('#closeCaseForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                Swal.fire("Hurrah", "Close case updated", "success");

                CloseCase(requestid, status);

            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}

function PostCloseCase(requestId, status) {
    $.ajax({
        method: "POST",
        url: "/Admin/PostCloseCase",
        data: { requestId: requestId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            Swal.fire("Hurrah", "Case closed", "success");

            GetDashboard();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}



// **************************** fetch admin encounter ************************

function AdminEncounter(requestid, status, callId) {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/Admin/AdminEncounter",
        data: { requestid: requestid, status: status, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            setTimeout(function () {
                hideLoader();
            }, 300);
            if (callId == 2) {
                $('#provider-home-tab-pane').html(result);
            }
            else if (callId == 1) {
                $('#Patient-tab-pane').html(result);
            }
            else {
                $('#home-tab-pane').html(result);
            }

        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function SubmitEncounter() {
    event.preventDefault();
    var formdata = $('#EncounterForm').serialize();

    $.ajax({
        method: "POST",
        url: "/Admin/SubmitEncounter",
        data: formdata,
        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }

            Swal.fire("Hurrah", "Encounter Form Updated", "success");

            AdminEncounter(result.reqId, result.status, result.callid);
        },
        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}



// *********************************************************** Profile data ***********************************************************
function GetAdminProfile(aspnetId, callId) {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/Admin/GetAdminProfile",
        data: { aspnetId: aspnetId, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            else {
                setTimeout(function () {
                    hideLoader();
                }, 300);

                if (callId == 1) {
                    $('#Admin-profile-tab-pane').empty();
                    $('#User-tab-pane').empty();
                    $('#Admin-profile-tab-pane').html(result);
                }
                if (callId == 2) {
                    $('#Admin-profile-tab-pane').empty();
                    $('#User-tab-pane').empty();
                    $('#User-tab-pane').html(result);
                }

            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function AdminResetPassword() {
    event.preventDefault();
    var formdata = $('#ResetPasswordForm').serialize();

    if ($('#ResetPasswordForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/AdminResetPassword",
            data: formdata,

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                if (result) {
                    Swal.fire("Hurrah", "Password Changed", "success");
                    $('#adminPassword').val("");
                }
                else {
                    Swal.fire("Oops", "Please Enter Password To Change", "error");
                }
            },
            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}

function AdminAccountEdit() {
    event.preventDefault();

    if ($('#ResetPasswordForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/AdminAccountEdit",
            data: $('#ResetPasswordForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire("Hurrah", "Account Information Changed", "success");

                disableFields3();
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        })
    }
}

function AdministratorDetail() {
    event.preventDefault();
    var formdata = $('#AdministratorForm').serialize();

    if ($('#AdministratorForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/AdministratorDetail",
            data: formdata,

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                if (result.success) {
                    Swal.fire("Hurrah", "Administrator Details Changed", "success");

                    disableFields();
                }
                else {
                    Swal.fire("Oops", "Confirm Email Should be Same", "error");

                }
            },
            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}

function MailingDetail() {
    event.preventDefault();
    var formdata = $('#MailingForm').serialize();

    if ($('#MailingForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/MailingDetail",
            data: formdata,

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire("Hurrah", "Mailing Information Changed", "success");

                disableFields();
            },
            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}

function DeleteAdminAccount(adminId) {
    $.ajax({
        method: "POST",
        url: "/Admin/DeleteAdminAccount",
        data: { adminId: adminId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire("Hurrah", "Admin Deleted", "success");

            GetUserAccess(0);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}



// *********************************************************** Provider data ***********************************************************
function GetProvider(regionId) {
    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetProvider",
        data: { regionId: regionId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            setTimeout(function () {
                hideLoader();
            }, 300);

            $('#Provider-tab-pane').html(result);
            $('#regionValue').val(regionId);
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}



// ******************************* Contact Provider *******************************
function ContactProvider(email) {
    $.ajax({
        method: "GET",
        url: "/Admin/ContactProvider",

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $('#ContactProviderModal').modal('show');
            $('#contactEmailId').val(email);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function SendEmailToProvider() {
    event.preventDefault();

    if ($('#ContactProviderForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/SendEmailToProvider",
            data: $('#ContactProviderForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                $('#ContactProviderModal').modal('hide');
                Swal.fire("Hurrah", "Mail sent", "success");

            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}



// ******************************* Edit Provider *******************************
function GetEditProvider(aspId, callId) {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/Admin/GetEditProvider",
        data: { aspId: aspId, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            setTimeout(function () {
                hideLoader();
            }, 300);
            if (callId == 1) {
                $('#User-tab-pane').empty();
                $('#provider-profile-tab-pane').empty();
                $('#Provider-tab-pane').html(result);
            }
            else if (callId == 2) {
                $('#Provider-tab-pane').empty();
                $('#provider-profile-tab-pane').empty();
                $('#User-tab-pane').html(result);
            }
            else {
                $('#Provider-tab-pane').empty();
                $('#User-tab-pane').empty();
                $('#provider-profile-tab-pane').html(result);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function PhysicianProfileResetPassword(callId) {
    event.preventDefault();

    if ($('#PhysicianAccountForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/PhysicianProfileResetPassword",
            data: $('#PhysicianAccountForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                if (result.success) {
                    Swal.fire("Hurrah", "Password Changed", "success");

                    GetEditProvider(result.aspId, callId);
                }
                else {
                    Swal.fire("Oops", "Please Enter Password", "error");
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        })
    }
}

function PhysicianAccountEdit(callId) {
    event.preventDefault();

    if ($('#PhysicianAccountForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/PhysicianAccountEdit",
            data: $('#PhysicianAccountForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire("Hurrah", "Account Information Changed", "success");

                GetEditProvider(result, callId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        })
    }
}

function PhysicianAdministratorEdit(callId) {
    event.preventDefault();

    if ($('#PhysicianAdministratorForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/PhysicianAdministratorEdit",
            data: $('#PhysicianAdministratorForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire("Hurrah", "Administrator Information Changed", "success");

                GetEditProvider(result, callId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        })
    }
}

function PhysicianMailingEdit(callId) {
    event.preventDefault();

    if ($('#PhysicianMailingForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/PhysicianMailingEdit",
            data: $('#PhysicianMailingForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire("Hurrah", "Mailing Information Changed", "success");

                GetEditProvider(result, callId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}

function PhysicianBusinessInfoEdit(callId) {
    event.preventDefault();

    if ($('#PhysicianBusinessInfoForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/PhysicianBusinessInfoEdit",
            data: new FormData($('#PhysicianBusinessInfoForm')[0]),
            processData: false,
            contentType: false,

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire("Hurrah", "Business Informaion Changed", "success");

                GetEditProvider(result, callId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}

function UpdateOnBoarding(callId) {
    event.preventDefault();

    if ($('#OnboardingEditForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/UpdateOnBoarding",
            data: new FormData($('#OnboardingEditForm')[0]),
            processData: false,
            contentType: false,

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire("Hurrah", "Onboarding Information Changed", "success");

                GetEditProvider(result, callId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}

function DeletePhysicianAccount(physicianId, callId) {
    $.ajax({
        method: "POST",
        url: "/Admin/DeletePhysicianAccount",
        data: { physicianId: physicianId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire("Hurrah", "Physician Deleted", "success");

            if (callId == 1) {
                GetProvider(0);
            }
            else {
                GetUserAccess(0);
            }
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}



// ******************************* Create Provider *******************************

function CreateProviderAccount(callId) {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/Admin/CreateProviderAccount",
        data: { callId: callId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            if (callId == 1) {
                $('#Provider-tab-pane').html(result);
            }
            else {
                $('#User-tab-pane').html(result);
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

function CreateProviderAccountPost(callId) {
    event.preventDefault();

    if ($('#CreatePhysicianAccountForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/CreateProviderAccountPost",
            data: new FormData($('#CreatePhysicianAccountForm')[0]),
            processData: false,
            contentType: false,

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                if (result.success) {
                    Swal.fire("Hurrah", "Physician Created", "success");

                }
                else {
                    Swal.fire("Hurrah", "Physician Already Exists", "success");

                }
                if (callId == 1) {
                    GetProvider(0);
                }
                else {
                    GetUserAccess(0);
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}



// *********************************************************** Get Account Access ***********************************************************
function GetAccountAccess() {
    showLoader();
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/Admin/GetAccountAccess",
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#Account-tab-pane').html(result);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}


// ******************************* Create Account Access *******************************

function GetCreateAccess() {
    event.preventDefault();
    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetCreateAccess",
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#Account-tab-pane').html(result);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function FilterRolesMenu(accounttype) {
    event.preventDefault();

    $.ajax({
        method: "GET",
        url: "/Admin/FilterRolesMenu",
        data: { accounttype: accounttype },
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#RolesMenuList').empty();
                result.forEach(obj => {
                    $('#RolesMenuList').append(`<div class='form-check form-check-inline px-2 mx-3 my-2 col-auto'><input class='form-check-input d2class' name='AccountMenu' type='checkbox' id='${obj.menuid}' value='${obj.menuid}'/><label class='form-check-label' for='${obj.menuid}'>${obj.name}</label></div>`)
                });
            }
        },
        error: function () {
            Swal.fire("Oops", "Something is Wrong", "error");
        }
    });
}

function SetCreateAccessAccount() {
    event.preventDefault();

    if ($('#AccountCreateForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/SetCreateAccessAccount",
            data: $('#AccountCreateForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire("Hurrah", "Role Created", "success");

                GetAccountAccess();
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        })
    }
}



// ******************************* Edit Account Access *******************************

function GetEditAccess(accounttypeid, roleid) {
    event.preventDefault();
    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetEditAccess",
        data: { accounttypeid: accounttypeid, roleid: roleid },
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#Account-tab-pane').html(result);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            Swal.fire("Oops!", "Something is Wrong !!!", "error");
        }
    });
}

function FilterEditRolesMenu(accounttypeid, roleid) {
    event.preventDefault();
    $.ajax({
        method: "GET",
        url: "/Admin/FilterEditRolesMenu",
        data: { accounttypeid: accounttypeid, roleid: roleid },
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#EditRoleMenu').empty();
                result.forEach(obj => {
                    $('#EditRoleMenu').append(`<div class='form-check form-check-inline px-2 mx-3 my-2 col-auto'><input class='form-check-input d2class' name='AccountMenu' type='checkbox' id='${obj.menuid}' value='${obj.menuid}' ${(obj.existsInTable ? 'checked' : '')} /><label class='form-check-label' for='${obj.menuid}'>${obj.name}</label></div>`)
                });
            }
        },
        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function SetEditAccessAccount(accounttypeid, roleid) {
    event.preventDefault();


    if ($('#AccountEditForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/SetEditAccessAccount",
            data: $('#AccountEditForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                Swal.fire("Hurrah", "Account Access Updated", "success");
                GetEditAccess(accounttypeid, roleid);
            },



            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        })
    }
}



// ******************************* Delete Account Access *******************************

function DeleteAccountAccess(roleid) {

    $.ajax({
        method: "POST",
        url: "/Admin/DeleteAccountAccess",
        data: { roleid: roleid },

        success: function (result) {
            Swal.fire("Hurrah", "Account Access Deleted", "success");

            GetAccountAccess();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    })

}



// *********************************************************** Get User Access ***********************************************************

function GetUserAccess(accountTypeId) {
    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetUserAccess",
        data: { accountTypeId: accountTypeId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#User-tab-pane').html(result);
                $('#roleValue').val(accountTypeId);
                if (accountTypeId == 1) {
                    $('#createAdmin').removeClass('d-none');
                }
                if (accountTypeId == 2) {
                    $('#createProvider').removeClass('d-none');
                }
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function GetCreateAdminAccount(callId) {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/Admin/GetCreateAdminAccount",
        data: { callId: callId },

        success: function (result) {
            if (callId == 1) {
                $('#Admin-tab-pane').html(result);
            }
            if (callId == 2) {
                $('#User-tab-pane').html(result);
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

function CreateAdminAccountPost(callId) {
    event.preventDefault();

    if ($('#CreateAdminAccountForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/CreateAdminAccountPost",
            data: $('#CreateAdminAccountForm').serialize(),

            success: function (result) {
                if (result.success) {
                    Swal.fire("Hurrah", "Admin Created", "success");

                }
                else {
                    Swal.fire("Hurrah", "Admin is Already Exists", "error");

                }
                if (callId == 1) {
                    GetDashboard();
                }
                if (callId == 2) {
                    GetUserAccess(0);
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}



// *********************************************************** Scheduling ***********************************************************

function GetScheduling(callId) {
    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetScheduling",
        data: { callId: callId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                setTimeout(function () {
                    hideLoader();
                }, 300);
                if (callId == 2) {
                    $('#provider-schedule-tab-pane').html(result);
                }
                else {
                    $('#Scheduling-tab-pane').html(result);
                }
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function CreateNewShift(callId) {
    $.ajax({
        method: "GET",
        url: "/Admin/CreateNewShift",
        data: { callId: callId },
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#schedulingModals').html(result);
                $('#createShiftModal').modal('show');
            }
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function createShiftPost(callId) {
    event.preventDefault();

    if ($('#createShiftForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/CreateShiftPost",
            data: $('#createShiftForm').serialize(),

            success: function (res) {
                if (res.code == 401) {
                    window.location.reload();
                } else {
                    if (res) {
                        $('#createShiftModal').modal('hide');
                        Swal.fire({
                            title: "Hurrah", text: "Shift Created Succesfully ", icon: "success"
                        });
                        GetScheduling(callId);
                    }
                    else {
                        Swal.fire({
                            title: "Oops", text: "This Time Slot is not Available", icon: "error"
                        });
                    }
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}

function ShiftReview(regionId, callId) {
    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/ShiftReview",
        data: { regionId: regionId, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#Scheduling-tab-pane').html(result);
                $('#regionValue').val(regionId);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function ApproveShift(shiftDetailsId, regionId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Admin/ApproveShift",
        data: { shiftDetailsId: shiftDetailsId },
        traditional: true,

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                if (shiftDetailsId.length === 0) {
                    Swal.fire("Oops", "Please Select Any Shift To Proceed Ahead", "error");
                }
                else {
                    Swal.fire("Hurrah", "Shifts Approved", "success");
                    ShiftReview(regionId);
                }
            }
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function DeleteSelectedShift(shiftDetailsId, regionId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "/Admin/DeleteSelectedShift",
        data: { shiftDetailsId: shiftDetailsId },
        traditional: true,

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                if (shiftDetailsId.length === 0) {
                    Swal.fire("Oops", "Please Select Any Shift To Proceed Ahead", "error");
                }
                else {
                    Swal.fire("Hurrah", "Shifts Deleted", "success");
                    ShiftReview(regionId);
                }
            }
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function MDOnCall(regionId) {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/Admin/MDOnCall",
        data: { regionId: regionId },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#Scheduling-tab-pane').html(result);
                $('#MDsRegion').val(regionId);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}



// *********************************************************** Provider Location ***********************************************************

function ProviderLocation() {
    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/ProviderLocation",

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            } else {
                $('#provider-location-tab-pane').html(result);
                setTimeout(function () {
                    hideLoader();
                }, 300);
            }
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}



// *********************************************************** Partners ***********************************************************

function Partners(professionid) {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/Admin/Partners",
        data: { professionid: professionid },
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }

            $("#Partners-tab-pane").html(result);
            if (professionid != 0) {
                $(".ProfessionsDropdown").val(professionid);

            }
            setTimeout(function () {
                hideLoader();
            }, 300);
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        },
    });
}

function AddBusiness(vendorID) {
    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/AddBusiness",
        data: { vendorID: vendorID },
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            $("#Partners-tab-pane").html(result);
            setTimeout(function () {
                hideLoader();
            }, 300);
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        },
    });
}

function CreateNewBusiness() {
    event.preventDefault();

    if ($("#SubmitInfoForm").valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/CreateNewBusiness",
            data: $("#SubmitInfoForm").serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                if (result.success) {
                    Swal.fire({
                        title: "Hurrah",
                        text: "New Business Added!",
                        icon: "success",
                    });
                    Partners(0);
                } else {
                    Swal.fire({
                        title: "Oops",
                        text: "Email Already Exists!",
                        icon: "error",
                    });
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            },
        });
    }
}

function UpdateBusiness() {
    event.preventDefault();

    if ($("#SubmitInfoForm").valid()) {
        $.ajax({
            method: "POST",
            url: "/Admin/UpdateBusiness",
            data: $("#SubmitInfoForm").serialize(),

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                if (result.success) {
                    Swal.fire({
                        title: "Hurrah",
                        text: "Business Data Updated!",
                        icon: "success",
                    });
                    AddBusiness(result.vendorid);
                } else {
                    Swal.fire({
                        title: "Oops",
                        text: "Email Already Exists!",
                        icon: "error",
                    });
                }
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            },
        });
    }
}

function DelPartner(VendorID) {
    $.ajax({
        method: "POST",
        url: "/Admin/DelPartner",
        data: { VendorID: VendorID },
        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "Hurrah",
                text: "Business Partner Deleted Successfully",
                icon: "success"
            });
            Partners(0);
        },
        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        },
    });
}



// *********************************************************** Records Tab ***********************************************************

// ******************************* Patient Records *******************************

function GetRecordsTab() {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/Admin/GetRecordsTab",
        data: $('#patientRecordsform').serialize(),

        success: function (res) {
            if (res.code == 401) {
                window.location.reload();
            }
            $('#Patient-tab-pane').html(res);
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

function patientRecordsFilter(callId) {
    event.preventDefault();

    var formdata;

    if (callId != 1) {
        formdata = $('#patientRecordsform').serialize()
    }

    $.ajax({
        method: "POST",
        url: "/Admin/GetRecordsTab",
        data: formdata,

        success: function (res) {
            if (res.code == 401) {
                window.location.reload();
            }
            $('#Patient-tab-pane').html(res);

        },
        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });

}

function GetPatientRecordExplore(UserId) {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/Admin/GetPatientRecordExplore",
        data: { UserId: UserId },

        success: function (res) {
            if (res.code == 401) {
                window.location.reload();
            }
            $('#Patient-tab-pane').html(res);
            setTimeout(function () {
                hideLoader();
            }, 300);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}



// ******************************* Search Records *******************************

function GetSearchRecords() {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/Admin/GetSearchRecords",

        success: function (res) {
            if (res.code == 401) {
                window.location.reload();
            }
            $('#Search-tab-pane').html(res);
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

function searchRecordsFilter(callId) {
    event.preventDefault();

    var formData;

    if (callId != 1) {
        formData = $('#searchRecordsForm').serialize();
    }

    $.ajax({
        method: "POST",
        url: "/Admin/GetSearchRecords",
        data: formData,

        success: function (response) {
            if (response.code == 401) {
                window.location.reload();
            }
            $('#Search-tab-pane').html(response);
        },
        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });

}

function ExportSearchRecords() {
    showLoader();

    $.ajax({
        method: "POST",
        url: "/Admin/ExportSearchRecords",
        data: $('#searchRecordsForm').serialize(),
        xhrFields: {
            responseType: 'blob'
        },

        success: function (data) {

            var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = 'Requests.xlsx';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);

            hideLoader();
            Swal.fire("Hurrah", "Records Exported", "success");

        },
        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });


}

function deletRequest(requestId) {

    $.ajax({
        method: "GET",
        url: "/Admin/DeletRequest",
        data: { requestId: requestId },

        success: function (res) {
            if (res.code == 401) {
                window.location.reload();
            }
            $('#Search-tab-pane').html(res);
            Swal.fire({
                title: "Hurrah",
                text: "Request Deleted!",
                icon: "success",
            });
            GetSearchRecords();
        },
        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}



// ******************************* Email Log *******************************

function GetEmailSmsLog() {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/Admin/GetEmailSmsLog",

        success: function (res) {
            if (res.code == 401) {
                window.location.reload();
            }
            $('#Email-tab-pane').html(res);
            setTimeout(function () {
                hideLoader();
            }, 300);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");

        }
    });
}

function emailSmsLogFilter(callId) {
    event.preventDefault();

    var formdata;

    if (callId != 1) {
        formdata = $('#logsRecordFilter').serialize();
    }

    $.ajax({
        method: "POST",
        url: "/Admin/GetEmailSmsLog",
        data: formdata,

        success: function (res) {
            if (res.code == 401) {
                window.location.reload();
            }
            $('#Email-tab-pane').html(res);
        },
        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });

}



// ******************************* Sms Log *******************************

function GetSmsLog() {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/Admin/GetSmsLog",

        success: function (res) {
            if (res.code == 401) {
                window.location.reload();
            }
            $('#SMS-tab-pane').html(res);
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

function SmsLogFilter(callId) {
    event.preventDefault();

    var formdata;

    if (callId != 1) {
        formdata = $('#logsRecordFilter').serialize();
    }

    $.ajax({
        method: "POST",
        url: "/Admin/GetSmsLog",
        data: formdata,

        success: function (res) {
            if (res.code == 401) {
                window.location.reload();
            }
            $('#SMS-tab-pane').html(res);
        },
        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });

}



// ******************************* Block Request *******************************

function GetBlockedRequest() {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/Admin/GetBlockedRequest",

        success: function (res) {
            if (res.code == 401) {
                window.location.reload();
            }
            $('#Block-tab-pane').html(res);
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

function BlockedRecordsFilter(callId) {
    event.preventDefault();

    var formdata;

    if (callId != 1) {
        formdata = $('#BlockedRecordsform').serialize();
    }

    $.ajax({
        method: "POST",
        url: "/Admin/GetBlockedRequest",
        data: formdata,

        success: function (res) {
            if (res.code == 401) {
                window.location.reload();
            }
            $('#Block-tab-pane').html(res);

        },
        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });

}

function UnblockRequest(requestId) {

    $.ajax({
        method: "GET",
        url: "/Admin/UnblockRequest",
        data: { requestId: requestId },

        success: function (res) {
            if (res.code == 401) {
                window.location.reload();
            }
            $('#Block-tab-pane').html(res);
            Swal.fire({
                title: "Hurrah",
                text: "Request Unblocked!",
                icon: "success",
            });
            GetBlockedRequest();
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}



// *********************************************************** Payrate ***********************************************************

function GetEnterPayrate(aspId, phyid) {
    showLoader();

    $.ajax({
        method: "GET",
        url: "/Admin/GetEnterPayrate",
        data: { aspId: aspId, phyid: phyid },

        success: function (response) {
            if (response.code == 401) {
                window.location.reload();
            }
            $('#Provider-tab-pane').html(response);
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

function PostPayrate(counter) {
    event.preventDefault();

    var category = $(`#payratecategory${counter}`).val();
    var payrate = $(`#payRateField${counter}`).val();
    var phyid = $(`#phyid${counter}`).val();

    $.ajax({
        method: "POST",
        url: "/Admin/PostPayrate",
        data: { category: category, payrate: payrate, phyid: phyid },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire({
                title: "Success",
                text: "Payrate Updated",
                icon: "success",
            })
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}


// *********************************************************** Invoicing ***********************************************************

function GetAdminInvoicing() {
    $.ajax({
        method: "GET",
        url: "/Admin/GetAdminInvoicing",

        success: function (response) {
            $('#Invoicing-tab-pane').html(response);
        },
        error: function () {
            Swal.fire("Oops", "Error in Getting Invoicing Page", "error");
        }
    });
}

function GetTimeSheetDetail() {
    var phyid = $('#phyDropdown').val();
    var dateSelected = $('#searchByTimeperiod').val();

    $.ajax({
        method: "GET",
        url: "/Admin/GetTimeSheetDetail",
        data: { phyid: phyid, dateSelected: dateSelected },

        success: function (response) {
            $('#Invoicing-tab-pane').html(response);
            $('#phyDropdown').val(phyid);
            $('#searchByTimeperiod').val(dateSelected);
        },
        error: function () {
            Swal.fire("Oops", "Error in Getting Invoicing Page", "error");
        }
    });
}

function GetAdminFinalizeTimeSheet(dateSelected, phyid) {

    if (dateSelected == null) {
        dateSelected = $('#searchByTimeperiod').val();
    }

    if (phyid == null) {
        phyid = $('#phyDropdown').val();
    }

    $.ajax({
        methd: "GET",
        url: "/Admin/GetAdminFinalizeTimeSheet",
        data: { dateSelected: dateSelected, phyid: phyid },

        success: function (result) {
            $('#Invoicing-tab-pane').html(result);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

function ConfirmApproveTimeSheet(timeSheetId) {
    event.preventDefault();

    var bonus = $('#bonusAmount').val();
    var notes = $('#adminNotes').val();

    $.ajax({
        method: "POST",
        url: "/Admin/ConfirmApproveTimeSheet",
        data: { timeSheetId: timeSheetId, bonus: bonus, notes: notes  },

        success: function (result) {
            if (result.code == 401) {
                window.location.reload();
            }
            Swal.fire("Hurrah", "Timesheet Approved", "success");
            GetAdminInvoicing();

        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}