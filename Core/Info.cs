class Info
{
    public const string Name = "RestEasy";
    public static readonly int[] Version = { 0, 0, 1 };
    public static string Title =
        $"[bold]{Info.Name}[/] [dim italic]v{string.Join(".", Info.Version)}[/]";

    public static string? FileName = Path.GetFileName(System.Environment.ProcessPath);
    public static Dictionary<string, string> Commands = new Dictionary<string, string>()
    {
        { "help", "Show help information." },
        { "init", $"Initialize a new {Info.Name} vault." },
        { "new", $"Create a new restic repository within an existing {Info.Name} vault." },
        { "remove", $"Remove an existing repository from an existing {Info.Name} vault." },
        { "list", "List all existing repositories." },
    };
}
