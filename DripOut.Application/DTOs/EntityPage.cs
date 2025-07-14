using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs
{
    public class EntityPage<T> where T : class
    {
        public IEnumerable<T>? Items;
        public int CurrentPage;
        public int PageSize;
        public int Count;
        public int TotalPages;
        public decimal MinPrice;
        public decimal MaxPrice;
        public IList<string> Sizes = new List<string>();
    }
}
