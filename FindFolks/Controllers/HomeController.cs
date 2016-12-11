using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FindFolks.Models;
using FindFolks.EF;

namespace FindFolks.Controllers
{
    public partial class HomeController : Controller
    {
        private FFContext ffContext = new FFContext();

        public HomeController() { }

        public HomeController(FFContext ffContext)
        {
            this.ffContext = ffContext;
        }

        public virtual ActionResult Index(HomeIndexViewModel model)
        {
            if (model == null)
                model = new HomeIndexViewModel();
            var now = new DateTime(DateTime.Now.Ticks);
            var threeDays = new DateTime(DateTime.Now.Ticks).AddDays(3);
            model.UpcomingEvents = ffContext.Events.Where(e => e.Start >= now && e.Start <= threeDays).ToList();
            model.Interests = ffContext.Interests.ToList();
            if (Request.IsAuthenticated)
            {
                model.FirstName = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().FirstName;
            }

            return View(model);
        }

        public virtual ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public virtual ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}