using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EventEaseDB.Models;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Web.UI.WebControls;

namespace EventEaseDB.Controllers
{
    public class VenueController : Controller
    {
        private EventEaseDBConsole db = new EventEaseDBConsole();

        // GET: Venue
        public ActionResult Index()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(db.Venues.ToList());
            //return View(db.Venues.ToList());
        }

        // GET: Venue/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Venue venue = db.Venues.Find(id);
            if (venue == null)
            {
                return HttpNotFound();
            }

            return View(venue);
        }

        // GET: Venue/Create
        public ActionResult Create()
        {//this was added last to allow venue list to pass to the view
            ViewBag.VenueList = new SelectList(db.Venues, "VenueID", "VenueName");
            ViewBag.EventTypeID = new SelectList(db.EventTypes, "EventTypeID", "EventTypeName"); // <-- Pass event types here
            return View();
        }

        // POST: Venue/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VenueID,VenueName,Location,Capacity,ImageURL")] Venue venue, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Upload image if provided
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // string blobUrl = UploadImageToBlob(ImageFile); // This is your existing sync method
                    venue.ImageURL = UploadImageToBlob(ImageFile);

                }

                db.Venues.Add(venue);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Venue created successfully.";
                return RedirectToAction("Index");
            }

            return View(venue);
        }

        // GET: Venue/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Venue venue = db.Venues.Find(id);
            if (venue == null)
            {
                return HttpNotFound();
            }
            return View(venue);
        }

        // POST: Venue/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Edit(Venue venue, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Upload new image and set ImageURL
                    venue.ImageURL = UploadImageToBlob(ImageFile);
                }
                else
                {
                    // Retain the old image if no new image was uploaded
                    db.Entry(venue).Property(x => x.ImageURL).IsModified = false;
                }

                db.Entry(venue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(venue);
        }
        
        // GET: Venue/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Venue venue = db.Venues.Find(id);
            if (venue == null)
            {
                return HttpNotFound();
            }

            return View(venue);
        }

        // POST: Venue/Delete/5
        /*
        [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
        Venue venue = db.Venues.Find(id);
        if (venue == null)
        {
            return HttpNotFound();
        }

        db.Venues.Remove(venue);
        db.SaveChanges();
        return RedirectToAction("Index");
    }
        */
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var venue = db.Venues.Find(id);
            bool hasBookings = db.Booking.Any(b => b.VenueID == id);

            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this venue because it is associated with existing bookings.";
                return RedirectToAction("Index");
            }

            db.Venues.Remove(venue);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Venue deleted successfully.";
            return RedirectToAction("Index");
        }


        // Uploads an image to Azure Blob Storage and returns the Blob URL
        private string UploadImageToBlob(HttpPostedFileBase imageFile)
        {
            // Replace with your actual Azure Blob Storage settings
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=practiceblobs;AccountKey=e/FhSr5+OJktOFzOv/Ad02pDebByOZ6sTl9N9lPaTGVEeMlxem83HpKCXZmUss/cbRKNuVmqjq9B+AStTexqkQ==;EndpointSuffix=core.windows.net";
            var containerName = "cldv6211container";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.InputStream)
            {
                blobClient.Upload(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
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