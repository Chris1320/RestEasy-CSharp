public record ProcessResult(int exit_code, string output, string error);

public class ShellHandler
{
    /// <summary>
    /// Run a restic command.
    /// </summary>
    ///
    /// <param name="repo_path">The path to the repository.</param>
    /// <param name="repo_password">The password for the repository.</param>
    /// <param name="command">The command to run.</param>
    /// <returns>A ProcessResult object.</returns>
    public static ProcessResult RunRestic(string repo_path, string repo_password, string command)
    {
        // FIXME: This should be in the ResticManager.cs file.
        var start_info = new System.Diagnostics.ProcessStartInfo()
        {
            FileName = "restic",
            Arguments = $"--repo \"{repo_path}\" {command}",
            CreateNoWindow = true,
            // WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        using (var process = new System.Diagnostics.Process())
        {
            process.StartInfo = start_info;
            process.Start();

            using (var stream_input = process.StandardInput)
            {
                if (stream_input.BaseStream.CanWrite)
                    stream_input.WriteLine(repo_password);
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return new ProcessResult(process.ExitCode, output, error);
        }
    }
}
