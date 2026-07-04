using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flower.Data.Entities
{
    public class CategoryProduct
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh m?c không du?c d? tr?ng")]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(300)]
        public string? Slug { get; set; }

        public virtual ICollection<Product>? Products { get; set; }
    }
}
