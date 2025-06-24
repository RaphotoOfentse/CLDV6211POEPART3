using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using EventEaseDB.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data.Entity.SqlServer;


namespace EventEaseDB.Controllers
{
    public class EventController : Controller
    {
        private readonly BlobStorageHelper _blobHelper;

        private EventEaseDBConsole db = new EventEaseDBConsole();
        public EventController()
        {
            var connString = ConfigurationManager.AppSettings["AzureBlobStorageConnectionString"];
            _blobHelper = new BlobStorageHelper(connString);
        }
        public ActionResult Index(string searchType, int? venueId, DateTime? startDate, DateTime? endDate)
        {
            var events = db.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchType))
            {
                events = events.Where(e => e.EventType.Name.Equals(searchType, StringComparison.OrdinalIgnoreCase));
            }

            if (venueId.HasValue)
            {
                events = events.Where(e => e.VenueID == venueId);
            }
            if (startDate.HasValue)
            {
                var start = startDate.Value.Date;
                events = events.Where(e => DbFunctions.TruncateTime(e.Date) >= start);
            }

            if (endDate.HasValue)
            {
                var end = endDate.Value.Date;
                events = events.Where(e => DbFunctions.TruncateTime(e.Date) <= end);
            }

            ViewBag.EventTypes = new SelectList(db.EventTypes, "Name", "Name", searchType);
            ViewBag.Venues = new SelectList(db.Venues, "VenueID", "VenueName", venueId);

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
            ViewBag.VenueID = new SelectList(db.Venues.ToList(), "VenueID", "VenueName");
            ViewBag.EventTypeID = new SelectList(db.EventTypes.ToList(), "EventTypeID", "Name"); // or "Name"
            return View();
        }

        // POST: Event/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,EventName,VenueID,Date,Description,EventTypeID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Event created successfully.";
                return RedirectToAction("Index");
            }

            ViewBag.VenueID = new SelectList(db.Venues, "VenueID", "VenueName", @event.VenueID);
            ViewBag.EventTypeID = new SelectList(db.EventTypes, "EventTypeID", "Name", @event.EventTypeID);
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
            ViewBag.EventTypeID = new SelectList(db.EventTypes, "EventTypeID", "Name", @event.EventTypeID);

            return View(@event);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,EventName,VenueID,Date,Description,EventTypeID")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Event updated successfully.";
                return RedirectToAction("Index");
            }

            ViewBag.VenueID = new SelectList(db.Venues, "VenueID", "VenueName", @event.VenueID);
            ViewBag.EventTypeID = new SelectList(db.EventTypes, "EventTypeID", "Name", @event.EventTypeID);
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var @event = db.Events.Find(id);
            bool hasBookings = db.Booking.Any(b => b.EventID == id);

            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this event because it is associated with existing bookings.";
                return RedirectToAction("Index");
            }
            db.Events.Remove(@event);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Event successfully deleted.";
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
