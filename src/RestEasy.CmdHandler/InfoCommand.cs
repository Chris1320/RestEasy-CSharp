using System.ComponentModel;
using RestEasy.API;
using RestEasy.Helpers;
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
            var vault_info = new Grid();
            vault_info.AddColumns(2);

            vault_info
                .AddRow(
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
                vault_info.AddRow(
                    new Text("Vault Password").RightJustified(),
                    new Text(vault.config.vault_password)
                );

            var vault_info_panel = new Panel(vault_info);
            vault_info_panel.Header = new PanelHeader(
                $"[bold]Information about vault [green]{vault.vault_name}[/][/]",
                alignment: Justify.Center
            );
            AnsiConsole.Write(vault_info_panel);

            if (vault.config.restic_repos.Count != 0)
            {
                var repo_list = new Columns(
                    vault
                        .config
                        .restic_repos
                        .Keys
                        .Select(repo => new Text(repo, new Style(foreground: Color.Blue)))
                ).Padding(2, 0, 2, 0);
                var repo_list_panel = new Panel(repo_list);
                repo_list_panel.Expand = true;
                repo_list_panel.Header = new PanelHeader(
                    $"[bold]Repositories in vault [green]{vault.vault_name}[/][/]",
                    alignment: Justify.Center
                );
                AnsiConsole.Write(repo_list_panel);
            }
            return 0;
        }

        if (!vault.config.restic_repos.ContainsKey(settings.repo_name))
        {
            AnsiConsole.Write(
                new Markup(CLIHelper.Error("The repository does not exist in this vault.\n"))
            );
            return 1;
        }

        var repo_info = new Grid();
        repo_info.AddColumns(2);

        repo_info
            .AddRow(
                new Text("Repository Location").RightJustified(),
                new TextPath(Path.Combine(vault.repos_dir, settings.repo_name))
                    .RootColor(Color.Default)
                    .SeparatorColor(Color.Grey30)
                    .StemColor(Color.Grey50)
                    .LeafColor(Color.Yellow)
            )
            .AddRow(
                new Text("Maximum Snapshots").RightJustified(),
                new Text(vault.config.restic_repos[settings.repo_name].max_snapshots.ToString())
            );
        var repo_info_panel = new Panel(repo_info);
        repo_info_panel.Header = new PanelHeader(
            $"[bold]Information about repository [green]{settings.repo_name}[/] from vault [blue]{vault.vault_name}[/][/]",
            alignment: Justify.Center
        );

        var bkfp_list = new Columns(
            vault
                .config
                .restic_repos[settings.repo_name]
                .backup_filepaths
                .Select(
                    fp =>
                        new TextPath(fp)
                            .RootColor(Color.Default)
                            .SeparatorColor(Color.Grey30)
                            .StemColor(Color.Blue)
                            .LeafColor(Color.Yellow)
                )
        ).Padding(2, 0, 2, 0);
        var bkfp_list_panel = new Panel(bkfp_list);
        bkfp_list_panel.Expand = true;
        bkfp_list_panel.Header = new PanelHeader(
            $"[bold]Backup filepaths in repository [green]{settings.repo_name}[/] from vault [blue]{vault.vault_name}[/][/]",
            alignment: Justify.Center
        );

        AnsiConsole.Write(repo_info_panel);
        AnsiConsole.Write(bkfp_list_panel);
        return 0;
    }
}
