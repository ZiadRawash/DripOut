using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs
{
    public class EntityPage<T> where T : class
    {
        public IQueryable<T>? List;
        public int CurrentPage;
        public int PageSize;
        public int TotalSize;
        public int TotalPages;
    }
}
