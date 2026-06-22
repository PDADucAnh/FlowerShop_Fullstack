
/* H? tên: Ph?m Ð?c Anh
 * Mã SV: 2123110135
 * L?p: CCQ2311D
 * Ngày t?o: 16/05/2026
 * Mô t?: t?o th?c th? Post
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flower.Data.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu d? không du?c d? tr?ng")]
        [MaxLength(500)]
        public string Title { get; set; } // Tiêu d? bài vi?t

        [Required(ErrorMessage = "N?i dung không du?c d? tr?ng")]
        public string Content { get; set; } // N?i dung chi ti?t

        [MaxLength(500)]
        public string? Summary { get; set; } // Mô t? ng?n

        [MaxLength(1000)]
        public string? ImageUrl { get; set; } // Hình ?nh d?i di?n
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Khóa ngo?i liên k?t t?i Category
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
