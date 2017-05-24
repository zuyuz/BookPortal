using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookPortal.Domain.Abstract;
using BookPortal.Domain.Entities;
using BookPortal.WebUI.Models;
using System.Runtime.InteropServices;
using System.IO;

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

        public ActionResult ViewBook(int id)
        {
            var book = repository.Books.Where(b => b.Id == id).FirstOrDefault();
            if(book == null)
            {
                return RedirectToAction("List");
            }
            return View(book);
        }

        public ActionResult List(string genre, int page = 1)
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

        public ActionResult Audio(string path)
        {
            // "D:\\FileSystem\\Audio\\Книги, що говорять (збірка) (2015-2017) [mp3]\\Казки\\Богдан Лепкий.  Мишка (казка для дітей, для малих і великих).mp3"
            var videoData = System.IO.File.ReadAllBytes(path);
            return File(videoData, "video/mp4");
        }

        [HttpGet]
        public ActionResult UploadNewBook()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add()
        {
            for (int i = 0;
             i < Convert.ToInt32(Request.Form["PackageFileCount"]);
             i++)
            {
                if (Request.Form["File0Mode_" + i] != "sourceFile")
                    throw
                      new Exception("Uploader expects to send original files.");
                Book newBook = Storage.SaveUploadedFile(Server.MapPath("/Files"),
                                           Request.Files["File0_" + i]);

                repository.SaveBook(newBook);
            }
            Response.Write("Upload Complete");
            return null;
        }
    }
}