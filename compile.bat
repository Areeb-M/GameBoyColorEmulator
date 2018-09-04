@echo off
cd %
"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" -out:build\build.exe src\*.cs
pause