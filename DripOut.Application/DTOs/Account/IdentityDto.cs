﻿using DripOut.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.DTOs.Account
{
	public class IdentityDto:ServiceResponse
	{	
	public string? ConfirmationCode { get; set; }
	public string? Email { set; get; }
	public string ? name { get; set; }	
	
	
	}
}
