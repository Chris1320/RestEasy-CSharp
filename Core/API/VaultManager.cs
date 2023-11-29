using System.Text.Json;

class VaultManager
{
    public const uint MAX_SNAPSHOTS = 5; // Default maximum number of snapshots to keep
    public const uint VAULT_PASSWORD_LENGTH = 16; // Default vault password length

    // Default data directory
    public string data_dir { get; } = Path.Combine(System.Environment.CurrentDirectory, "vault");
    private string? restic_bin = null;
    private VaultConfig? _config = null; // Will only be assigned a value when the vault is loaded.

    public string vault_name
    {
        get { return Path.GetFileName(this.data_dir); }
    }
    public string repos_dir
    {
        get { return Path.Combine(this.data_dir, "repos"); }
    }
    public string config_path
    {
        get { return Path.Combine(this.data_dir, "config.json"); }
    }
    public VaultConfig config // provided for outside use.
    {
        get
        {
            if (this._config == null)
                throw new VaultNotLoadedException("The vault has not been loaded yet.");

            return this._config;
        }
    }

    /// <param name="data_dir">The path to the data directory.</param>
    public VaultManager(string data_dir = "", string? restic_bin = null)
    {
        this.restic_bin = restic_bin;
        if (!String.IsNullOrEmpty(data_dir))
            this.data_dir = Path.IsPathRooted(data_dir)
                ? data_dir
                : Path.Combine(System.Environment.CurrentDirectory, data_dir);
    }

    /// <summary>
    /// Check the directory structure of the vault.
    /// </summary>
    ///
    /// <returns>True if the directory structure is valid, false otherwise.</returns>
    private bool CheckVaultStructure()
    {
        // If these files and directories exist,
        // then the vault structure is valid.
        return Directory.Exists(this.data_dir)
            && Directory.Exists(this.repos_dir)
            && File.Exists(this.config_path);
    }

    /// <summary>
    /// Check if the structure of the vault is valid,
    /// if the configuration files are valid,
    /// and if the restic repositories are healthy.
    /// </summary>
    /// <returns>True if the vault is healthy, false otherwise.</returns>
    private bool CheckVaultHealth()
    {
        // TODO: Implement this.
        return true;
    }

    /// <summary>
    /// Create the directory structure for the vault.
    /// </summary>
    private void CreateDataDir()
    {
        if (Directory.Exists(this.data_dir))
            throw new VaultAlreadyExists($"The vault directory `{this.data_dir}` already exists.");

        Directory.CreateDirectory(this.data_dir);
        Directory.CreateDirectory(this.repos_dir);
    }

    /// <summary>
    /// Create a new vault.
    /// </summary>
    ///
    /// <param name="vault_password">
    /// The password for the vault.
    /// If left empty, it will be a random string of characters.
    /// </param>
    /// <param name="max_snapshots">How many snapshots to keep in each repository by default.</param>
    public void CreateVault(string vault_password = "", uint? max_snapshots = null)
    {
        var rand = new Randomizer();
        vault_password = string.IsNullOrEmpty(vault_password)
            ? rand.GenerateRandomString(VaultManager.VAULT_PASSWORD_LENGTH)
            : vault_password;

        this._config = new VaultConfig(
            vault_password,
            new List<ResticRepoConfig> { },
            max_snapshots ?? VaultManager.MAX_SNAPSHOTS
        );

        this.CreateDataDir();
        this.SaveVault();
    }

    /// <summary>
    /// Load the vault configuration file.
    /// </summary>
    public void LoadVault()
    {
        if (!this.CheckVaultStructure())
            throw new InvalidVaultException(
                $"The directory `{this.data_dir}` is not a valid vault."
            );

        string config_file_data = File.ReadAllText(this.config_path);
        this._config = JsonSerializer.Deserialize<VaultConfig>(config_file_data);
    }

    /// <summary>
    /// Save the vault configuration file changes.
    /// </summary>
    public void SaveVault()
    {
        if (this._config == null)
            throw new VaultNotLoadedException("The vault has not been loaded yet.");

        File.WriteAllText(this.config_path, JsonSerializer.Serialize(this._config));
    }

    /// <summary>
    /// Add a repository to the vault.
    /// </summary>
    ///
    /// <param name="repo">The repository to add.</param>
    ///
    /// <returns>The result of the restic command.</returns>
    public ProcessResult AddRepository(ResticRepoConfig repo)
    {
        if (this._config == null)
            throw new VaultNotLoadedException("The vault has not been loaded yet.");

        if (repo.backup_filepaths.Count == 0)
            throw new ArgumentException("There should be at least one filepath to back up.");

        foreach (var filepath in repo.backup_filepaths)
        {
            if (!File.Exists(filepath) && !Directory.Exists(filepath))
                throw new ArgumentException($"The backup filepath `{filepath}` does not exist.\n");
        }

        foreach (var existing_repo in this._config.restic_repos)
        {
            if (existing_repo.repo_name == repo.repo_name)
                throw new ArgumentException($"The repository `{repo.repo_name}` already exists.");
        }

        var result = new ResticManager(
            Path.Combine(this.repos_dir, repo.repo_name),
            this._config.vault_password,
            this.restic_bin
        ).Init();

        this._config.restic_repos.Add(repo);
        this.SaveVault();
        return result;
    }

    /// <summary>
    /// Perform a backup of a repository.
    /// </summary>
    ///
    /// <param name="repo_name">The name of the repository to back up.</param>
    ///
    /// <returns>The result of the restic command.</returns>
    public ProcessResult BackupRepository(string repo_name)
    {
        foreach (var existing_repo in this.config.restic_repos)
            if (existing_repo.repo_name == repo_name)
                return new ResticManager(
                    Path.Combine(this.repos_dir, repo_name),
                    this.config.vault_password,
                    this.restic_bin
                ).Backup(existing_repo.backup_filepaths);

        throw new RepositoryNotFoundException($"The repository `{repo_name}` does not exist.");
    }
}
