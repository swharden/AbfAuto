@echo off
echo WARNING: ensure the folder watcher is not running on the server before proceeding
pause
set sourceFolder="C:\Users\swharden\Documents\GitHub\ABFauto\src\AbfFolderWatcher\bin\Debug\net8.0"
set targetFolder="X:\Software\AbfAuto\Watcher"
rmdir /s /q %sourceFolder%
rmdir /s /q %targetFolder%
dotnet build ../src/
robocopy %sourceFolder% %targetFolder% /MIR
pause