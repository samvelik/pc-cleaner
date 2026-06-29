# PC Cleaner

A WinUI 3 desktop app for cleaning temporary files, caches, and system junk on Windows 10/11.

![.NET](https://img.shields.io/badge/.NET-8-blue)
![Platform](https://img.shields.io/badge/platform-Windows%20x64-lightgrey)

## Download

Get the latest installer from [Releases](https://github.com/samvelik/pc-cleaner/releases).

No separate .NET runtime is required — the app is self-contained.

## Features

| Category | What it cleans |
|----------|----------------|
| **Basic** | Windows Temp, User Temp, Recycle Bin, browser cache, Windows Update cache |
| **Graphics** | NVIDIA cache & installers, AMD cache, DirectX shader cache |
| **System** | Prefetch, Windows Store cache, Delivery Optimization, event logs, network usage stats, registry history |
| **History** | Recent files, jump lists |

- Calculate size before cleaning
- Select all / deselect all
- Operation log with timestamps
- Modern Fluent UI with Mica backdrop

## Screenshots

Run the app from Visual Studio or install the release build to see the UI.

## Requirements

- Windows 10/11 (64-bit)
- Administrator rights recommended for system-level cleaning

## Build from source

### Prerequisites

- Visual Studio 2022 (17.8+)
- .NET 8 SDK
- Windows App SDK 1.8

### Run in Visual Studio

1. Open `Cleaner.slnx`
2. Press **F5**

### Build release

```batch
build-release.bat
```

Output: `bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\publish\Cleaner.exe`

### Create installer

Requires [Inno Setup 6](https://jrsoftware.org/isdl.php).

```batch
build-installer.bat
```

Output: `installer_output\PCCleaner_Setup_1.0.0.exe`

| Script | Description |
|--------|-------------|
| `build-release.bat` | Publish self-contained Release build |
| `build-installer.bat` | Build + create installer |
| `create-installer.bat` | Create installer only (after build) |
| `run-release.bat` | Run the published app |

## Project structure

```
Cleaner/
├── Assets/           # App icon and logo
├── Models/           # CleaningType enum
├── Services/         # CleaningService
├── MainWindow.xaml   # UI
├── App.xaml          # Strings and styles
├── installer.iss     # Inno Setup script
└── *.bat             # Build scripts
```

## Permissions

Some actions work without admin (user temp, browser cache, recent files). Others may need elevation (Windows temp, recycle bin, event logs, prefetch, etc.).

## Troubleshooting

- **App crashes** — check `CleanerError.log` on the Desktop
- **Build fails** — run Clean + Rebuild in Visual Studio
- **Installer not found** — run `build-release.bat` before `create-installer.bat`

## License

See [LICENSE.txt](LICENSE.txt).

## Author

[samvelik](https://github.com/samvelik)
