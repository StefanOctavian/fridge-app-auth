using Auth.Enums;

namespace Auth.DTOs;

public class CleanUserDTO
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string Name => $"{FirstName} {LastName}";
    public required string Email { get; set; }
    public required UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}