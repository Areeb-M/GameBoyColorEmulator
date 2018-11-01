@echo off
cd /D "%~dp0"
"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" -out:build\build.exe src\*.cs src\cpu\*.cs

cd build
build.exe "%~dp0\ROMS\01-special.gb"
pause