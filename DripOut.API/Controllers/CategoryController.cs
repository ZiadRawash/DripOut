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
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
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
        public async Task<IActionResult> Create(CatInputDTO inputCategory)
        {
            if (inputCategory == null)
                return BadRequest();
            var category = new Category
            {
                Name = inputCategory.Name
            };
            await _unitOfWork.Categories.AddAsync(category);
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CatInputDTO inputCategory)
        {
            var category = await _unitOfWork.Categories.FindAsync(id);
            if (category == null)
                return NotFound();
            category!.Name = inputCategory.Name;
            await _unitOfWork.Categories.UpdateAsync(category)!;
            return Created();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Products.FindAsync(id)!;
            if (category == null)
                return NotFound();
            await _unitOfWork.Products.DeleteAsync(category);
            return NoContent();
        }
        [HttpGet("{id}/sizes")]
        public async Task<IActionResult> GetAvailableSizes(int id)
        {
            
            var products = await _unitOfWork.Products.GetAllAsync(p => p.CategoryId == id , p => p.Variants!);
            if(products == null || !products.Any())
                return NotFound("No Products Found in this Category");
            var variants = products.SelectMany(p => p.Variants!).ToList();
            var sizes = variants.Select(v => v.Size).Distinct().ToList();

            return Ok(sizes);
        }
    }
}
