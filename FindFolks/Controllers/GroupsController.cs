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
    public class GroupsController : Controller
    {
        private FFContext ffContext = new FFContext();
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public GroupsController()
        {
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ffContext));
        }

        // GET: Groups
        public ActionResult Index(GroupIndexModel model)
        {
            if (model == null)
                model = new GroupIndexModel();
            model.Groups = ffContext.Groups.ToList();
            return View(model);
        }

        public ActionResult Info(int Id)
        {
            var model = new GroupInfoModel();
            model.Group = ffContext.Groups.Where(g => g.GroupId == Id).FirstOrDefault();
            if (model.Group == null)
                return RedirectToAction("Index");
            return View(model);
        }

        // GET: CreateGroup
        public ActionResult CreateGroup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGroup(CreateGroupModel model)
        {
            var NewGroup = new Group()
            {
                GroupName = model.Name,
                GroupDescription = model.Description,
                ApplicationUser = UserManager.FindByName(User.Identity.Name),
                GroupCreator = User.Identity.Name
            };
            var NewInterest = new Interest()
            {
                Category = model.Category,
                Keyword = model.Keyword
            };
            ffContext.Groups.Add(NewGroup);
            var foundInterest = ffContext.Interests.Where(i => i.Category == NewInterest.Category && i.Keyword == NewInterest.Keyword).FirstOrDefault();
            if (foundInterest == null)
                ffContext.Interests.Add(NewInterest);
            try
            {
                ffContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var ErrorMessages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                var FullErrorMessage = string.Join("; ", ErrorMessages);
                var ExceptionMessage = string.Concat(ex.Message, " The validation errors are as follows: ", FullErrorMessage);
                throw new DbEntityValidationException(ExceptionMessage, ex.EntityValidationErrors);
            }
            var NewAbout = new About()
            {
                Category = NewInterest.Category,
                Keyword = NewInterest.Keyword,
                GroupId = NewGroup.GroupId
            };
            ffContext.Abouts.Add(NewAbout);
            ffContext.SaveChanges();
            return Info(NewGroup.GroupId);


        }
    }
}
