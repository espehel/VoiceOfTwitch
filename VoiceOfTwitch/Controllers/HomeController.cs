using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VoiceOfTwitch.Models;
using VoiceOfTwitch.Tools;

namespace VoiceOfTwitch.Controllers
{
    public class HomeController : Controller
    {
        private readonly VoiceDatabaseEntities _voiceDatabaseEntities = new VoiceDatabaseEntities();
        public ActionResult Index()
        {
            ViewBag.Title = "Voice of Twitch";
            ViewBag.Message = "Experience the common voice of Twitch chat!";
            List<Channel> list = _voiceDatabaseEntities.Channels.ToList();
            return View(list);
        }

        public ActionResult Channel(string channel)
        {
            return RedirectToAction("Livedata", "Statements",channel);
        }
        public PartialViewResult Details(int id)
        {
            Statement model = _voiceDatabaseEntities.Statements.Find(id);
            return PartialView(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
