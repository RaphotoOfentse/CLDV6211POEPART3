using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EventEaseDB.Models;


namespace EventEaseDB.Controllers
{
    public class BookingController : Controller
    {
        private EventEaseDBConsole db = new EventEaseDBConsole();

        // GET: Event
        public ActionResult Index(string searchString)
        {
            var bookings = db.Booking
                .Include(b => b.Event)
                .Include(b => b.Event.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
              bookings = bookings.Where(b =>
            b.Event.EventName.Contains(searchString) ||
            b.Event.Venue.VenueName.Contains(searchString));
            }

            return View(bookings.ToList());
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking @booking = db.Booking.Find(id);
            if (@booking == null)
            {
                return HttpNotFound();
            }
            return View(@booking);
        }

        // GET: Booking/Create
        public ActionResult Create()
        {
            // Populate EventID and EventName for the dropdown list
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventName");

            // Populate the venue list as well
            ViewBag.VenueID = new SelectList(db.Venues, "VenueID", "VenueName");

            return View();
        }
        /*
        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        
public ActionResult Create([Bind(Include = "BookingID,BookingDate,CustomerName,CustomerEmail,NumberOfGuests")] Booking booking)
{
    // Populate the dropdown lists again in case the model is invalid and the form is returned
    ViewBag.EventID = new SelectList(db.Events, "EventID", "EventName", booking.EventID);
    ViewBag.VenueID = new SelectList(db.Venues, "VenueID", "VenueName", booking.VenueID);

    if (ModelState.IsValid)
    {
         // 🔐 STEP 1: Prevent double bookings
         var isDuplicate = db.Booking.Any(b =>
         b.BookingDate.Date == booking.BookingDate.Date &&
         b.VenueID == booking.VenueID);

                if (isDuplicate)
                {
                    ModelState.AddModelError("", "A booking already exists for this venue on the selected date.");
                    return View(booking);
                }

                try
                {
            // Add the booking to the database
            db.Booking.Add(booking);
            db.SaveChanges();

            // Redirect to Index after successful save
            TempData["SuccessMessage"] = "Booking created successfully.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Log the error (you can log this to a file or the database for production)
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");

            // Show a generic error message
            ModelState.AddModelError("", "An error occurred while saving the booking.");
        }
    }
        
    // Return the view with any errors in the model state
    return View(booking);
}
        */
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingID,CustomerName,EventID,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                var selectedEvent = db.Events.Find(booking.EventID);
                if (selectedEvent == null)
                {
                    ModelState.AddModelError("", "Selected event does not exist.");
                    ViewBag.EventID = new SelectList(db.Events, "EventID", "EventName", booking.EventID);
                    ViewBag.VenueID = new SelectList(db.Venues, "VenueID", "VenueName"); // Add this
                    return View(booking);
                }

                // Prevent double booking: same venue, date, time
                bool isDoubleBooked = db.Booking.Any(b =>
                    b.Event.VenueID == selectedEvent.VenueID &&
                    DbFunctions.TruncateTime(b.Event.Date) == DbFunctions.TruncateTime(selectedEvent.Date) &&
                    b.Event.Date.TimeOfDay == selectedEvent.Date.TimeOfDay
                );

                if (isDoubleBooked)
                {
                    ModelState.AddModelError("", "This venue is already booked for the selected date and time.");
                    ViewBag.EventID = new SelectList(db.Events, "EventID", "EventName", booking.EventID);
                    ViewBag.VenueID = new SelectList(db.Venues, "VenueID", "VenueName"); // Add this
                    return View(booking);
                }

                db.Booking.Add(booking);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Booking successfully added.";
                return RedirectToAction("Index");
            }

            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventName", booking.EventID);
            ViewBag.VenueID = new SelectList(db.Venues, "VenueID", "VenueName"); // Add this
            return View(booking);
        }*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingID,CustomerName,CustomerEmail,NumberOfGuests,EventID,VenueID,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Round down seconds and milliseconds from booking.BookingDate
                var requestedDate = new DateTime(
                    booking.BookingDate.Year,
                    booking.BookingDate.Month,
                    booking.BookingDate.Day,
                    booking.BookingDate.Hour,
                    booking.BookingDate.Minute,
                    0
                );

                bool isDoubleBooked = db.Booking.Any(b =>
                    b.VenueID == booking.VenueID &&
                    DbFunctions.TruncateTime(b.BookingDate) == requestedDate.Date &&
                    b.BookingDate.Hour == requestedDate.Hour &&
                    b.BookingDate.Minute == requestedDate.Minute
                );

                if (isDoubleBooked)
                {
                    ModelState.AddModelError("", "This venue is already booked at the selected date and time.");
                }
                else
                {
                    booking.BookingDate = requestedDate; // Store the rounded version
                    db.Booking.Add(booking);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Booking successfully added.";
                    return RedirectToAction("Index");
                }
            }

            // Repopulate dropdowns if validation fails
            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventName", booking.EventID);
            ViewBag.VenueID = new SelectList(db.Venues, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }



        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingID,CustomerName,CustomerEmail,NumberOfGuests,EventID,VenueID,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                var selectedEvent = db.Events.Find(booking.EventID);
                if (selectedEvent == null)
                {
                    ModelState.AddModelError("", "Selected event does not exist.");
                }
                else
                {
                    // Check if venue is already booked for the booking's date and time
                    bool isDoubleBooked = db.Booking
                        .Include(b => b.Event)
                        .Any(b =>
                            b.Event.VenueID == selectedEvent.VenueID &&
                            DbFunctions.TruncateTime(b.BookingDate) == DbFunctions.TruncateTime(booking.BookingDate) &&
                            b.BookingDate.Hour == booking.BookingDate.Hour &&
                            b.BookingDate.Minute == booking.BookingDate.Minute
                        );

                    if (isDoubleBooked)
                    {
                        ModelState.AddModelError("", "This venue is already booked for the selected date and time.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.EventID = new SelectList(db.Events, "EventID", "EventName", booking.EventID);
                    ViewBag.VenueID = new SelectList(db.Venues, "VenueID", "VenueName", booking.VenueID);
                    return View(booking);
                }

                db.Booking.Add(booking);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Booking successfully added.";
                return RedirectToAction("Index");
            }

            ViewBag.EventID = new SelectList(db.Events, "EventID", "EventName", booking.EventID);
            ViewBag.VenueID = new SelectList(db.Venues, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }*/

        // GET: Booking/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var booking = db.Booking.Find(id);

            if (booking == null)
            {
                return HttpNotFound();
            }

            // ✅ Proper SelectList with selected value
            ViewBag.Events = new SelectList(db.Events.ToList(), "EventID", "EventName", booking.EventID);
            ViewBag.Venues = new SelectList(db.Venues.ToList(), "VenueID", "VenueName", booking.VenueID);

            return View(booking);
        }


        // POST: Booking/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingID,BookingDate,CustomerName,CustomerEmail,EventName,NumberOfGuests, EventID, VenueID")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Booking successfully updated.";
                return RedirectToAction("Index");
            }
            return View(booking);
        }

        // GET: Booking/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Booking.Find(id);
            if (booking == null)
                return HttpNotFound();
 
            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Booking.Find(id);
            db.Booking.Remove(booking);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Booking successfully deleted.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
