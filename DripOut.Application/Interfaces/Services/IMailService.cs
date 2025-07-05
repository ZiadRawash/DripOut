using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
	public interface IMailService
	{
		Task <bool> SendEmailAsync(String MailTo, String Subject, string Body, IList<IFormFile>? Attachment = null);
		Task<bool> SendConfirmationAsync(String MailTo, String Subject, string UserName, string UserEmail, string Code);
	}
}
