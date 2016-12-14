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
    public class UserController : Controller
    {
        private FFContext ffContext = new FFContext();
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public UserController()
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

        public CurrentUserViewModel CreateUserModel(ApplicationUser u)
        {
            var FriendId = ffContext.Users.Where(us => us.UserName == u.UserName).FirstOrDefault().Id;
            var UserId = ffContext.Users.Where(us => us.UserName == User.Identity.Name).FirstOrDefault().Id;
            var Now = DateTime.Now;
            var LastWeek = Now.Subtract(new TimeSpan(7, 0, 0, 0));
            var SignUps = ffContext.SignUps.Where(s => s.UserName == u.Id).ToList();
            var EventIds = new List<int>();
            foreach (var s in SignUps)
                EventIds.Add(s.EventId);
            var FriendConnections = ffContext.Friends.Where(f => f.FriendOf == u.Id).ToList();
            var FriendIds = new List<string>();
            foreach (var f in FriendConnections)
                FriendIds.Add(f.FriendTo);
            return new CurrentUserViewModel()
            {
                Id = u.Id,
                Username = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                ZipCode = u.ZipCode,
                CurrentUser = u.UserName == User.Identity.Name,
                IsFriend = ffContext.Friends.Where(f => f.FriendOf == UserId && f.FriendTo == FriendId).FirstOrDefault() != null,
                Events = GetEventsViewHelper(ffContext.Events.Where(e => EventIds.Contains(e.EventId) && e.Start <= Now && e.Start >= LastWeek).ToList()),
                Friends = ffContext.Users.Where(us => FriendIds.Contains(us.Id)).ToList()
            };
        }

        // GET: User
        public ActionResult Index(string Id = null)
        {
            ApplicationUser CurrentUser = null;
            if (User.Identity.IsAuthenticated)
                CurrentUser = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (CurrentUser == null)
                return RedirectToAction("Login", "Account");
            if (Id == null)
            {
                return View(CreateUserModel(CurrentUser));
            }
            else
            {
                var newUser = ffContext.Users.Where(u => u.UserName == Id).FirstOrDefault();
                if (newUser == null)
                    RedirectToAction("Index");
                return View(CreateUserModel(newUser));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFriend(AddFriendModel model)
        {
            var NewFriendId = ffContext.Users.Where(u => u.UserName == model.UserName).FirstOrDefault().Id;
            var UserId = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;
            var alreadyAdded = ffContext.Friends.Where(f => f.FriendOf == UserId && f.FriendTo == NewFriendId).FirstOrDefault();
            if (alreadyAdded != null)
                return RedirectToAction("Index", "Friends");
            var NewConnection = new Friend();
            NewConnection.AUFriendOf = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            NewConnection.AUFriendTo = ffContext.Users.Where(u => u.UserName == model.UserName).FirstOrDefault();
            NewConnection.FriendOf = UserId;
            NewConnection.FriendTo = NewFriendId;
            ffContext.Friends.Add(NewConnection);
            ffContext.SaveChanges();
            return RedirectToAction("Index", "Friends");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFriend(AddFriendModel model)
        {
            var FriendId = ffContext.Users.Where(u => u.UserName == model.UserName).FirstOrDefault().Id;
            var UserId = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;
            var alreadyAdded = ffContext.Friends.Where(f => f.FriendOf == UserId && f.FriendTo == FriendId).FirstOrDefault();
            if (alreadyAdded == null)
                return RedirectToAction("Index", "Friends");
            ffContext.Friends.Remove(alreadyAdded);
            ffContext.SaveChanges();
            return RedirectToAction("Index", "Friends");
        }
    }
}
