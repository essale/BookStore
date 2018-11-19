using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Order
    {
        [Key]
        [Required] public int OrderId { get; set; }
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        [Required] public Customer Customer { get; set; }
        public int BookId { get; set; }
        [ForeignKey("BookId")]
        [Required] public Book Book { get; set; }
        [Required] public int Quantity { get; set; }
        [Required] public int TotalPrice { get; set; }
        [Required] public DateTime Date { get; set; }

    }
}
