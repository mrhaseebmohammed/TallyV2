using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace boardreview
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Poll",
                url: "{shortUrl}",
                defaults: new { controller = "Home", action = "Poll" }

                );

            routes.MapRoute(
                name: "PollResults",
                url: "PollResults/{shortUrl}",
                defaults: new { controller = "Home", action = "PollResults" }

                );

            routes.MapRoute(
                name: "AdminPoll",
                url: "AdminPoll/{password}",
                defaults: new { controller = "Home", action = "AdminPoll" }

                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
