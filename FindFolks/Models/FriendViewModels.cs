using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FindFolks.Models
{
    public class ListFriendsModel
    {
        public List<ApplicationUser> Friends { get; set; }
    }

    public class AddFriendModel
    {
        public string UserName { get; set; }
    }
}