using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FindFolks.Models
{
    public class AddInterestModel
    {
        public string Category { get; set; }
        public string Keyword { get; set; }
    }

    public class GroupsInInterestModel
    {
        public Interest Interest { get; set; }
        public List<Group> Groups { get; set; }
    }

    public class InterestViewModel
    {
        public List<Interest> Interests { get; set; }
        public AddInterestModel AddInterest { get; set; }
        public GroupsInInterestModel GroupsInInterest { get; set; }
    }
}