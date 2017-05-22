using System;
using System.Collections.Generic;
using BookPortal.Domain.Entities;

namespace BookPortal.Domain.Abstract
{
    public interface IBookRepository
    {
        IEnumerable<Book> Books { get; }

        void SaveBook(Book product);

        Book DeleteBook(int id);
    }
}
