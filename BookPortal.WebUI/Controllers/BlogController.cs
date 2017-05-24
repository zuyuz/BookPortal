using BookPortal.Domain.Abstract;
using BookPortal.Domain.Entities;
using BookPortal.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookPortal.WebUI.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBookRepository _bookRepository;

        public BlogController(IBookRepository blogRepository)
        {
            _bookRepository = blogRepository;
        }

        public ViewResult ViewPost()
        {
            var posts = _bookRepository.Posts;
            return View(posts);
        }

        [HttpGet]
        public ViewResult AddPost()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPost(Post newPost)
        {
            _bookRepository.SavePost(newPost);
            return RedirectToAction("ViewPost");
        }
        
        public ViewResult EditPost(int id)
        {
            var posts = _bookRepository.Posts.ToList();
            return View(posts.Find(p => p.Id == id));
        }
        
        public ViewResult PostDetails(int id)
        {
            var posts = _bookRepository.Posts.ToList();
            return View(posts.Find(p => p.Id == id));
        }
        
        [HttpGet]
        public ViewResult DeletePost(int id)
        {
            var posts = _bookRepository.Posts.ToList();
            return View(posts.Find(p => p.Id == id));
        }

        [HttpPost]
        public ActionResult DeletePost(Post post)
        {
            _bookRepository.DeletePost(post.Id);
            return RedirectToAction("ViewPost");
        }

        public ViewResult Posts(int p = 1)
        {
            var viewModel = new ListViewModel(_bookRepository, p);

            ViewBag.Title = "Latest Posts";
            return View("List", viewModel);
        }

        //public ViewResult Category(string category, int p = 1)
        //{
        //    var viewModel = new ListViewModel(_bookRepository, category, p);

        //    if (viewModel.Category == null)
        //        throw new HttpException(404, "Category not found");

        //    ViewBag.Title = String.Format(@"Latest posts on category ""{0}""",
        //                        viewModel.Category.Name);
        //    return View("List", viewModel);
        //}
    }
}