namespace Auth.DTOs;

/// <summary>
/// This class is used to encapsulate the data for the login request.
/// </summary>
public record LoginDTO(string Email, string Password);