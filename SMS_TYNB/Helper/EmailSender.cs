﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Mail;

namespace SMS_TYNB.Helper
{

	public class EmailSender : IEmailSender
	{
		private readonly IConfiguration _configuration;

		public EmailSender(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			DotNetEnv.Env.Load();
			var fromEmail = Environment.GetEnvironmentVariable("EMAIL_ADDRESS");
			var password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

			var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
			{
				Port = int.Parse(_configuration["EmailSettings:Port"]),
				Credentials = new NetworkCredential(
					fromEmail,
					password
				),
				EnableSsl = true,
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress(fromEmail),
				Subject = subject,
				Body = htmlMessage,
				IsBodyHtml = true,
			};

			mailMessage.To.Add(email);
			await smtpClient.SendMailAsync(mailMessage);
		}

	}
}
