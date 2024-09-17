cd ../src/
dotnet build
robocopy "C:\Users\swharden\Documents\GitHub\ABFauto\src\AbfAuto.Analyze\bin\Debug\net8.0-windows" "X:\Software\AbfAuto\Analyze" /MIR
robocopy "C:\Users\swharden\Documents\GitHub\ABFauto\src\AbfAuto.Watcher\bin\Debug\net8.0" "X:\Software\AbfAuto\Watcher" /MIR
pause