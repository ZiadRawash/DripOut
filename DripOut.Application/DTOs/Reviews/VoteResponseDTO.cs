using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Reviews
{
	public class VoteResponseDTO
	{
		public int Ups { get; set; }
		public int Downs { get; set; }
		public int Score { get; set; }
		public string? UserVoteType { get; set; }
	}
}
