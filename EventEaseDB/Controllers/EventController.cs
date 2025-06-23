using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventEaseDB.Models;
using System.Data.Entity;
using System.Threading.Tasks;


namespace EventEaseDB.Controllers
{
    public class EventController : Controller
    {
        private EventEaseDBConsole db = new EventEaseDBConsole();

        // GET: Event
        /*
        public ActionResult Index()
        {
//return View(db.Events.ToList());
            var events = db.Events.Include(e => e.Venue).ToList();
            return View(events);

        }*/
        public ActionResult Index(string searchType, int? venueId, DateTime? startDate, DateTime? endDate)
        {
            var events = db.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchType))
            {
                events = events.Where(e => e.EventType.Name == searchType);
            }

            if (venueId.HasValue)
            {
                events = events.Where(e => e.VenueID == venueId);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                events = events.Where(e => e.Date >= startDate && e.Date <= endDate);
            }

            // Provide dropdown data for filtering
            ViewData["EventTypes"] = db.EventTypes.ToList();
            ViewData["Venues"] = db.Venues.ToList();

            return View(events.ToList());
        }


        // GET: Event/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Event/Create
        public ActionResult Create()
        {
            ViewBag.VenueID = new SelectList(db.Venues, "VenueID", "VenueName");
            ViewBag.EventTypeID = new SelectList(db.EventTypes, "EventTypeID", "Name"); // or "Name"
            return View();
        }

        // POST: Event/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,EventName,VenueID,Date,Description,ImageURL")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@event);
        }
        // GET: Event/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            ViewBag.VenueID = new SelectList(db.Venues, "VenueID", "VenueName", @event.VenueID);
            ViewBag.EventTypeID = new SelectList(db.EventTypes, "EventTypeID", "EventTypeName", @event.EventTypeID);

            /*
            // Load venue dropdown
            ViewBag.Venues = new SelectList(db.Venues.ToList(), "VenueID", "VenueName", @event.VenueID);

            // Load event type dropdown
            ViewBag.EventTypes = new SelectList(db.EventTypes.ToList(), "EventTypeID", "TypeName", @event.EventTypeID);
            */
            return View(@event);
        }
     
        /*
        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@event);
        }*/

        // POST: Event/Edit/5
[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,EventName,VenueID,Date,Description,ImageURL,EventTypeID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Event updated successfully.";
                return RedirectToAction("Index");
            }

            // If model state is invalid, reload dropdowns
            ViewBag.Venues = new SelectList(db.Venues.ToList(), "VenueID", "VenueName", @event.VenueID);
            ViewBag.EventTypes = new SelectList(db.EventTypes.ToList(), "EventTypeID", "TypeName", @event.EventTypeID);

            return View(@event);
        }


        // GET: Event/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }
        /*
        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Find the event by id
            Event eventToDelete = db.Events.Find(id);

            if (eventToDelete == null)
            {
                return HttpNotFound();
            }

            // Check if there are any bookings for the event
            var isBooked = db.Booking.Any(b => b.EventID == id);

            if (isBooked)
            {
                TempData["ErrorMessage"] = "Cannot delete event because it has existing bookings.";
                return RedirectToAction("Index");
            }

            // Remove the event from the database
            db.Events.Remove(eventToDelete);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Event deleted successfully.";
            return RedirectToAction("Index");
        }
        */

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var ev = db.Events.Find(id);
            bool hasBookings = db.Booking.Any(b => b.EventID == id);

            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this event because it is associated with existing bookings.";
                return RedirectToAction("Index");
            }

            db.Events.Remove(ev);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Event deleted successfully.";
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
