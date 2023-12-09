namespace RestEasy.Models;

/// <summary>
/// This record represents the configuration of a restic repository.
/// </summary>
public record ResticRepoConfig
{
    public List<string> backup_filepaths { get; init; } // A list of filepaths to backup.
    public uint max_snapshots { get; init; } // The maximum number of snapshots to keep in this repository.

    public ResticRepoConfig(List<string> backup_filepaths, uint max_snapshots)
    {
        this.backup_filepaths = backup_filepaths;
        this.max_snapshots = max_snapshots;
    }
}
