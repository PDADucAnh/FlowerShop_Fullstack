using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderInputDTO input)
        {
            if (input == null)
            {
                return BadRequest(new { message = "Dữ liệu đơn hàng không hợp lệ" });
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var items = input.Items?.Select(i => new OrderItemInput
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList() ?? new List<OrderItemInput>();

            var (success, message, orderId) = await _orderService.CreateOrder(
                input.CustomerId, input.Notes, items);

            if (!success)
            {
                return StatusCode(500, new { message, detail = message });
            }

            return StatusCode(201, new
            {
                message,
                orderId
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAll();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var order = await _orderService.GetDetail(id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateOrderDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _orderService.Update(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _orderService.Delete(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
