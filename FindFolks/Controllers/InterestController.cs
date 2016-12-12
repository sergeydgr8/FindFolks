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
    public class InterestController : Controller
    {
        private FFContext ffContext = new FFContext();
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public InterestController()
        {
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ffContext));
        }

        public InterestController(FFContext ffContext)
        {
            this.ffContext = ffContext;
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ffContext));
        }

        // GET: Interest
        public virtual ActionResult Index(InterestViewModel model)
        {
            if (model == null)
                model = new InterestViewModel();
            model.Interests = ffContext.Interests.ToList();
            model.Categories = new List<string>();
            foreach (var i in model.Interests)
                if (!model.Categories.Contains(i.Category))
                    model.Categories.Add(i.Category);
            if (User.Identity.IsAuthenticated)
            {
                var UserId = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;
                var UserInterests = ffContext.InterestedIns.Where(i => i.UserName == UserId).ToList();
                model.UserInterests = new List<Interest>();
                foreach (var ui in UserInterests)
                {
                    model.UserInterests.Add(new Interest()
                        {
                            Category = ui.Category,
                            Keyword = ui.Keyword
                        });
                }
            }
            return View(model);
        }

        public List<Group> GetGroupsWithInterest(string Category, string Keyword)
        {
            return GetGroupsWithInterest(new Interest()
                {
                    Category = Category,
                    Keyword = Keyword
                });
        }

        public List<Group> GetGroupsWithInterest(Interest interest)
        {
            var GroupIds = new List<int>();
            var Abouts = ffContext.Abouts.Where(a => a.Category == interest.Category && a.Keyword == interest.Keyword).ToList();
            foreach (var a in Abouts)
                GroupIds.Add(a.GroupId);
            return ffContext.Groups.Where(g => GroupIds.Contains(g.GroupId)).ToList();
        }

        public virtual ActionResult Groups(string Category, string Keyword)
        {
            var model = new GroupsInInterestModel();
            model.Interest = new Interest()
            {
                Category = Category,
                Keyword = Keyword
            };
            model.Groups = GetGroupsWithInterest(model.Interest);
            if (User.Identity.IsAuthenticated)
            {
                var UserId = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;
                model.HasInterest = ffContext.InterestedIns.Where(i => i.Category == Category && i.Keyword == Keyword && i.UserName == UserId).FirstOrDefault() != null;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult AddInterest(AddInterestModel model)
        {
            var UserId = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;
            var MemberIsAlready = ffContext.InterestedIns.Where(i => i.Category == model.Category && i.Keyword == model.Keyword && i.UserName == UserId).FirstOrDefault();
            if (MemberIsAlready == null)
            {
                var NewInterest = new InterestedIn()
                {
                    Category = model.Category,
                    Keyword = model.Keyword,
                    UserName = UserId,
                    Interest = ffContext.Interests.Where(i => i.Category == model.Category && i.Keyword == model.Keyword).FirstOrDefault(),
                    ApplicationUser = UserManager.FindByName(User.Identity.Name)
                };
                ffContext.InterestedIns.Add(NewInterest);
                ffContext.SaveChanges();
            }
            return RedirectToAction("Groups", new { Category = model.Category, Keyword = model.Keyword });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult RemoveInterest(AddInterestModel model)
        {
            var UserId = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;
            var HasInterest = ffContext.InterestedIns.Where(i => i.Category == model.Category && i.Keyword == model.Keyword && i.UserName == UserId).FirstOrDefault();
            if (HasInterest != null)
            {
                ffContext.InterestedIns.Remove(HasInterest);
                ffContext.SaveChanges();
            }
            return RedirectToAction("Groups", new { Category = model.Category, Keyword = model.Keyword });
        }

        public virtual ActionResult Category(string Id)
        {
            var retModel = new InterestViewModel();
            retModel.Category = Id;
            retModel.Interests = ffContext.Interests.Where(i => i.Category == retModel.Category).ToList();
            if (retModel.Interests == null)
                return RedirectToAction("Index");
            return View(retModel);
        }
    }
}
