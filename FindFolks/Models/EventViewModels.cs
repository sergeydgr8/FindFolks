﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FindFolks.Models
{
    public class EventInfoModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public string Location { get; set; }
        public int ZipCode { get; set; }
        public string GroupName { get; set; }
        public int GroupId { get; set; }
        public bool Attending { get; set; }
        public string UserId { get; set; }
        public List<ApplicationUser> Attendees { get; set; }

        [Display(Name = "Rate the event")]
        public int Rating { get; set; }

    }

    public class RateEventModel
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
    }

    public class EventsIndexModel
    {
        public List<EventInfoModel> AllUpcomingEvents { get; set; }
        public List<EventInfoModel> AllHappeningNow { get; set; }
        public List<EventInfoModel> UserInterestedEvents { get; set; }
        public List<EventInfoModel> UserUpcomingEvents { get; set; }
        public List<EventInfoModel> UserCurrentEvents { get; set; }
        public List<EventInfoModel> PastAttendedEvents { get; set; }
    }

    public class SignUpToEventModel
    {
        public int EventId { get; set; }
        public string UserId { get; set; }
    }

}