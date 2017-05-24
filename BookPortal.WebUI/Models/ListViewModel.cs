using BookPortal.Domain.Abstract;
using BookPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookPortal.WebUI.Models
{
    public class ListViewModel
    {
        [Key]
        public int PostNo { get; set; }

        public ListViewModel(IBookRepository _bookRepository, int p)
        {
            Posts = _bookRepository.PostsByNumAndSize(p - 1, 10);
            TotalPosts = _bookRepository.TotalPosts();
        }

        public ListViewModel(IBookRepository blogRepository,
        string categorySlug, int p)
        {
            Posts = blogRepository.PostsForCategory(categorySlug, p - 1, 10);
            TotalPosts = blogRepository.TotalPostsForCategory(categorySlug);
            Category = blogRepository.Category(categorySlug);
        }

        public IList<Post> Posts { get; private set; }
        public int TotalPosts { get; private set; }
        public Category Category { get; private set; }
    }
}