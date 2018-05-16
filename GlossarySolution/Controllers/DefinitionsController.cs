using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Glossary.Data;

namespace GlossarySolution.Controllers
{
    public class DefinitionsController : ApiController
    {
        private GlossaryEntities db = new GlossaryEntities();

        // GET: api/Definitions
        public IQueryable<Definition> GetDefinitions()
        {
            return db.Definitions;
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

        // PUT: api/Definitions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDefinition(int id, Definition definition)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != definition.DefinitionId)
            {
                return BadRequest();
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
        [ResponseType(typeof(Definition))]
        public IHttpActionResult PostDefinition(Definition definition)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Definitions.Add(definition);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = definition.DefinitionId }, definition);
        }

        // DELETE: api/Definitions/5
        [ResponseType(typeof(Definition))]
        public IHttpActionResult DeleteDefinition(int id)
        {
            Definition definition = db.Definitions.Find(id);
            if (definition == null)
            {
                return NotFound();
            }

            db.Definitions.Remove(definition);
            db.SaveChanges();

            return Ok(definition);
        }

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