using RestEasy.Core;
using RestEasy.Exceptions;
using RestEasy.Helpers;
using RestEasy.Models;

namespace RestEasy.API;

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
    public ResticManager(string repo_path, string repo_password, string? restic_path = null)
    {
        this.repo_path = repo_path;
        this.repo_password = repo_password;
        this.restic_path = File.Exists(restic_path)
            ? restic_path
            : null // Should we throw an exception instead of falling back to the default restic path?
                ?? FilesystemHelper.FindExecutable(Info.ResticExecutableName)
                ?? throw new ResticException("Could not find restic executable.");
    }

    /// <summary>
    /// Run a restic command in a repository.
    /// </summary>
    ///
    /// <param name="command">
    /// The restic command to run, with its arguments if necessary.
    /// WARNING: this is not sanitized.
    /// </param>
    ///
    /// <returns>The result of the command.</returns>
    private ProcessResult Run(string command)
    {
        var start_info = new System.Diagnostics.ProcessStartInfo()
        {
            FileName = this.restic_path,
            Arguments = $"--json --repo \"{this.repo_path}\" {command}",
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

    /// <summary>
    /// Backup a list of files and/or directories.
    /// </summary>
    ///
    /// <param name="backup_filepaths">The list of files and/or directories to back up.</param>
    /// <param name="tags">The tags to apply to the backup.</param>
    ///
    /// <returns>The result of the command.</returns>
    public ProcessResult Backup(List<string> backup_filepaths, List<string>? tags = null)
    {
        tags ??= new List<string> { $"{Info.Name}-v{string.Join(".", Info.Version)}" };
        var result = this.Run(
            $"backup --tag \"{string.Join(",", tags)}\" {string.Join(" ", backup_filepaths.Select(filepath => $"\"{filepath}\""))}"
        );
        if (result.exit_code != 0)
            throw new ResticException(result.error);

        return result;
    }
}
