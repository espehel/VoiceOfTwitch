using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace VoiceOfTwitch
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //to be used when we can present other data than live data
            /*routes.MapRoute(
                name: "Statements",
                url: "statements/{action}/{ordering}",
                defaults: new { controller = "statements", action = "index", ordering = "top" }
                );*/

            ////routes to livedata by default, ordering by top
            //routes.MapRoute(
            //    name: "Statements",
            //    url: "statements/{action}/{ordering}",
            //    defaults: new {controller = "statements", action = "livedata", ordering = "top" }
            //    );
            //routes to livedata by default, pick default channel
            routes.MapRoute(
                name: "Statements",
                url: "statements/{action}/{channel}",
                defaults: new { controller = "statements"}//, action = "livedata", channel = "default" }
                );
            //default routing
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}