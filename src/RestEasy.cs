using RestEasy.CmdHandler;
using RestEasy.Core;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RestEasy;

class RestEasy
{
    public static int Main(string[] args)
    {
        AnsiConsole.Write(new Markup($"{Info.Title}\n\n"));
        var cmd_handler = new CommandApp();
        cmd_handler.Configure(config =>
        {
            config.SetApplicationName(Info.Name);
            config.SetApplicationVersion(string.Join(".", Info.Version));

            config
                .AddCommand<InitCommand>("init")
                .WithDescription($"Initialize a new {Info.Name} vault.")
                .WithExample(
                    new string[] { "init", "--vault", "~/backups/desktop1/resteasy/vault" }
                );
            config
                .AddCommand<AddCommand>("add")
                .WithDescription(
                    $"Add a new restic repository within an existing {Info.Name} vault."
                )
                .WithExample(
                    new string[] { "add", "--name", "test-repo", "foo.txt", "bar.png", "baz/" }
                );
            config
                .AddCommand<ListCommand>("list")
                .WithDescription($"List restic repositories in a {Info.Name} vault.")
                .WithExample(
                    new string[] { "list", "--vault", "~/backups/desktop1/resteasy/vault" }
                );
            config
                .AddCommand<InfoCommand>("info")
                .WithDescription("Get information about a restic repository.")
                .WithExample(new string[] { "info", "repo-A" });
            config
                .AddCommand<BackupCommand>("backup")
                .WithDescription(
                    "Perform a backup of a single, a group of, or all restic repositories."
                )
                .WithExample(new string[] { "backup", "repo-A", "repo-B", "repo-C" });
            config
                .AddCommand<RestoreCommand>("restore")
                .WithDescription(
                    "Restore a backup of a single, a group of, or all restic repositories."
                )
                .WithExample(new string[] { "restore", "repo-A", "repo-B", "repo-C" });
            config
                .AddCommand<RemoveCommand>("remove")
                .WithDescription("Remove one or more restic repositories.")
                .WithExample(new string[] { "remove", "repo-A", "repo-B", "group-A" });
        });

        return cmd_handler.Run(args);
    }
}
