using DripOut.Application.Common.Settings;
using DripOut.Application.Interfaces.Services;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;


namespace DripOut.Infrastructure.Implementation
{
	public class MailService:IMailService
	{
		public readonly MailSettings _MailSettings;
		
		public MailService( IOptions<MailSettings> settings)
		{
			_MailSettings = settings.Value;
		}

		public async Task <bool> SendConfirmationAsync(string MailTo, string Subject, string UserName, string UserEmail, string Code)
		{
			var Email = new MimeMessage
			{
				Sender = MailboxAddress.Parse(_MailSettings.Email),
				Subject = Subject,

			};
			Email.To.Add(MailboxAddress.Parse(MailTo));
			var Builder = new BodyBuilder();

			var file = System.IO.File.Exists("D:\\ASP.NET\\Projects\\DripOut\\DripOut.Application\\Common\\DripOutTemplate.html");
			if (!file)
			{
				return false;
			}
			var htmlbody = System.IO.File.ReadAllText("D:\\ASP.NET\\Projects\\DripOut\\DripOut.Application\\Common\\DripOutTemplate.html");
			string u = UserName;
			string E = UserEmail;
			htmlbody = htmlbody.Replace("{{UserEmail}}", UserEmail);
			htmlbody = htmlbody.Replace("{{ConfirmationCode}}", Code);
			Builder.HtmlBody = htmlbody;
			Email.Body = Builder.ToMessageBody();
			Email.From.Add(new MailboxAddress(_MailSettings.DisplayName, _MailSettings.Email));
			using var smtp = new SmtpClient();
			await smtp.ConnectAsync(_MailSettings.Host, _MailSettings.Port, SecureSocketOptions.StartTls);
			await smtp.AuthenticateAsync(_MailSettings.Email, _MailSettings.Password);
			await smtp.SendAsync(Email);
			await smtp.DisconnectAsync(true);
			return true;
		}
			

		public async Task<bool> SendEmailAsync(string MailTo, string Subject, string Body, IList<IFormFile>? Attachment = null)
		{
			var Email = new MimeMessage
			{
				Sender = MailboxAddress.Parse(_MailSettings.Email),
				Subject = Subject,

			};
			Email.To.Add(MailboxAddress.Parse(MailTo));
			var Builder = new BodyBuilder();
			if (Attachment != null)
			{
				byte[] FileBytes;
				foreach (var File in Attachment)
				{
					if (File.Length > 0)
					{
						using var ms = new MemoryStream();
						await File.CopyToAsync(ms);
						FileBytes = ms.ToArray();
						object value = Builder.Attachments.Add(File.FileName, FileBytes, ContentType.Parse(File.ContentType));
					}
				}
			}
			Builder.HtmlBody = Body;
			Email.Body = Builder.ToMessageBody();
			Email.From.Add(new MailboxAddress(_MailSettings.DisplayName, _MailSettings.Email));
			using var smtp = new SmtpClient();
			await smtp.ConnectAsync(_MailSettings.Host, _MailSettings.Port, SecureSocketOptions.StartTls);
			await smtp.AuthenticateAsync(_MailSettings.Email, _MailSettings.Password);
			await smtp.SendAsync(Email);
			await smtp.DisconnectAsync(true);
			return true;
		}
	}
}
