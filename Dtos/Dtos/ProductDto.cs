using System.ComponentModel.DataAnnotations;

namespace Dtos.Dtos
{
    /// <summary>
    /// Serves as ProductEditDto for this application, as the structure would be identical.
    /// Normally I would use separate DTO's for each Crud flow.
    /// </summary>
    public class ProductDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
    }
}
