using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FindFolks.EF;
using FindFolks.Models;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FindFolks.Controllers
{
    public class EventsController : Controller
    {
        private FFContext ffContext = new FFContext();
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public EventsController()
        {
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ffContext));
        }

        // GET: Events
        public ActionResult Index()
        {
            var UserId = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;
            var model = new EventsIndexModel();
            model.AllUpcomingEvents = ffContext.Events.Where(e => e.Start >= DateTime.Now).ToList();
            model.AllHappeningNow = ffContext.Events.Where(e => e.Start <= DateTime.Now && e.End >= DateTime.Now).ToList();
            var EventsUserAttends = ffContext.SignUps.Where(s => s.UserName == UserId).ToList();
            var EventIdsUserAttends = new List<int>();
            foreach (var e in EventsUserAttends)
            {
                EventIdsUserAttends.Add(e.EventId);
            }
            model.UserUpcomingEvents = ffContext.Events.Where(e => EventIdsUserAttends.Contains(e.EventId) && e.Start >= DateTime.Now).ToList();
            model.UserCurrentEvents = ffContext.Events.Where(e => EventIdsUserAttends.Contains(e.EventId) && e.Start <= DateTime.Now && e.End >= DateTime.Now).ToList();
            
            // interests
            var UserInterestIns = ffContext.InterestedIns.Where(i => i.UserName == UserId).ToList();
            var UserInterests = new List<Interest>();
            foreach (var i in UserInterestIns)
            {
                UserInterests.Add(new Interest() { Category = i.Category, Keyword = i.Keyword });
            }
            var Abouts = ffContext.Abouts.Where(a => UserInterests.Contains(new Interest() { Category = a.Category, Keyword = a.Keyword })).ToList();
            

            return View(model);
        }

    }
}
