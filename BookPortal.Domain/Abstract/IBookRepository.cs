using System;
using System.Collections.Generic;
using BookPortal.Domain.Entities;

namespace BookPortal.Domain.Abstract
{
    public interface IBookRepository
    {
        IEnumerable<Book> Books { get; }
        IEnumerable<Post> Posts { get; }

        void SaveBook(Book product);
        void SavePost(Post post);
        Book DeleteBook(int id);
        Post DeletePost(int id);
        IList<Post> PostsByNumAndSize(int pageNo, int pageSize);

        int TotalPosts();

        IList<Post> PostsForCategory(string categorySlug, int pageNo, int pageSize);
        int TotalPostsForCategory(string categorySlug);
        Category Category(string categorySlug);
    }
}
