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
    public class FavouriteController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FavouriteController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavourites()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }
            var favourites = await _unitOfWork.Favourites.GetAllAsync(f => f.AppUserId == userId, f => f.Product!);
            if (favourites is null || !favourites.Any())
                return NotFound("No Favourites Found");
            var products = favourites.Select(f => f.Product.MapToProductDTO()).ToList();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavourites(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }
            var product = await _unitOfWork.Products.FindAsync(productId);
            if(product is null)
                return BadRequest("Product not found");
            var favourites = await _unitOfWork.Favourites.GetAllAsync(f => f.AppUserId == userId && f.ProductId == productId);
            if (favourites is not null && favourites.Any())
            {
                return BadRequest("Product already in favourites");
            }
            var favourite = new Favourite
            {
                AppUserId = userId,
                ProductId = productId,
            };
            await _unitOfWork.Favourites.AddAsync(favourite);
            return Created();
        }
    }
}
