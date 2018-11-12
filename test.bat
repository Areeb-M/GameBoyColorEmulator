@echo off
cd /D "%~dp0"
"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" @emulator.rsp
pause

cd build
build.exe "%~dp0\ROMS\Tetris.gb" "%~dp0\ROMS\DMG_ROM.bin"
pause