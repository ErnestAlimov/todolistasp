namespace todolistasp.Repository.OrderRepository
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}