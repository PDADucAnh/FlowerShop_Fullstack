/* H? tên: Ph?m Ð?c Anh
 * Mã SV: 2123110135
 * L?p: CCQ2311D
 * Ngày t?o: 16/05/2026
 * Mô t?: t?o th?c th? User
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flower.Data.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên dang nh?p không du?c d? tr?ng")]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(200)]
        public string FullName { get; set; }

        [EmailAddress]
        [MaxLength(200)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [MaxLength(50)]
        public string Role { get; set; }
    }
}
