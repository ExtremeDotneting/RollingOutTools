@echo off
echo Founded files:
FORFILES /M "*.nupkg" /S /C "cmd /c echo @file"
pause
FORFILES /M "*.nupkg" /S /C "cmd /c xcopy @file E:\cf\projects\LocalNugetDir\ /y /i /s "
echo Copied!
echo Press enter if you want to delete .nupkg files from subfolders (recommendet) or close console.
pause
FORFILES /M "*.nupkg" /S /C "cmd /c del @file"
echo Deleted.
pause