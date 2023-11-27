using Spectre.Console;

namespace RestEasy
{
    class Program
    {
        public static int Main(string[] args)
        {
            AnsiConsole.Write(new Markup($"{Info.Title}\n\n"));
            if (args.Length == 0)
            {
                AnsiConsole.Write(
                    new Markup(
                        CLI.Warn(
                            String.Format(
                                "No arguments provided. Run `{0} help` for more information.\n",
                                Info.FileName
                            )
                        )
                    )
                );
                AnsiConsole.Write(new Markup(CLI.Help("Usage: resteasy <command> [options]\n")));
                return 0;
            }

            switch (args[0])
            {
                case "help":
                    AnsiConsole.Write(
                        new Markup(CLI.Help("Usage: resteasy <command> [options]\n\n"))
                    );

                    var command_table = new Table();
                    command_table.AddColumn("Command");
                    command_table.AddColumn("Description");

                    foreach (KeyValuePair<string, string> command in Info.Commands)
                    {
                        command_table.AddRow(command.Key, command.Value);
                    }

                    AnsiConsole.Write(command_table);
                    break;

                default:
                    AnsiConsole.Write(
                        new Markup(
                            CLI.Warn(
                                String.Format(
                                    "Unknown command `{0}`. Run `{1} help` for more information.\n",
                                    args[0],
                                    Info.FileName
                                )
                            )
                        )
                    );
                    AnsiConsole.Write(
                        new Markup(CLI.Help("Usage: resteasy <command> [options]\n"))
                    );
                    return 1;
            }

            return 0;
        }
    }
}
