using Spectre.Console;

class InitCommand
{
    public int Main(string[] args)
    {
        var config = new VaultConfig();
        for (int i = 0; i < args.Length; i++)
        {
            try
            {
                switch (args[i])
                {
                    case "-h":
                    case "--help":
                        HelpMenu.GenerateHelpMenu("init [options]", Info.InitOptions);
                        return 0;

                    case "-d":
                    case "--data":
                        config.data_dir = args[++i];
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

        Console.WriteLine($"Data Dir: {config.data_dir}");
        return 0;
    }
}
