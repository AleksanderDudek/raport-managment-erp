namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seeder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IteratorTable",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        _1 = c.Int(name: "1", nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Raport",
                c => new
                    {
                        raport_id = c.String(nullable: false, maxLength: 128),
                        userID = c.String(maxLength: 128),
                        creation_time = c.DateTime(nullable: false),
                        isMinus = c.Int(nullable: false),
                        last_modyfication = c.DateTime(nullable: false),
                        place_Id = c.String(),
                    })
                .PrimaryKey(t => t.raport_id)
                .ForeignKey("dbo.UserFrontInfo", t => t.userID)
                .Index(t => t.userID);
            
            CreateTable(
                "dbo.SendRecordType",
                c => new
                    {
                        IdSendRecord = c.Int(nullable: false, identity: true),
                        raport_id = c.String(maxLength: 128),
                        send_time = c.String(),
                        recipient = c.String(),
                        trash_class = c.String(),
                        weight = c.Double(nullable: false),
                        isSend = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdSendRecord)
                .ForeignKey("dbo.Raport", t => t.raport_id)
                .Index(t => t.raport_id);
            
            CreateTable(
                "dbo.ThrashType",
                c => new
                    {
                        IdTrash = c.Int(nullable: false, identity: true),
                        raport_id = c.String(maxLength: 128),
                        ClassName = c.String(),
                        isNegative = c.Boolean(nullable: false),
                        Quantity = c.Double(nullable: false),
                        Information = c.String(),
                        isSend = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdTrash)
                .ForeignKey("dbo.Raport", t => t.raport_id)
                .Index(t => t.raport_id);
            
            CreateTable(
                "dbo.UserFrontInfo",
                c => new
                    {
                        userId = c.String(nullable: false, maxLength: 128),
                        userName = c.String(),
                    })
                .PrimaryKey(t => t.userId);
            
            CreateTable(
                "dbo.UserPlace",
                c => new
                    {
                        usersId = c.String(nullable: false, maxLength: 128),
                        placesId = c.String(nullable: false, maxLength: 128),
                        TrashPlaces_Id = c.String(maxLength: 128),
                        UserFrontInfo_userId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.usersId, t.placesId })
                .ForeignKey("dbo.TrashPlaces", t => t.TrashPlaces_Id)
                .ForeignKey("dbo.UserFrontInfo", t => t.UserFrontInfo_userId)
                .Index(t => t.TrashPlaces_Id)
                .Index(t => t.UserFrontInfo_userId);
            
            CreateTable(
                "dbo.TrashPlaces",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        nameOfThePlace = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SendSend",
                c => new
                    {
                        IdSendRecord = c.Int(nullable: false, identity: true),
                        raport_id = c.String(),
                        send_time = c.String(),
                        recipient = c.String(),
                        trash_class = c.String(),
                        weight = c.Double(nullable: false),
                        isSend = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdSendRecord);
            
            CreateTable(
                "dbo.TrashClassTable",
                c => new
                    {
                        referenceName = c.String(nullable: false, maxLength: 128),
                        customId = c.String(),
                        className = c.String(),
                        groupOfClass = c.String(),
                        isActive = c.Boolean(nullable: false),
                        isTerminal = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.referenceName);
            
            CreateTable(
                "dbo.TrashViewModel",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        someOption = c.String(),
                        someText = c.String(),
                        someNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPlace", "UserFrontInfo_userId", "dbo.UserFrontInfo");
            DropForeignKey("dbo.UserPlace", "TrashPlaces_Id", "dbo.TrashPlaces");
            DropForeignKey("dbo.Raport", "userID", "dbo.UserFrontInfo");
            DropForeignKey("dbo.ThrashType", "raport_id", "dbo.Raport");
            DropForeignKey("dbo.SendRecordType", "raport_id", "dbo.Raport");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserPlace", new[] { "UserFrontInfo_userId" });
            DropIndex("dbo.UserPlace", new[] { "TrashPlaces_Id" });
            DropIndex("dbo.ThrashType", new[] { "raport_id" });
            DropIndex("dbo.SendRecordType", new[] { "raport_id" });
            DropIndex("dbo.Raport", new[] { "userID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropTable("dbo.TrashViewModel");
            DropTable("dbo.TrashClassTable");
            DropTable("dbo.SendSend");
            DropTable("dbo.TrashPlaces");
            DropTable("dbo.UserPlace");
            DropTable("dbo.UserFrontInfo");
            DropTable("dbo.ThrashType");
            DropTable("dbo.SendRecordType");
            DropTable("dbo.Raport");
            DropTable("dbo.IteratorTable");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
        }
    }
}
