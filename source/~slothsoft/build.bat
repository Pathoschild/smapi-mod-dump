echo off

Rem Release the entire solution
dotnet publish -c Release

Rem Create a folder that contains everything that should be in the ZIP
set projectFolder=%cd%\Challenger
set zipFolder=%projectFolder%\bin\ReleaseZip
set outputFolder=%zipFolder%\Challenger
if exist "%zipFolder%" rmdir /s /q "%zipFolder%"
if not exist "%outputFolder%" mkdir "%outputFolder%"

xcopy /y %projectFolder%\bin\Release\net5.0\publish\Challenger.dll %outputFolder%
xcopy /y %projectFolder%\bin\Release\net5.0\publish\Challenger.pdb %outputFolder%
xcopy /y %projectFolder%\manifest.json %outputFolder%
xcopy /y %projectFolder%\i18n\ %outputFolder%\i18n\
xcopy /y %cd%\LICENSE %outputFolder%

Rem Make a HTML file out of the README
Rem If this stops working, maybe run "choco install pandoc" on the PowerShell again?
pandoc %cd%\README.md -t html -o %outputFolder%\Readme.html -s --metadata title="Slothsoft Challenger"

Rem Replace the image URLs of the HTML
powershell -Command "((gc %outputFolder%\Readme.html -encoding utf8) -replace 'readme/dev', 'https://github.com/slothsoft/stardew-challenger/blob/main/readme/dev' -replace 'readme/screen', 'https://github.com/slothsoft/stardew-challenger/raw/main/readme/screen') | Out-File -encoding utf8 %outputFolder%\Readme.html"

Rem Clear target folder
rmdir "%cd%\bin" /s /q
mkdir "%cd%\bin"

Rem Now zip the entire folder
"C:\Program Files\7-Zip\7z.exe" a %cd%\bin\Challenger-%1.zip %zipFolder%/*



Rem And now do the same for ChallengerAutomate: Create a folder that contains everything that should be in the ZIP
set projectFolderAutomate=%cd%\ChallengerAutomate
set zipFolderAutomate=%projectFolderAutomate%\bin\ReleaseZip
set outputFolderAutomate=%zipFolderAutomate%\ChallengerAutomate
if exist "%zipFolderAutomate%" rmdir /s /q "%zipFolderAutomate%"
if not exist "%outputFolderAutomate%" mkdir "%outputFolderAutomate%"

xcopy /y %projectFolderAutomate%\bin\Release\net5.0\publish\ChallengerAutomate.dll %outputFolderAutomate%
xcopy /y %projectFolderAutomate%\bin\Release\net5.0\publish\ChallengerAutomate.pdb %outputFolderAutomate%
xcopy /y %projectFolderAutomate%\manifest.json %outputFolderAutomate%

Rem Now zip the entire folder
"C:\Program Files\7-Zip\7z.exe" a %cd%\bin\ChallengerAutomate-%1.zip %zipFolderAutomate%/*



Rem Create Readme Files for Nexus
call build/create-plaintext.bat %cd%\README.md %cd%\bin\Readme.txt
call build/create-bbcode.bat %cd%\README.md %cd%\bin\bbcode.txt
xcopy /y %cd%\bin\bbcode.txt %cd%\readme\
xcopy /y %cd%\readme\bbcode-version.txt %cd%\bin\



Rem Open the folder with the results
%SystemRoot%\explorer.exe "%cd%\bin"