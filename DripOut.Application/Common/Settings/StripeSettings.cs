using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Common.Settings
{
	public class StripeSettings
	{
		public string Publishablekey { get; set; } = string.Empty;
		public string Secretkey { get; set; } = string.Empty;
	}
}
