using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using FindFolks.EF;

namespace FindFolks.Models
{
    public class Friend
    {
        [Key, Column(Order = 1), ForeignKey("AUFriendOf")]
        public string FriendOf { get; set; }

        [Key, Column(Order = 2), ForeignKey("AUFriendTo")]
        public string FriendTo { get; set; }

        public virtual ApplicationUser AUFriendOf { get; set; }
        public virtual ApplicationUser AUFriendTo { get; set; }
    }

    public class Group
    {
        [Key]
        public string GroupId { get; set; }

        [Required]
        public string GroupName { get; set; }

        [Required]
        public string GroupDescription { get; set; }

        [ForeignKey("ApplicationUser")]
        public string GroupCreator { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }

    public class Interest
    {
        [Key, Column(Order = 1)]
        [Required]
        public string Category { get; set; }

        [Key, Column(Order = 2)]
        [Required]
        public string Keyword { get; set; }
    }

    public class InterestedIn
    {
        [Key, Column(Order = 1), ForeignKey("ApplicationUser")]
        public string Username { get; set; }

        [Key, Column(Order = 2), ForeignKey("Interest")]
        public string Category { get; set; }

        [Key, Column(Order = 3), ForeignKey("Interest")]
        public string Keyword { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Interest Interest { get; set; }
    }

    public class About
    {
        [Key, Column(Order = 1), ForeignKey("Interest")]
        public string Category { get; set; }

        [Key, Column(Order = 2), ForeignKey("Interest")]
        public string Keyword { get; set; }

        [Key, Column(Order = 3), ForeignKey("Group")]
        public string GroupId { get; set; }

        public virtual Group Group { get; set; }
        public virtual Interest Interest { get; set; }
    }

    public class BelongsTo
    {
        [Key, Column(Order = 1), ForeignKey("Group")]
        public string GroupId { get; set; }

        [Key, Column(Order = 2),ForeignKey("ApplicationUser")]
        public string Username { get; set; }

        public bool Authorized { get; set; }

        public virtual Group Group { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }

    public class Location
    {
        [Key, Column(Order = 1)]
        public string LocationName { get; set; }

        [Key, Column(Order = 2)]
        public int ZipCode { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Description { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }

    public class Event
    {
        [Key]
        public string EventId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        [Required]
        [ForeignKey("Location"), Column(Order = 1)]
        public string LocationName { get; set; }

        [Required]
        [ForeignKey("Location"), Column(Order = 2)]
        public int ZipCode { get; set; }

        public virtual Location Location { get; set; }
    }

    public class Organize
    {
        [Key, Column(Order = 1), ForeignKey("Event")]
        public string EventId { get; set; }

        [Key, Column(Order = 2), ForeignKey("Group")]
        public string GroupId { get; set; }

        public virtual Event Event { get; set; }
        public virtual Group Group { get; set; }
    }

    public class SignUp
    {
        [Key, Column(Order = 1), ForeignKey("Event")]
        public string EventId { get; set; }

        [Key, Column(Order = 2), ForeignKey("ApplicationUser")]
        public string Username { get; set; }

        public int Rating { get; set; }

        public virtual Event Event { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }

}