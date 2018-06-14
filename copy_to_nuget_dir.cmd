@echo off
FORFILES  /M "*.nupkg" /S /C "cmd /c xcopy @file D:\Programing\LocalNuget /y /i /s
echo !!!!Press enter if you want to delete .nupkg files from subfolders (recommended) or close console.!!!!
pause
FORFILES  /M "*.nupkg" /S /C "cmd /c del @file
echo Deleted.
pause