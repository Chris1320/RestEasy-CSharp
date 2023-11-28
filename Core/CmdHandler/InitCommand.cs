using Spectre.Console;

/// <summary>
/// This class handles the `init` command.
/// </summary>
class InitCommand
{
    public int Main(string[] args)
    {
        uint? max_snapshots = null;
        string? data_dir = null;
        string vault_password = String.Empty;
        for (int i = 0; i < args.Length; i++)
        {
            try
            {
                switch (args[i])
                {
                    case "-h":
                    case "--help":
                        HelpMenu.GenerateHelpMenu(
                            "init [options]",
                            Info.InitOptions,
                            "Initialize a new RestEasy vault."
                        );
                        return 0;

                    case "-d":
                    case "--data":
                        data_dir = args[++i];
                        continue;

                    case "-p":
                    case "--password":
                        vault_password = args[++i];
                        continue;

                    case "-s":
                    case "--snapshots":
                        try
                        {
                            max_snapshots = uint.Parse(args[++i]);
                            continue;
                        }
                        catch (OverflowException)
                        {
                            AnsiConsole.Write(
                                new Markup(
                                    CLI.Error(
                                        $"The number of snapshots must be between 0 and {uint.MaxValue}.\n"
                                    )
                                )
                            );
                            return 1;
                        }

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

        VaultManager vault;
        if (data_dir == null)
            vault = new VaultManager();
        else
            vault = new VaultManager(data_dir);

        try
        {
            vault.CreateVault(vault_password, max_snapshots);
        }
        catch (Exception e)
        {
            AnsiConsole.Write(new Markup(CLI.Error($"{e.Message}\n")));
            return 1;
        }
        AnsiConsole.Write(
            new Markup(CLI.Note($"The vault was created successfully in `{vault.data_dir}`.\n"))
        );

        return 0;
    }
}
