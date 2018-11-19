using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore
{
    interface IBookService
    {
        IEnumerable<Book> GetAll();
        IEnumerable<Book> GetByAuthor(string author);
        Book Get(int id);
        void Add(Book newBook);


    }
}
