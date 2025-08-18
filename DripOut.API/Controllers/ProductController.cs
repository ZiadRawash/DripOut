using DripOut.Application.DTOs;
using DripOut.Application.DTOs.Products;
using DripOut.Application.Helpers;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Interfaces.Services;
using DripOut.Application.Mappers;
using DripOut.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DripOut.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	//[Authorize]
	public class ProductController : ControllerBase
	{
		private readonly IProductService _prdService;

		public ProductController(IProductService prdService)
		{
			_prdService = prdService;
		}

		[HttpGet]
		public async Task<IActionResult> Index([FromQuery] QueryModel queryModel)
		{
			var productsPage = await _prdService.GetAllAsync(queryModel);
			var productsDTO = productsPage.MapToProductDTO();

			if (productsPage.Items == null)
				return NotFound("No Products Found");

			return Created();
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> FindAsync(int id)
		{
			var product = await _prdService.GetByIdAsync(id);

			if (product == null)
				return NotFound("No Such Id");

			return Ok(product.MapToDetailedProdDTO());
		}

		[HttpPost]
		public async Task<IActionResult> AddAsync(ProductInputDTO productDTO)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var product = await _prdService.CreateProductAsync(productDTO);
			return Ok(product);
		}

		[HttpPost("Size")]
		public async Task<IActionResult> AddVarient(VariantDTO variantDTO)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _prdService.AddVariantsAsync(variantDTO);

			if (!result)
				return NotFound("No Such Id");

			return Created();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Put(int id, ProductInputDTO inputProduct)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			var result = await _prdService.UpdateProductAsync(id, inputProduct);

			if (!result)
				return NotFound();

			return Created();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var result = await _prdService.DeleteProductAsync(id);

			if (!result)
				return NotFound();

			return NoContent();
		}


	}
}