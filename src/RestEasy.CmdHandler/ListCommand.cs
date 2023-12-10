using System.ComponentModel;
using RestEasy.API;
using RestEasy.Exceptions;
using RestEasy.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RestEasy.CmdHandler;

/// <summary>
/// This class handles the `list` command.
/// </summary>
public class ListCommand : Command<ListCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Specify the vault directory.")]
        [CommandOption("-v|--vault")]
        public string data_dir { get; init; } = String.Empty;
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        try
        {
            var vault = new VaultManager(settings.data_dir);
            vault.LoadVault();

            if (vault.config.restic_repos.Count == 0)
            {
                AnsiConsole.Write(
                    new Markup(CLIHelper.Note("There are no restic repositories in this vault.\n"))
                );
                return 0;
            }

            var table = new Table();
            table.AddColumn("Repository");
            table.AddColumn("Backup Filepaths");
            table.AddColumn("Maximum Snapshots");
            foreach (var repo in vault.config.restic_repos)
                table.AddRow(
                    new Text(repo.Key),
                    new Text(repo.Value.backup_filepaths.Count.ToString()),
                    new Text(repo.Value.max_snapshots.ToString())
                );

            AnsiConsole.Write(table);
            return 0;
        }
        catch (InvalidVaultException e)
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
