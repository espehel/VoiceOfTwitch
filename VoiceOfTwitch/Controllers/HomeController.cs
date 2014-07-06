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
        //private StatementsDatabaseEntities statementDB = new StatementsDatabaseEntities();
        private StatementsDatabaseEntities1 statementDB = new StatementsDatabaseEntities1();
        public ActionResult Index()
        {
            ViewBag.Title = "Voice of Twitch";
            ViewBag.Message = "Experience the common voice of Twitch chat!";
            List<Statement> list = statementDB.Statements.ToList();
            list.Sort(StatementComparer.OrderByTop);
            return View(list);
        }
        public PartialViewResult Details(int id)
        {
            Statement model = statementDB.Statements.Find(id);
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
