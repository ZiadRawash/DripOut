using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IProductRepository Products { get; private set; }

        public IBaseRepository<Category> Categories { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
            Categories = new BaseRepository<Category>(_context);
        }


        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
