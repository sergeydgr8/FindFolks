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

        public EventInfoModel EventModelHelper(Event e)
        {
            var model = new EventInfoModel();
            model.Id = e.EventId;
            model.Title = e.Title;
            model.Description = e.Description;
            model.Start = e.Start;
            model.Location = e.LocationName;
            model.ZipCode = e.ZipCode;
            model.GroupId = ffContext.Organizes.Where(o => o.EventId == e.EventId).FirstOrDefault().GroupId;
            model.GroupName = ffContext.Groups.Where(g => g.GroupId == model.GroupId).FirstOrDefault().GroupName;
            var signUps = ffContext.SignUps.Where(s => s.EventId == e.EventId).ToList();
            var userNames = new List<string>();
            foreach (var s in signUps)
                userNames.Add(s.UserName);
            model.Attendees = ffContext.Users.Where(u => userNames.Contains(u.Id)).ToList();
            if (User.Identity.IsAuthenticated)
            {
                model.Attending = model.Attendees.Contains(ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault());
                model.UserId = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;
                var signedUp = ffContext.SignUps.Where(s => s.EventId == model.Id && s.UserName == model.UserId).FirstOrDefault();
                model.Rating = signedUp != null ? signedUp.Rating : 0;
            }
            return model;
        }

        public List<EventInfoModel> GetEventsViewHelper(List<Event> l)
        {
            var ret = new List<EventInfoModel>();
            foreach (var e in l)
                ret.Add(EventModelHelper(e));
            return ret;
        }

        // GET: Events
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            var UserId = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;
            var model = new EventsIndexModel();
            model.AllUpcomingEvents = GetEventsViewHelper(ffContext.Events.Where(e => e.Start >= DateTime.Now).ToList());
            model.AllHappeningNow = GetEventsViewHelper(ffContext.Events.Where(e => e.Start <= DateTime.Now && e.End >= DateTime.Now).ToList());
            var EventsUserAttends = ffContext.SignUps.Where(s => s.UserName == UserId).ToList();
            var EventIdsUserAttends = new List<int>();
            foreach (var e in EventsUserAttends)
            {
                EventIdsUserAttends.Add(e.EventId);
            }
            model.UserUpcomingEvents = GetEventsViewHelper(ffContext.Events.Where(e => EventIdsUserAttends.Contains(e.EventId) && e.Start >= DateTime.Now).ToList());
            model.UserCurrentEvents = GetEventsViewHelper(ffContext.Events.Where(e => EventIdsUserAttends.Contains(e.EventId) && e.Start <= DateTime.Now && e.End >= DateTime.Now).ToList());
            
            // logic for finding events from groups user could be interested about
            var UserInterestIns = ffContext.InterestedIns.Where(i => i.UserName == UserId).ToList();
            var UserInterests = new List<Interest>();
            foreach (var i in UserInterestIns)
            {
                UserInterests.Add(new Interest() { Category = i.Category, Keyword = i.Keyword });
            }
            var Abouts = new List<About>();
            foreach (var i in UserInterests)
            {
                var tempAbouts = ffContext.Abouts.Where(a => a.Category == i.Category && a.Keyword == i.Keyword).ToList();
                if (tempAbouts != null)
                    foreach (var a in tempAbouts)
                        Abouts.Add(a);
            }
            var GroupIds = new List<int>();
            foreach (var a in Abouts)
                GroupIds.Add(a.GroupId);
            var EventIdsMaybeInterested = new List<int>();
            var OrganizesQueries = ffContext.Organizes.Where(o => GroupIds.Contains(o.GroupId)).ToList();
            foreach (var o in OrganizesQueries)
                EventIdsMaybeInterested.Add(o.EventId);
            var timeNow = DateTime.Now;
            var timeWeek = timeNow.AddDays(7);
            model.UserInterestedEvents = GetEventsViewHelper(ffContext.Events.Where(e => EventIdsMaybeInterested.Contains(e.EventId) && e.Start >= timeNow && e.Start <= timeWeek).ToList());

            return View(model);
        }


        public ActionResult Info(int Id = 0)
        {
            if (Id == 0)
                return RedirectToAction("Index");
            var ev = ffContext.Events.Where(e => e.EventId == Id).FirstOrDefault();
            if (ev == null)
                return RedirectToAction("Index");
            var model = EventModelHelper(ev);
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(SignUpToEventModel model)
        {
            if (model == null)
                return RedirectToAction("Index");
            var signUp = new SignUp()
            {
                Event = ffContext.Events.Where(e => e.EventId == model.EventId).FirstOrDefault(),
                EventId = model.EventId,
                ApplicationUser = ffContext.Users.Where(u => u.Id == model.UserId).FirstOrDefault(),
                UserName = model.UserId
            };
            ffContext.SignUps.Add(signUp);
            ffContext.SaveChanges();
            return RedirectToAction("Info", new { Id = model.EventId });

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateRating(RateEventModel model)
        {
            var ffEvent = ffContext.Events.Where(e => e.EventId == model.EventId).FirstOrDefault();
            if (ffEvent == null || ffEvent.Start >= DateTime.Now || ffContext.SignUps.Where(s => s.EventId == model.EventId && s.UserName == model.UserId) == null)
                return RedirectToAction("Info", new { Id = model.EventId });
            var signUp = ffContext.SignUps.Where(s => s.UserName == model.UserId && s.EventId == model.EventId).FirstOrDefault();
            signUp.Rating = model.Rating;
            ffContext.SaveChanges();
            return RedirectToAction("Info", new { Id = model.EventId });
        }
    }
}
