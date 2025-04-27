using Auth.DTOs;

namespace Auth.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDTO> Login(LoginDTO login);

    Task Register(RegisterDTO register);

    Task VerifyEmail(string token);
}