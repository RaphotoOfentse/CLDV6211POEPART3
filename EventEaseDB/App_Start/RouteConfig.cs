using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EventEaseDB
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            /*
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Venue", action = "Index", id = UrlParameter.Optional }
            );
            */
            routes.MapRoute(
    name: "VenueDelete",
    url: "Venue/Delete/{id}",
    defaults: new { controller = "Venue", action = "Delete", id = UrlParameter.Optional }
);
            /*
            // Default route, handles all controllers and actions
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Venue", action = "Index", id = UrlParameter.Optional }
            );
            */
            routes.MapRoute(
  name: "Default",
  url: "{controller}/{action}/{id}",
  defaults: new { controller = "Event", action = "Index", id = UrlParameter.Optional }
);



        }
    }
}
