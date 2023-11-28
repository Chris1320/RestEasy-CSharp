public class VaultAlreadyExists : Exception
{
    public VaultAlreadyExists() { }

    public VaultAlreadyExists(string message)
        : base(message) { }

    public VaultAlreadyExists(string message, Exception inner)
        : base(message, inner) { }
}
