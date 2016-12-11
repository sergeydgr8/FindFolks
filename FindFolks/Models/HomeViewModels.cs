using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FindFolks.EF;

namespace FindFolks.Models
{
    public class HomeIndexModel
    {
        public List<Event> UpcomingEvents { get; set; }
        public List<Interest> Interests { get; set; }
        public bool LoggedIn { get; set; }
    }
}