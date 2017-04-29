using BookPortal.Domain.Entities;
using System.Data.Entity;

namespace BookPortal.Domain.Concrete
{
    public class EFDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
    }
}
