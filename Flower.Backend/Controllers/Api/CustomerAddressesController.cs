using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAddressesController : ControllerBase
    {
        private readonly ICustomerAddressService _addressService;

        public CustomerAddressesController(ICustomerAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("{customerId:int}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            return Ok(await _addressService.GetByCustomerId(customerId));
        }

        [HttpGet("by-id/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var address = await _addressService.GetById(id);
            if (address == null) return NotFound();
            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerAddressDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _addressService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerAddressDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _addressService.Update(id, dto);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _addressService.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPut("{id:int}/set-default")]
        public async Task<IActionResult> SetDefault(int id, [FromQuery] int customerId)
        {
            var updated = await _addressService.SetDefault(id, customerId);
            if (!updated) return NotFound();
            return NoContent();
        }
    }
}
