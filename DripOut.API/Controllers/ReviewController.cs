using DripOut.Application.DTOs;
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
        private readonly IReviewService _revService;
		public ReviewController(IUnitOfWork unitOfWork , IProductService _prdService, IReviewService IReviewService)
        {
            _unitOfWork = unitOfWork;
            this._prdService = _prdService;
			_revService = IReviewService;

		}

		[HttpPost]
		public async Task<IActionResult> CreateReview([FromBody] ReviewInputDTO inputReview)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrWhiteSpace(userId))
				return Unauthorized(new ApiResponse { Success = false, Message = "Unauthorized" });

			var result = await _revService.CreateReview(inputReview, userId);

			var response = new ApiResponse
			{
				Success = result.IsSucceeded,
				Message = result.Message,
				Errors = result.Errors
			};

			return result.IsSucceeded ? Ok(response) : BadRequest(response);
		}

		[HttpGet("Reviews/{productId:int}")]
		public async Task<IActionResult> GetReviewsAsync(int productId)
		{
			var result = await _revService.GetReviewsAsync(productId);

			return result.IsSucceeded
				? Ok(new ApiResponse<IEnumerable<ReviewDTO>> { Success = true, Data = result.Data, Message = result.Message })
				: NotFound(new ApiResponse { Success = false, Errors = result.Errors, Message = result.Message });
		}



		[HttpPost("ToggleUpVote/{reviewId}")]
		public async Task<IActionResult> ToggleUpVoteAsync(int reviewId)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Unauthorized();

			var result = await _revService.ToggleUpVoteAsync(reviewId, userId);

			return result.IsSucceeded
				? Ok(new ApiResponse<VoteResponseDTO> { Success = true, Data = result.Data, Message = result.Message })
				: BadRequest(new ApiResponse { Success = false, Errors = result.Errors, Message = result.Message });
		}

		[HttpPost("ToggleDownVote/{reviewId}")]
		public async Task<IActionResult> ToggleDownVoteAsync(int reviewId)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
				return Unauthorized();

			var result = await _revService.ToggleDownVoteAsync(reviewId, userId);

			return result.IsSucceeded
				? Ok(new ApiResponse<VoteResponseDTO> { Success = true, Data = result.Data, Message = result.Message })
				: BadRequest(new ApiResponse { Success = false, Errors = result.Errors, Message = result.Message });
		}

	}
}
