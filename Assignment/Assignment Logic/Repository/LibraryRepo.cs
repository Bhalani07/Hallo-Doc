using Assignment_Data.Custom_Model;
using Assignment_Data.Models;
using Assignment_Logic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_Logic.Repository
{
    public class LibraryRepo : ILibraryInterface
    {
        private readonly ApplicationDbContext _context;

        public LibraryRepo(ApplicationDbContext context)
        {
            _context = context;
        }


        public List<Book> GetLibraryData()
        {
            var bookList = _context.Books.Where(i => i.Isdeleted != true).ToList();

            return bookList;
        }


        public bool AddBook(LibraryCm libraryCm)
        {
            try
            {
                var bookRecord = new Book()
                {
                    Bookname = libraryCm.BookName,
                    Author = libraryCm.Author,
                    Borrowername = libraryCm.BorrowerName,
                    Dateofissue = DateTime.Parse(libraryCm.IssueDate),
                    City = libraryCm.City,
                    Genere = libraryCm.Genere,
                };

                if (_context.Books.Any(i => i.Borrowername.ToLower().Trim() == libraryCm.BorrowerName.ToLower().Trim()))
                {
                    var borrowerId = _context.Books.FirstOrDefault(i => i.Borrowername.ToLower().Trim() == libraryCm.BorrowerName.ToLower().Trim()).Borrowerid;

                    bookRecord.Borrowerid = borrowerId;

                    _context.Books.Add(bookRecord);
                    _context.SaveChanges();

                    return true;
                }

                var borrower = new Borrower()
                {
                    City = libraryCm.City,
                };

                _context.Borrowers.Add(borrower);
                _context.SaveChanges();

                bookRecord.Borrowerid = borrower.Id;

                _context.Books.Add(bookRecord);
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }


        public LibraryCm GetEditData(int bookId)
        {
            Book? bookData = _context.Books.FirstOrDefault(i => i.Id == bookId);

            LibraryCm libraryCm = new();
               
            if(bookData != null) {

                libraryCm.BookId = bookId;
                libraryCm.BookName = bookData.Bookname;
                libraryCm.Author = bookData.Author;
                libraryCm.BorrowerName = bookData.Borrowername;
                libraryCm.IssueDate = bookData.Dateofissue?.ToString("yyyy-MM-dd");
                libraryCm.Genere = bookData.Genere;
                libraryCm.City = bookData.City;

            }

            return libraryCm;
        }


        public bool EditBook(LibraryCm libraryCm)
        {
            try
            {
                Book? book = _context.Books.FirstOrDefault(i => i.Id == libraryCm.BookId);

                if(book != null)
                {
                    book.Bookname = libraryCm.BookName;
                    book.Author = libraryCm.Author;
                    book.Borrowername = libraryCm.BorrowerName;
                    book.Dateofissue = DateTime.Parse(libraryCm.IssueDate);
                    book.City = libraryCm.City;
                    book.Genere = libraryCm.Genere;

                    if (_context.Books.Any(i => i.Borrowername.ToLower().Trim() == libraryCm.BorrowerName.ToLower().Trim()))
                    {
                        var borrowerId = _context.Books.FirstOrDefault(i => i.Borrowername.ToLower().Trim() == libraryCm.BorrowerName.ToLower().Trim()).Borrowerid;
                        book.Borrowerid = borrowerId;

                        var borrower = _context.Borrowers.FirstOrDefault(i => i.Id == borrowerId);
                        borrower.City = libraryCm.City;

                        _context.SaveChanges();

                        return true;
                    }

                    var newBorrower = new Borrower()
                    {
                        City = libraryCm.City,
                    };

                    _context.Borrowers.Add(newBorrower);
                    _context.SaveChanges();

                    book.Borrowerid = newBorrower.Id;
                    _context.SaveChanges();

                    return true;

                }

                return false;
            }
            catch
            {
                return false;
            }
        }


        public bool DeleteBook(int bookId)
        {
            try
            {
                Book? book = _context.Books.FirstOrDefault(i => i.Id == bookId);

                if(book != null)
                {
                    book.Isdeleted = true;
                    _context.SaveChanges();

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }


    }
}
