using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FindFolks.Models
{
    public class GroupIndexModel
    {
        public List<Group> Groups { get; set; }
    }

    public class GroupMemberModel
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public bool Authorized { get; set; }
        public string UserName { get; set; }
    }

    public class GroupInfoModel
    {
        public Group Group { get; set; }
        public string GroupCreator { get; set; }
        public List<GroupMemberModel> Members { get; set; }
        public List<Event> Events { get; set; }
        public bool IsInGroup { get; set; }
        public JoinGroupModel JoinGroup { get; set; }
        public bool Authorized { get; set; }
    }

    public class CreateGroupModel
    {
        [Required]
        [Display(Name = "Group name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Group description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Interest category")]
        public string Category { get; set; }

        [Required]
        [Display(Name = "Interest keyword")]
        public string Keyword { get; set; }

        public string Creator { get; set; }
    }

    public class JoinGroupModel
    {
        public int GroupId { get; set; }
        public string UserId { get; set; }
    }


    public class CreateEventModel
    {
        public class LocationModel
        {
            public int LocId { get; set; }
            public string Value { get; set; }
        }

        [Required]
        [Display(Name = "Event title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Event description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Start")]
        public DateTime Start { get; set; }

        [Required]
        [Display(Name = "End")]
        public DateTime End { get; set; }

        [Required]
        [Display(Name = "Location name")]
        public string LocationName { get; set; }

        [Required]
        [Display(Name = "Location address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Location description")]
        public string LocationDescription { get; set; }

        [Required]
        [Display(Name = "Location ZIP")]
        public int ZipCode { get; set; }

        [Required]
        [Display(Name = "Location longitude")]
        public double Longitude { get; set; }

        [Required]
        [Display(Name = "Location latitude")]
        public double Latitude { get; set; }

        public int GroupId { get; set; }
        public List<Location> Locations { get; set; }

        public string LocationNameAndZip { get; set; }
    }
}