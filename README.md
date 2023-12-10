# RestEasy

> **NOTE**: This project is still under development.

A [restic](https://restic.net/) wrapper for automating backups written in C#.

```bash
resteasy init  # Create a new RestEasy vault in ~/.config/resteasy/vault/

# Create a restic repository in the vault
resteasy add ~/some-project

resteasy backup some-project  # Perform backup

# Do something here, and then realize you messed up...

resteasy restore some-project  # Restore from the backup
```
