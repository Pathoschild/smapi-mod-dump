@ECHO OFF

setlocal

for /f "delims=" %%x in (version) do set Version=%%x

for /F "tokens=* USEBACKQ" %%F in (`git tag -l %Version%`) do (
	set Exists=%%F
)

echo Attempting to push tag '%Version%'

if not [%Exists%] == [] (
	echo The tag '%Version%' already exists.
	exit /b 0
)

git tag %Version%
if %ERRORLEVEL% NEQ 0 (
	set Error=Failed to set tag
	goto :ERROR
) 
git push origin --tags
if %ERRORLEVEL% NEQ 0 (
	set Error=Failed to push tag
	goto :ERROR
) 
git fetch --tags origin
if %ERRORLEVEL% NEQ 0 (
	set Error=Failed to fetch tags
	goto :ERROR
) 

echo The tag '%Version%' has been pushed.

exit /b 0

:ERROR
echo Failed: %Error% 1>&2
exit /b 1
