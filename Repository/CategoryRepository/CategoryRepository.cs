namespace todolistasp.Repository.CategoryRepository
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}