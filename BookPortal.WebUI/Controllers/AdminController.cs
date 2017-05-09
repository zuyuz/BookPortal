using BookPortal.Domain.Abstract;
using System.Web.Mvc;
using System.Linq;
using BookPortal.Domain.Entities;

namespace BookPortal.WebUI.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdminController : Controller
    {
        private IBookRepository repository;
        public AdminController(IBookRepository repo)
        {
            repository = repo;
        }
        public ViewResult Index()
        {
            return View(repository.Books);
        }
        public ViewResult Edit(int Id)
        {
            Book book = repository.Books
                .FirstOrDefault(p => p.Id == Id);
            return View(book);
        }
        [HttpPost]
        public ActionResult Edit(Book book)
        {
            if (ModelState.IsValid)
            {
                repository.SaveBook(book);
                TempData["message"] = string.Format("{0} has been saved",
                    book.Title);
                return RedirectToAction("Index");
            }
            else
            {
                // there is something wrong with the data values
                return View(book);
            }
        }
        public ViewResult Create()
        {
            return View("Edit", new Book());
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Book deletedProduct = repository.DeleteBook(id);
            if (deletedProduct != null)
            {
                TempData["message"] = string.Format("{0} was deleted",
                deletedProduct.Title);
            }
            return RedirectToAction("Index");
        }
    }
}