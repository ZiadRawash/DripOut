using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Common
{
	public class Result
	{
		public bool IsSucceeded { get; set; }=false;
		public string? Message { get; set; }= string.Empty;
		public List<string> Errors { get; set; } = new List<string>();
		
		public static Result Success( string? message= null)
				=> new Result { IsSucceeded = true, Message = message };

		public static Result Failure(string? message = null,List<string>?Errors=null)
				=> new Result { Message = message , Errors= Errors?? new List<string>() };
		public static Result Failure(List<string>?Errors)
				=> new Result {Errors= Errors ?? new List<string>() };
	}
	public class Result<T> : Result
	{
		public  T? Data { get; set; }
		public static Result<T> Success(T data, string? message = null)
			=> new Result<T> { IsSucceeded = true, Message = message, Data = data };
		public new static Result<T> Failure(string? message = null, List<string>? Errors = null)
			=> new Result<T> { Message = message , Errors= Errors ?? new List<string>() };
		public new static Result<T> Failure(List<string>? Errors = null)		
				=> new Result<T> { Errors = Errors ?? new List<string>() };

	}
}
