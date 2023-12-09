namespace RestEasy.Exceptions;

public class ResticException : Exception
{
    public ResticException() { }

    public ResticException(string message)
        : base(message) { }

    public ResticException(string message, Exception inner)
        : base(message, inner) { }
}
