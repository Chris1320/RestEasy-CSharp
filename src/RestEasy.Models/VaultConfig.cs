namespace RestEasy.Models;

/// <summary>
/// This record represents the vault configuration.
/// </summary>
public record VaultConfig
{
    public int vault_version { get; init; } = 1;

    // The password used to encrypt the vault. A password is mandatory for all restic repositories.
    // Relevant issues:
    // - https://github.com/restic/restic/issues/1018
    // - https://github.com/restic/restic/issues/1786
    public string vault_password { get; init; }

    // A list of directory names of restic repositories.
    public Dictionary<string, ResticRepoConfig> restic_repos { get; set; }

    // The maximum number of snapshots to keep in each repository.
    // This is overridden by the max_snapshots property of each restic repository.
    public uint default_max_snapshots { get; init; }

    public VaultConfig(
        string vault_password,
        Dictionary<string, ResticRepoConfig> restic_repos,
        uint default_max_snapshots
    )
    {
        this.vault_password = vault_password;
        this.restic_repos = restic_repos;
        this.default_max_snapshots = default_max_snapshots;
    }
}
