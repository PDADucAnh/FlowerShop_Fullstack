/* H? tên: Ph?m Ð?c Anh
 * Mã SV: 2123110135
 * L?p: CCQ2311D
 * Ngày t?o: 16/05/2026
 * Mô t?: t?o th?c th? CategoryProduct
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flower.Data.Entities
{
    public class CategoryProduct
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh m?c không du?c d? tr?ng")]
        [StringLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        // Quan h?: M?t danh m?c có nhi?u s?n ph?m
        public virtual ICollection<Product>? Products { get; set; }
    }
}
