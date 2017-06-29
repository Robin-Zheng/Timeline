@ECHO OFF

IF "%CONFIGURATION%"=="" SET CONFIGURATION=Debug

star --resourcedir="%~dp0src\Timeline\wwwroot" "%~dp0src/Timeline/bin/%CONFIGURATION%/Timeline.exe"