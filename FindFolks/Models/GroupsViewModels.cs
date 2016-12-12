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

    public class GroupInfoModel
    {
        public Group Group { get; set; }
        public List<ApplicationUser> Members { get; set; }
        public List<Event> Events { get; set; }
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
}