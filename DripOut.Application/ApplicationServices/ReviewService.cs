using DripOut.Application.Common;
using DripOut.Application.DTOs.Reviews;
using DripOut.Application.Interfaces.ReposInterface;
using DripOut.Application.Interfaces.Services;
using DripOut.Application.Mappers;
using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.ApplicationServices
{
	public class ReviewService : IReviewService
	{
		public readonly IUnitOfWork _unitOfWork;
		public readonly IProductService _prdService;
		public ReviewService(IUnitOfWork unitOfWork, IProductService IProductService )
		{
			_unitOfWork = unitOfWork;
			_prdService = IProductService;
		}

		public async Task<Result> CreateReview(ReviewInputDTO inputDTO, string appUserId)
		{
			if (string.IsNullOrWhiteSpace(appUserId))
				return Result.Failure("Unauthorized");

			var product = await _unitOfWork.Products.FindAsync(inputDTO.ProductId);
			if (product == null)
				return Result.Failure("No product found with the given ID");

			var review = new Review
			{
				ReviewText = inputDTO.ReviewText,
				Stars = inputDTO.Stars,
				CreatedOn = DateTime.UtcNow,
				ProductId = inputDTO.ProductId,
				AppUserId = appUserId,
			};

			await _unitOfWork.Reviews.AddAsync(review);
			await _prdService.UpdateRateAsync(inputDTO.ProductId);
			await _unitOfWork.SaveChangesAsync();

			return Result.Success("Review created successfully");
		}

		public async Task<Result<IEnumerable<ReviewDTO>>> GetReviewsAsync(int productId)
		{
			var review= await _unitOfWork.Reviews.FindAsync(x=>x.ProductId== productId);
			if (review == null) return Result<IEnumerable<ReviewDTO>>.Failure("Product not found");
			var reviews = await _unitOfWork.Reviews
				.GetAllAsync(r => r.ProductId == productId, r => r.User!, r => r.User!.Image!);

			if (reviews == null || !reviews.Any())
				return Result<IEnumerable<ReviewDTO>>.Failure("No Reviews Found");

			return Result<IEnumerable<ReviewDTO>>.Success(
				reviews.Select(r => r.MapToDTO()).ToList(),
				"Reviews retrieved successfully"
			);

		}

		public async Task<Result<VoteResponseDTO>> ToggleUpVoteAsync(int reviewId, string appUserId)
		{
			var review = await _unitOfWork.Reviews.FindAsync(x => x.Id == reviewId);
			if (review == null)
				return Result<VoteResponseDTO>.Failure("Review not found");

			var existingVote = await _unitOfWork.ReviewVotes
				.FindAsync(v => v.ReviewId == reviewId && v.AppUserId == appUserId);

			if (existingVote == null)
			{
				review.Ups += 1;
				await _unitOfWork.ReviewVotes.AddAsync(new ReviewVote
				{
					AppUserId = appUserId,
					ReviewId = reviewId,
					Votedup = true,
					Voteddown = false
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
				await _unitOfWork.ReviewVotes.UpdateAsync(existingVote);
			}

			await _unitOfWork.SaveChangesAsync();

			return Result<VoteResponseDTO>.Success(
				new VoteResponseDTO
				{
					Ups = review.Ups,
					Downs = review.Downs,
					Score = review.Ups - review.Downs
				},
				"Vote updated successfully"
			);
		}
		public async Task<Result<VoteResponseDTO>> ToggleDownVoteAsync(int reviewId, string appUserId)
		{
			var review = await _unitOfWork.Reviews.FindAsync(x => x.Id == reviewId);
			if (review == null)
				return Result<VoteResponseDTO>.Failure("Review not found");

			var existingVote = await _unitOfWork.ReviewVotes
				.FindAsync(v => v.ReviewId == reviewId && v.AppUserId == appUserId);

			if (existingVote == null)
			{
				review.Downs += 1;
				await _unitOfWork.ReviewVotes.AddAsync(new ReviewVote
				{
					AppUserId = appUserId,
					ReviewId = reviewId,
					Votedup = false,
					Voteddown = true
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

			return Result<VoteResponseDTO>.Success(
				new VoteResponseDTO
				{
					Ups = review.Ups,
					Downs = review.Downs,
					Score = review.Ups - review.Downs
				},
				"Vote updated successfully"
			);
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
