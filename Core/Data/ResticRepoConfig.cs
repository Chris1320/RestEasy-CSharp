/// <summary>
/// This record represents the configuration of a restic repository.
/// </summary>
public record ResticRepoConfig
{
    public string repo_name { get; init; } // The name of the restic repository.
    public uint max_snapshots { get; init; } // The maximum number of snapshots to keep in this repository.
    public List<string> backup_filepaths { get; init; } // A list of filepaths to backup.

    public ResticRepoConfig(string repo_name, List<string> backup_filepaths, uint max_snapshots)
    {
        this.repo_name = repo_name;
        this.backup_filepaths = backup_filepaths;
        this.max_snapshots = max_snapshots;
    }
}
