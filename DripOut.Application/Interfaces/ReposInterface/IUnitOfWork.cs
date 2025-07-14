using DripOut.Application.Interfaces.Services;
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
        IBaseRepository<ProductVariant> Variants { get; }
        IBaseRepository<Review> Reviews { get; }
        IBaseRepository<Favourite> Favourites { get; }

        Task<int> SaveChangesAsync();

    }
}
