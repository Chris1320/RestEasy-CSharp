using System.ComponentModel;
using RestEasy.API;
using RestEasy.Exceptions;
using RestEasy.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RestEasy.CmdHandler;

/// <summary>
/// This class handles the `backup` command.
/// </summary>
public class BackupCommand : Command<BackupCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("The repositories to back up. (use `*` to backup all repositories)")]
        [CommandArgument(0, "<target>")]
        public string[] targets { get; init; } = Array.Empty<string>();

        [Description("Specify the vault directory.")]
        [CommandOption("-v|--vault")]
        public string data_dir { get; init; } = String.Empty;

        [Description("Specify the path to the restic binary.")]
        [CommandOption("-b|--binary")]
        public string? restic_bin { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        try
        {
            var vault = new VaultManager(settings.data_dir, settings.restic_bin);
            vault.LoadVault();
            var targets = settings.targets[0].Equals("*")
                ? vault.config.restic_repos.Keys.ToArray()
                : settings.targets;

            var backup_statistics = AnsiConsole
                .Progress()
                .Start(ctx =>
                {
                    var task = ctx.AddTask(
                        "Performing backup operation...",
                        maxValue: targets.Length
                    );
                    int failed = 0;
                    int successful = 0;

                    foreach (var target in targets)
                    {
                        try
                        {
                            AnsiConsole.Write(
                                new Markup(CLIHelper.Note($"Backing up `{target}`...\n"))
                            );
                            vault.BackupRepository(target);
                            successful++;
                        }
                        catch (Exception e)
                            when (e is RepositoryNotFoundException || e is ResticException)
                        {
                            AnsiConsole.Write(new Markup(CLIHelper.Error($"{e.Message}\n")));
                            failed++;
                        }
                        catch (Exception e)
                        {
                            AnsiConsole.WriteException(e);
                            failed++;
                        }

                        task.Increment(1);
                    }

                    return (successful, failed);
                });

            AnsiConsole.Write(
                new BreakdownChart()
                    // occupy 50% of the console window
                    .Width((int)(Console.WindowWidth * 0.50))
                    .AddItem("Successful Backups", backup_statistics.successful, Color.Green)
                    .AddItem("Failed Backups", backup_statistics.failed, Color.Red)
            );

            Console.WriteLine("\nDone.");
            return backup_statistics.failed;
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
