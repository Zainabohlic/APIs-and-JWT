using Microsoft.IdentityModel.Tokens;
using Project2.DTOs;
using Project2.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Project2.Services
{
    public class AuthorizeService : IAuthorizeService
    {
        private readonly IConfiguration _configuration;
        public AuthorizeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string TestAPI()
        {
            return "Api is working";
        }

        public bool AuthenticateUser(string pUserName, string pPassword)
        {
            string EncryptedPassword = EncryptData(pPassword);

            var Password = _configuration["Password"];
            var Username = _configuration["ApiUserName"];
            if ((pUserName == Username) && (Password == EncryptedPassword))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public AuthenticationModel CreateToken(string userName)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims(userName);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            var response = new AuthenticationModel
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(tokenOptions),
                expires_in = _configuration["Jwt:API_TOKEN_EXPIRY"],
                token_type = "Bearer"
            };

            return response;
        }
        private SigningCredentials GetSigningCredentials()
        {
            var key = _configuration["JwtSettings:SecretKey"];
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {

            var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:API_TOKEN_EXPIRY"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials
                );
            return token;
        }
        private List<Claim> GetClaims(string userName)
        {
            var claims = new List<Claim>()
    {
        new Claim("username" ,userName),
        new Claim("email","umar.farooq@iengineering.com")
    };
            return claims;
        }


        public static string EncryptData(string Value)
        {
            if (Value == null)
            {
                throw new ArgumentNullException("Value", "Parameter Value cannot be a null string.");
            }

            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, tripleDESCryptoServiceProvider.CreateEncryptor(KEY_192, IV_192), CryptoStreamMode.Write);
            StreamWriter streamWriter = new StreamWriter(cryptoStream);
            streamWriter.Write(Value);
            streamWriter.Flush();
            cryptoStream.FlushFinalBlock();
            memoryStream.Flush();
            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, int.Parse(memoryStream.Length.ToString()));
        }

        public static string DecryptData(string Value)
        {
            if (Value == null)
            {
                throw new ArgumentNullException("Value", "Parameter Value cannot be a null string.");
            }

            TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
            byte[] buffer = Convert.FromBase64String(Value);
            MemoryStream stream = new MemoryStream(buffer);
            CryptoStream stream2 = new CryptoStream(stream, tripleDESCryptoServiceProvider.CreateDecryptor(KEY_192, IV_192), CryptoStreamMode.Read);
            StreamReader streamReader = new StreamReader(stream2);
            return streamReader.ReadToEnd();
        }

        private static readonly byte[] KEY_192 = new byte[24]
            {
        45, 67, 89, 123, 234, 56, 78, 90, 12, 34,
        56, 78, 90, 21, 43, 65, 87, 109, 210, 32,
        54, 76, 98, 111
            };

        private static readonly byte[] IV_192 = new byte[24]
          {
        45, 67, 89, 123, 234, 56, 78, 90, 12, 34,
        56, 78, 90, 21, 43, 65, 87, 109, 210, 32,
        54, 76, 98, 111
          };
    }
}
