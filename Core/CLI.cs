using Spectre.Console;

class CLI
{
    public static string Note(string str)
    {
        return $"[gray][[[/][green]i[/][gray]]][/] {str.EscapeMarkup()}";
    }

    public static string Warn(string str)
    {
        return $"[gray][[[/][yellow]![/][gray]]][/] {str.EscapeMarkup()}";
    }

    public static string Error(string str)
    {
        return $"[gray][[[/][red]E[/][gray]]][/] {str.EscapeMarkup()}";
    }

    public static string Help(string str)
    {
        return $"[gray][[[/][blue]?[/][gray]]][/] {str.EscapeMarkup()}";
    }
}
