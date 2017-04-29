using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookPortal.Domain.Abstract;
using BookPortal.Domain.Entities;
using BookPortal.WebUI.Models;

namespace BookPortal.WebUI.Controllers
{
    public class BookController : Controller
    {
        private IBookRepository repository;
        public int PageSize = 4;
        public BookController(IBookRepository bookRepository)
        {
            this.repository = bookRepository;
        }
        public ViewResult List(string genre, int page = 1)
        {
            BooksListViewModel model = new BooksListViewModel
            {
                Books = repository.Books
                    .Where(p => genre == null || p.Genre == genre)
                    .OrderBy(p => p.Id)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = genre == null ?
                        repository.Books.Count() :
                        repository.Books.Where(e => e.Genre ==
                        genre).Count()
                },
                CurrentGenre = genre
            };
            return View(model);
        }
    }
}