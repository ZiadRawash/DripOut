using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Categories
{
    public class AllCategoriesDTO
    {
        public IEnumerable<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
    }
}
