using DripOut.Application.DTOs.Reviews;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Interfaces.Services;
using DripOut.Application.Mappers;
using DripOut.Domain.Models;
using DripOut.Persistence.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using System.Linq;
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
        public async Task<IActionResult> GetReviewsAsync(int productId )
        {
            var reviews = await _unitOfWork.Reviews.GetAllAsync(r => r.ProductId == productId, r => r.User!, r => r.User!.Image!);
            if (reviews == null)
                return NotFound("No Reviews Found");
            return Ok(reviews.Select(r => r.MapToDTO()));

        }


        [HttpPost("UpVote/{reviewId}")]
		public async Task<IActionResult> ToggleUpVoteAsync(int reviewId)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
				return Unauthorized();

		
			var review = await _unitOfWork.Reviews.FindAsync(x => x.Id == reviewId);
			if (review == null)
				return NotFound("Review not found");

			
			var existingVote = await _unitOfWork.ReviewVotes
				.FindAsync(v => v.ReviewId == reviewId && v.AppUserId == userId);

			if (existingVote == null)
			{
				
				review.Ups += 1;
				await _unitOfWork.ReviewVotes.AddAsync(new ReviewVote
				{
					AppUserId = userId,
					ReviewId = reviewId,
					Voteddown = false,
					Votedup = true,
				});
			}
			else if (existingVote.Votedup)
			{
			
				review.Ups -= 1;
				await _unitOfWork.ReviewVotes.DeleteAsync(existingVote);
			}
			else if (existingVote.Voteddown)
			{
				
				review.Downs -= 1;
				review.Ups += 1;
				existingVote.Votedup = true;
				existingVote.Voteddown = false;
				await _unitOfWork.ReviewVotes.DeleteAsync(existingVote);
			}

			await _unitOfWork.SaveChangesAsync();

			return Ok(new
			{
				ups = review.Ups,
				downs = review.Downs,
				score = review.Ups - review.Downs,
				userVoteType = GetCurrentVoteType(existingVote)
			});
		}

		[HttpPost("DownVote/{reviewId}")]
		public async Task<IActionResult> ToggleDownVoteAsync(int reviewId)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
				return Unauthorized();

			
			var review = await _unitOfWork.Reviews.FindAsync(x => x.Id == reviewId);
			if (review == null)
				return NotFound("Review not found");

			
			var existingVote = await _unitOfWork.ReviewVotes
				.FindAsync(v => v.ReviewId == reviewId && v.AppUserId == userId);

			if (existingVote == null)
			{
				
				review.Downs += 1;
				await _unitOfWork.ReviewVotes.AddAsync(new ReviewVote
				{
					AppUserId = userId,
					ReviewId = reviewId,
					Voteddown = true,
					Votedup = false,
				});
			}
			else if (existingVote.Voteddown)
			{
				
				review.Downs -= 1;
				await _unitOfWork.ReviewVotes.DeleteAsync(existingVote);
			}
			else if (existingVote.Votedup)
			{
				
				review.Ups -= 1;
				review.Downs += 1;
				existingVote.Votedup = false;
				existingVote.Voteddown = true;
			    await _unitOfWork.ReviewVotes.UpdateAsync(existingVote);
			}

			await _unitOfWork.SaveChangesAsync();

			return Ok(new
			{
				ups = review.Ups,
				downs = review.Downs,
				score = review.Ups - review.Downs,
				userVoteType = GetCurrentVoteType(existingVote)
			});
		}


		private string? GetCurrentVoteType(ReviewVote? vote)
		{
			if (vote == null) return null;
			if (vote.Votedup) return "upvote";
			if (vote.Voteddown) return "downvote";
			return null;
		}

	}
}
