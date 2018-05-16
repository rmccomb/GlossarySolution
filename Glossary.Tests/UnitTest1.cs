using System;
using System.Diagnostics;
using System.Linq;
using Glossary.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Glossary.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AccessDatabase()
        {
            using (var ctx = new GlossaryEntities())
            {
                var defs = (from d in ctx.Definitions select d).ToList();
                defs.ForEach(d => Debug.WriteLine($"{d.DefinitionId} {d.Term} {d.TermDefinition}"));
            }
        }

        [TestMethod]
        public void SaveData()
        {
            using (var ctx = new GlossaryEntities())
            {
                ctx.Definitions.AddRange(
                    new Definition[]
                    {
                        new Definition { Term = "abyssal plain", TermDefinition = "The ocean floor off the continental margin, usually very flat with a slight slope." },
                        new Definition { Term="accrete", TermDefinition="To add terranes..." },
                        new Definition { Term="alkaline", TermDefinition = "Term pertaining to highly basic, as opposed to acidic, substance" }
                    });
                ctx.SaveChanges();
            }
        }
    }
}
