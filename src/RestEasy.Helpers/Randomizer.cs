using System.Text;

namespace RestEasy.Helpers;

public class Randomizer
{
    private Random random = new Random();
    public string charset { get; }

    /// <param name="charset">The charset to use for generating random strings.</param>
    public Randomizer(
        string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
    )
    {
        this.charset = charset;
    }

    /// <summary>
    /// Generate a random string with the specified length.
    /// </summary>
    ///
    /// <param name="length">The length of the random string.</param>
    ///
    /// <returns>A random string.</returns>
    public string GenerateRandomString(uint length)
    {
        var result = new StringBuilder();

        for (uint i = 0; i < length; i++)
            result.Append(this.charset[this.random.Next() % this.charset.Length]);

        return result.ToString();
    }
}
