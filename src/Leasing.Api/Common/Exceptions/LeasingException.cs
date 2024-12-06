namespace Leasing.Api.Common.Exceptions;

[Serializable]
public class LeasingException : Exception
{
    public LeasingException(string message)
        : base(message)
    {
    }

    public LeasingException()
    {
    }

    public LeasingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}