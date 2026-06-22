/* H? tên: Ph?m Ð?c Anh
 * Mã SV: 2123110135
 * L?p: CCQ2311D
 * Ngày t?o: 16/05/2026
 * Mô t?: t?o th?c th? Category
 */


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flower.Data.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh m?c không du?c d? tr?ng")]
        [MaxLength(200)]
        public string Name { get; set; } // Tên danh m?c (vd: Tin Giáo D?c)

        [MaxLength(2000)]
        public string? Description { get; set; }

        // Quan h?: M?t danh m?c có nhi?u bài vi?t
        public virtual ICollection<Post> Posts { get; set; }
    }
}
