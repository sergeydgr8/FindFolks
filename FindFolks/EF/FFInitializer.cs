using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FindFolks.Models;

namespace FindFolks.EF
{
    public class FFInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<FFContext>
    {
        protected override void Seed(FFContext context)
        {
            //base.Seed(context);

            var interests = new List<Interest>
            {
                new Interest { Category = "Sports", Keyword = "Tennis" },
                new Interest { Category = "Sports", Keyword = "Soccer" },
                new Interest { Category = "Technology", Keyword = "PHP" },
                new Interest { Category = "Technology", Keyword = "Node.JS" }
            };

            interests.ForEach(i => context.Interests.Add(i));
            context.SaveChanges();
        }
    }
}