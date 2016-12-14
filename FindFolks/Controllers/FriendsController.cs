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
            return View(model);
        }

        
    }
}
