	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	namespace DripOut.Domain.Models
	{
		public class Governorate
		{
			[Key]
			public int Id { get; set; }

			[Required]
			[StringLength(100)]
			public string Name { get; set; } = string.Empty;

			[Required]
			[Column(TypeName = "decimal(18,2)")]
			public decimal ShippingCost { get; set; }

			[Column(TypeName = "decimal(18,2)")]
			public decimal Threshold { get; set; }

			[Range(0, 100)]
			[Column(TypeName = "decimal(18,2)")]
			public decimal DiscountPercentage { get; set; }
			public bool IsActive { get; set; } = true;


			public ICollection<Order> Orders { get; set; } = new List<Order>();


	}
	}
