@echo off

Rem Release the entire solution
dotnet publish -c Release

Rem Create the ZIPs for the Challenger
powershell -ExecutionPolicy Bypass -File .\nexusmods-build\scripts\stardew-valley-build.ps1 %cd%\Challenger %cd%\build\prepare-html-for-nexus.ps1 1
powershell -ExecutionPolicy Bypass -File .\nexusmods-build\scripts\stardew-valley-build.ps1 %cd%\ChallengerAutomate %cd%\build\prepare-html-for-nexus.ps1 0

# Copy the versions text file
xcopy /y %cd%\readme\bbcode-version.txt %cd%\bin\

Rem Open the folder with the results
%SystemRoot%\explorer.exe "%cd%\bin"