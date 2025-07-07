using DripOut.Application.DTOs;
using DripOut.Application.DTOs.Categories;
using DripOut.Application.Interfaces;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Mappers;
using DripOut.Domain.Models;
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
        private readonly IUnitOfWork _unitOfWork;

        public CategroyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            if (categories == null)
                return NotFound("No Categories Found");
            var products = await _unitOfWork.Products.GetAllAsync();
            if (products == null)
                return NotFound("No Products Found");
            var categoriesDTO = new AllCategoriesDTO
            {
                Categories = categories.Select(c => c.MapToCategoryDTO()),
                MinPrice = (int)products.Min(p => p.Price),
                MaxPrice = (int)products.Max(p => p.Price)
            };

            return Ok(categoriesDTO);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _unitOfWork.Categories.FindAsync(c => c.Id == id, c => c.Products!);
            if (category == null)
                return NotFound("No Such Category");
            return Ok(category.MapToCatWithPrdDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (category == null)
                return BadRequest();
            var newCategory = await _unitOfWork.Categories.AddAsync(category);
            return CreatedAtAction(nameof(GetById), new { id = newCategory!.Id }, newCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id)
                return BadRequest();
            var updatedCategory = await _unitOfWork.Categories.UpdateAsync(category)!;
            if (updatedCategory == null)
                return NotFound();
            return CreatedAtAction(nameof(GetById), new { id = updatedCategory.Id }, updatedCategory);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Products.FindAsync(id)!;
            if (category == null)
                return NotFound();
            var deletedCategory = await _unitOfWork.Products.DeleteAsync(category);
            return NoContent();
        }
    }
}
