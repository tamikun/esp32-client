using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace test;

[TestFixture]
public class JWTTest
{
    private readonly string secret = "secret at least 128 bit for HmacSha256 SecurityAlgorithms";

    [SetUp]
    public void Setup()
    { }

# nullable disable

    [Test]
    [Order(1)]
    public async Task ShouldGenerateJwtToken()
    {
        var token = generateJwtToken();
        System.Console.WriteLine("==== token: " + token);
        // bool isExpired = IsTokenExpired(token);
        // if (isExpired)
        // {
        //     Console.WriteLine("Token has expired.");
        // }
        // else
        // {
        //     Console.WriteLine("Token is still valid.");
        // }
        await Task.Delay(5000);
        getDataFromToken(token);
        await Task.CompletedTask;
    }

    private string generateJwtToken()
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("UserId", "1") }),
            Expires = DateTime.UtcNow.AddSeconds(5),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private void getDataFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            System.Console.WriteLine("userID " + userId);
        }
        catch (SecurityTokenExpiredException)
        {
            // Token has expired
            System.Console.WriteLine("Token has expired");
        }
        catch (SecurityTokenException)
        {
            // Token validation failed for other reasons
            System.Console.WriteLine("Token validation failed for other reasons");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("==== ex: " + Newtonsoft.Json.JsonConvert.SerializeObject(ex));
        }

    }

}