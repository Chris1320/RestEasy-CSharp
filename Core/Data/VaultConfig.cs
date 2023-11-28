public record VaultConfig
{
    private string _data_dir = Path.Combine(System.Environment.CurrentDirectory, "data");
    public string data_dir
    {
        get { return _data_dir; }
        set
        {
            if (Path.IsPathRooted(value))
            {
                _data_dir = value;
            }
            else
            {
                _data_dir = Path.Combine(System.Environment.CurrentDirectory, value);
            }
        }
    }
}
