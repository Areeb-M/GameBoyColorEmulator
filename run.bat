@echo off
cd /D "%~dp0"
cd build
build.exe "%~dp0\ROMS\Pokemon Yellow.gbc"
pause