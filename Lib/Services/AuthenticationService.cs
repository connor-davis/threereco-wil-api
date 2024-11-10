using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using three_api.Lib.Helpers;
using three_api.Lib.Models;

namespace three_api.Lib.Services;

public class AuthenticationService
{
    public string GenerateToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AuthenticationSettings.PrivateKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email)
        };

        // Add roles to the claims
        if (user.Roles != null && user.Roles.Any())  // Assuming the user has a collection of roles
        {
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = "https://api.3reco.co.za",
            Audience = "https://3reco.co.za",
            SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
        };

        var token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }
}