using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Mail;

namespace SMS_TYNB.Helper
{
	public class MailJetSettings
	{
		public string ApiKey { get; set; }
		public string SecretKey { get; set; }
	}
	public class EmailSender : IEmailSender
	{
		private readonly IConfiguration _configuration;
		public MailJetSettings _mailJetSettings { get; set; }

		public EmailSender(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
			{
				Port = int.Parse(_configuration["EmailSettings:Port"]),
				Credentials = new NetworkCredential(
					_configuration["EmailSettings:Username"],
					_configuration["EmailSettings:Password"]
				),
				EnableSsl = true,
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
				Subject = subject,
				Body = htmlMessage,
				IsBodyHtml = true,
			};

			mailMessage.To.Add(email);
			await smtpClient.SendMailAsync(mailMessage);
		}

	}
}
