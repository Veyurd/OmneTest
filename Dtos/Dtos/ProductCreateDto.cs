using System.ComponentModel.DataAnnotations;

namespace Dtos.Dtos
{
    public class ProductCreateDto
    {
        [Required]
        public string Name { get; set; }
        public float Price { get; set; }
        public string? Description { get; set; }
    }
}
