using System.ComponentModel;
using Spectre.Console.Cli;

public class RemoveCommand : Command<RemoveCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("The repositories to remove.")]
        [CommandArgument(0, "<repositories>")]
        public string[] repositories { get; init; } = Array.Empty<string>();
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        // TODO: Continue this implementation.
        throw new NotImplementedException();
    }
}
