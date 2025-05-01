using DripOut.Application.DTOs;
using DripOut.Application.Interfaces;
using DripOut.Domain.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DripOut.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategroyController : ControllerBase
    {
        private readonly IBaseRepository<Category> _repo;
        private readonly IProductService _prdService;

        public CategroyController(IBaseRepository<Category> repo, IProductService prdService)
        {
            _repo = repo;
            _prdService = prdService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _repo.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string search = "", int id = 0, int crntPage = 1, int pageSize = 10)
        {
            var category = await _repo.FindAsync(id)!;
            if (category == null)
                return NotFound();
            var output = new CatWithPrdDTO()
            {
                Id = category.Id,
                Name = category.Name,
                ProductsPage = await _prdService.GetAllAsync(search, id, crntPage, pageSize)
            };
            return Ok(output);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (category == null)
                return BadRequest();
            var newCategory = await _repo.AddAsync(category);
            return CreatedAtAction(nameof(GetById), new { id = newCategory.Id }, newCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id)
                return BadRequest();
            var updatedCategory = await _repo.UpdateAsync(category)!;
            if (updatedCategory == null)
                return NotFound();
            return CreatedAtAction(nameof(GetById), new { id = updatedCategory.Id }, updatedCategory);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _repo.FindAsync(id)!;
            if (category == null)
                return NotFound();
            var deletedCategory = await _repo.DeleteAsync(category);
            return NoContent();
        }
    }
}
