using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FindFolks.Models;
using FindFolks.EF;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FindFolks.Controllers
{
    public partial class HomeController : Controller
    {
        private FFContext ffContext = new FFContext();
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public HomeController()
        {
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ffContext));
        }

        public HomeController(FFContext ffContext)
        {
            this.ffContext = ffContext;
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ffContext));
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
                var UserId = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;
                var UserBelongsIn = ffContext.BelongTos.Where(u => u.UserName == UserId).ToList();
                var GroupIds = new List<int>();
                foreach (var b in UserBelongsIn)
                    GroupIds.Add(b.GroupId);
                model.UserGroups = ffContext.Groups.Where(g => GroupIds.Contains(g.GroupId)).ToList();
                var AllGroupEvents = ffContext.Organizes.Where(o => GroupIds.Contains(o.GroupId)).ToList();
                var AllEventIds = new List<int>();
                foreach (var e in AllGroupEvents)
                    AllEventIds.Add(e.EventId);
                var UserEvents = ffContext.SignUps.Where(s => AllEventIds.Contains(s.EventId)).ToList();
                var EventIds = new List<int>();
                foreach (var e in UserEvents)
                    EventIds.Add(e.EventId);
                model.UpcomingUserEvents = ffContext.Events.Where(e => EventIds.Contains(e.EventId) && e.Start >= DateTime.Now).ToList();
            }

            return View(model);
        }

        public virtual ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}