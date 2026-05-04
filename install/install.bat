@echo off

echo [*] Compilation de Frydia.
dotnet publish ..\Frydia.slnx -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o .\bin >nul 2>&1
del .\bin\*.pdb
echo [+] Frydia compile avec succes.
echo.

echo [*] Compilation de Launcher.
csc /target:winexe /out:bin\launcher.exe launcher.cs >nul 2>&1
echo [+] Launcher compile avec succes.
echo.

echo [*] Compilation de Setup.
csc /out:bin\setup.exe setup.cs >nul 2>&1
echo [+] Setup compile avec succes.