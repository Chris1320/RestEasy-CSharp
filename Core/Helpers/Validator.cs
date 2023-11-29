public class Validator
{
    /// <summary>
    /// Check if the given vault name is valid.
    /// </summary>
    ///
    /// <param name="vaultname">The vault name to check.</param>
    ///
    /// <returns>True if the vault name is valid, false otherwise.</returns>
    public static bool ValidateVaultName(string vaultname)
    {
        string allowed_chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
        return !String.IsNullOrWhiteSpace(vaultname)
            && vaultname.All(c => allowed_chars.Contains(c));
    }
}
