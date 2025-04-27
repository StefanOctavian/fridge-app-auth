namespace Auth.DTOs;

/// <summary>
/// This class is used to encapsulate the data for the register request.
/// </summary>
public record RegisterDTO(
    string FirstName, 
    string LastName, 
    string Email, 
    string Password
);
