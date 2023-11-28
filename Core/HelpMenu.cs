using Spectre.Console;

public class HelpMenu
{
    /// <summary>
    /// A method for generating and printing out a help menu.
    /// </summary>
    ///
    /// <param name="usage">A string showing how to use the command.</param>
    /// <param name="commands">A dictionary of commands and their descriptions.</param>
    public static void GenerateHelpMenu(string usage, Dictionary<string, string> commands)
    {
        AnsiConsole.Write(new Markup(CLI.Help($"Usage: {Info.FileName} {usage}\n\n")));

        var command_table = new Table();
        command_table.AddColumn("Command");
        command_table.AddColumn("Description");

        foreach (KeyValuePair<string, string> command in commands)
        {
            command_table.AddRow(command.Key, command.Value);
        }

        AnsiConsole.Write(command_table);
    }
}
