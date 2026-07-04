using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAll();
        Task<PagedResult<ProductDTO>> GetPaged(int page, int pageSize, decimal? minPrice = null, decimal? maxPrice = null, int? categoryProductId = null);
        Task<IEnumerable<ProductDTO>> GetByCategoryProduct(int categoryProductId);
        Task<ProductDTO?> GetDetail(int id);
        Task<ProductDTO> Create(CreateProductDTO dto);
        Task<bool> Update(int id, UpdateProductDTO dto);
        Task<bool> Delete(int id);
        Task<IEnumerable<ProductDTO>> Search(string query);
        Task<IEnumerable<ProductDTO>> GetTrending(int count = 10);
        Task TrackView(int productId);
        Task TrackAddToCart(int productId);
    }
}
