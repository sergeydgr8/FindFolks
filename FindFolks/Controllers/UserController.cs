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

namespace FindFolks.Controllers
{
    public class UserController : Controller
    {
        private FFContext ffContext = new FFContext();

        public UserController()
        {
        }

        public CurrentUserViewModel CreateUserModel(ApplicationUser u)
        {
            return new CurrentUserViewModel()
            {
                Id = u.Id,
                Username = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                ZipCode = u.ZipCode
            };
        }

        // GET: User
        public ActionResult Index(string Id = null)
        {
            ApplicationUser CurrentUser = null;
            if (User.Identity.IsAuthenticated)
                CurrentUser = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (CurrentUser == null)
                return RedirectToAction("Index", "Home");
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
    }
}
