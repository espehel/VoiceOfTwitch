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
        private readonly StatementsDatabaseEntities1 _statementDb = new StatementsDatabaseEntities1();

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

            ViewBag.Title = "Voice of Twitch";
            ViewBag.Message = "Experience the common voice of Twitch chat live!";
            var list = _statementDb.Statements.ToList();
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
            Statement model = _statementDb.Statements.Find(id);
            return PartialView(model);
        }

    }
}
