@echo off
echo ========================================
echo PC Cleaner - Create Installer Only
echo ========================================
echo.

REM Check if Release build exists
if not exist "bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\Cleaner.exe" (
    echo ERROR: Release build not found!
    echo Please run build-release.bat first
    pause
    exit /b 1
)

if not exist "bin\x64\Release\net8.0-windows10.0.19041.0\win-x64\MainWindow.xbf" (
    echo ERROR: WinUI resources missing from build output!
    echo Please run build-release.bat again
    pause
    exit /b 1
)

REM Check if Inno Setup is installed
set INNO_PATH=
if exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" (
    set INNO_PATH=C:\Program Files (x86)\Inno Setup 6\ISCC.exe
) else if exist "C:\Program Files\Inno Setup 6\ISCC.exe" (
    set INNO_PATH=C:\Program Files\Inno Setup 6\ISCC.exe
) else (
    echo ERROR: Inno Setup 6 not found!
    echo Please install it from: https://jrsoftware.org/isdl.php
    echo.
    echo After installing, run this script again.
    pause
    exit /b 1
)

echo Creating installer...
"%INNO_PATH%" installer.iss

if %errorlevel% equ 0 (
    echo.
    echo ========================================
    echo SUCCESS! Installer created!
    echo ========================================
    echo.
    echo Location: installer_output\
    dir installer_output\*.exe
) else (
    echo.
    echo ERROR: Failed to create installer!
)

echo.
pause
