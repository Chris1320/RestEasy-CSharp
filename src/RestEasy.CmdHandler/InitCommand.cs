using System.ComponentModel;
using RestEasy.API;
using RestEasy.Exceptions;
using RestEasy.Helpers;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RestEasy.CmdHandler;

/// <summary>
/// This class handles the `init` command.
/// </summary>
class InitCommand : Command<InitCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Specify how many snapshots to keep by default.")]
        [CommandOption("-s|--snapshots")]
        public uint? max_snapshots { get; set; }

        [Description("Specify the vault directory.")]
        [CommandOption("-v|--vault")]
        public string? data_dir { get; init; }

        [Description("Specify the password for the repositories in the vault.")]
        [CommandOption("-p|--password")]
        public string vault_password { get; set; } = String.Empty;
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        VaultManager vault =
            settings.data_dir == null ? new VaultManager() : new VaultManager(settings.data_dir);

        try
        {
            vault.CreateVault(settings.vault_password, settings.max_snapshots);
        }
        catch (VaultAlreadyExists e)
        {
            AnsiConsole.Write(new Markup(CLIHelper.Error($"{e.Message}\n")));
            return 1;
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
            return 1;
        }
        AnsiConsole.Write(
            new Markup(
                CLIHelper.Note($"The vault was created successfully in `{vault.data_dir}`.\n")
            )
        );

        return 0;
    }
}
