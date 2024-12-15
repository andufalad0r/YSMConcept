using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using YSMConcept.Application.Interfaces;

namespace YSMConcept.Application.AuthServices
{
    public class JwtTokenProvider: IJwtTokenProvider
    {
        private readonly IConfiguration configuration;

        public JwtTokenProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GenerateToken()
        {
            var signingCredentials = GetSigningCredentials();

            var token = GenerateTokenOptions(signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials)
        {
            return new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(double.Parse(configuration["JWT:ExpiryDurationInMinutes"])),
                signingCredentials: signingCredentials);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }
    }
}
