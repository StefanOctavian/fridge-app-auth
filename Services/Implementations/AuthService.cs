using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

using Auth.Services.Interfaces;
using Auth.DTOs;
using Auth.Enums;
using Auth.Errors;
using Auth.Utils;
using Auth.Configurations;
using Auth.Constants;
using Auth.Extensions;

namespace Auth.Services.Implementations;

public class AuthService(
    IOptions<JwtConfiguration> jwtConfiguration,
    IMailService mailService,
    HttpClient httpClient
) : IAuthService
{
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    private string GetToken(CleanUserDTO user, DateTime issuedAt, TimeSpan expiresIn)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfiguration.Key); // Use the configured key as the encryption key to sing the JWT.
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new([new(ClaimTypes.NameIdentifier, user.Id.ToString())]), // Set the user ID as the "nameid" claim in the JWT.
            Claims = new Dictionary<string, object> // Add any other claims in the JWT, you can even add custom claims if you want.
            {
                { ClaimTypes.Name, user.Name },
                { ClaimTypes.Email, user.Email },
                { ClaimTypes.Role, user.Role.ToString() }
            },
            IssuedAt = issuedAt, // This sets the "iat" claim to indicate then the JWT was emitted.
            Expires = issuedAt.Add(expiresIn), // This sets the "exp" claim to indicate when the JWT expires and cannot be used.
            Issuer = _jwtConfiguration.Issuer, // This sets the "iss" claim to indicate the authority that issued the JWT.
            Audience = _jwtConfiguration.Audience, // This sets the "aud" claim to indicate to which client the JWT is intended to.
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // Sign the JWT, it will set the algorithm in the JWT header to "HS256" for HMAC with SHA256.
        };

        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)); // Create the token.
    }

    public async Task<LoginResponseDTO> Login(LoginDTO login)
    {
        var user = await httpClient.GetAsync("User?email=" + login.Email).FromJson<UserDTO>()
            ?? throw CommonErrors.UserNotFound;

        if (!PasswordUtils.VerifyPassword(login.Password, user.Password, user.Salt))
            throw new BadRequestException("Wrong password.");

        if (!user.IsVerified)
            throw new ForbiddenException("Please verify your email before logging in.");

        var userDTO = new CleanUserDTO
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        var token = GetToken(userDTO, DateTime.UtcNow, TimeSpan.FromDays(7));
        return new LoginResponseDTO(token, userDTO);
    }

    public async Task Register(RegisterDTO register)
    {
        var existingUser = await httpClient.GetAsync("User?email=" + register.Email)
            .FromJson<UserDTO>();

        if (existingUser != null) 
            throw new AlreadyExistsException("A user with this email already exists.");

        var (password, salt) = PasswordUtils.HashPassword(register.Password);

        var createUser = new CreateUserDTO
        {
            FirstName = register.FirstName,
            LastName = register.LastName,
            Email = register.Email,
            Password = password,
            Salt = salt,
            Role = UserRole.User
        };

        var newUser = await httpClient.PostAsync("User", createUser).FromJson<UserDTO>()
            ?? throw new ServiceUnavailableException("User couldn't be created.");

        var activationToken = Guid.NewGuid().ToString();
        var activationEntry = new CreateActivationTokenDTO
        {
            Token = activationToken,
            ExpirationDate = DateTime.UtcNow.AddDays(1)
        };
        await httpClient.PostAsync($"User/{newUser.Id}/ActivationToken", activationEntry).Unpack();

        try {
            await mailService.SendMail(
                register.Email,
                "Welcome to FridgeApp!",
                MailTemplates.VerificationMail(register.FirstName, activationToken),
                isHtmlBody: true,
                senderTitle: "FridgeApp Team"
            );
        } catch (Exception ex) {
            await httpClient.DeleteAsync($"User/{newUser.Id}").Unpack();
            throw new ServiceUnavailableException("Failed to send verification email.", ex);
        }
    }

    public async Task VerifyEmail(string token)
    {
        var user = await httpClient.GetAsync("User/ActivationToken/" + token).FromJson<UserDTO>()
            ?? throw new NotFoundException("Wrong activation token. Please check that you have entered the correct url from the email.");

        if (user.IsVerified) throw new BadRequestException("This account is already verified.");

        await httpClient.PatchAsync("User/" + user.Id, new { IsVerified = true }).Unpack();
    }
}