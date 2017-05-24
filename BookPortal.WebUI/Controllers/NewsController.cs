using BookPortal.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookPortal.WebUI.Controllers
{
    public class NewsController : Controller
    {
        private readonly IBookRepository _bookRepository;

        public NewsController(IBookRepository blogRepository)
        {
            _bookRepository = blogRepository;
        }

        public ActionResult Index()
        {
            var posts = _bookRepository.Posts;
            return View(posts);
        }
    }
}