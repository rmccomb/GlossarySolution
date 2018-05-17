using System.ComponentModel.DataAnnotations;

namespace GlossarySolution.Models
{
    public class DefinitionModel
    {
        public int DefinitionId { get; set; }

        [Required]
        [StringLength(80, MinimumLength=2)]
        public string Term { get; set; }
        [Required]
        [StringLength(255, MinimumLength=10)]
        public string TermDefinition { get; set; }
    }
}