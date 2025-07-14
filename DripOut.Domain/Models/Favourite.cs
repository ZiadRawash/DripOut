using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Models
{
    public class Favourite
    {
        
        public string AppUserId { get; set; }
        public int ProductId { get; set; }
        public AppUser AppUser { get; set; }
        public Product Product { get; set; }
    }
}
