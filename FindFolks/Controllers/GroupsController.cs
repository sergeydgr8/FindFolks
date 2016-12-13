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
            foreach (var g in model.Groups)
            {
                g.GroupCreator = ffContext.Users.Where(u => u.Id == g.GroupCreator).FirstOrDefault().UserName;
            }
            return View(model);
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
            return model;
        }

        public List<EventInfoModel> GetEventsViewHelper(List<Event> l)
        {
            var ret = new List<EventInfoModel>();
            foreach (var e in l)
                ret.Add(EventModelHelper(e));
            return ret;
        }

        public ActionResult Info(int Id = 0)
        {
            var model = new GroupInfoModel();
            model.Group = ffContext.Groups.Where(g => g.GroupId == Id).FirstOrDefault();
            if (model.Group == null)
                return RedirectToAction("Index");
            var creator = ffContext.Users.Where(u => u.Id == model.Group.GroupCreator).FirstOrDefault();
            model.GroupCreator = creator.FirstName + " " + creator.LastName;
            var MemberBT = ffContext.BelongTos.Where(b => b.GroupId == model.Group.GroupId).ToList();
            var MemberIds = new List<string>();
            foreach (var m in MemberBT)
                MemberIds.Add(m.UserName);
            model.Members = new List<GroupMemberModel>();
            var members = ffContext.Users.Where(u => MemberIds.Contains(u.Id)).ToList();
            foreach (var member in members)
            {
                var NewMember = new GroupMemberModel();
                NewMember.FirstName = member.FirstName;
                NewMember.LastName = member.LastName;
                NewMember.UserName = member.UserName;
                NewMember.Authorized = ffContext.BelongTos.Where(u => u.UserName == member.Id && u.GroupId == Id).FirstOrDefault().Authorized;
                model.Members.Add(NewMember);
            }

            var organizes = ffContext.Organizes.Where(o => o.GroupId == Id).ToList();
            var eventIds = new List<int>();
            foreach (var o in organizes)
                eventIds.Add(o.EventId);
            model.Events = GetEventsViewHelper(ffContext.Events.Where(e => eventIds.Contains(e.EventId)).ToList());

            if (!User.Identity.IsAuthenticated)
                return View(model);
            var UserId = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;
            model.IsInGroup = ffContext.BelongTos.Where(b => b.GroupId == model.Group.GroupId && b.UserName == UserId).FirstOrDefault() != null;
            if (model.IsInGroup)
                model.Authorized = ffContext.BelongTos.Where(b => b.GroupId == model.Group.GroupId && b.UserName == UserId).FirstOrDefault().Authorized;
            model.JoinGroup = new JoinGroupModel();
            model.JoinGroup.GroupId = model.Group.GroupId;
            model.JoinGroup.UserId = UserId;

            // rating calculation logic
            var lastWeek = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0, 0));
            var EventsLast7Days = ffContext.Events.Where(e => eventIds.Contains(e.EventId) && e.Start >= lastWeek && e.Start <= DateTime.Now).ToList();
            var EventIdsLast7Days = new List<int>();
            foreach (var e in EventsLast7Days)
                EventIdsLast7Days.Add(e.EventId);
            var SignUpsLast7 = ffContext.SignUps.Where(s => EventIdsLast7Days.Contains(s.EventId)).ToList();
            if (SignUpsLast7 != null && SignUpsLast7.Count() != 0)
            {
                var total = 0;
                foreach (var s in SignUpsLast7)
                    total += s.Rating;
                var rating = total / SignUpsLast7.Count();
                model.RatingString = rating.ToString() + "/5";
                model.Rating = rating;
            }
            else
                model.RatingString = "N/A";

            return View(model);
        }

        // GET: CreateGroup
        public ActionResult CreateGroup()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: CreateGroup
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
            var NewBelongsTo = new BelongsTo()
            {
                GroupId = NewGroup.GroupId,
                UserName = User.Identity.Name,
                Authorized = true,
                Group = NewGroup,
                ApplicationUser = UserManager.FindByName(User.Identity.Name)
            };
            ffContext.BelongTos.Add(NewBelongsTo);
            ffContext.SaveChanges();
            return RedirectToAction("Info", new { id = NewGroup.GroupId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JoinGroup(JoinGroupModel model)
        {
            var UserId = ffContext.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;
            var IsMemberAlready = ffContext.BelongTos.Where(b => b.UserName == UserId && b.GroupId == model.GroupId).FirstOrDefault();
            if (IsMemberAlready == null)
            {
                var NewMembership = new BelongsTo()
                {
                    GroupId = model.GroupId,
                    UserName = User.Identity.Name,
                    Authorized = false,
                    Group = ffContext.Groups.Where(g => g.GroupId == model.GroupId).FirstOrDefault(),
                    ApplicationUser = UserManager.FindByName(User.Identity.Name)
                };
                ffContext.BelongTos.Add(NewMembership);
                ffContext.SaveChanges();
            }
            return RedirectToAction("Info", new { id = model.GroupId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LeaveGroup(JoinGroupModel model)
        {
            var BelongsTo = ffContext.BelongTos.Where(b => b.UserName == model.UserId && b.GroupId == model.GroupId).FirstOrDefault();
            if (BelongsTo != null)
            {
                ffContext.BelongTos.Remove(BelongsTo);
                ffContext.SaveChanges();
            }
            return RedirectToAction("Info", new { id = model.GroupId });
        }

        // GET: CreateEvent
        public ActionResult CreateEvent(int GroupId)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            var model = new CreateEventModel();
            model.GroupId = GroupId;
            model.Locations = ffContext.Locations.ToList();
            model.Start = DateTime.Now;
            model.End = DateTime.Now.AddHours(3);
            return PartialView(model);
        }

        // POST: CreateEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEvent(CreateEventModel model)
        {
            var LocationExists = ffContext.Locations.Where(l => l.LocationName == model.LocationName && l.ZipCode == model.ZipCode).FirstOrDefault();
            if (LocationExists == null)
            {
                var NewLoc = new Location()
                {
                    LocationName = model.LocationName,
                    ZipCode = model.ZipCode,
                    Address = model.Address,
                    Description = model.LocationDescription,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude
                };
                ffContext.Locations.Add(NewLoc);
                ffContext.SaveChanges();
            }

            var NewEvent = new Event();
            NewEvent.Title = model.Title;
            NewEvent.Description = model.Description;
            NewEvent.Start = model.Start;
            NewEvent.End = model.End;
            NewEvent.Location = ffContext.Locations.Where(l => l.LocationName == model.LocationName && l.ZipCode == model.ZipCode).FirstOrDefault();
            NewEvent.LocationName = model.LocationName;
            NewEvent.ZipCode = model.ZipCode;
            ffContext.Events.Add(NewEvent);
            ffContext.SaveChanges();

            var NewOrganizes = new Organize();
            NewOrganizes.Event = NewEvent;
            NewOrganizes.EventId = NewEvent.EventId;
            NewOrganizes.Group = ffContext.Groups.Where(g => g.GroupId == model.GroupId).FirstOrDefault();
            NewOrganizes.GroupId = model.GroupId;
            ffContext.Organizes.Add(NewOrganizes);
            ffContext.SaveChanges();

            var SignsUp = new SignUp();
            SignsUp.ApplicationUser = UserManager.FindByName(User.Identity.Name);
            SignsUp.Event = NewEvent;
            SignsUp.UserName = User.Identity.Name;
            SignsUp.EventId = NewEvent.EventId;
            ffContext.SignUps.Add(SignsUp);
            ffContext.SaveChanges();

            return RedirectToAction("Info", new { Id = model.GroupId });
        }
    }
}
