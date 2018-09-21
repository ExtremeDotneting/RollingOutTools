@echo off
echo Founded files:
FORFILES /M "*.nupkg" /S /C "cmd /c echo @file"
pause
FORFILES /M "*.nupkg" /S /C "cmd /c xcopy @file E:\cf\projects\LocalNugetDir\ /y /i /s "
echo Copied!
call delete_nupkgs.cmd
pause