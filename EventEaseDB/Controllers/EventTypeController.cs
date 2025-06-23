using EventEaseDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventEaseDB.Controllers
{
    public class EventTypeController : Controller
    {
        private EventEaseDBConsole db = new EventEaseDBConsole();

        // GET: EventType
        public ActionResult Index()
        {
            return View(db.EventTypes.ToList());
        }
        // GET: EventType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EventType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventTypeID,TypeName")] EventType eventType)
        {
            if (ModelState.IsValid)
            {
                db.EventTypes.Add(eventType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eventType);
        }

        // Add Edit, Details, Delete actions similarly...
    }
}
