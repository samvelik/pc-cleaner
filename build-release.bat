@echo off
echo Building PC Cleaner - Release x64 (Self-Contained)...
dotnet publish -c Release /p:Platform=x64 --self-contained true -r win-x64
echo.
echo Build complete!
echo Executable location: bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\Cleaner.exe
if not exist "bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\MainWindow.xbf" (
    echo WARNING: WinUI resources are missing from build output!
)
pause
