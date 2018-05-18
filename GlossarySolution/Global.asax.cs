using Glossary.Data;
using GlossarySolution.Models;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Linq;

namespace GlossarySolution
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

#if DEBUG
            ReadInitialDefs();
#endif
        }

        private void ReadInitialDefs()
        {
            using (var db = new GlossaryEntities())
            {
                foreach (var def in GivenTerms())
                {
                    var existing = (from d in db.Definitions where d.Term == def.Term select d).FirstOrDefault();
                    if (existing == null)
                    {
                        db.Definitions.Add(new Definition { Term = def.Term, TermDefinition = def.TermDefinition });
                    }
                    else
                    {
                        existing.TermDefinition = def.TermDefinition;
                    }
                }

                db.SaveChanges();
            }
        }

        private DefinitionModel[] GivenTerms()
        {
            return new DefinitionModel[]
            {
                new DefinitionModel { Term = "abyssal plain", TermDefinition = "The ocean floor off the continental margin, usually very flat with a slight slope." },
                new DefinitionModel { Term="accrete", TermDefinition="To add terranes (small land masses or pieces of crust) to another, usually larger, land mass." },
                new DefinitionModel { Term="alkaline", TermDefinition = "Term pertaining to highly basic, as opposed to acidic, substance" }
            };
        }
    }
}
