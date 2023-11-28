/// <summary>
/// The class that is responsible for performing all the operations with the restic binary.
/// </summary>
public class ResticManager
{
    public string repo_path { get; }
    public string repo_password { get; }
    public string restic_path { get; }

    /// <param name="repo_path">The path to the repository.</param>
    /// <param name="repo_password">The password for the repository.</param>
    /// <param name="restic_path">The path to the restic binary.</param>
    public ResticManager(string repo_path, string repo_password, string restic_path = "restic")
    {
        this.repo_path = repo_path;
        this.repo_password = repo_password;
        this.restic_path = restic_path;
    }

    /// <summary>
    /// Run a restic command in a repository.
    /// </summary>
    ///
    /// <param name="command">The restic command to run, with its arguments if necessary.</param>
    ///
    /// <returns>The result of the command.</returns>
    private ProcessResult Run(string command)
    {
        var start_info = new System.Diagnostics.ProcessStartInfo()
        {
            FileName = this.restic_path,
            Arguments = $"--json --repo \"{repo_path}\" {command}",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        using (var process = new System.Diagnostics.Process())
        {
            process.StartInfo = start_info;
            process.Start();

            // Write the password to the process' input stream.
            // HACK: Should we use --password-file instead?
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

    /// <summary>
    /// Initialize a repository.
    /// </summary>
    ///
    /// <returns>The result of the command.</returns>
    public ProcessResult Init()
    {
        var result = this.Run("init");

        if (result.exit_code != 0)
            throw new ResticException(result.error);

        return result;
    }
}
