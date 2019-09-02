﻿using Forum.Contracts;
using Forum.Core.Models.Errors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Forum.ExternalClients.SendGrid {
	class SendGridSenderClient : IEmailSender {
		public bool Ready { get; }

		SendGridSenderClientOptions Options { get; }
		ILogger Logger { get; }

		public SendGridSenderClient(
			IOptions<SendGridSenderClientOptions> optionsAccessor,
			ILogger<SendGridSenderClient> logger
		) {
			Options = optionsAccessor.Value;
			Logger = logger;

			if (Options.SendGridKey != null
				&& Options.SendGridUser != null
				&& Options.FromName != null
				&& Options.FromAddress != null) {
				Ready = true;
			}
		}

		public Task SendEmailAsync(string email, string subject, string message) {
			Execute(Options.SendGridKey, subject, message, email).Wait();
			return Task.CompletedTask;
		}

		public Task SendEmailConfirmationAsync(string email, string link) {
			return SendEmailAsync(
				email,
				"Confirm your email",
				$"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>.");
		}

		public async Task Execute(string apiKey, string subject, string message, string email) {
			if (!Ready) {
				throw new HttpInternalServerError("EmailSender is not ready.");
			}

			var client = new SendGridClient(apiKey);

			var msg = new SendGridMessage() {
				From = new EmailAddress(Options.FromAddress, Options.FromName),
				Subject = subject,
				PlainTextContent = message,
				HtmlContent = message
			};

			msg.AddTo(new EmailAddress(email));

			var response = await client.SendEmailAsync(msg);

			if (response.StatusCode != HttpStatusCode.Accepted) {
				Logger.LogCritical($"Error sending email. Response body: {response.Body}");
			}
		}
	}
}