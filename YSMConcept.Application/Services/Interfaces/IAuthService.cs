using YSMConcept.Application.DTOs.AuthDTOs;

namespace YSMConcept.Application.Services.Interfaces
{
    public interface IAuthService
    {
        public (int, string) Login(LoginDTO loginDto);
    }
}
