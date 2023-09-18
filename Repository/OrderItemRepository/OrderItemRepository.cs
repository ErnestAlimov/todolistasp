using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todolistasp.Repository.OrderItemRepository
{
    public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}