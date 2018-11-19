using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class ClusterResulter
    {
        [Key]
        [Required] public int ClusterResulterID { get; set; }
        [Required] public int BookID { get; set; }
        [Required] public int ClusterRes { get; set; }

    }
}
