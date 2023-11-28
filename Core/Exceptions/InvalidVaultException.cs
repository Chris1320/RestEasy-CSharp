public class InvalidVaultException : Exception
{
    public InvalidVaultException() { }

    public InvalidVaultException(string message)
        : base(message) { }

    public InvalidVaultException(string message, Exception inner)
        : base(message, inner) { }
}
