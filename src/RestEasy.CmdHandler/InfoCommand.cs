using System.ComponentModel;
using Spectre.Console.Cli;

public class InfoCommand : Command<InfoCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("The repository to show information about.")]
        [CommandArgument(0, "<repo_name>")]
        public string repo_name { get; init; } = String.Empty;
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        Console.WriteLine($"Showing information about {settings.repo_name}");
        return 0;
    }
}
