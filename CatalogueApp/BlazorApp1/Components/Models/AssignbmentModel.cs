using System.ComponentModel.DataAnnotations;

namespace CatalogueApp.Components.Models
{
    public class AssignmentModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string ClassName { get; set; }

        public DateTime DueDate { get; set; }

        [Required]
        public int MaxPoints { get; set; }
    }
}
