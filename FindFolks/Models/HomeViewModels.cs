using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FindFolks.EF;

namespace FindFolks.Models
{
    public class HomeIndexViewModel
    {
        public List<EventInfoModel> UpcomingEvents { get; set; }
        public List<Interest> Interests { get; set; }
        public string FirstName { get; set; }
        public List<EventInfoModel> UpcomingUserEvents { get; set; }
        public List<Group> UserGroups { get; set; }
    }
}