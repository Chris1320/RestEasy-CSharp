using System.ComponentModel;
using RestEasy.API;
using RestEasy.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;

public class RemoveCommand : Command<RemoveCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("The repositories to remove.")]
        [CommandArgument(0, "<repositories>")]
        public string[] repositories { get; init; } = Array.Empty<string>();

        [Description("Do not ask for confirmation.")]
        [CommandOption("-y|--yes")]
        public bool assume_yes { get; init; } = false;

        [Description("Specify the vault directory.")]
        [CommandOption("-v|--vault")]
        public string data_dir { get; init; } = String.Empty;
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        int[] statistics = { 0, 0, 0 }; // success, skipped, failed
        var vault = new VaultManager(data_dir: settings.data_dir);
        vault.LoadVault();

        foreach (var repo_name in settings.repositories)
        {
            // Prepare for removal.
            if (!vault.config.restic_repos.ContainsKey(repo_name))
            {
                AnsiConsole.MarkupLine(
                    CLIHelper.Error(
                        $"The repository [blue]{repo_name}[/] does not exist in the vault.",
                        escape_markup: false
                    )
                );
                statistics[2]++;
                continue;
            }

            // Ask user for confirmation if assume_yes is false.
            if (!settings.assume_yes)
            {
                if (
                    AnsiConsole
                        .Prompt(
                            new SelectionPrompt<string>()
                                .Title(
                                    $"Are you sure you want to [bold red]remove[/] the [blue]{repo_name}[/] repository?"
                                )
                                .AddChoices(new string[] { "Yes", "No" })
                        )
                        .CompareTo("No") == 0
                )
                {
                    AnsiConsole.MarkupLine(
                        CLIHelper.Warn(
                            $"The repository [blue]{repo_name}[/] has been [yellow]skipped[/].",
                            escape_markup: false
                        )
                    );
                    statistics[1]++;
                    continue;
                }
            }

            // Start the removal process.
            AnsiConsole
                .Status()
                .Start(
                    $"Removing the [blue]{repo_name}[/] repository...",
                    ctx =>
                    {
                        try
                        {
                            vault.RemoveRepository(repo_name);
                            AnsiConsole.MarkupLine(
                                CLIHelper.Note(
                                    $"The repository [blue]{repo_name}[/] has been [red]removed[/].",
                                    escape_markup: false
                                )
                            );
                            statistics[0]++;
                        }
                        catch (Exception e)
                        {
                            AnsiConsole.MarkupLine(
                                CLIHelper.Error(
                                    $"Failed to remove the repository [blue]{repo_name}[/]: {e.Message}",
                                    escape_markup: false
                                )
                            );
                            statistics[2]++;
                        }
                    }
                );
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(
            new BreakdownChart()
                // occupy 50% of the console window
                .Width((int)(Console.WindowWidth * 0.50))
                .AddItem("Successful", statistics[0], Color.Green)
                .AddItem("Skipped", statistics[1], Color.Yellow)
                .AddItem("Failed", statistics[2], Color.Red)
        );
        return statistics[2];
    }
}
