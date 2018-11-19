using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Models;
using BookStore.Data;

namespace BookStore.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }
        public Order Get(int id)
        {
            return GetAll().FirstOrDefault(order => order.OrderId == id);
        }

        public IEnumerable<Order> GetAll()
        {
            return _context.Orders;
        }
    }
}
