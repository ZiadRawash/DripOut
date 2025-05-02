using DripOut.Application.DTOs.Account;
using DripOut.Application.DTOs.Reviews;
using DripOut.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Mappers
{
    public static class ReviewMapper
    {
        public static ReviewDTO MapToDTO(this Review review)
        {
            return new ReviewDTO
            {
                ReviewText = review.ReviewText,
                Stars = review.Stars,
                CreatedOn = review.CreatedOn,
                User = new UserDTO
                {
                    FirstName = review.User!.FirstName,
                    LastName = review.User.LastName,
                    Email = review.User.Email
                }
            };
        }
    }
}
