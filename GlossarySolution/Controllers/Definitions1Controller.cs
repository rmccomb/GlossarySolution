using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Glossary.Data;
using System.Diagnostics;

namespace GlossarySolution.Controllers
{
    public class Definitions1Controller : Controller
    {
        private GlossaryEntities db = new GlossaryEntities();

        // GET: Definitions1
        public async Task<ActionResult> Index()
        {
            var defs = await db.Definitions.ToListAsync();
            var model = new List<Models.DefinitionModel>(defs.Select(d => new
                Models.DefinitionModel
            { DefinitionId = d.DefinitionId, Term = d.Term, TermDefinition = d.TermDefinition }));
            //return View(await db.Definitions.ToListAsync());
            return View(model);
        }

        // GET: Definitions1/Details/5
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

        // GET: Definitions1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Definitions1/Create
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

        // GET: Definitions1/Edit/5
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

        // POST: Definitions1/Edit/5
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

        // GET: Definitions1/Delete/5
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

        // POST: Definitions1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Definition definition = await db.Definitions.FindAsync(id);
            db.Definitions.Remove(definition);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
