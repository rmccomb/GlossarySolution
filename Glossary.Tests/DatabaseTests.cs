using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Glossary.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Glossary.Tests
{
    [TestClass]
    public class DatabaseTests
    {
        [ClassInitialize()]
        public static void SetDataDirectory(TestContext testContext)
        {
            // Set the database path to the development data folder
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            int index = baseDir.IndexOf("Glossary.Tests");
            var dataDir = Path.Combine(baseDir.Substring(0, index), "Glossary.Data");
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDir);
        }

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
