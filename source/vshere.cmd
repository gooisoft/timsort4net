@echo off

if not "%VS80COMNTOOLS%"=="" set VS=%VS80COMNTOOLS%
if not "%VS90COMNTOOLS%"=="" set VS=%VS90COMNTOOLS%

%comspec% /k "%VS%\vsvars32.bat" x86