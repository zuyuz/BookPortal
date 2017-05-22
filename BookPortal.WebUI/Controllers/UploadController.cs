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
    [Authorize]
    public class UploadController : Controller
    {
        private IBookRepository repository;

        public UploadController(IBookRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public ActionResult Index()
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