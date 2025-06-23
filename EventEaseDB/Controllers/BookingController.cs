using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
        .Include(b => b.Venue)
        .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
              bookings = bookings.Where(b =>
    b.Event.EventName.Contains(searchString) ||
    b.Venue.VenueName.Contains(searchString));
            }

            return View(bookings.ToList());
        }

        /*
        public ActionResult Index()
        {
            var bookings = db.Set<Booking>()EventName'.
        .Include(b => b.Event)   // Ensure Event is included
        .Include(b => b.Venue)   // Optional
        .ToList();               // No projection, keep full Booking model

            return View(bookings);
        }
        public  async Task <ActionResult> Index(string searchString)
        {
            var bookings = _content.Booking
        .Include(b => b.Event)   // Ensure Event is included
        .Include(b => b.Venue)   // Optional
        .AsQueryable();               // No projection, keep full Booking model
            if(!string = bookings.Where(b=>))
            b.Venue.VenueName.Contains(searchString) ||
            b.Venue.EventName.Contains(searchString));

        }
        return View(ConfiguredTaskAwaitable bookings.TolIstAsync());
        }*/

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


        // GET: Booking/Edit/5
        public ActionResult Edit(int? id)
        {
            var booking = db.Booking.Find(id);

            if (booking == null)
            {
                return HttpNotFound();
            }

            ViewBag.Events = db.Events.ToList();  // Fetch events
            ViewBag.Venues = db.Venues.ToList();  // Fetch venues

            return View(booking);  // Pass the booking to the view
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
            {
                return HttpNotFound();
            }
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
