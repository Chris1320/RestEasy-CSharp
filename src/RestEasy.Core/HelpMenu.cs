using RestEasy.Helpers;
using Spectre.Console;

namespace RestEasy.Core;

public class HelpMenu
{
    /// <summary>
    /// A method for generating and printing out a help menu.
    /// </summary>
    ///
    /// <param name="usage">A string showing how to use the command.</param>
    /// <param name="commands">A dictionary of commands and their descriptions.</param>
    /// <param name="description">
    ///     An optional description of the command.
    ///     The table will print out "Option" if this is not empty.
    /// </param>
    public static void GenerateHelpMenu(
        string usage,
        Dictionary<string, string> commands,
        string description = ""
    )
    {
        if (!string.IsNullOrEmpty(description))
            AnsiConsole.Write(new Markup(CLIHelper.Help($"{description}\n")));

        AnsiConsole.Write(
            new Markup(
                CLIHelper.Help(
                    String.Format(
                        "[dim italic]Usage:[/] [underline]{0} {1}[/]\n\n",
                        Info.FileName.EscapeMarkup(),
                        usage.EscapeMarkup()
                    ),
                    false
                )
            )
        );

        var command_table = new Table();
        command_table.AddColumn(String.IsNullOrEmpty(description) ? "Command" : "Option");
        command_table.AddColumn("Description");

        foreach (KeyValuePair<string, string> command in commands)
            command_table.AddRow(command.Key, command.Value);

        AnsiConsole.Write(command_table);
    }
}
