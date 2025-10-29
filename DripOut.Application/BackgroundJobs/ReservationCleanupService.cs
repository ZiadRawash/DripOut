using DripOut.Application.Interfaces.ReposInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.BackgroundJobs
{
	public class ReservationCleanupService
	{
		private readonly IUnitOfWork _unitOfWork;

		public ReservationCleanupService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task CleanupExpiredReservations()
		{
			var expired = await _unitOfWork.StockReservations
				.GetAllAsync(r => r.ExpiresAt < DateTime.UtcNow);

			if (expired.Any())
			{
				await _unitOfWork.StockReservations.DeleteRangeAsync(expired);
				await _unitOfWork.SaveChangesAsync();
			}
		}
	}
}
