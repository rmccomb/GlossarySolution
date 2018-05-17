using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Glossary.Data;
using Newtonsoft.Json.Linq;

namespace GlossarySolution.Controllers
{
    /// <summary>
    /// Controller for accessing definitions
    /// </summary>
    public class DefinitionsController : ApiController
    {
        private GlossaryEntities db = new GlossaryEntities();

        // GET: api/Definitions
        /// <summary>
        /// Get all the definitions as JSON
        /// </summary>
        /// <returns> JSON array of { Term: "A", TermDefinition: "B" } </returns>
        [HttpGet]
        public IHttpActionResult GetDefinitions()
        {
            try
            {
                return Json(db.Definitions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Definitions
        /// <summary>
        /// Post a JSON array of Definitions
        /// </summary>
        /// <param name="json"> Array of { Term: "A", TermDefinition: "B" } </param>
        /// <returns> Count of rows added to database as JSON </returns>
        [HttpPost]
        public IHttpActionResult PostDefinitions([FromBody] object json)
        {
            Debug.WriteLine(json);
            var definitions = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Models.DefinitionModel[]>(json.ToString());

            try
            {
                foreach (var definition in definitions)
                {
                    // Validate
                    if (definition.Term.Length >= 2 && definition.Term.Length <= 80
                        && definition.TermDefinition.Length >= 10)
                    {
                        db.Definitions.Add(new Definition { Term = definition.Term, TermDefinition = definition.TermDefinition });
                    }
                    else
                    {
                        return BadRequest($"Validation error in definition: {definition.Term}");
                    }
                }

                var count = db.SaveChanges(); // commit

                return Json(new { RecordCount = count });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get the definition of a given term
        /// </summary>
        /// <param name="term">The term to lookup</param>
        /// <returns>The definition of the term</returns>
        // GET: api/Definitions/{term}
        [ResponseType(typeof(Definition))]
        public IHttpActionResult GetDefinition(string term)
        {
            var definition = (from d in db.Definitions where d.Term == term select d).FirstOrDefault();
            if (definition == null)
            {
                return NotFound();
            }

            return Ok(definition);
        }

        /// <summary>
        /// Put an updated Definition
        /// </summary>
        // PUT: api/Definitions/5
        //[ResponseType(typeof(void))]
        public IHttpActionResult PutDefinition(int id, Definition definition)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != definition.DefinitionId)
            {
                return BadRequest();
                //return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(definition).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DefinitionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);

        }

        // POST: api/Definitions
        //[ResponseType(typeof(Definition))]
        //public IHttpActionResult PostDefinition(Definition definition)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Definitions.Add(definition);
        //    db.SaveChanges();

        //    return CreatedAtRoute("DefaultApi", new { id = definition.DefinitionId }, definition);
        //}

        // DELETE: api/Definitions/5
        //[ResponseType(typeof(Definition))]
        //public IHttpActionResult DeleteDefinition(int id)
        //{
        //    Definition definition = db.Definitions.Find(id);
        //    if (definition == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Definitions.Remove(definition);
        //    db.SaveChanges();

        //    return Ok(definition);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DefinitionExists(int id)
        {
            return db.Definitions.Count(e => e.DefinitionId == id) > 0;
        }
    }
}