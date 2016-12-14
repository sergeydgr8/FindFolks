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

        public CurrentUserViewModel CreateUserModel(ApplicationUser u)
        {
            var FriendId = ffContext.Users.Where(us => us.UserName == u.UserName).FirstOrDefault().Id;
            var UserId = ffContext.Users.Where(us => us.UserName == User.Identity.Name).FirstOrDefault().Id;
            return new CurrentUserViewModel()
            {
                Id = u.Id,
                Username = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                ZipCode = u.ZipCode,
                CurrentUser = u.UserName == User.Identity.Name,
                IsFriend = ffContext.Friends.Where(f => f.FriendOf == UserId && f.FriendTo == FriendId).FirstOrDefault() != null
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
