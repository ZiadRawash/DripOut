using DripOut.Application.Common;
using DripOut.Application.DTOs.Reviews;
using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
	public interface IReviewService
	{
		Task<Result<IEnumerable<ReviewDTO>>> GetReviewsAsync(int productId);
		Task<Result> CreateReview(ReviewInputDTO inputDTO, string AppUserId);
		Task<Result<VoteResponseDTO>> ToggleUpVoteAsync(int reviewId, string appUserId);
		Task<Result<VoteResponseDTO>> ToggleDownVoteAsync(int reviewId, string appUserId);
	}
}
