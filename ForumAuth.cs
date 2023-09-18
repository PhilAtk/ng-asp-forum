using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class ForumAuthenticator {
	// Hashing Parameters
	private const int _saltSize = 16; // 128 bits
	private const int _keySize = 32; // 256 bits
	private const int _iterations = 50000;
	private static readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA256;
	private const char _delimiter = ':';

	private const string PASS_RESET = "PasswordReset";
	private const string REGISTER = "Register";
	
	private readonly SymmetricSecurityKey securityKey;

	private readonly string _issuer;
	private readonly string _audience;

	public ForumAuthenticator(IConfiguration config) {
		var audience = config["AUTH_AUDIENCE"];
		var issuer = config["AUTH_ISSUER"];
		var secret = config["AUTH_SECRET"];

		if (string.IsNullOrWhiteSpace(audience)) {
			throw new Exception("No audience provided for authentication. Please set 'AUTH_AUDIENCE'");
		}
		_audience = audience;

		if (string.IsNullOrWhiteSpace(issuer)) {
			throw new Exception("No issuer provided for authentication. Please set 'AUTH_ISSUER'");
		}
		_issuer = issuer;

		if (string.IsNullOrWhiteSpace(secret)) {
			throw new Exception("No secret provided for authentication. Please set 'AUTH_SECRET'");
		}
		securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
	}

	public string GetRandom6charCode()
	{
		string path = Path.GetRandomFileName();
		path = path.Replace(".", ""); 

		return path.Substring(0, 6);
	}

	public string GenerateBearerToken(int userID) {

		var handler = new JwtSecurityTokenHandler();

		// TODO: Generate a sessionID, save into Database, include in token
		var descriptor = new SecurityTokenDescriptor(){
			Subject = new ClaimsIdentity(new Claim[]{
				new Claim(ClaimTypes.NameIdentifier, userID.ToString())
			}),
			Expires = DateTime.UtcNow.AddDays(7),
			Issuer = _issuer,
			Audience = _audience,
			SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
		};

		var token = handler.CreateToken(descriptor);

		return handler.WriteToken(token);
	}

	public bool VerifyBearerToken(string token, out int userID) {

		var handler = new JwtSecurityTokenHandler();

		SecurityToken validatedToken;
		try {
			var thing = handler.ValidateToken(token, new TokenValidationParameters(){
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = securityKey,

				ValidateIssuer = true,
				ValidIssuer = _issuer,

				ValidateAudience = true, 
				ValidAudience = _audience
			}, out validatedToken);

			var tokenUserID = int.Parse(thing.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

			userID = tokenUserID;
		}
		catch {
			userID = 0;
			return false;
		}

		return true;
	}

	public string Hash(string input) {
		byte[] salt = RandomNumberGenerator.GetBytes(_saltSize);
		byte[] hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, _iterations, _algorithm, _keySize);

		return string.Join(_delimiter, Convert.ToHexString(hash), Convert.ToHexString(salt), _iterations, _algorithm);
	}

	public bool HashVerify(string input, string hashString)
	{
		// Break apart the string
		string[] segments = hashString.Split(_delimiter);
		byte[] hash = Convert.FromHexString(segments[0]);
		byte[] salt = Convert.FromHexString(segments[1]);
		int iterations = int.Parse(segments[2]);
		HashAlgorithmName algorithm = new HashAlgorithmName(segments[3]);

		// Use the given paramters to compute the hash
		byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, hash.Length);

		return CryptographicOperations.FixedTimeEquals(inputHash, hash);
	}

}