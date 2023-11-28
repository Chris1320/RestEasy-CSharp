/// <summary>
/// This record represents the configuration of a restic repository.
/// </summary>
public record ResticRepoConfig(
    string repo_name, // The name of the restic repository.
    string[] backup_filepaths, // A list of filepaths to backup.
    int max_snapshots // The maximum number of snapshots to keep in this repository.
);
