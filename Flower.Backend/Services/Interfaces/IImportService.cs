using Flower.Backend.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace Flower.Backend.Services.Interfaces;

public interface IImportService
{
    Task<ImportResult> ImportProductsAsync(
        IFormFile excelFile,
        IFormFile? zipFile,
        string onDuplicate);

    Task<ImportResult> ImportCategoriesAsync(
        IFormFile excelFile,
        IFormFile? zipFile,
        string onDuplicate);
}
