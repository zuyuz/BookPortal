using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using System.Security.Principal;

namespace BookPortal.WebUI.Controllers
{
    public class UserInfoController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View(GetData());
        }
        private Dictionary<string, object> GetData()
        {
            Dictionary<string, object> dict
            = new Dictionary<string, object>();
            dict.Add("User", HttpContext.User.Identity.Name);
            dict.Add("Authenticated", HttpContext.User.Identity.IsAuthenticated);
            dict.Add("Auth Type", HttpContext.User.Identity.AuthenticationType);
            dict.Add("In Users Role", HttpContext.User.IsInRole("Users"));
            return dict;
        }
    }
}