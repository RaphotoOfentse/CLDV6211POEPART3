using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EventEaseDB.Models
{


    [Table("EventType")]
    public class EventType
	{
        public int EventTypeID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }


        // Navigation property for related Events
        public virtual ICollection<Event> Events { get; set; }
    }
}