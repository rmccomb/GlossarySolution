using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlossarySolution.Models
{
    public class IndexModel
    {
        public DefinitionModel TermOTD { get; set; }

        public string ListPart { get; set; }

        public IEnumerable<DefinitionModel> Definitions { get; set; }
    }
}