using System.Net;
using System.Net.Mail;

public class ForumEmail {
	private readonly ILogger<ForumEmail> _logger;
	private readonly IConfiguration _config;

	SmtpClient _smtp;
	MailAddress _from;

	string _baseURL;
	string _resetURL;
	string _regConfURL;

	public ForumEmail(ILogger<ForumEmail> logger, IConfiguration config) {
		_logger = logger;
		_config = config;

		_baseURL = "https://localhost:44406/"; // TODO: Programmatically load this
		_resetURL = _baseURL + "reset/";
		_regConfURL = _baseURL + "register/confirm/";

		var emailPass = _config["email:password"];
		var emailUser = _config["email:username"];
		var mailserver = _config["email:mailServer"];

		if (string.IsNullOrWhiteSpace(emailUser)) {
			throw new Exception("No email user provided for authentication. Please set 'email:username'");
		}
		if (string.IsNullOrWhiteSpace(emailPass)) {
			throw new Exception("No email password provided for authentication. Please set 'email:password'");
		}
		if (string.IsNullOrWhiteSpace(mailserver)) {
			throw new Exception("No mailserver provided. Please set 'email:mailServer'");
		}

		_from = new MailAddress(emailUser);

		_smtp = new SmtpClient(mailserver) {
			EnableSsl = true,
			Port = 587,
			Credentials = new NetworkCredential(emailUser, emailPass)
		};
    }


	public void sendPasswordReset(string to, string token) {

		var message = new MailMessage{
			From = _from,
			Subject = "Password Reset",
			IsBodyHtml = true,
			Body = "<p>A request was made to reset your password. Go here to reset your password: " + _resetURL + token + "</p>"
		};

		message.To.Add(to);

		_smtp.Send(message);
	}

	public void sendRegistrationConfirmation(string to, string token) {

		var message = new MailMessage{
			From = _from,
			Subject = "Confirm Registration",
			IsBodyHtml = true,
			Body = "<p>An account was created with your email. To confirm this reigstration, go here: " + _regConfURL + token + "</p>"
		};

		message.To.Add(to);
		_smtp.Send(message);
	}
}