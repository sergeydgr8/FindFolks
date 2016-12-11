using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;
using FindFolks.Models;

namespace FindFolks.EF
{
    public class FFContext : DbContext
    {
        public FFContext() : base("FFContext") { }

        public DbSet<Friend> Friends { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Interest> Interests { get; set; }
        public DbSet<InterestedIn> InterestedIns { get; set; }
        public DbSet<About> Abouts { get; set; }
        public DbSet<BelongsTo> BelongTos { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Organize> Organizes { get; set; }
        public DbSet<SignUp> SignUps { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}