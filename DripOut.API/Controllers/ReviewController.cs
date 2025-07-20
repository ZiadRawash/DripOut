using DripOut.Application.DTOs.Reviews;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Interfaces.Services;
using DripOut.Application.Mappers;
using DripOut.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DripOut.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _prdService;
        public ReviewController(IUnitOfWork unitOfWork , IProductService _prdService)
        {
            _unitOfWork = unitOfWork;
            this._prdService = _prdService;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateReview(ReviewInputDTO inputReview)
        {
            if (ModelState.IsValid)
            {
                var product = await _unitOfWork.Products.FindAsync(inputReview.ProductId)!;
                if(product == null)
                    return BadRequest("Product not found");
                var review = new Review
                {
                    ReviewText = inputReview.ReviewText,
                    Stars = inputReview.Stars,
                    CreatedOn = DateTime.UtcNow,
                    ProductId = inputReview.ProductId,
                    Product = product,
                    AppUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!,
                };
                await _unitOfWork.Reviews.AddAsync(review);
                await _prdService.UpdateRateAsync(inputReview.ProductId);
                return Created();
            }
            return BadRequest(ModelState);
        }

        [HttpGet("Reviews/{productId:int}")]
        public async Task<IActionResult> GetReviewsAsync(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetAllAsync(r => r.ProductId == productId, r => r.User!, r => r.User!.Image!);
            if (reviews == null)
                return NotFound("No Reviews Found");
            return Ok(reviews.Select(r => r.MapToDTO()));

        }

    }
}
