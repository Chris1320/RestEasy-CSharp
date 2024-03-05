using System.ComponentModel;
using Spectre.Console.Cli;

public class RestoreCommand : Command<RestoreCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("The repositories to restore. (use `*` to restore all repositories)")]
        [CommandArgument(0, "<repositories>")]
        public string[] repositories { get; init; } = Array.Empty<string>();
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        // TODO: Continue this implementation.
        throw new NotImplementedException();
    }
}
