using System.ComponentModel.DataAnnotations;

namespace todolistasp.Models
{
    public class Category : BaseModel
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}