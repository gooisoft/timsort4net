@echo off
setlocal

set SRC=..\source

rem ----------------------------------------------------------------------------
rem Run compilation
rem ----------------------------------------------------------------------------
call rebuild.cmd /s %SRC%\TimSort.sln /c Release
call rebuild.cmd /s %SRC%\TimSort.sln /c Unsafe

rem ----------------------------------------------------------------------------
rem Copy files to target folders
rem ----------------------------------------------------------------------------
echo F | xcopy /y /d ..\source\TimSort\bin\Release\TimSort.dll safe\TimSort.dll
echo F | xcopy /y /d ..\source\TimSort\bin\Unsafe\TimSort.dll unsafe\TimSort.dll
goto :end

:end
