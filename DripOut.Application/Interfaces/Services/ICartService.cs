using DripOut.Application.Common;
using DripOut.Application.DTOs.Account;
using DripOut.Application.DTOs.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
	public interface ICartService
	{
		public Task<Result> AddToCart(AddToCartDTO model, string userId);
		public Task<Result> DeleteCartItem(int CartItemId, string userId);
		public Task<Result<CartReturnDto>> GetAllCartItems( string userId);
	}
}
