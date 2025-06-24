namespace EventEaseDB.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class EventEaseDBConsole : DbContext
    {
        public EventEaseDBConsole()
            : base("name=EventEaseDBConsole")
        {
        }

        public virtual DbSet<Venue> Venues { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Booking> Booking { get; set; }
        public virtual DbSet<BookingSummaryView> BookingSummaryViews { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
        public DbSet<EventType> EventTypes { get; set; }


        //public System.Data.Entity.DbSet<EventEaseDB.Models.Event> Events { get; set; }

        //public DbSet<Booking> Bookings { get; set; }  // A
    }
}
