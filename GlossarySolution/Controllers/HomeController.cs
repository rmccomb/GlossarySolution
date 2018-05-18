using Glossary.Data;
using GlossarySolution.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GlossarySolution.Controllers
{
    /// <summary>
    /// The main controller
    /// </summary>
    public class HomeController : Controller
    {
        private GlossaryEntities db = new GlossaryEntities();

        /// <summary>
        /// Home page
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            // TODO nicer index and random term of the day

            return View();
        }

        // GET: List
        public async Task<ActionResult> List()
        {
            var defs = await db.Definitions.ToListAsync();
            var model = new List<Models.DefinitionModel>(defs.Select(d => new
                Models.DefinitionModel
            { DefinitionId = d.DefinitionId, Term = d.Term, TermDefinition = d.TermDefinition }));
            //return View(await db.Definitions.ToListAsync());
            return View(model);
        }

        // GET: Home/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Definition definition = await db.Definitions.FindAsync(id);
            if (definition == null)
            {
                return HttpNotFound();
            }
            var model = new Models.DefinitionModel { DefinitionId = definition.DefinitionId, Term = definition.Term, TermDefinition = definition.TermDefinition };
            return View(model);
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "DefinitionId,Term,TermDefinition")] Definition definition)
        public async Task<ActionResult> Create([Bind(Include = "DefinitionId,Term,TermDefinition")] Models.DefinitionModel model)
        {
            if (ModelState.IsValid)
            {
                var definition = new Definition { Term = model.Term, TermDefinition = model.TermDefinition };
                db.Definitions.Add(definition);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Home/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Definition definition = await db.Definitions.FindAsync(id);
            if (definition == null)
            {
                return HttpNotFound();
            }
            return View(new Models.DefinitionModel
            {
                DefinitionId = definition.DefinitionId,
                Term = definition.Term,
                TermDefinition = definition.TermDefinition
            });
        }

        // POST: Home/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DefinitionId,Term,TermDefinition")] Definition definition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(definition).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(definition);
        }

        // GET: Home/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Definition definition = await db.Definitions.FindAsync(id);
            if (definition == null)
            {
                return HttpNotFound();
            }
            return View(new Models.DefinitionModel { DefinitionId = definition.DefinitionId, Term = definition.Term, TermDefinition = definition.TermDefinition });
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Definition definition = await db.Definitions.FindAsync(id);
            db.Definitions.Remove(definition);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Display the API help
        /// </summary>
        public ActionResult ApiHelp()
        {
            return View();
        }

    }
}
