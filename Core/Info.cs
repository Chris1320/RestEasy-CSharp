class Info
{
    public const string Name = "RestEasy";
    public static readonly int[] Version = { 0, 0, 1 };
    public static string Title =
        $"[bold]{Info.Name}[/] [dim italic]v{string.Join(".", Info.Version)}[/]";

    public static string? FileName = Path.GetFileName(System.Environment.ProcessPath);

    // For help menus
    public static Dictionary<string, string> Commands = new Dictionary<string, string>()
    {
        { "help", "Show help information." },
        { "init", $"Initialize a new {Info.Name} vault." },
        { "add", $"Add a new restic repository within an existing {Info.Name} vault." },
        { "remove", $"Remove an existing repository from an existing {Info.Name} vault." },
        { "list", $"List restic repositories in a {Info.Name} vault." },
        { "info", $"Show information about a {Info.Name} vault or a specific restic repository." },
        { "backup", $"Perform a backup of a single, a group of, or all restic repositories." },
    };
    public static Dictionary<string, string> InitOptions = new Dictionary<string, string>()
    {
        { "-h, --help", "Show help information." },
        { "-v, --vault", "Specify the vault directory." },
        { "-p, --password", "Specify the password for the repositories in the vault." },
        { "-s, --snapshots", "Specify how many snapshots to keep by default." },
    };
    public static Dictionary<string, string> AddOptions = new Dictionary<string, string>()
    {
        { "-h, --help", "Show help information." },
        { "-v, --vault", "Specify the vault directory." },
        { "-n, --name", "Specify the name of the repository." },
        { "-s, --snapshots", "Specify how many snapshots to keep in this repository." },
        { "-b, --binary", "Specify the path to the restic binary." },
    };
    public static Dictionary<string, string> ListOptions = new Dictionary<string, string>()
    {
        { "-h, --help", "Show help information." },
        { "-v, --vault", "Specify the vault directory." },
    };
}
