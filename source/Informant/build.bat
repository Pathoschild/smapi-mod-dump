@echo off

Rem Release the entire solution
dotnet publish -c Release

Rem Create a ZIP for the informant
powershell -ExecutionPolicy Bypass -File .\nexusmods-build\scripts\stardew-valley-build.ps1 %cd%\Informant %cd%\build\prepare-html-for-nexus.ps1 1

Rem Open the folder with the results
%SystemRoot%\explorer.exe "%cd%\bin"