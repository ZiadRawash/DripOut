using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Domain.Enums
{
	public enum OrderStatus
	{
		Pending = 0,
		AwaitingPayment = 1,
		Confirmed = 2,
		Delivered = 3,
		Cancelled = 4
	}
}
