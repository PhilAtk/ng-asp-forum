using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;

public class ForumEmail {
	private readonly ILogger<ForumEmail> _logger;

	SmtpClient _smtp;
	MailAddress _from;

	string _baseURL;
	string _resetURL;
	string _regConfURL;

	public ForumEmail(ILogger<ForumEmail> logger, IConfiguration config, IHttpContextAccessor httpContextAccessor) {
		_logger = logger;

		if (httpContextAccessor.HttpContext == null) {
			throw new Exception("Couldn't get HTTP Context for ForumEmail");
		}

		_baseURL = httpContextAccessor.HttpContext.Request.Scheme + "://" + httpContextAccessor.HttpContext.Request.Host + '/';
		_resetURL = _baseURL + "reset/";
		_regConfURL = _baseURL + "register/confirm/";

		var emailPass = config["EMAIL_PASSWORD"];
		var emailUser = config["EMAIL_USERNAME"];
		var mailserver = config["EMAIL_SERVER"];

		if (string.IsNullOrWhiteSpace(emailUser)) {
			throw new Exception("No email user provided for authentication. Please set 'EMAIL_USERNAME'");
		}
		if (string.IsNullOrWhiteSpace(emailPass)) {
			throw new Exception("No email password provided for authentication. Please set 'EMAIL_PASSWORD'");
		}
		if (string.IsNullOrWhiteSpace(mailserver)) {
			throw new Exception("No mailserver provided. Please set 'EMAIL_SERVER'");
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