using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FindFolks.EF
{
    public class FFContext : DbContext
    {
        public FFContext() : base("FFContext") { }

        
    }
}