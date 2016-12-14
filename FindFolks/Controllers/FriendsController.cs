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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FindFolks.Controllers
{
    public class FriendsController : Controller
    {
        private FFContext ffContext = new FFContext();
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public FriendsController()
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

        public List<EventInfoModel> GetUserEvents(string UserId)
        {
            var signUps = ffContext.SignUps.Where(s => s.UserName == UserId).ToList();
            if (signUps == null)
                return new List<EventInfoModel>();
            var eventIds = new List<int>();
            foreach (var s in signUps)
                eventIds.Add(s.EventId);
            var userEvents = ffContext.Events.Where(e => eventIds.Contains(e.EventId) && e.Start >= DateTime.Now).ToList();
            return GetEventsViewHelper(userEvents);
        }

        // GET: Friends
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            var model = new ListFriendsModel();
            var UserId = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;
            var FriendConnections = ffContext.Friends.Where(f => f.FriendOf == UserId).ToList();
            if (FriendConnections == null)
                return View(model);
            var FriendIds = new List<string>();
            foreach (var f in FriendConnections)
                FriendIds.Add(f.FriendTo);
            model.Friends = ffContext.Users.Where(u => FriendIds.Contains(u.Id)).ToList();
            model.Events = new List<EventInfoModel>();
            foreach (var f in model.Friends)
                model.Events.AddRange(GetUserEvents(f.Id).Distinct());
            return View(model);
        }

        
    }
}
