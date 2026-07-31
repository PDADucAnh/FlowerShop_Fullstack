using Flower.Backend.Models.DTOs;
using Flower.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IApplicationDbContext _context;

        public PaymentMethodsController(IApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var methods = await _context.PaymentMethods
                .Where(m => m.IsActive)
                .OrderBy(m => m.DisplayOrder)
                .ThenBy(m => m.Id)
                .ToListAsync();

            return Ok(methods.Select(m => m.ToDTO()));
        }
    }
}
