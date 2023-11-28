using System.Text.Json;

class VaultManager
{
    public const uint MAX_SNAPSHOTS = 5; // Default maximum number of snapshots to keep
    public const uint VAULT_PASSWORD_LENGTH = 16; // Default vault password length

    // Default data directory
    private string _data_dir = Path.Combine(System.Environment.CurrentDirectory, "vault");
    private VaultConfig? config = null; // Will only be assigned a value when the vault is loaded.

    public string data_dir
    {
        get { return _data_dir; }
    }
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

    public VaultManager() { }

    /// <param name="data_dir">The path to the data directory.</param>
    public VaultManager(string data_dir)
    {
        if (Path.IsPathRooted(data_dir))
        {
            this._data_dir = data_dir;
        }
        else
        {
            this._data_dir = Path.Combine(System.Environment.CurrentDirectory, data_dir);
        }
    }

    /// <summary>
    /// Check the directory structure of the vault.
    /// </summary>
    /// <returns>True if the directory structure is valid, false otherwise.</returns>
    private bool CheckVaultStructure()
    {
        // If these files and directories exist,
        // then the vault structure is valid.
        return Directory.Exists(this.data_dir)
            && Directory.Exists(this.repos_dir)
            && File.Exists(this.config_path);
    }

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
    public void CreateVault(string vault_password = "", uint? max_snapshots = null)
    {
        var rand = new Randomizer();
        vault_password = string.IsNullOrEmpty(vault_password)
            // HACK: Since the default password length is a constant,
            // is the possibility of an overflow attack still possible?
            ? rand.GenerateRandomString((int)VaultManager.VAULT_PASSWORD_LENGTH)
            : vault_password;

        this.config = new VaultConfig(
            vault_password,
            new List<ResticRepoConfig> { },
            max_snapshots ?? VaultManager.MAX_SNAPSHOTS
        );

        this.CreateDataDir();
        string config_file_data = JsonSerializer.Serialize(this.config);

        File.WriteAllText(this.config_path, config_file_data);
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
        this.config = JsonSerializer.Deserialize<VaultConfig>(config_file_data);
    }

    public List<ResticRepoConfig> GetVaultRepos()
    {
        if (this.config is null)
            throw new VaultNotLoadedException("The vault has not been loaded yet.");

        return this.config.restic_repos;
    }
}
