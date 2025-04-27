using Auth.Enums;

namespace Auth.Errors;

/// <summary>
/// Common errors that may be reused in various places in the code.
/// </summary>
public static class CommonErrors
{
    public static ServerException UserNotFound => 
        new NotFoundException("User doesn't exist!");
    public static ServerException UnknownError => 
        new InternalServerErrorException("An unknown error occurred, contact the technical support!");
}
