using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Flower.Data.Entities;

namespace Flower.Backend.Controllers.Api
{
    [Authorize(Policy = "AdminOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;

        public CustomersController(ICustomerService customerService, IOrderService orderService)
        {
            _customerService = customerService;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAll();
            return Ok(customers);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null)
        {
            var result = await _customerService.GetPaged(page, pageSize, search);
            return Ok(result);
        }

        [HttpGet("{id}/orders")]
        public async Task<IActionResult> GetCustomerOrders(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.GetPaged(page, pageSize, customerId: id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerService.GetById(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _customerService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _customerService.Update(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _customerService.Delete(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
