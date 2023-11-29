public class RepositoryNotFoundException : Exception
{
    public RepositoryNotFoundException() { }

    public RepositoryNotFoundException(string message)
        : base(message) { }

    public RepositoryNotFoundException(string message, Exception inner)
        : base(message, inner) { }
}
