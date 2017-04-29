using System.Collections.Generic;
using System.Web.Mvc;
using BookPortal.Domain.Abstract;
using System.Linq;

namespace BookPortal.WebUI.Controllers
{
    public class NavController : Controller
    {
        private IBookRepository repository;
        public NavController(IBookRepository repo)
        {
            repository = repo;
        }
        public PartialViewResult Menu(string genre = null)
        {
            ViewBag.SelectedGenre = genre;

            IEnumerable<string> genres = repository.Books
                .Select(x => x.Genre)
                .Distinct()
                .OrderBy(x => x);
            return PartialView(genres);
        }
    }
}