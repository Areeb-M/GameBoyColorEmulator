@echo off
cd /D "%~dp0"
cd src
"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" -out:build\build.exe *.cs cpu\*.cs
pause