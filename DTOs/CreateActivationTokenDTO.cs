using Auth.Enums;

namespace Auth.DTOs;

public class CreateActivationTokenDTO
{
    public required string Token { get; set; }
    public required DateTime ExpirationDate { get; set; }
}