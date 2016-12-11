using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;
using FindFolks.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FindFolks.EF
{
    public class FFContext : DbContext
    {
        public FFContext() : base("FFContext") { }
        //public FFContext() { }

        public DbSet<ApplicationUser> Users { get; set; }
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
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(l => l.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(l => new { l.RoleId, l.UserId });

            /*modelBuilder.Entity<Friend>()
                .HasRequired(f => f.ApplicationUser1)
                .WithMany(u => u.Friends)
                .HasForeignKey(f => f.FriendOf)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Friend>()
                .HasRequired(f => f.ApplicationUser2)
                .WithMany(u => u.Friends)
                .HasForeignKey(f => f.FriendTo)
                .WillCascadeOnDelete(false);*/

            modelBuilder.Entity<Friend>()
                .HasRequired(f => f.AUFriendOf)
                .WithMany(u => u.FriendsOf)
                .HasForeignKey(f => f.FriendOf)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Friend>()
                .HasRequired(f => f.AUFriendTo)
                .WithMany(u => u.FriendsTo)
                .HasForeignKey(f => f.FriendTo)
                .WillCascadeOnDelete(false);


            base.OnModelCreating(modelBuilder);
        }

        public static FFContext Create()
        {
            return new FFContext();
        }
    }
}