using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Models.Utility;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenService: ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Account account)
    {
        // secret key, signing credentials, and security algorithms
        var secret_key = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // claims for token
        var claims = new[]
        {
            new System.Security.Claims.Claim(CustomClaims.Name, account.userName),
            new System.Security.Claims.Claim(CustomClaims.Role, account.role.ToString()),
            new System.Security.Claims.Claim(CustomClaims.UserId, account.id.ToString())
        };

        // create the token
        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}