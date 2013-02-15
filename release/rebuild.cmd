@echo off
setlocal

set CONFIG="Release"
set PLATFORM="Any CPU"

call :parse_args %*

if "%SOLUTION%"=="" goto :help
if "%LOG%"=="" set LOG=%TEMP%\build.log

rem ----------------------------------------------------------------------------
rem Find Visual Studio
rem ----------------------------------------------------------------------------
set VS=UNKNOWN
if not "%VS80COMNTOOLS%"=="" set VS=%VS80COMNTOOLS%
if not "%VS90COMNTOOLS%"=="" set VS=%VS90COMNTOOLS%
if not "%VS100COMNTOOLS%"=="" set VS=%VS100COMNTOOLS%
call "%VS%\vsvars32.bat"

echo.
echo.
echo.-----------------------------------------------------------------------------
echo.
echo. Solution: %SOLUTION%
echo. Config: %CONFIG%
echo. Platform: %PLATFORM%
echo.
echo.-----------------------------------------------------------------------------
echo.
echo.

msbuild "%SOLUTION%" /t:Rebuild /p:Configuration=%CONFIG%;Platform=%PLATFORM%
if %ERRORLEVEL% neq 0 echo %1 >> %LOG%

echo.-----------------------------------------------------------------------------
echo. Finished %1
echo.-----------------------------------------------------------------------------

goto :end

:parse_args
if "%1"=="" exit /b
if "%1"=="/s" (
	set SOLUTION=%2
	shift
	shift
)
if "%1"=="/c" (
	set CONFIG=%2
	shift
	shift
)
if "%1"=="/p" (
	set PLATFORM=%2
	shift
	shift
)
goto :parse_args

:help
echo.-----------------------------------------------------------------------------
echo.
echo. Syntax: rebuild.cmd /s solution [/p platform] [/c config]
echo.
echo.-----------------------------------------------------------------------------
goto :end

:end
