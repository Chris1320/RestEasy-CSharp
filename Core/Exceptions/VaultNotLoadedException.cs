public class VaultNotLoadedException : Exception
{
    public VaultNotLoadedException() { }

    public VaultNotLoadedException(string message)
        : base(message) { }

    public VaultNotLoadedException(string message, Exception inner)
        : base(message, inner) { }
}
