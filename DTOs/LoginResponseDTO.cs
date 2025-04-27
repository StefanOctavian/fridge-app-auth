namespace Auth.DTOs;

/// <summary>
/// This class is used to encapsulate the data for the login response.
/// </summary>
public record LoginResponseDTO(string Token, CleanUserDTO User);
