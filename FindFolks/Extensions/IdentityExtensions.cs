using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace FindFolks.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetUsername(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Username");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetFirstName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("FirstName");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetLastName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("LastName");
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetZipCode(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("ZipCode");
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}