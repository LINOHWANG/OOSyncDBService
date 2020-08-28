@echo off
cd C:\Windows\Microsoft.NET\Framework\v4.0.30319
installutil.exe -u "C:\OOSyncDBSvc\OOSyncDBSvc.exe"
installutil.exe "C:\OOSyncDBSvc\OOSyncDBSvc.exe"

if ERRORLEVEL 1 goto error
exit
:error
echo There was a problem installing OOSyncDBSvc Service
pause