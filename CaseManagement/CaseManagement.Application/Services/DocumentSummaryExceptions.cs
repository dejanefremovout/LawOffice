namespace CaseManagement.Application.Services;

public sealed class AiQuotaExceededException : Exception
{
    public AiQuotaExceededException(string message) : base(message)
    {
    }
}

public sealed class AiInputTooLargeException : Exception
{
    public AiInputTooLargeException(string message) : base(message)
    {
    }
}
