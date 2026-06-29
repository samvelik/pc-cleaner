@echo off
echo ========================================
echo PC Cleaner - Full Build and Installer
echo ========================================
echo.

REM Step 1: Clean previous builds
echo [1/4] Cleaning previous builds...
if exist "bin\x64\Release" rmdir /s /q "bin\x64\Release"
if exist "installer_output" rmdir /s /q "installer_output"
echo Done!
echo.

REM Step 2: Build Release version (Self-Contained)
echo [2/4] Building Release version (x64, Self-Contained)...
dotnet publish -c Release /p:Platform=x64 --self-contained true -r win-x64
if %errorlevel% neq 0 (
    echo ERROR: Build failed!
    pause
    exit /b %errorlevel%
)
echo Done!
echo.

REM Step 3: Check if Inno Setup is installed
echo [3/4] Checking Inno Setup Compiler...
set INNO_PATH=
if exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" (
    set INNO_PATH=C:\Program Files (x86)\Inno Setup 6\ISCC.exe
) else if exist "C:\Program Files\Inno Setup 6\ISCC.exe" (
    set INNO_PATH=C:\Program Files\Inno Setup 6\ISCC.exe
) else (
    echo ERROR: Inno Setup 6 not found!
    echo Please install it from: https://jrsoftware.org/isdl.php
    pause
    exit /b 1
)
echo Found: %INNO_PATH%
echo.

REM Step 4: Create installer
echo [4/4] Creating installer...
"%INNO_PATH%" installer.iss
if %errorlevel% neq 0 (
    echo ERROR: Installer creation failed!
    pause
    exit /b %errorlevel%
)
echo Done!
echo.

echo ========================================
echo SUCCESS! Installer created!
echo ========================================
echo.
echo Installer location: installer_output\
dir installer_output\*.exe
echo.
pause
