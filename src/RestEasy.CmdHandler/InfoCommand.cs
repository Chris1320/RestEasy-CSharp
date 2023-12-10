using System.ComponentModel;
using RestEasy.API;
using Spectre.Console;
using Spectre.Console.Cli;

public class InfoCommand : Command<InfoCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("The repository to show information about.")]
        [CommandArgument(0, "[repo_name]")]
        public string repo_name { get; init; } = String.Empty;

        [Description("Show all information, including sensitive information.")]
        [CommandOption("-a|--all")]
        [DefaultValue(false)]
        public bool show_all { get; init; }

        [Description("Specify the vault directory.")]
        [CommandOption("-v|--vault")]
        public string data_dir { get; init; } = String.Empty;

        [Description("Specify the path to the restic binary.")]
        [CommandOption("-b|--binary")]
        public string? restic_bin { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var vault = new VaultManager(data_dir: settings.data_dir, restic_bin: settings.restic_bin);
        vault.LoadVault();

        if (String.IsNullOrWhiteSpace(settings.repo_name))
        {
            var grid = new Grid();
            grid.AddColumns(2);

            grid.AddRow(
                    new Text("Vault Location").RightJustified(),
                    new TextPath(vault.data_dir)
                        .RootColor(Color.Default)
                        .SeparatorColor(Color.Grey30)
                        .StemColor(Color.Grey50)
                        .LeafColor(Color.Yellow)
                )
                .AddRow(
                    new Text("Vault Version").RightJustified(),
                    new Text(vault.config.vault_version.ToString())
                )
                .AddRow(
                    new Text("Default Max Snapshots").RightJustified(),
                    new Text(vault.config.default_max_snapshots.ToString())
                )
                .AddRow(
                    new Text("Repositories").RightJustified(),
                    new Text(vault.config.restic_repos.Count.ToString())
                );

            if (settings.show_all)
                grid.AddRow(
                    new Text("Vault Password").RightJustified(),
                    new Text(vault.config.vault_password)
                );

            var panel = new Panel(grid);
            panel.Header = new PanelHeader(
                $"[bold]Information about vault [green]{vault.vault_name}[/][/]",
                alignment: Justify.Center
            );
            AnsiConsole.Write(panel);
            return 0;
        }

        // TODO: Show info about the repo
        return 0;
    }
}
