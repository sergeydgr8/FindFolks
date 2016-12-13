using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FindFolks.Models
{
    public class EventsIndexModel
    {
        public List<Event> AllUpcomingEvents { get; set; }
        public List<Event> AllHappeningNow { get; set; }
        public List<Event> UserInterestedEvents { get; set; }
        public List<Event> UserUpcomingEvents { get; set; }
        public List<Event> UserCurrentEvents { get; set; }
    }
}