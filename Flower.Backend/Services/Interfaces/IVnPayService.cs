using Flower.Backend.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace Flower.Backend.Services.Interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(VnPaymentRequestModel model, HttpContext context);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
