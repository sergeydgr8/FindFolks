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
    public class InterestController : Controller
    {
        private FFContext ffContext = new FFContext();

        public InterestController() { }

        public InterestController(FFContext ffContext)
        {
            this.ffContext = ffContext;
        }

        // GET: Interest
        public virtual ActionResult Index(InterestViewModel model)
        {
            if (model == null)
                model = new InterestViewModel();
            model.Interests = ffContext.Interests.ToList();
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
            return View(model);
        }

        public virtual ActionResult AddInterest(AddInterestModel model)
        {
            ffContext.Interests.Add(new Interest()
                {
                    Category = model.Category,
                    Keyword = model.Keyword
                });
            ffContext.SaveChanges();
            var retModel = new GroupsInInterestModel()
            {
                Interest = new Interest()
                {
                    Category = model.Category,
                    Keyword = model.Keyword
                },
                Groups = GetGroupsWithInterest(new Interest()
                {
                    Category = model.Category,
                    Keyword = model.Keyword
                })
            };
            return RedirectToAction("Group", retModel);

        }

        /*protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ffContext.Dispose();
            }
            base.Dispose(disposing);
        }*/
    }
}
