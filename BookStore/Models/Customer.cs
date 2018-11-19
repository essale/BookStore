using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Customer 
    {
        [Key]
        [Required] public int CustomerId { get; set; }
        [Display(Name ="Email Address")]
        [EmailAddress]
        [Required] public string EmailAddress { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public DateTime RegistrationDate { get; set; }
        [Required] public string Country { get; set; }
    }
}
    