using GlossarySolution.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlossarySolution.Controllers
{
    /// <summary>
    /// A handler for errors
    /// </summary>
    public class ErrorController : Controller
    {
        // GET: Error
        /// <summary>
        /// Default error handler
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Default handler 404 missing resource
        /// </summary>
        public ActionResult NotFound()
        {
            var model = new ErrorModel { Message = "A resource was not found" };
            return View("Index", model);
        }
    }
}