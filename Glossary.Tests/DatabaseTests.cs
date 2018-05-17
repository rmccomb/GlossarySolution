using System;
using System.Diagnostics;
using System.Linq;
using Glossary.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Glossary.Tests
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public void ListDefinitions()
        {
            using (var ctx = new GlossaryEntities())
            {
                var defs = (from d in ctx.Definitions select d).ToList();
                defs.ForEach(d => Debug.WriteLine($"{d.DefinitionId} {d.Term} {d.TermDefinition}"));
                Assert.IsTrue(defs.Count >= 3);
            }
        }

        [TestInitialize]
        public void GivenInitialDataset()
        {
            using (var ctx = new GlossaryEntities())
            {
                if (ctx.Definitions.Any())
                {
                    var defs = from d in ctx.Definitions select d;
                    ctx.Definitions.RemoveRange(defs);
                }

                ctx.Definitions.AddRange(
                    new Definition[]
                    {
                        new Definition { Term = "abyssal plain", TermDefinition = "The ocean floor off the continental margin, usually very flat with a slight slope." },
                        new Definition { Term="accrete", TermDefinition="To add terranes (small land masses or pieces of crust) to another, usually larger, land mass." },
                        new Definition { Term="alkaline", TermDefinition = "Term pertaining to highly basic, as opposed to acidic, substance" }
                    });
                ctx.SaveChanges();
            }
        }
    }
}
