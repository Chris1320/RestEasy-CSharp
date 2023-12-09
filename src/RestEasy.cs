using RestEasy.CmdHandler;
using RestEasy.Core;
using RestEasy.Helpers;
using Spectre.Console;

namespace RestEasy;

class RestEasy
{
    public static int Main(string[] args)
    {
        AnsiConsole.Write(new Markup($"{Info.Title}\n\n"));
        if (args.Length == 0)
        {
            AnsiConsole.Write(new Markup(CLIHelper.Warn(String.Format("No arguments provided\n"))));
            HelpMenu.GenerateHelpMenu("<command> [options]", Info.Commands);
            return 0;
        }

        switch (args[0])
        {
            case "help":
                HelpMenu.GenerateHelpMenu("<command> [options]", Info.Commands);
                return 0;

            case "init":
                return new InitCommand().Main(args[1..]);

            case "add":
                return new AddCommand().Main(args[1..]);

            case "remove":
                // TODO: Implement remove command.
                AnsiConsole.Write(new Markup(CLIHelper.Error("Not implemented yet.\n")));
                return 2;

            case "list":
                return new ListCommand().Main(args[1..]);

            case "info":
                // TODO: Implement info command.
                AnsiConsole.Write(new Markup(CLIHelper.Error("Not implemented yet.\n")));
                return 2;

            case "backup":
                return new BackupCommand().Main(args[1..]);

            default:
                AnsiConsole.Write(new Markup(CLIHelper.Warn($"Unknown command `{args[0]}`.\n")));
                HelpMenu.GenerateHelpMenu("<command> [options]", Info.Commands);
                return 1;
        }
    }
}
