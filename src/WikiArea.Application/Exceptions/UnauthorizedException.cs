namespace WikiArea.Application.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class BusinessValidationException : Exception
{
    public BusinessValidationException(string message) : base(message)
    {
    }

    public BusinessValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
} 