namespace Leasing.Domain.Exceptions;

[Serializable]
public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException()
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public static void ThrowIfNull(object? argument, string message)
    {
        if (argument == null)
        {
            throw new NotFoundException(message);
        }
    }
}