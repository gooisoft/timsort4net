@if not "%1"=="nopause" echo off
set FAILED_BUILDS_LOG=failed-builds.log

if not "%VS80COMNTOOLS%"=="" set VS=%VS80COMNTOOLS%
if not "%VS90COMNTOOLS%"=="" set VS=%VS90COMNTOOLS%
if not "%VS100COMNTOOLS%"=="" set VS=%VS100COMNTOOLS%

call "%VS%\vsvars32.bat"
del %FAILED_BUILDS_LOG%
for %%v in (lib\gac\*.dll) do gacutil /i "%%v"
for /r . %%v in (*.sln) do call :build "%%v"

if exist %FAILED_BUILDS_LOG% call :report_failed

goto :end

:build
@echo .
@echo .
@echo .-----------------------------------------------------------------------------
@echo .
@echo . Building %1
@echo .
@echo .-----------------------------------------------------------------------------
@echo .
@echo .

msbuild "%1" /t:Rebuild /p:Configuration=Release
if %ERRORLEVEL% neq 0 echo %1 >> %FAILED_BUILDS_LOG%

@echo .-----------------------------------------------------------------------------
@echo . Finished %1
@echo .-----------------------------------------------------------------------------

exit /b

:report_failed
@echo .
@echo .
@echo .-----------------------------------------------------------------------------
@echo .
@echo . WARNING! Some solutions have failed to build
@echo . See %FAILED_BUILDS_LOG%
@echo .
@echo .-----------------------------------------------------------------------------
@echo .
@echo .
type %FAILED_BUILDS_LOG%

exit /b

:end
@if not "%1"=="nopause" pause
