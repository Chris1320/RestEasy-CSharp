using Spectre.Console;

/// <summary>
/// A helper class that contains methods for formatting CLI output.
/// </summary>
class CLI
{
    public static string Note(string str, bool escape_markup = true) =>
        $"[gray][[[/][green]i[/][gray]]][/] {(escape_markup ? str.EscapeMarkup() : str)}";

    public static string Warn(string str, bool escape_markup = true) =>
        $"[gray][[[/][yellow]![/][gray]]][/] {(escape_markup ? str.EscapeMarkup() : str)}";

    public static string Error(string str, bool escape_markup = true) =>
        $"[gray][[[/][red]E[/][gray]]][/] {(escape_markup ? str.EscapeMarkup() : str)}";

    public static string Help(string str, bool escape_markup = true) =>
        $"[gray][[[/][cyan]?[/][gray]]][/] {(escape_markup ? str.EscapeMarkup() : str)}";
}
