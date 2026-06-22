/* H? tên: Ph?m Ð?c Anh
 * Mã SV: 2123110135
 * L?p: CCQ2311D
 * Ngày t?o: 16/05/2026
 * Mô t?: t?o th?c th? Order
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flower.Data.Entities
{
    public enum OrderStatus
    {
        Pending = 0,
        Shipping = 1,
        Completed = 2
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public int CustomerId { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string? Notes { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
