using System.ComponentModel.DataAnnotations;

namespace Dtos.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
    }
}
