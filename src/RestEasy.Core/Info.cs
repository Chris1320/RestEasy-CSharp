namespace RestEasy.Core;

class Info
{
    public const string Name = "RestEasy";
    public static readonly int[] Version = { 0, 0, 1 };
    public static string Title =
        $"[bold]{Info.Name}[/] [dim italic]v{string.Join(".", Info.Version)}[/]";

    public static string? FileName = Path.GetFileName(System.Environment.ProcessPath);
    public static string ResticExecutableName =
        System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
            System.Runtime.InteropServices.OSPlatform.Windows
        )
            ? "restic.exe"
            : "restic";
}
