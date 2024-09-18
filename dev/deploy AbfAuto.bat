set sourceFolder="C:\Users\swharden\Documents\GitHub\ABFauto\src\AbfAuto\bin\Debug\net8.0-windows"
set targetFolder="X:\Software\AbfAuto\Analyze"
rmdir /s /q %sourceFolder%
rmdir /s /q %targetFolder%
dotnet build ../src/
robocopy %sourceFolder% %targetFolder% /MIR
pause