function getLibraryList() {
    $.ajax({
        method: "GET",
        url: "Home/GetLibraryList",

        success: function (result) {
            $('#ListContainerId').html(result);
        },

        error: function () {
            swal.fire("Error", "Oops Something Went Wrong!!", "error");
        }
    });
}

function showAddBookModal() {
    $.ajax({
        method: "GET",
        url: "Home/AddBookModal",

        success: function (result) {
            $('#showModalContainer').html(result);
            $('#addBookModal').modal('show');
        },

        error: function () {
            swal.fire("Error", "Oops Something Went Wrong!!", "error");
        }
    });
}

function addBookToLibrary() {
    event.preventDefault();

    if ($('#addBookFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "Home/AddBookToLibrary",
            data: $('#addBookFormId').serialize(),

            success: function () {
                $('#addBookModal').modal('hide');
                getLibraryList();
                swal.fire("Success", "Book Added!!", "success");
            },

            error: function () {
                swal.fire("Error", "Oops Something Went Wrong!!", "error");
            }
        });
    }
}

function showEditBookModal(bookId) {
    $.ajax({
        method: "GET",
        url: "Home/EditBookModal",
        data: { bookId: bookId },

        success: function (result) {
            $('#showModalContainer').html(result);
            $('#editBookModal').modal('show');
        },

        error: function () {
            swal.fire("Error", "Oops Something Went Wrong!!", "error");
        }
    });
}

function editBookToLibrary() {
    event.preventDefault();

    if ($('#editBookFormId').valid()) {
        $.ajax({
            method: "POST",
            url: "Home/EditBookToLibrary",
            data: $('#editBookFormId').serialize(),

            success: function () {
                $('#editBookModal').modal('hide');
                getLibraryList();
                swal.fire("Success", "Book Edited!!", "success");
            },

            error: function () {
                swal.fire("Error", "Oops Something Went Wrong!!", "error");
            }
        });
    }
}

function deleteBook(bookId) {
    $.ajax({
        method: "GET",
        url: "Home/DeleteBook",
        data: { bookId: bookId },

        success: function (result) {
            $('#showModalContainer').html(result);
            $('#deleteModal').modal('show');
        },

        error: function () {
            swal.fire("Error", "Oops Something Went Wrong!!", "error");
        }
    });
}

function ConfirmDelete(bookId) {
    event.preventDefault();

    $.ajax({
        method: "POST",
        url: "Home/DeleteBookFromLibrary",
        data: { bookId: bookId },

        success: function () {
            $('#deleteModal').modal('hide');
            getLibraryList();
            swal.fire("Success", "Book Deleted!!", "success");
        },

        error: function () {
            swal.fire("Error", "Oops Something Went Wrong!!", "error");
        }
    });
}