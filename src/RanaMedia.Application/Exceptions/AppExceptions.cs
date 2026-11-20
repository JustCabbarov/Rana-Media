namespace RanaMedia.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException() : base("The requested resource was not found.") { }
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
}

public class NotNullExceptions : Exception
{
    public NotNullExceptions() : base("Can not be Null") { }
    public NotNullExceptions(string message) : base(message) { }
    public NotNullExceptions(string message, Exception innerException) : base(message, innerException) { }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException() : base("Invalid login attempt.") { }
    public UnauthorizedException(string message) : base(message) { }
    public UnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
}

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base("İstifadəçi adı və ya şifrə səhvdir.") { }
    public InvalidCredentialsException(string message) : base(message) { }
    public InvalidCredentialsException(string message, Exception innerException) : base(message, innerException) { }
}

public class ForbiddenException : Exception
{
    public string ErrorCode { get; }

    public ForbiddenException(string message, string errorCode = "FORBIDDEN") : base(message)
    {
        ErrorCode = errorCode;
    }
}

public class ConflictException : Exception
{
    public string ErrorCode { get; }

    public ConflictException(string message, string errorCode = "CONFLICT") : base(message)
    {
        ErrorCode = errorCode;
    }
}

public class BusinessRuleException : Exception
{
    public string ErrorCode { get; }

    public BusinessRuleException(string message, string errorCode = "BUSINESS_RULE_ERROR") : base(message)
    {
        ErrorCode = errorCode;
    }
}

