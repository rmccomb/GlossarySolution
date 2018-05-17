using Glossary.Data;
using GlossarySolution.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlossarySolution.Controllers
{
    public class HomeController : Controller
    {
        private GlossaryEntities db = new GlossaryEntities();

        /// <summary>
        /// Home page
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        /// <summary>
        /// Get all the words in a view
        /// </summary>
        public ActionResult AllWords()
        {
            var model = (from d in db.Definitions select d).ToList();
            Debug.WriteLine($"wordcount: {model.Count}");
            return View(model);
        }

        /// <summary>
        /// Page for entering new definition
        /// </summary>
        [HttpGet]
        public ActionResult Edit()
        {
            ViewData["message"] = "Enter a new term";
            ViewData["action"] = "Create";
            return View();
        }
        [HttpPost]
        public ActionResult Edit(string term, string def)
        {
            ViewData["message"] = "Enter a new term";

            return View();
        }

        [HttpPost]
        public ActionResult Create(DefinitionModel model)
        {
            if (ModelState.IsValid)
            {
                db.Definitions.Add(new Definition { Term = model.Term, TermDefinition = model.TermDefinition });
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //var definition = new DefinitionModel { Term = term, TermDefinition = termDefinition };
            return View("Edit", model);
        }

    }
}
