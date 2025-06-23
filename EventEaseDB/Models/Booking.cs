using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace EventEaseDB.Models
{
    [Table("Booking")]
    public class Booking
    {
        public int BookingID { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Customer Email")]
        [EmailAddress]
        [StringLength(100)]
        public string CustomerEmail { get; set; }

        [Required]
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; }

        //foreign keys
        [ForeignKey("Event")]
        public int EventID { get; set; }

        [ForeignKey("Venue")]
        public int? VenueID { get; set; }
        [NotMapped]
        public string EventName => Event?.EventName;

        // Add navigation properties
        public virtual Event Event { get; set; }
        public virtual Venue Venue { get; set; }
        // Add EventName property for display
        //public string EventName { get; set; }
    }
}