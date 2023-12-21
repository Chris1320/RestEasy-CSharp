# RestEasy

> **NOTE**: This project is still under development.

A [restic](https://restic.net/) wrapper for automating backups written in C#.

I have been using [GameSave Manager](https://www.gamesave-manager.com/),
[Ludusavi](https://github.com/mtkennerly/ludusavi), and my good ol' ZIP archives
for backing up my game saves for a very long time. I discovered
[restic](https://restic.net/) a year ago and I have been using it to back up
my server data. I think that I can use restic for both game saves and non-game
data, but I don't want to manually manage the organization of the backups and
the actual process of backing up each files and directories. This is why
RestEasy is made.

RestEasy attempts to bring the features of game save backup tools to restic,
while also providing backup automations for non-game data such as texts, media
files, or even Podman/Docker containers.

**Features**:

- Basic restic features
  - [x] File/Directory backups
  - [ ] Backup restoration (restore manually for now)
- RestEasy features
  - [ ] Repository tags/groups
  - [ ] (_Windows_) Registry backups
  - [ ] Automatic game save detection
  - [ ] Podman/Docker volume backups
  - [ ] (_Windows_) WSL data backups from Windows host

## Usage Examples

```bash
resteasy init  # Create a new RestEasy vault in ~/.config/resteasy/vault/

# Create a restic repository in the vault
resteasy add ~/some-project

resteasy backup some-project  # Perform backup

# Do something here, and then realize you messed up...

resteasy restore some-project  # Restore from the backup
```
