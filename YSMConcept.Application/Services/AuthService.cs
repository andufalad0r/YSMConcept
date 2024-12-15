using Microsoft.Extensions.Configuration;
using YSMConcept.Application.DTOs.AuthDTOs;
using YSMConcept.Application.Interfaces;
using YSMConcept.Application.Services.Interfaces;

namespace YSMConcept.Application.Services
{
    public class AuthService : IAuthService
    {
        public readonly IConfiguration _configuration;
        public readonly IJwtTokenProvider _jwtTokenProvider;

        public AuthService(
            IConfiguration configuration,
            IJwtTokenProvider jwtTokenProvider)
        {
            _configuration = configuration;
            _jwtTokenProvider = jwtTokenProvider;
        }

        public (int, string) Login(LoginDTO loginDto)
        {
            var passwordHash = _configuration["Admin:PasswordHash"];
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, passwordHash))
                return (0, "Invalid password");

            var token = _jwtTokenProvider.GenerateToken();
            return (1, token);
        }
    }
}
