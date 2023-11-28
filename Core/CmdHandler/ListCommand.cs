using Spectre.Console;

/// <summary>
/// This class handles the `list` command.
/// </summary>
public class ListCommand
{
    public int Main(string[] args)
    {
        string? data_dir = null;
        for (int i = 0; i < args.Length; i++)
        {
            try
            {
                switch (args[i])
                {
                    case "-h":
                    case "--help":
                        HelpMenu.GenerateHelpMenu(
                            "list [options]",
                            Info.ListOptions,
                            $"List restic repositories in a {Info.Name} vault."
                        );
                        return 0;

                    case "-d":
                    case "--data":
                        data_dir = args[++i];
                        continue;

                    default:
                        AnsiConsole.Write(new Markup(CLI.Error($"Unknown option: {args[i]}\n")));
                        return 1;
                }
            }
            catch (IndexOutOfRangeException)
            {
                AnsiConsole.Write(new Markup(CLI.Error("Missing argument.\n")));
                return 1;
            }
        }

        try
        {
            var vault = data_dir is null ? new VaultManager() : new VaultManager(data_dir);
            vault.LoadVault();

            if (vault.config.restic_repos.Count == 0)
            {
                AnsiConsole.Write(
                    new Markup(CLI.Note("There are no restic repositories in this vault.\n"))
                );
            }
            else
            {
                var table = new Table();
                table.AddColumn("Vault Repositories");
                foreach (var repo in vault.config.restic_repos)
                    table.AddRow(new Text(repo.repo_name));

                AnsiConsole.Write(table);
            }
            return 0;
        }
        catch (InvalidVaultException e)
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
