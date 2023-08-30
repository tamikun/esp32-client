using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using esp32_client.Models.Singleton;
using Microsoft.IdentityModel.Tokens;

namespace esp32_client.Builder
{
    public static class JwtToken
    {
        public class User
        {
#nullable disable
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string LoginName { get; set; }
        }
        private static readonly Settings _settings = EngineContext.Resolve<Settings>();
        public static string GenerateJwtToken(int userId, string userName, string loginName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.JwtTokenSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("UserId", userId.ToString()), new Claim("UserName", userName), new Claim("LoginName", loginName) }),
                Expires = DateTime.UtcNow.AddSeconds(_settings.SessionExpiredTimeInSecond),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static User GetDataFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_settings.JwtTokenSecret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var result = new User();
                result.UserId = int.Parse(jwtToken.Claims.First(x => x.Type == "UserId").Value);
                result.UserName = jwtToken.Claims.First(x => x.Type == "UserName").Value;
                result.LoginName = jwtToken.Claims.First(x => x.Type == "LoginName").Value;
                return result;
            }
            catch (SecurityTokenExpiredException)
            {
                // Token has expired
                throw;
            }
            catch (SecurityTokenException)
            {
                // Token validation failed for other reasons
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}