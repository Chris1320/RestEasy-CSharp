using Spectre.Console;

/// <summary>
/// This class handles the `add` command.
/// </summary>
public class AddCommand
{
    public int Main(string[] args)
    {
        string repo_name = String.Empty;
        var backup_filepaths = new List<string>();

        string data_dir = String.Empty;
        string? restic_bin = null;
        uint? max_snapshots = null;

        for (int i = 0; i < args.Length; i++)
        {
            try
            {
                switch (args[i])
                {
                    case "-h":
                    case "--help":
                        HelpMenu.GenerateHelpMenu(
                            "add <filepaths> [options]",
                            Info.AddOptions,
                            $"Add a new restic repository within an existing {Info.Name} vault."
                        );
                        return 0;

                    case "-v":
                    case "--vault":
                        data_dir = args[++i];
                        continue;

                    case "-n":
                    case "--name":
                        repo_name = args[++i];
                        continue;

                    case "-s":
                    case "--snapshots":
                        max_snapshots = uint.Parse(args[++i]);
                        continue;

                    case "-b":
                    case "--binary":
                        restic_bin = args[++i];
                        continue;

                    default:
                        backup_filepaths.Add(args[i]);
                        continue;
                }
            }
            catch (IndexOutOfRangeException)
            {
                AnsiConsole.Write(new Markup(CLI.Error("Missing argument.\n")));
                return 1;
            }
        }

        if (backup_filepaths.Count == 0)
        {
            AnsiConsole.Write(
                new Markup(CLI.Error("There should be at least one filepath to back up.\n"))
            );
            return 1;
        }

        // Infer repository name from the first backup filepath.
        repo_name = String.IsNullOrEmpty(repo_name)
            ? Path.GetFileName(backup_filepaths[0])
            : repo_name;

        if (!Validator.ValidateVaultName(repo_name))
        {
            AnsiConsole.Write(
                new Markup(CLI.Error($"The repository name `{repo_name}` is invalid.\n"))
            );
            return 1;
        }

        // Add the repository to the vault.
        var vault = new VaultManager(data_dir, restic_bin);
        try
        {
            vault.LoadVault();
            vault.AddRepository(
                new ResticRepoConfig(
                    repo_name,
                    backup_filepaths,
                    max_snapshots ?? vault.config.default_max_snapshots
                )
            );
            AnsiConsole.Write(
                new Markup(CLI.Note($"Repository `{repo_name}` added successfully.\n"))
            );
            return 0;
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
