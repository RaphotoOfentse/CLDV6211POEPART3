namespace EventEaseDB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("Venue")]
    public partial class Venue
    {
        public Venue()
        {
            Bookings = new HashSet<Booking>();
        }
        public int VenueID { get; set; }

        [Required]
        [StringLength(100)]
        public string VenueName { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; }

        public int Capacity { get; set; }

        public string ImageURL { get; set; }
        [NotMapped]
        public string ImageFile { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    
}
}
