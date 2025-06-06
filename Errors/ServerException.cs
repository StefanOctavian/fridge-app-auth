﻿using System.Net;

namespace Auth.Errors;

/// <summary>
/// Represents an Exception that was generated by a Service or Controller and is to be
/// sent to the user as an error message. Optionally, it may hold a cause that is an error
/// to be logged.
/// </summary>
public class ServerException(HttpStatusCode status, string message, Exception? cause = null) 
    : Exception(message, cause)
{
    public HttpStatusCode Status { get; } = status;
}

public class InternalServerErrorException(string message = "Something went wrong!", Exception? cause = null) 
    : ServerException(HttpStatusCode.InternalServerError, message, cause);

public class ForbiddenException(string message, Exception? cause = null) 
    : ServerException(HttpStatusCode.Forbidden, message, cause);

public class BadRequestException(string message, Exception? cause = null) 
    : ServerException(HttpStatusCode.BadRequest, message, cause);

public class UnauthorizedException(string message, Exception? cause = null) 
: ServerException(HttpStatusCode.Unauthorized, message, cause);

public class NotFoundException(string message, Exception? cause = null) 
    : ServerException(HttpStatusCode.NotFound, message, cause);

public class ServiceUnavailableException(string message, Exception? cause = null)
    : ServerException(HttpStatusCode.ServiceUnavailable, message, cause);

public class AlreadyExistsException(string message, Exception? cause = null)
    : ServerException(HttpStatusCode.Conflict, message, cause);
