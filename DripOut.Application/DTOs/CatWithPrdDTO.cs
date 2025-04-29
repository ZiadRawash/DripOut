using DripOut.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs
{
    public class CatWithPrdDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public EntityPage<ProductsDTO> ProductsPage { get; set; } = null!;
    }
}
