using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Models
{
    public class Image
	{
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
