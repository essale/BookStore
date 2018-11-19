using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore
{
    interface IOrderService
    {
        IEnumerable<Order> GetAll();
        Order Get(int id);




    }
}
