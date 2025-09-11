using DripOut.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Governorate
{
	public class GovernorateWithOrdersDTO
	{
		public string GovernorateName {  get; set; }=string.Empty;
		public int OrderId { get; set; }
		public DateTime date { get; set; }
		public decimal TotalCost { get; set; }
		public OrderStatus Status { get; set; }


	}
}
