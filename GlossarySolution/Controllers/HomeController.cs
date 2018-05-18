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
            var model = new IndexModel();
            var defs = (from d in db.Definitions select d).ToArray();
            if (defs.Length > 0)
            {
                var r = new Random().Next(defs.Length);
                model.TermOTD = new DefinitionModel { Term = defs[r].Term, TermDefinition = defs[r].TermDefinition };

            }

            return View(model);
        }

        /// <summary>
        /// Show the part list of terms
        /// </summary>
        /// <param name="part">Alphabetical part</param>
        public ActionResult Contents(string part)
        {
            try
            {
                if (part == null)
                {
                    return RedirectToAction("List");
                }

                var list = new List<DefinitionModel>();
                string partDesc = "";
                part.ToUpper().ToCharArray().ToList().ForEach(c =>
                {
                    var s = c.ToString();
                    partDesc = partDesc + s + ", ";

                    var defs = (from d in db.Definitions
                                where d.Term.StartsWith(s)
                                select new DefinitionModel
                                {
                                    DefinitionId = d.DefinitionId,
                                    Term = d.Term,
                                    TermDefinition = d.TermDefinition
                                }).ToList();

                    list.AddRange(defs);
                });

                ViewData["ListPart"] = part;
                if (part != null)
                {
                    ViewData["ListPartDesc"] = $"({part[0]} - {part.Last()})".ToUpper();
                }

                return View("List", list);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToActionPermanent("Error");
            }
        }

        // GET: List
        /// <summary>
        /// Main list of terms
        /// </summary>
        public async Task<ActionResult> List()
        {
            try
            {
                var defs = await db.Definitions.ToListAsync();
                var model = new List<Models.DefinitionModel>(defs.Select(d => new
                    Models.DefinitionModel
                { DefinitionId = d.DefinitionId, Term = d.Term, TermDefinition = d.TermDefinition }));
                return View(model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToActionPermanent("Error");
            }
        }

        // GET: Home/Details/5
        /// <summary>
        /// Item details view
        /// </summary>
        public async Task<ActionResult> Details(int? id)
        {
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToActionPermanent("Error");
            }
        }

        // GET: Home/Create
        /// <summary>
        /// Create a new Term
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToActionPermanent("Error");
            }
        }

        // POST: Home/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "DefinitionId,Term,TermDefinition")] Definition definition)
        public async Task<ActionResult> Create([Bind(Include = "DefinitionId,Term,TermDefinition")] Models.DefinitionModel model)
        {
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToActionPermanent("Error");
            }
        }

        // GET: Home/Edit/5
        /// <summary>
        /// Edit a given Term by ID
        /// </summary>
        public async Task<ActionResult> Edit(int? id)
        {
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToActionPermanent("Error");
            }
        }

        // POST: Home/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Edit a given bound Term 
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DefinitionId,Term,TermDefinition")] Definition definition)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(definition).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(definition);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToActionPermanent("Error");
            }
        }

        // GET: Home/Delete/5
        /// <summary>
        /// Delete a Term by ID
        /// </summary>
        public async Task<ActionResult> Delete(int? id)
        {
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToActionPermanent("Error");
            }
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Definition definition = await db.Definitions.FindAsync(id);
                db.Definitions.Remove(definition);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToActionPermanent("Error");
            }
        }

        public ActionResult Error()
        {
            try
            {
                return View("Error");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
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
