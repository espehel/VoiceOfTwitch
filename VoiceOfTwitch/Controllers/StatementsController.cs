using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VoiceOfTwitch.Models;
using VoiceOfTwitch.Tools;

namespace VoiceOfTwitch.Controllers
{
    public class StatementsController : Controller
    {
        private readonly VoiceDatabaseEntities _voiceDatabaseEntities = new VoiceDatabaseEntities();
        private long _channelId = -1;

        //
        // GET: /Statements/Index
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Statements/
        // GET: /Statements/Livedata
        // GET: /Statements/Livedata/{ordering}
        public ActionResult Livedata(string ordering)//default value top
        {
            if (TempData["channel"] != null)
            {
                _channelId = Convert.ToInt64(TempData["channel"]);
            }

            ViewBag.Title = "Voice of Twitch";
            ViewBag.Message = "Experience the common voice of Twitch chat live!";
            var list = _voiceDatabaseEntities.Statements.Where(statement => statement.channelId == _channelId).ToList();
            var caseSwitch = ordering.ToLower();
            switch (caseSwitch)
            {
                case "top" :
                    list.Sort(StatementComparer.OrderByTop);
                    break;
                case "count" :
                    list.Sort(StatementComparer.OrderByCount);
                    break;
                default:
                    list.Sort(StatementComparer.OrderByTop);
                    break;
            }
            
            return View(list);
        }
        public PartialViewResult Details(int id)
        {
            Statement model = _voiceDatabaseEntities.Statements.Find(id);
            return PartialView(model);
        }

    }
}
