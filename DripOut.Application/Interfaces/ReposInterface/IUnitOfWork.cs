using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.ReposInterface
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IProductRepository Products { get; }
        IBaseRepository<Category> Categories { get; }

        Task<int> SaveChangesAsync();

    }
}
