using System.ComponentModel;
using RestEasy.API;
using RestEasy.Exceptions;
using RestEasy.Helpers;
using RestEasy.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RestEasy.CmdHandler;

/// <summary>
/// This class handles the `add` command.
/// </summary>
public class AddCommand : Command<AddCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("The filepaths to backup.")]
        [CommandArgument(0, "<filepaths>")]
        public string[] backup_filepaths { get; init; } = Array.Empty<string>();

        [Description("Specify the vault directory.")]
        [CommandOption("-v|--vault")]
        public string data_dir { get; init; } = String.Empty;

        [Description("Specify the name of the repository.")]
        [CommandOption("-n|--name")]
        public string? repo_name { get; set; }

        [Description("Specify how many snapshots to keep in this repository.")]
        [CommandOption("-s|--snapshots")]
        public uint? max_snapshots { get; set; }

        [Description("Specify the path to the restic binary.")]
        [CommandOption("-b|--binary")]
        public string? restic_bin { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        if (settings.backup_filepaths.Length == 0)
        {
            AnsiConsole.Write(
                new Markup(CLIHelper.Error("There should be at least one filepath to back up.\n"))
            );
            return 1;
        }

        // Infer repository name from the first backup filepath.
        settings.repo_name = String.IsNullOrEmpty(settings.repo_name)
            ? Path.GetFileName(settings.backup_filepaths[0])
            : settings.repo_name;

        if (!Validator.ValidateVaultName(settings.repo_name))
        {
            AnsiConsole.Write(
                new Markup(
                    CLIHelper.Error($"The repository name `{settings.repo_name}` is invalid.\n")
                )
            );
            return 1;
        }

        // Add the repository to the vault.
        var vault = new VaultManager(settings.data_dir, settings.restic_bin);
        try
        {
            vault.LoadVault();
            vault.AddRepository(
                settings.repo_name,
                new ResticRepoConfig(
                    new List<string>(settings.backup_filepaths),
                    settings.max_snapshots ?? vault.config.default_max_snapshots
                )
            );

            AnsiConsole.Write(
                new Markup(
                    CLIHelper.Note($"Repository `{settings.repo_name}` added successfully.\n")
                )
            );
            return 0;
        }
        catch (Exception e) when (e is InvalidVaultException || e is ArgumentException)
        {
            AnsiConsole.Write(new Markup(CLIHelper.Error($"{e.Message}\n")));
            return 1;
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
            return 1;
        }
    }
}
