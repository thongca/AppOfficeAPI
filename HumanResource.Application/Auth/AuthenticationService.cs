using System;
using System.Collections.Generic;
using System.Text;
using HumanResource.Application.Helper.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace HumanResource.Application.Helper
{
    public class AuthenticationService : IAuthentication
    {
        private readonly TokenManagement _tokenManagement;
        public AuthenticationService(IOptions<TokenManagement> tokenManagement)
        {
            _tokenManagement = tokenManagement.Value;
        }
        public string GenerateToken(string claimTypes, RequestToken request)
        {
            string token = string.Empty;
            var claim = new[]
            {
                new Claim(claimTypes, request.UserID.ToString())
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

            var jwtToken = new JwtSecurityToken(
                _tokenManagement.Issuer,
                _tokenManagement.Audience,
                claim,
                expires: claimTypes == "Username" ? DateTime.UtcNow.AddDays(_tokenManagement.RefreshExpiration) : DateTime.UtcNow.AddYears(_tokenManagement.AccessExpiration),
                signingCredentials: credentials
                );
            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);


            return token;
        }
    }
}
