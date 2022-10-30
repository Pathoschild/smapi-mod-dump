
set markdownFile=%~1
set outputFile=%~2

pandoc %markdownFile% -t plain -o %outputFile% --reference-links