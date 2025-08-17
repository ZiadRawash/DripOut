using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DripOut.Application.Interfaces.Services
{
	/// <summary>
	/// Provides functionality for sending emails and confirmation messages.
	/// </summary>
	public interface IMailService
	{
		/// <summary>
		/// Sends a general email with optional attachments.
		/// </summary>
		/// <param name="MailTo">The recipient's email address.</param>
		/// <param name="Subject">The subject line of the email.</param>
		/// <param name="Body">The body of the email in HTML format.</param>
		/// <param name="Attachment">Optional list of files to attach to the email.</param>
		/// <returns>
		/// A task that represents the asynchronous operation.  
		/// The task result contains <c>true</c> if the email was sent successfully; otherwise <c>false</c>.
		/// </returns>
		Task<bool> SendEmailAsync(string MailTo, string Subject, string Body, IList<IFormFile>? Attachment = null);

		/// <summary>
		/// Sends a confirmation email (e.g., for account verification) using a predefined template.
		/// </summary>
		/// <param name="MailTo">The recipient's email address.</param>
		/// <param name="Subject">The subject line of the confirmation email.</param>
		/// <param name="UserName">The name of the user receiving the confirmation.</param>
		/// <param name="UserEmail">The email address of the user to confirm.</param>
		/// <param name="Code">The confirmation code to be included in the email.</param>
		/// <returns>
		/// A task that represents the asynchronous operation.  
		/// The task result contains <c>true</c> if the confirmation email was sent successfully; otherwise <c>false</c>.
		/// </returns>
		Task<bool> SendConfirmationAsync(string MailTo, string Subject, string UserName, string UserEmail, string Code);
	}
}
