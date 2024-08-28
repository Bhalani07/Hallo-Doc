using Assignment_Data.Custom_Model;
using Assignment_Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_Logic.Interface
{
    public interface ILibraryInterface
    {
        List<Book> GetLibraryData();

        bool AddBook(LibraryCm libraryCm);

        LibraryCm GetEditData(int bookId);

        bool EditBook(LibraryCm libraryCm);

        bool DeleteBook(int bookId);
    }
}
