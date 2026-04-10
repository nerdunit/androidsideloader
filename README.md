# Rookie Sideloader (AndroidSideloader)

![GitHub last commit](https://img.shields.io/github/last-commit/nerdunit/androidsideloader)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/nerdunit/androidsideloader)
[![Downloads](https://img.shields.io/github/downloads/nerdunit/androidsideloader/total.svg)](https://github.com/nerdunit/androidsideloader/releases)
![Issues](https://img.shields.io/github/issues/nerdunit/androidsideloader)

**Rookie Sideloader** (also known as **Rookie** or **RSL**) is a free, open-source, general-purpose Android sideloading tool for Windows. It provides a user-friendly interface for installing, managing, and updating Android applications on your devices — with a particular focus on standalone VR headsets such as Meta Quest.

Rookie is a legitimate, legal tool built around standard Android Debug Bridge (ADB) functionality. It does not modify, crack, or bypass any software protections. Intended use cases include developer testing, installing applications not available through official storefronts, personal backups, and managing your own app libraries. Users are solely responsible for ensuring that their use of this tool complies with all applicable laws and the terms of service of their devices.

## Features

- **Online & Offline Modes** — On first launch, a startup dialog lets you choose how to operate:
  - **Online Mode** — Connect to a remote library using a public config or your own rclone config file. Configs can be loaded from a local file, a URL, or pasted directly.
  - **Offline / Local Library Mode** — Point Rookie at a local directory of APKs and OBBs. It scans the folder, calculates sizes, fetches version codes, and caches results — no server required.
- **Gallery & List Views** — Browse apps in a visual gallery with adjustable tile sizes or a detailed sortable list. Both views support search, filters (Installed, Update Available, Downloaded, Ahead of Server), favorites, one-click install and one-click uninstall.
- **Drag & Drop Installation** — Drag APK files or folders directly onto Rookie to install them to your device.
- **Download Queue** — Queue multiple downloads with real-time progress, bandwidth limiting, and the option to resume interrupted transfers.
- **Device Management** — Detect and switch between multiple connected Android devices, monitor connection status, and run custom ADB commands.
- **Backup & Restore** — Create and restore save states of installed applications.
- **YouTube Trailers** — Preview app trailers directly inside Rookie.
- **Automatic Updates** — Rookie checks for new versions on startup and can update itself from GitHub Releases.

## Download

Grab the latest release from the [Releases](https://github.com/nerdunit/androidsideloader/releases) page.

## Important Notes

> **Antivirus False Positives** — Some antivirus software may flag Rookie due to its use of ADB, archive extraction, and runtime downloads. Both the Sideloader and the Sideloader Launcher are fully open source — you can inspect and build the code yourself.

> **Folder Placement** — Rookie must be extracted to an unprotected folder on your drive. We recommend `C:\RSL\Rookie`. **Do not** use protected or cloud-synced paths such as `C:\Users\Desktop`, `C:\Program Files`, OneDrive, or Google Drive. Rookie manages its own working directory and may clean up files within it — do not place it in a folder that contains other important files.

## Build Instructions

This project is built with C# / WinForms targeting **.NET Framework 4.5.2**.

1. Clone this repository.
2. Install the [.NET Framework 4.5.2 Developer Pack](https://dotnet.microsoft.com/download/dotnet-framework) if you don't already have it.
3. Open `AndroidSideloader.sln` in Visual Studio 2022 or later.

   > **Note:** If the build fails due to `packages.config`, migrate to PackageReference by right-clicking **References** in Solution Explorer and selecting **Migrate packages.config to PackageReference**.

4. Build the solution (`Ctrl + Shift + B`).
5. Run the application.

## Contributing

We welcome contributions from the community. Please fork the repository, branch from the newest beta branch, make your changes, and submit a pull request.

## License

Rookie Sideloader is distributed under the **GNU General Public License v3.0** — any derivative work must also be made available as open source. See the [LICENSE](LICENSE) file for full details.