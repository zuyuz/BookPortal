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

        public IEnumerable<Post> Posts
        {
            get { return context.Posts; }
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

        public void SavePost(Post post)
        {
            if (post.Id == 0)
            {
                context.Posts.Add(post);
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

        public Post DeletePost(int id)
        {
            Post dbEntry = context.Posts.Find(id);
            if (dbEntry != null)
            {
                context.Posts.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public IList<Post> PostsByNumAndSize(int pageNo, int pageSize)
        {
            var posts = Posts.AsQueryable<Post>()
                                  .Where(p => p.Published)
                                  .OrderByDescending(p => p.PostedOn)
                                  .Skip(pageNo * pageSize)
                                  .Take(pageSize)
                                  .ToList();

            var postIds = posts.Select(p => p.Id).ToList();

            return Posts.AsQueryable<Post>()
                  .Where(p => postIds.Contains(p.Id))
                  .OrderByDescending(p => p.PostedOn)
                  .ToList();
        }

        public int TotalPosts()
        {
            return Posts.AsQueryable<Post>().Where(p => p.Published).Count();
        }

        public IList<Post> PostsForCategory(string categorySlug, int pageNo, int pageSize)
        {
            var posts = Posts.AsQueryable<Post>()
                                .Where(p => p.Published && p.Category.UrlSlug.Equals(categorySlug))
                                .OrderByDescending(p => p.PostedOn)
                                .Skip(pageNo * pageSize)
                                .Take(pageSize)
                                .ToList();

            var postIds = posts.Select(p => p.Id).ToList();

            return Posts.AsQueryable<Post>()
                          .Where(p => postIds.Contains(p.Id))
                          .OrderByDescending(p => p.PostedOn)
                          .ToList();
        }

        public int TotalPostsForCategory(string categorySlug)
        {
            return Posts.AsQueryable<Post>()
                        .Where(p => p.Published && p.Category.UrlSlug.Equals(categorySlug))
                        .Count();
        }

        public Category Category(string categorySlug)
        {
            return Posts.ToList().Select(p=>p.Category).FirstOrDefault(t => t.UrlSlug.Equals(categorySlug));
        }

    }
}
