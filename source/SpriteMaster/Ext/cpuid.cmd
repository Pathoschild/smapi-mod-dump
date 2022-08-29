@ECHO OFF

clang -std=gnu++20 cpuid.cpp -o cpuid.exe
if %ERRORLEVEL% NEQ 0 (
	echo Build Error
	return 1
)
cls
cpuid.exe
rm -f cpuid.exe
