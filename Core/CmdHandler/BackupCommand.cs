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
            int failed_backups = 0;
            var vault = new VaultManager(data_dir, restic_bin);
            vault.LoadVault();

            foreach (var target in targets)
            {
                try
                {
                    AnsiConsole.Write(new Markup(CLI.Note($"Backing up `{target}`...\n")));
                    vault.BackupRepository(target);
                }
                catch (Exception e) when (e is RepositoryNotFoundException || e is ResticException)
                {
                    AnsiConsole.Write(new Markup(CLI.Error($"{e.Message}\n")));
                    failed_backups++;
                }
                catch (Exception e)
                {
                    AnsiConsole.WriteException(e);
                    failed_backups++;
                }
            }

            if (failed_backups > 0)
            {
                AnsiConsole.Write(
                    new Markup(
                        CLI.Error($"{failed_backups} out of {targets.Count} backups failed.\n")
                    )
                );
            }

            Console.WriteLine("\nDone.");
            return failed_backups;
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
