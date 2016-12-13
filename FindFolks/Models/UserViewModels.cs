using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FindFolks.Models
{
    public class CurrentUserViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ZipCode { get; set; }
    }
}