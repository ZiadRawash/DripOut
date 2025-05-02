using DripOut.Application.DTOs.Reviews;
using DripOut.Application.Interfaces.ReposInterface;

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

        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                return Created();
            }
            return BadRequest(ModelState);
        }

    }
}
