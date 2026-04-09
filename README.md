# AndroidSideloader

![GitHub last commit](https://img.shields.io/github/last-commit/VRPirates/rookie)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/VRPirates/rookie)
[![Downloads](https://img.shields.io/github/downloads/VRPirates/rookie/total.svg)](https://github.com/VRPirates/rookie/releases)
![Issues](https://img.shields.io/github/issues/VRPirates/rookie)

## Disclaimer
This application might get flagged as malware by some antivirus software; however, both the Sideloader and the Sideloader Launcher are open source.

To run properly, Rookie must be extracted to a non-Protected folder on your drive. We recommend running Rookie from C:\RSL\Rookie
Do Not use folders such as- C:\Users; C:\Users\Desktop; C:\Program Files; OneDrive; Google Drive; etc...
Rookie will cleanup its own folder. We are not responsible if you run Rookie from a folder containing other files as Rookie may delete them.

## Build Instructions
This project is developed using C# with WinForms targeting the .NET Framework 4.5.2. To build the project successfully in Visual Studio 2022, follow these steps:

1. Clone this repository to your local machine.
2. Ensure you have the .NET Framework 4.5.2 installed on your machine.
3. Open the solution file (`*.sln`) in Visual Studio 2026.
4. Sometimes the building process can fail due to the packages.config, you should migrate it to PackageReference, do this by right clicking on References in the Solution Explorer, and pressing "Migrate packages.config to PackageReference"
5. Build the solution by selecting "Build" > "Build Solution" from the Visual Studio menu or pressing `Ctrl + Shift + B`. (or right click the solution in the solution explorer, then press Build)
6. Run the application.

## Contributing
We welcome contributions from the community. If you would like to contribute, please fork the repository, branch from the newest beta branch from this repository, make your changes, and submit a pull request.

## License
AndroidSideloader is distributed under the GPL license, meaning any forks of it must have their source code made public on the internet. See the [LICENSE](LICENSE) file for details.
