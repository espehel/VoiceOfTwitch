﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace VoiceOfTwitch
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "StatementsApi",
                routeTemplate: "api/{controller}/{ordering}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}