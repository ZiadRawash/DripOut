using DripOut.Application.Interfaces.ReposInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DripOut.Domain.Models;
using System.Security.Claims;
using DripOut.Application.Mappers;
using Microsoft.AspNetCore.Authorization;

namespace DripOut.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoriteController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FavoriteController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }
            var favorites = await _unitOfWork.Favourites.GetAllAsync(f => f.AppUserId == userId, f => f.Product!);
            if (favorites is null || !favorites.Any())
                return NotFound("No favorite Found");
            var products = favorites.Select(f => f.Product.MapToProductDTO()).ToList();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorite(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }
            var product = await _unitOfWork.Products.FindAsync(productId);
            if(product is null)
                return BadRequest("Product not found");
            var favorites = await _unitOfWork.Favourites.GetAllAsync(f => f.AppUserId == userId && f.ProductId == productId);
            if (favorites is not null && favorites.Any())
            {
                return BadRequest("Product already in favorite");
            }
            var favorite = new Favourite
            {
                AppUserId = userId,
                ProductId = productId,
            };
            await _unitOfWork.Favourites.AddAsync(favorite);
			await _unitOfWork.SaveChangesAsync();
			return Created();
        }
    }
}
