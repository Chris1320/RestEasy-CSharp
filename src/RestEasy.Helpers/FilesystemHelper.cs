namespace RestEasy.Helpers;

public class FilesystemHelper
{
    /// <summary>
    /// Check if a file is in PATH.
    /// </summary>
    ///
    /// <param name="filename">The file to check.</param>
    ///
    /// <returns>The filepath of the file.</returns>
    public static string? FindExecutable(string filename)
    {
        var paths = Environment.GetEnvironmentVariable("PATH");
        if (paths == null)
            return null;

        foreach (var path in paths.Split(Path.PathSeparator))
        {
            var filepath = Path.Combine(path, filename);
            if (File.Exists(filepath))
                return filepath;
        }

        return null;
    }
}
