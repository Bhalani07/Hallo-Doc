using Assignment.Models;
using Assignment_Data.Custom_Model;
using Assignment_Logic.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILibraryInterface _library;

        public HomeController(ILibraryInterface libraryInterface)
        {
            _library = libraryInterface;
        }


        public IActionResult Index()
        {
            LibraryCm libraryCm = new()
            {
                LibraryList = _library.GetLibraryData(),
            };

            return View(libraryCm);
        }

        public IActionResult GetLibraryList()
        {
            LibraryCm libraryCm = new()
            {
                LibraryList = _library.GetLibraryData(),
            };

            return PartialView("_LibraryTable", libraryCm);
        }

        public IActionResult AddBookModal()
        {
            return PartialView("_AddBookModal");
        }


        [HttpPost]
        public IActionResult AddBookToLibrary(LibraryCm libraryCm)
        {
            if (_library.AddBook(libraryCm))
            {
                return Ok();
            }

            return BadRequest();
        }


        public IActionResult EditBookModal(int bookId)
        {
            LibraryCm libraryCm = _library.GetEditData(bookId);

            return PartialView("_EditBookModal", libraryCm);
        }


        [HttpPost]
        public IActionResult EditBookToLibrary(LibraryCm libraryCm)
        {
            if (_library.EditBook(libraryCm))
            {
                return Ok();
            }

            return BadRequest();
        }


        public IActionResult DeleteBook(int bookId)
        {
            LibraryCm libraryCm = new()
            {
                BookId = bookId
            };

            return PartialView("_DeleteModal", libraryCm);
        }

        [HttpPost]
        public IActionResult DeleteBookFromLibrary(int bookId)
        {
            if (_library.DeleteBook(bookId))
            {
                return Ok();
            }

            return BadRequest();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}