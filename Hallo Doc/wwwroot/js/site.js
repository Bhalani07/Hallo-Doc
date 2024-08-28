var phoneInput = document.querySelector(".phone");

if (phoneInput) {
    var input = window.intlTelInput(phoneInput, {
        autoInsertDialCode: true,
        hiddenInput: "full",
        preferredCountries: ["in", "us", "ca"],
        utilsScript: "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/15.0.0/js/utils.js"
    });
    $('.phone').change(function () {
        var fullNumber = input.getNumber(intlTelInputUtils.numberFormat.E164);
        $("#patientPhone").val(fullNumber);
    })

    $('.phone').keyup(function () {
        var phoneErrorMessage = document.getElementById('phoneErrorBox');
        if (!input.isValidNumber()) {
            phoneErrorMessage.innerText = "Phone Number is not valid";
        }
        else {
            phoneErrorMessage.innerText = "";
        }
    })
}

var phone1Input = document.querySelector(".phone1");

if (phone1Input) {
    var input1 = window.intlTelInput(phone1Input, {
        autoInsertDialCode: true,
        hiddenInput: "full",
        preferredCountries: ["in", "us", "ca"],
        utilsScript: "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/15.0.0/js/utils.js"
    });
    $('.phone1').change(function () {
        var fullNumber = input1.getNumber(intlTelInputUtils.numberFormat.E164);
        $("#otherPhone").val(fullNumber);
    })

    $('.phone1').keyup(function () {
        var otherPhoneErrorMessage = document.getElementById('otherPhoneErrorBox');
        if (!input1.isValidNumber()) {
            otherPhoneErrorMessage.innerText = "Phone Number is not valid";
        }
        else {
            otherPhoneErrorMessage.innerText = "";
        }
    })
}

function checkForm() {
    var fileName = document.getElementById('inputGroupFile').value.toLowerCase();
    var tagToDisplayMessage = document.getElementById('errorBox');
    if (fileName) {
        if (!(fileName.endsWith('.pdf') || fileName.endsWith('.png') || fileName.endsWith('.jpeg') || fileName.endsWith('.jpg') || fileName.endsWith('.mp4') || fileName.endsWith('.mp3') || fileName.endsWith('.mkv'))) {
            tagToDisplayMessage.innerText = "Upload only pdf, image, audio or video";
            return false;
        }
        tagToDisplayMessage.innerText = "";
    }
    else {
        tagToDisplayMessage.innerText = "";
    }

    var phoneErrorMessage = document.getElementById('phoneErrorBox');
    if (!input.isValidNumber()) {
        return false;
    }

    var otherPhoneErrorMessage = document.getElementById('otherPhoneErrorBox');
    if (!input1.isValidNumber()) {
        return false;
    }
}

function isEmailBlocked() {
    var email = $('#pEmail').val();

    if (email != "") {
        $.ajax({
            method: "GET",
            url: "/Patient/IsEmailBlocked",
            data: { email: email },

            success: function (result) {
                if (result) {
                    Swal.fire({
                        title: "Oops",
                        text: "This email is blocked for further requests!!",
                        icon: "error",
                    });
                    $('#pEmail').val("");
                }

            },

            error: function () {
                Swal.fire({
                    title: "Oops",
                    text: "Something Went Wrong!!",
                    icon: "error",
                });
            }
        });
    }
}

function isPhoneBlocked() {
    var phone = $('#patientPhone').val();

    if (phone != "") {
        $.ajax({
            method: "GET",
            url: "/Patient/IsPhoneBlocked",
            data: { phone: phone },

            success: function (result) {
                if (result) {
                    Swal.fire({
                        title: "Oops",
                        text: "This phone number is blocked for further requests!!",
                        icon: "error",
                    });
                    $('#pPhone').val("");
                    $('#patientPhone').val("");
                }

            },

            error: function () {
                Swal.fire({
                    title: "Oops",
                    text: "Something Went Wrong!!",
                    icon: "error",
                });
            }
        });
    }
}

function notAdminProvider() {
    var email = $('#pEmail').val();

    if (email != "") {
        $.ajax({
            method: "GET",
            url: "/Patient/NotAdminProvider",
            data: { email: email },

            success: function (result) {
                if (result) {
                    Swal.fire({
                        title: "Oops",
                        text: "You can't use this email!!",
                        icon: "error",
                    });
                    $('#pEmail').val("");
                }

            },

            error: function () {
                Swal.fire({
                    title: "Oops",
                    text: "Something Went Wrong!!",
                    icon: "error",
                });
            }
        });
    }
}