namespace todolistasp.Repository.ProductRepository
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}