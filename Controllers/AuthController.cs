using Microsoft.AspNetCore.Mvc;
using Auth.DTOs;
using Auth.Services.Interfaces;

namespace Auth.Controllers;

/// <summary>
/// This is a controller to respond to authentication requests.
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController(IAuthService authService): ControllerBase
{
    /// <summary>
    /// This method will respond to login requests.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginDTO login)
    {
        return Ok(await authService.Login(login));
    }

    /// <summary>
    /// This method will respond to register requests.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> Register([FromBody] RegisterDTO register)
    {
        await authService.Register(register);
        return Ok();
    }

    /// <summary>
    /// This method will respond to email verification requests.
    /// </summary>
    /// <param name="token">The token to verify the email.</param>
    [HttpPost("{token}")]
    public async Task<ActionResult> VerifyEmail([FromRoute] string token)
    {
        await authService.VerifyEmail(token);
        return Ok();
    }
}