using Glossary.Data;
using GlossarySolution.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
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
            try
            {
                // Random 'term of the day'
                DefinitionModel model = null;
                var defs = (from d in db.Definitions select d).ToArray();
                if (defs.Length > 0)
                {
                    var r = new Random().Next(defs.Length);
                    model = new DefinitionModel { Term = defs[r].Term, TermDefinition = defs[r].TermDefinition };
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToActionPermanent("Index", "Error");
            }
        }


        // GET: List
        /// <summary>
        /// Main list of terms
        /// </summary>
        /// <param name="part">Alphabetical part</param>
        public async Task<ActionResult> List(string part)
        {
            Debug.WriteLine(part);
            try
            {
                Debug.WriteLine(part);

                if (part == null)
                {
                    //var defs = await db.Definitions.ToListAsync();

                    var defs = from d in db.Definitions orderby d.Term select d;

                    var model = new List<DefinitionModel>(defs.Select(d => new
                        DefinitionModel
                    { DefinitionId = d.DefinitionId, Term = d.Term, TermDefinition = d.TermDefinition }));
                    return View(model);
                }

                var chars = part.Distinct();
                var sb = new StringBuilder();

                foreach (var c in chars)
                    sb.Append(c);

                var spart = sb.ToString();
                int counter = 0;
                var list = new List<DefinitionModel>();
                spart.ToUpper().ToCharArray().ToList().ForEach(c =>
                {
                    Debug.WriteLine(c);
                    if (counter > 64) return;
                    var s = c.ToString();

                    var defs = (from d in db.Definitions
                                where d.Term.StartsWith(s)
                                orderby d.Term
                                select new DefinitionModel
                                {
                                    DefinitionId = d.DefinitionId,
                                    Term = d.Term,
                                    TermDefinition = d.TermDefinition
                                }).ToList();

                    list.AddRange(defs);
                    counter++;
                });

                ViewData["ListPart"] = spart;
                if (spart != null)
                {
                    ViewData["ListPartDesc"] = $"({spart[0]} - {spart.Last()})".ToUpper();
                }

                return View(list);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToActionPermanent("Index", "Error");
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
                return RedirectToActionPermanent("Index", "Error");
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
                return RedirectToActionPermanent("Index", "Error");
            }
        }

        // POST: Home/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Create([Bind(Include = "DefinitionId,Term,TermDefinition")] DefinitionModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var def = (from d in db.Definitions where d.Term == model.Term select d).FirstOrDefault();

                    if (def != null)
                    {
                        //def.TermDefinition = model.TermDefinition;
                        ModelState.AddModelError("Term", "A definition already exists for this Term");
                        return View(model);
                    }
                    else
                    {
                        var definition = new Definition { Term = model.Term, TermDefinition = model.TermDefinition };
                        db.Definitions.Add(definition);
                    }
                    await db.SaveChangesAsync();
                    return RedirectToAction("List");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToActionPermanent("Index", "Error");
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
                return RedirectToActionPermanent("Index", "Error");
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
        [ValidateInput(false)]
        public async Task<ActionResult> Edit([Bind(Include = "DefinitionId,Term,TermDefinition")] DefinitionModel definition)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var def = (from d in db.Definitions where d.Term == definition.Term select d).FirstOrDefault();
                    if (def != null)
                    {
                        if (def.DefinitionId != definition.DefinitionId)
                        {
                            // Validation error - there is an existing definition for that term
                            ModelState.AddModelError("Term", "A definition already exists for this Term");
                            return View(definition);
                        }
                        else
                        {
                            def.Term = definition.Term;
                            def.TermDefinition = definition.TermDefinition;
                            db.Entry(def).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            return RedirectToAction("List");
                        }
                    }
                    else
                    {
                        // Re-defining current term (Term does not conflict with existing)
                        var rename = (from d in db.Definitions where d.DefinitionId == definition.DefinitionId select d).FirstOrDefault();
                        rename.Term = definition.Term;
                        rename.TermDefinition = definition.TermDefinition;
                        db.Entry(rename).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return RedirectToAction("List");
                    }
                }
                return View(definition);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToActionPermanent("Index", "Error");
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
                return RedirectToActionPermanent("Index", "Error");
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
                return RedirectToAction("List");
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

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            Debug.WriteLine(filterContext.Exception.GetType());
            Debug.WriteLine(filterContext.Exception.Message);

            filterContext.Result = RedirectToAction("Index", "Error");
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
