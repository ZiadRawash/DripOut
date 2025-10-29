using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Order
{
	public class PostShippingOrderDTO
	{
		public int  GovernorateId {  get; set; }
		public string ShippingAddress{ get; set; }=string.Empty;
		public string ShippingPhone { get; set; }=string.Empty;

	}
}
