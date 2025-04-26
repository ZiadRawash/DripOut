using DripOut.Application.DTOs;
using DripOut.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
    public interface IProductService
    {
        public Task<EntityPage<Product>> GetAllAsync(string search , int crntPage , int pageSize);
    }
}
