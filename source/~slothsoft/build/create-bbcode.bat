
set markdownFile=%~1
set outputFile=%~2
set htmlFile=%~2.html
set htmlFileSnip=%~2-snip.html

pandoc %markdownFile% -t html -o %htmlFile% --wrap=none
powershell -ExecutionPolicy Bypass -File %cd%/build/prepare-html-for-nexus.ps1 %htmlFile% %htmlFileSnip%
powershell -ExecutionPolicy Bypass -File %cd%/build/html-to-bbcode.ps1 %htmlFileSnip% %outputFile%

del %htmlFile%
del %htmlFileSnip%