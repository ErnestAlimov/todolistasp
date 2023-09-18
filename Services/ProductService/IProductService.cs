using todolistasp.Services.BaseService;

namespace todolistasp.Services.ProductService
{
    public interface IProductService : IBaseService<Product, ProductReadDto, ProductCreateDto, ProductUpdateDto>
    {
        Task<ProductReadDto> AddOneAsync(ProductCreateDto dto, int sellerId);
    }
}