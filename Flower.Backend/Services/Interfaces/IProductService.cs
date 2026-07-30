using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAll(bool includeInactive = false);
        Task<PagedResult<ProductDTO>> GetPaged(int page, int pageSize, decimal? minPrice = null, decimal? maxPrice = null, int? categoryProductId = null, bool includeInactive = false, bool? isActive = null);
        Task<IEnumerable<ProductDTO>> GetByCategoryProduct(int categoryProductId, bool includeInactive = false);
        Task<ProductDTO?> GetDetail(int id, bool includeInactive = false);
        Task<ProductDTO> Create(CreateProductDTO dto);
        Task<bool> Update(int id, UpdateProductDTO dto);
        Task<bool> Delete(int id);
        Task<int> BulkDeleteAsync(List<int> ids);
        Task<int> BulkRestoreAsync(List<int> ids);
        Task<IEnumerable<ProductDTO>> Search(string query, bool includeInactive = false);
        Task<IEnumerable<ProductDTO>> GetTrending(int count = 10, bool includeInactive = false);
        Task TrackView(int productId);
        Task TrackAddToCart(int productId);
    }
}
