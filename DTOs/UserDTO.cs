namespace Auth.DTOs;

public class UserDTO : CleanUserDTO
{
    public required string Password { get; set; }
    public required string Salt { get; set; }
    public required bool IsVerified { get; set; }
}