@echo off
setlocal

REM Set your value (you can change this as needed)
set "myValue=10"

REM Check if myValue is greater than or equal to 10
if %myValue% GEQ 10 (
    echo Value is greater than or equal to 10.
    msg * "Value is greater than or equal to 10."
) else (
    echo Value is less than 10.
    msg * "Value is less than 10."
)