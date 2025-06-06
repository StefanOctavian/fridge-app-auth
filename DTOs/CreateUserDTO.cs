using Auth.Enums;

namespace Auth.DTOs;

public class CreateUserDTO
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Password { get; set; }
    public required string Salt { get; set; }
    public required string Email { get; set; }
    public required UserRole Role { get; set; }
}