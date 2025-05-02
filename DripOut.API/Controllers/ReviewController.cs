using DripOut.Application.DTOs;

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
        private readonly IBaseRepository<Review> _repo;
        private readonly IProductRepository _productService;
        private readonly SignInManager<AppUser> _signInManager;

        public ReviewController(IBaseRepository<Review> repo 
                                , SignInManager<AppUser> signInManager
                                 , IProductRepository productService  )
        {
            _repo = repo;
            _signInManager = signInManager;
            _productService = productService;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateReview(ReviewDTO inputReview)
        {
            var user = await _signInManager.UserManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            if(user == null)
                return BadRequest("User not found");
            if (ModelState.IsValid)
            {
                var product = await _productService.FindAsync(inputReview.ProductId)!;
                if(product == null)
                    return BadRequest("Product not found");
                var review = new Review
                {
                    ReviewText = inputReview.ReviewText,
                    Stars = inputReview.Stars,
                    CreatedOn = inputReview.CreatedOn,
                    ProductId = inputReview.ProductId,
                    Product = product,
                    AppUserId = user.Id,
                    User = user
                };
                await _repo.AddAsync(review);
                return Created();
            }
            return BadRequest(ModelState);
        }

    }
}
