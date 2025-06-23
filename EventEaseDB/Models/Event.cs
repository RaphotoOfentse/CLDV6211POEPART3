namespace EventEaseDB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Event")]
    public partial class Event
    {
        public int EventID { get; set; }

        [Required]
        [StringLength(100)]
        public string EventName { get; set; }

        public int? VenueID { get; set; }

        //[Column("Date")]
        public DateTime Date { get; set; }

        public string Description { get; set; }

        public string ImageURL { get; set; }
        public int? EventTypeID { get; set; }
        public virtual Venue Venue { get; set; }
        public virtual EventType EventType { get; set; } // ? For showing event type

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
