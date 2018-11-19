using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Book
    {
        [Key]
        [Required] public int BookId { get; set; }
        [Required] public string Title { get; set; } 
        [Required] public string Genre { get; set; } 
        [Required] public int Price { get; set; }
        [Required] public string Author { get; set; }
        [Required] public string Publisher { get; set; }
        [Required] public int Quantity { get; set; }
        [Required] public string ImageUrl { get; set; }
        [Required] public DateTime ReleaseDate { get; set; }


    }
}
