using Spectre.Console;

/// <summary>
/// This class handles the `backup` command.
/// </summary>
public class BackupCommand
{
    public int Main(string[] args)
    {
        var targets = new List<string>();
        string data_dir = String.Empty;
        string? restic_bin = null;

        for (int i = 0; i < args.Length; i++)
        {
            try
            {
                switch (args[i])
                {
                    case "-h":
                    case "--help":
                        HelpMenu.GenerateHelpMenu(
                            "backup <target> [options]",
                            Info.BackupOptions,
                            "Perform a backup of a single, a group of, or all restic repositories."
                        );
                        return 0;

                    case "-v":
                    case "--vault":
                        data_dir = args[++i];
                        continue;

                    case "-b":
                    case "--binary":
                        restic_bin = args[++i];
                        continue;

                    default:
                        targets.Add(args[i]);
                        continue;
                }
            }
            catch (IndexOutOfRangeException)
            {
                AnsiConsole.Write(new Markup(CLI.Error("Missing argument.\n")));
                return 1;
            }
        }

        if (targets.Count == 0)
        {
            AnsiConsole.Write(
                new Markup(CLI.Error("There should be at least one target to back up.\n"))
            );
            return 1;
        }

        try
        {
            var backup_statistics = AnsiConsole
                .Progress()
                .Start(ctx =>
                {
                    var task = ctx.AddTask(
                        "Performing backup operation...",
                        maxValue: targets.Count
                    );
                    var vault = new VaultManager(data_dir, restic_bin);
                    int failed = 0;
                    int successful = 0;
                    vault.LoadVault();

                    foreach (var target in targets)
                    {
                        try
                        {
                            AnsiConsole.Write(new Markup(CLI.Note($"Backing up `{target}`...\n")));
                            vault.BackupRepository(target);
                            successful++;
                        }
                        catch (Exception e)
                            when (e is RepositoryNotFoundException || e is ResticException)
                        {
                            AnsiConsole.Write(new Markup(CLI.Error($"{e.Message}\n")));
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
            AnsiConsole.Write(new Markup(CLI.Error($"{e.Message}\n")));
            return 1;
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
            return 1;
        }
    }
}
