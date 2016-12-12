namespace FindFolks.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Abouts",
                c => new
                    {
                        Category = c.String(nullable: false, maxLength: 128),
                        Keyword = c.String(nullable: false, maxLength: 128),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Category, t.Keyword, t.GroupId })
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Interests", t => new { t.Category, t.Keyword }, cascadeDelete: true)
                .Index(t => new { t.Category, t.Keyword })
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        GroupName = c.String(nullable: false),
                        GroupDescription = c.String(nullable: false),
                        UserName = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.GroupId)
                .ForeignKey("dbo.ApplicationUsers", t => t.UserName)
                .Index(t => t.UserName);
            
            CreateTable(
                "dbo.ApplicationUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        ZipCode = c.String(),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IdentityUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Friends",
                c => new
                    {
                        FriendOf = c.String(nullable: false, maxLength: 128),
                        FriendTo = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.FriendOf, t.FriendTo })
                .ForeignKey("dbo.ApplicationUsers", t => t.FriendOf)
                .ForeignKey("dbo.ApplicationUsers", t => t.FriendTo)
                .Index(t => t.FriendOf)
                .Index(t => t.FriendTo);
            
            CreateTable(
                "dbo.IdentityUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.IdentityUserRoles",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        IdentityRole_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.IdentityRoles", t => t.IdentityRole_Id)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.IdentityRole_Id);
            
            CreateTable(
                "dbo.Interests",
                c => new
                    {
                        Category = c.String(nullable: false, maxLength: 128),
                        Keyword = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Category, t.Keyword });
            
            CreateTable(
                "dbo.BelongsToes",
                c => new
                    {
                        GroupId = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 128),
                        Authorized = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupId, t.UserName })
                .ForeignKey("dbo.ApplicationUsers", t => t.UserName, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.UserName);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        LocationName = c.String(nullable: false, maxLength: 128),
                        ZipCode = c.Int(nullable: false),
                        EventId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EventId)
                .ForeignKey("dbo.Locations", t => new { t.LocationName, t.ZipCode }, cascadeDelete: true)
                .Index(t => new { t.LocationName, t.ZipCode });
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        LocationName = c.String(nullable: false, maxLength: 128),
                        ZipCode = c.Int(nullable: false),
                        Address = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.LocationName, t.ZipCode });
            
            CreateTable(
                "dbo.InterestedIns",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 128),
                        Category = c.String(nullable: false, maxLength: 128),
                        Keyword = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserName, t.Category, t.Keyword })
                .ForeignKey("dbo.ApplicationUsers", t => t.UserName, cascadeDelete: true)
                .ForeignKey("dbo.Interests", t => new { t.Category, t.Keyword }, cascadeDelete: true)
                .Index(t => t.UserName)
                .Index(t => new { t.Category, t.Keyword });
            
            CreateTable(
                "dbo.Organizes",
                c => new
                    {
                        EventId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EventId, t.GroupId })
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.SignUps",
                c => new
                    {
                        EventId = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 128),
                        Rating = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.EventId, t.UserName })
                .ForeignKey("dbo.ApplicationUsers", t => t.UserName, cascadeDelete: true)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.UserName);
            
            CreateTable(
                "dbo.IdentityRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IdentityUserRoles", "IdentityRole_Id", "dbo.IdentityRoles");
            DropForeignKey("dbo.SignUps", "EventId", "dbo.Events");
            DropForeignKey("dbo.SignUps", "UserName", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Organizes", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Organizes", "EventId", "dbo.Events");
            DropForeignKey("dbo.InterestedIns", new[] { "Category", "Keyword" }, "dbo.Interests");
            DropForeignKey("dbo.InterestedIns", "UserName", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Events", new[] { "LocationName", "ZipCode" }, "dbo.Locations");
            DropForeignKey("dbo.BelongsToes", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.BelongsToes", "UserName", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Abouts", new[] { "Category", "Keyword" }, "dbo.Interests");
            DropForeignKey("dbo.Abouts", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Groups", "UserName", "dbo.ApplicationUsers");
            DropForeignKey("dbo.IdentityUserRoles", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.IdentityUserLogins", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Friends", "FriendTo", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Friends", "FriendOf", "dbo.ApplicationUsers");
            DropForeignKey("dbo.IdentityUserClaims", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.SignUps", new[] { "UserName" });
            DropIndex("dbo.SignUps", new[] { "EventId" });
            DropIndex("dbo.Organizes", new[] { "GroupId" });
            DropIndex("dbo.Organizes", new[] { "EventId" });
            DropIndex("dbo.InterestedIns", new[] { "Category", "Keyword" });
            DropIndex("dbo.InterestedIns", new[] { "UserName" });
            DropIndex("dbo.Events", new[] { "LocationName", "ZipCode" });
            DropIndex("dbo.BelongsToes", new[] { "UserName" });
            DropIndex("dbo.BelongsToes", new[] { "GroupId" });
            DropIndex("dbo.IdentityUserRoles", new[] { "IdentityRole_Id" });
            DropIndex("dbo.IdentityUserRoles", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.IdentityUserLogins", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Friends", new[] { "FriendTo" });
            DropIndex("dbo.Friends", new[] { "FriendOf" });
            DropIndex("dbo.IdentityUserClaims", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Groups", new[] { "UserName" });
            DropIndex("dbo.Abouts", new[] { "GroupId" });
            DropIndex("dbo.Abouts", new[] { "Category", "Keyword" });
            DropTable("dbo.IdentityRoles");
            DropTable("dbo.SignUps");
            DropTable("dbo.Organizes");
            DropTable("dbo.InterestedIns");
            DropTable("dbo.Locations");
            DropTable("dbo.Events");
            DropTable("dbo.BelongsToes");
            DropTable("dbo.Interests");
            DropTable("dbo.IdentityUserRoles");
            DropTable("dbo.IdentityUserLogins");
            DropTable("dbo.Friends");
            DropTable("dbo.IdentityUserClaims");
            DropTable("dbo.ApplicationUsers");
            DropTable("dbo.Groups");
            DropTable("dbo.Abouts");
        }
    }
}
