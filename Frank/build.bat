@echo off

dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
cd .\bin\Release\net10.0-windows\win-x64\publish\
explorer .