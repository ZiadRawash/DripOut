using DripOut.Application.Interfaces.Services;
using DripOut.Domain.Models.Entities;
using DripOut.Infrastructure.Implementaion;
using DripOut.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace DripOut.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IBaseRepository<Product> _repo;
        private readonly ProductSrevice _productSrevice;

        public ProductController(IBaseRepository<Product> repo , ProductSrevice productSrevice)
        {
            _repo = repo;
            _productSrevice = new ProductSrevice((BaseRepository<Product>)repo);
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int? crntPage , int? pageSize)
            => Ok(await _productSrevice.GetAllAsync(search!, crntPage ?? 1, pageSize ?? 10));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _repo.FindAsync(id)!;
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Product product)
        {
            if (product == null)
                return BadRequest();
            var newProduct = await _repo.AddAsync(product);
            return CreatedAtAction(nameof(Get), new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();
            var updatedProduct = await _repo.UpdateAsync(product)!;
            if (updatedProduct == null)
                return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _repo.FindAsync(id)!;
            if (product == null)
                return NotFound();
            await _repo.DeleteAsync(product);
            return NoContent();
        }
    }
}
