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
        private readonly VoiceDatabaseEntities ef = new VoiceDatabaseEntities();
        private long _channelId = -1;
        private List<Statement> _statements;

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
        public ActionResult Livedata(string channel)
        {
            var caseSwitch = "top";
            if (TempData["ordering"] != null)
            {
                caseSwitch = (string)TempData["ordering"];
                
            }

            ViewBag.Title = "Voice of Twitch";
            ViewBag.Message = "Experience the common voice of Twitch chat live!";
            ViewBag.Channel = channel;
            var c = ef.Channels.FirstOrDefault(cn => cn.name == channel);
            if (c != null)
                _channelId = c.id;
                //_channelId = 0;
            else
            {
                _channelId = 0;
            }
            _statements = ef.Statements.Where(statement => statement.channelId == _channelId).ToList();
            caseSwitch = caseSwitch.ToLower();
            switch (caseSwitch)
            {
                case "top" :
                    _statements.Sort(StatementComparer.OrderByTop);
                    break;
                case "count" :
                    _statements.Sort(StatementComparer.OrderByCount);
                    break;
                case "hot" : 
                    _statements.Sort(StatementComparer.OrderByHot);
                    break;
                default:
                    _statements.Sort(StatementComparer.OrderByTop);
                    break;
            }
            
            return View(_statements);
        }
        public PartialViewResult Details(int id)
        {
            Statement model = _statements.Find(s => s.id == id);
            return PartialView(model);
        }

        public ActionResult Ordering(string order, string channelN)
        {
            TempData["ordering"] = order;
            return RedirectToAction("Livedata", "Statements",new {channel = channelN});
        }

    }
}
