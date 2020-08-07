@ECHO OFF
SETLOCAL

CALL lint.cmd
msbuild /p:Configuration=Release
