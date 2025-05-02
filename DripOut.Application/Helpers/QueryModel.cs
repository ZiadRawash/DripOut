using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Helpers
{
    public class QueryModel
    {
        public string search { get; set; } = string.Empty;
        public int CategoryID { get; set; } = 0;
        public int MinPrice { get; set; } = 0;
        public int MaxPrice { get; set; } = 10000;
        public string Size { get; set; } = string.Empty;
        public string? OrderBy { get; set; } = null;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 8;
    }
}
