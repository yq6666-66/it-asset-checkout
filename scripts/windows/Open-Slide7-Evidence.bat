@echo off
rem Windows PowerShell 5.1. powershell.exe is not PowerShell 7.
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Open-Slide7-Evidence.ps1"
exit /b %ERRORLEVEL%
