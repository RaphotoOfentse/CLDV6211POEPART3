namespace EventEaseDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateBookingTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Booking",
                c => new
                    {
                        BookingID = c.Int(nullable: false, identity: true),
                        BookingDate = c.DateTime(nullable: false),
                        CustomerName = c.String(nullable: false, maxLength: 100),
                        CustomerEmail = c.String(nullable: false, maxLength: 100),
                        NumberOfGuests = c.Int(nullable: false),
                        EventID = c.Int(nullable: false),
                        VenueID = c.Int(nullable: false),
                        EventName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.BookingID)
                .ForeignKey("dbo.Event", t => t.EventID, cascadeDelete: true)
                .ForeignKey("dbo.Venue", t => t.VenueID, cascadeDelete: true)
                .Index(t => t.EventID)
                .Index(t => t.VenueID);
            
            CreateTable(
                "dbo.Event",
                c => new
                    {
                        EventID = c.Int(nullable: false, identity: true),
                        EventName = c.String(nullable: false, maxLength: 100),
                        VenueID = c.Int(),
                        Date = c.DateTime(nullable: false),
                        Description = c.String(),
                        ImageURL = c.String(),
                    })
                .PrimaryKey(t => t.EventID)
                .ForeignKey("dbo.Venue", t => t.VenueID)
                .Index(t => t.VenueID);
            
            CreateTable(
                "dbo.Venue",
                c => new
                    {
                        VenueID = c.Int(nullable: false, identity: true),
                        VenueName = c.String(nullable: false, maxLength: 100),
                        Location = c.String(nullable: false, maxLength: 200),
                        Capacity = c.Int(nullable: false),
                        ImageURL = c.String(),
                    })
                .PrimaryKey(t => t.VenueID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Booking", "VenueID", "dbo.Venue");
            DropForeignKey("dbo.Booking", "EventID", "dbo.Event");
            DropForeignKey("dbo.Event", "VenueID", "dbo.Venue");
            DropIndex("dbo.Event", new[] { "VenueID" });
            DropIndex("dbo.Booking", new[] { "VenueID" });
            DropIndex("dbo.Booking", new[] { "EventID" });
            DropTable("dbo.Venue");
            DropTable("dbo.Event");
            DropTable("dbo.Booking");
        }
    }
}
