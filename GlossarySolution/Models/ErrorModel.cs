using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlossarySolution.Models
{
    public class ErrorModel
    {
        public Exception Exception { get; set; }
        public string Message { get; set; }
    }
}