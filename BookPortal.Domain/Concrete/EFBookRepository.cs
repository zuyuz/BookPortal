using BookPortal.Domain.Abstract;
using BookPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace BookPortal.Domain.Concrete
{
    public class EFBookRepository : IBookRepository
    {
        private EFDbContext context = new EFDbContext();
        public IEnumerable<Book> Books
        {
            get { return context.Books; }
        }

        public void SaveBook(Book product)
        {
            if (product.Id == 0)
            {
                context.Books.Add(product);
            }
            else
            {
                Book dbEntry =
                    context.Books.Find(product.Id);
                if (dbEntry != null)
                {
                    dbEntry.Title = product.Title;
                    dbEntry.Genre = product.Genre;
                    dbEntry.Language = product.Language;
                    dbEntry.Publisher = product.Publisher;
                }
            }
            context.SaveChanges();
        }
        public Book DeleteBook(int id)
        {
            Book dbEntry = context.Books.Find(id);
            if (dbEntry != null)
            {
                context.Books.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        
    }
}
